using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Text;
using DocumentManager;
using DocumentManagerModel;
using Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace DocumentManagerPersistence;

public class DocumentRepository
{
    private readonly string _dataRootFolder;
    private readonly string _dbPath;
    private readonly string _deletedFolder;

    private readonly FilePersistence _filePersistence;
    private readonly FilesystemViewService _filesystemViewService;
    private readonly PersistenceDefinitions _definitions;
    private ContactRepository _contactRepository;
    private readonly IMessageService _messageService;

    public DocumentRepository(IOptions<PersistenceDefinitions> definitions, FilePersistence filePersistence,
        FilesystemViewService filesystemViewService, ContactRepository contactRepository, IMessageService messageService)
    {
        _definitions = definitions.Value;
        _filePersistence = filePersistence;
        _filesystemViewService = filesystemViewService;
        _contactRepository = contactRepository;
        _messageService = messageService;
        _dataRootFolder = _definitions.DataRootFolder;
        _deletedFolder = _definitions.DeletedFolder;
        _dbPath = GetCompleteFilePath(_definitions.DbName);
    }

    

    public DocumentMetadataDao CreateDocument(DocumentMetadataDao metadata, IDocumentFile file)
    {
        var id = Guid.NewGuid();
        metadata.Id = id;
        metadata.FilePath = _filePersistence.SaveFile(file, metadata.GenerateFileName(), metadata.Scope);
        metadata.Hash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(metadata.TextContent)));
        if (metadata.Contact != null)
        {
            metadata.Contact = _contactRepository.GetOrCreate(metadata.Contact.Name).ToDao();    
        }

        using (var db = new DocumentContext { DbPath = _dbPath })
        {
            var duplicate = db.Metadatas.FirstOrDefault(data => data.Hash == metadata.Hash);
            if (duplicate != null)
            {
                _messageService.SendMessage($"{new Link($"/{metadata.Scope}/document/{metadata.Id}", $"{metadata.Title}")} und {new Link($"{duplicate.Scope}/document/{duplicate.Id}", $"{duplicate.Title}")} scheinen Duplikate zu sein.", MessageSeverity.Warning );
            }
            db.Metadatas.Add(metadata);
            foreach (var tagDao in metadata.Tags.Where(dao => dao.Id != default))
            {
                db.Tags.Attach(tagDao);
            }
            if (metadata.Contact?.Id != default)
            {
                db.Contacts.Attach(metadata.Contact);
            }

            db.SaveChanges();
        }

        _filesystemViewService.AddDocumentToView(metadata.ToDto());
        return metadata;
    }

    public string GetFilePath(Guid id)
    {
        using var db = new DocumentContext { DbPath = _dbPath };
        var metadata = db.Metadatas.Single(data => data.Id == id);
        return GetCompleteFilePath(metadata.FilePath);
    }

    private string GetCompleteFilePath(string filePath)
    {
        return Path.Combine(_dataRootFolder, filePath);
    }

    public IEnumerable<DocumentMetadataDto> GetDocuments(string scope)
    {
        using (var db = new DocumentContext { DbPath = _dbPath })
        {
            var metadataList = db.Metadatas
                .Where(dao => dao.Scope == scope)
                .Include(dao => dao.Tags)
                .Include(dao => dao.Contact)
                .ToList();
            return metadataList.Select(dao => dao.ToDto());
        }
    }

    internal IEnumerable<DocumentMetadataDto> GetAllDocuments()
    {
        using (var db = new DocumentContext { DbPath = _dbPath })
        {
            var metadataList = db.Metadatas
                .Include(dao => dao.Tags)
                .Include(dao => dao.Contact)
                .ToList();
            return metadataList.Select(dao => dao.ToDto());
        }
    }

    public IEnumerable<DocumentMetadataDto> GetDocuments(string scope, string search)
    {
        using (var db = new DocumentContext { DbPath = _dbPath })
        {
            var metadataList = db.Metadatas.Where(dao => dao.Scope == scope)
                .Where(dao => EF.Functions.Like(dao.TextContent, $"%{search}%"))
                .Include(dao => dao.Tags)
                .Include(dao => dao.Contact)
                .ToList();
            return metadataList.Select(dao => dao.ToDto());
        }
    }

    public DocumentMetadataDto GetDocument(Guid id)
    {
        using var db = new DocumentContext { DbPath = _dbPath };
        var metadata = db.Metadatas.Include(dao => dao.Tags)
            .Include(dao => dao.Contact)
            .Single(data => data.Id == id);
        return metadata.ToDto();
    }

    public DocumentMetadataDto UpdateMetadata(DocumentMetadataDto metadata)
    {
        using var db = new DocumentContext { DbPath = _dbPath };

        var persistedDao = db.Metadatas.Include(dao => dao.Tags)
            .Include(dao => dao.Contact)
            .Single(dao => dao.Id == metadata.Id);

        var oldMetadata = persistedDao.ToDto();

        var newDao = metadata.ToDao("");
        persistedDao.UpdateFromDao(newDao);
        if (!string.IsNullOrEmpty(persistedDao.Contact?.Name) && (persistedDao.Contact?.Id == null || persistedDao.Contact?.Id == Guid.Empty))
        {
            persistedDao.Contact = _contactRepository.GetOrCreate(persistedDao.Contact!.Name).ToDao();
        }

        var newFileName = persistedDao.GenerateFileName();
        var newRelativePath = _filePersistence.GetRelativePath(metadata.Scope, newFileName, newDao.FileExtension);

        if (persistedDao.FilePath != newRelativePath)
        {
            var oldFilePath = persistedDao.FilePath;

            persistedDao.FilePath = _filePersistence.CopyToNewName(metadata.Scope, oldFilePath,
                persistedDao.GenerateFileName(), metadata.FileExtension);

            db.SaveChanges();

            try
            {
                _filePersistence.DeleteManagedFile(oldFilePath);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        else
        {
            db.SaveChanges();
        }

        _filesystemViewService.UpdateDocumentInView(oldMetadata, metadata);

        return metadata;
    }

    public void DeleteDocument(Guid id)
    {
        DocumentMetadataDto deletedMetadata;
        using (var db = new DocumentContext { DbPath = _dbPath })
        {
            var metadata = db.Metadatas.Include(dao => dao.Tags)
                .Include(dao => dao.Contact)
                .Single(data => data.Id == id);
            deletedMetadata = metadata.ToDto();
            try
            {
                File.Move(GetCompleteFilePath(metadata.FilePath),
                    Path.Combine(_dataRootFolder, _deletedFolder, Path.GetFileName(metadata.FilePath)));
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine(e);
            }

            db.Metadatas.Remove(metadata);

            db.SaveChanges();
        }

        _filesystemViewService.RemoveDocumentFromView(deletedMetadata);
    }

    public IEnumerable<string> GetScopes()
    {
        using var db = new DocumentContext { DbPath = _dbPath };
        return db.Metadatas
            .Select(dao => dao.Scope)
            .Distinct()
            .OrderBy(scope => scope)
            .ToList();
    }

    internal bool IsMigrationCompleted(string migrationName)
    {
        using var db = new DocumentContext { DbPath = _dbPath };
        return db.DataMigrations.Any(m => m.Name == migrationName && m.Completed);
    }

    internal void SaveMigration(DataMigrationDao migrationDao)
    {
        using var db = new DocumentContext { DbPath = _dbPath };
        db.DataMigrations.Add(migrationDao);
        db.SaveChanges();
    }
}