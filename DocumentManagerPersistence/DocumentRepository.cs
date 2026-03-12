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

    public DocumentRepository(IOptions<PersistenceDefinitions> definitions, FilePersistence filePersistence)
    {
        _filePersistence = filePersistence;
        _dataRootFolder = definitions.Value.DataRootFolder;
        _deletedFolder = definitions.Value.DeletedFolder;
        _dbPath = GetCompleteFilePath("Documents.db");
    }

    public void Init()
    {
        using var db = new DocumentContext { DbPath = _dbPath };
        db.Database.Migrate();
    }

    public DocumentMetadataDao CreateDocument(DocumentMetadataDao metadata, IDocumentFile file)
    {
        var id = Guid.NewGuid();
        metadata.Id = id;
        metadata.FilePath = _filePersistence.SaveFile(file, metadata.GenerateFileName());
        
        using (var db = new DocumentContext{ DbPath = _dbPath })
        {
            db.Metadatas.Add(metadata);
            foreach (var tagDao in metadata.Tags.Where(dao => dao.Id != default))
            {
                db.Tags.Attach(tagDao);
            }
            db.SaveChanges();
        }

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
    
        var newDao = metadata.ToDao("");
        persistedDao.UpdateFromDao(newDao);
    
        var newFileName = persistedDao.GenerateFileName();
        var newRelativePath = _filePersistence.GetRelativePath(newFileName, newDao.FileExtension);
    
        if (persistedDao.FilePath != newRelativePath)
        {
            var oldFilePath = persistedDao.FilePath;
            
            persistedDao.FilePath = _filePersistence.CopyToNewName(oldFilePath, persistedDao.GenerateFileName(), metadata.FileExtension);
            
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
    
        return metadata;
    }

    public void DeleteDocument(Guid id)
    {
        using (var db = new DocumentContext { DbPath = _dbPath })
        {
            var metadata = db.Metadatas.Single(data => data.Id == id);
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