using DocumentManager;
using DocumentManagerModel;
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

    public DocumentRepository(IOptions<PersistenceDefinitions> definitions, FilePersistence filePersistence, FilesystemViewService filesystemViewService)
    {
        _definitions = definitions.Value;
        _filePersistence = filePersistence;
        _filesystemViewService = filesystemViewService;
        _dataRootFolder = _definitions.DataRootFolder;
        _deletedFolder = _definitions.DeletedFolder;
        _dbPath = GetCompleteFilePath("Documents.db");
    }

    public void Init()
    {
        Console.WriteLine("Creating DBContext");
        using var db = new DocumentContext { DbPath = _dbPath };
        Console.WriteLine("migrating Database");
        db.Database.Migrate();
        Console.WriteLine("Migration done");
        if (!_definitions.GenerateFilesystemView)
            return;
        var documents = GetAllDocuments();
        _filesystemViewService.RegenerateView(documents);
    }

    public DocumentMetadataDao CreateDocument(DocumentMetadataDao metadata, IDocumentFile file)
    {
        var id = Guid.NewGuid();
        metadata.Id = id;
        metadata.FilePath = _filePersistence.SaveFile(file, metadata.GenerateFileName(), metadata.Scope);
        
        using (var db = new DocumentContext{ DbPath = _dbPath })
        {
            db.Metadatas.Add(metadata);
            foreach (var tagDao in metadata.Tags.Where(dao => dao.Id != default))
            {
                db.Tags.Attach(tagDao);
            }
            db.SaveChanges();
        }
        
        _filesystemViewService.AddDocumentToView(metadata.ToDto());
        return metadata;
    }
    
    public string GetFilePath(Guid id)
    {
        using var db = new DocumentContext{ DbPath = _dbPath };
        var metadata = db.Metadatas.Single(data => data.Id == id);
        return GetCompleteFilePath(metadata.FilePath);
    }

    private string GetCompleteFilePath(string filePath)
    {
        return Path.Combine(_dataRootFolder, filePath);
    }

    public IEnumerable<DocumentMetadataDto> GetDocuments(string scope)
    {
        using (var db = new DocumentContext{ DbPath = _dbPath })
        { 
            var metadataList = db.Metadatas
                .Where(dao => dao.Scope == scope)
                .Include(dao => dao.Tags)
                .ToList();
            return metadataList.Select(dao => dao.ToDto());
        }
    }
    
    internal IEnumerable<DocumentMetadataDto> GetAllDocuments()
    {
        using (var db = new DocumentContext{ DbPath = _dbPath })
        { 
            var metadataList = db.Metadatas
                .Include(dao => dao.Tags)
                .ToList();
            return metadataList.Select(dao => dao.ToDto());
        }
    }

    public IEnumerable<DocumentMetadataDto> GetDocuments(string scope, string search)
    {
        using (var db = new DocumentContext { DbPath = _dbPath })
        {
            var metadataList = db.Metadatas.Where(dao => dao.Scope == scope).Where(dao => EF.Functions.Like(dao.TextContent, $"%{search}%")).Include(dao => dao.Tags)
                .ToList();
            return metadataList.Select(dao => dao.ToDto());
        }
    }

    public DocumentMetadataDto GetDocument(Guid id)
    {
        using var db = new DocumentContext{ DbPath = _dbPath };
        var metadata = db.Metadatas.Include(dao => dao.Tags).Single(data => data.Id == id);
        return metadata.ToDto();
    }

    public DocumentMetadataDto UpdateMetadata(DocumentMetadataDto metadata)
    {
        using var db = new DocumentContext{ DbPath = _dbPath };
    
        var persistedDao = db.Metadatas.Include(dao => dao.Tags).Single(dao => dao.Id == metadata.Id);
        
        var oldMetadata = persistedDao.ToDto();
    
        var newDao = metadata.ToDao("");
        persistedDao.UpdateFromDao(newDao);
    
        var newFileName = persistedDao.GenerateFileName();
        var newRelativePath = _filePersistence.GetRelativePath(metadata.Scope, newFileName, newDao.FileExtension);
    
        if (persistedDao.FilePath != newRelativePath)
        {
            var oldFilePath = persistedDao.FilePath;
            
            persistedDao.FilePath = _filePersistence.CopyToNewName(metadata.Scope, oldFilePath, persistedDao.GenerateFileName(), metadata.FileExtension);
            
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
            var metadata = db.Metadatas.Include(dao => dao.Tags).Single(data => data.Id == id);
            deletedMetadata = metadata.ToDto();
            try
            {
                File.Move(GetCompleteFilePath(metadata.FilePath), Path.Combine(_deletedFolder, metadata.Id + metadata.FileExtension));
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