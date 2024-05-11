using DocumentManager;
using DocumentManagerModel;
using Microsoft.EntityFrameworkCore;

namespace DocumentManagerPersistence;

public class DocumentRepository
{
    private readonly string _dataRootFolder;
    private readonly string _documentFolder;
    private readonly string _dbPath;

    public DocumentRepository(PersistenceDefinitions definitions)
    {
        _dataRootFolder = definitions.DataRootFolder;
        _documentFolder = definitions.DocumentFolder;
        _dbPath = GetCompleteFilePath("Documents.db");
    }

    public void Init()
    {
        using var db = new DocumentContext { DbPath = _dbPath };
        db.Database.Migrate();
    }

    public DocumentMetadataDao CreateDocument(DocumentMetadataDao metadata)
    {
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

    private string GetCompleteFilePath(string filePath)
    {
        return Path.Combine(_dataRootFolder, filePath);
    }

    public IEnumerable<DocumentMetadataDto> GetDocuments()
    {
        using (var db = new DocumentContext{ DbPath = _dbPath })
        { 
            var metadataList = db.Metadatas.Include(dao => dao.Tags).ToList();
            return metadataList.Select(dao => dao.ToDto());
        }
    }

    public IEnumerable<DocumentMetadataDto> GetDocuments(string search)
    {
        using (var db = new DocumentContext { DbPath = _dbPath })
        {
            var metadataList = db.Metadatas.Where(dao => dao.TextContent.Contains(search)).Include(dao => dao.Tags)
                .ToList();
            return metadataList.Select(dao => dao.ToDto());
        }
    }

    public DocumentMetadataDto GetDocument(Guid id)
    {
        using (var db = new DocumentContext{ DbPath = _dbPath })
        {
            var metadata = db.Metadatas.Include(dao => dao.Tags).Single(data => data.Id == id);
            Directory.CreateDirectory(Path.Combine("wwwroot", "documents"));
            File.Copy(GetCompleteFilePath(metadata.FilePath), Path.Combine("wwwroot", "documents", metadata.Id + metadata.FileExtension), true);
            return metadata.ToDto();
        }
    }

    public DocumentMetadataDto UpdateMetadata(DocumentMetadataDto metadata)
    {
        using (var db = new DocumentContext{ DbPath = _dbPath })
        {
            var persistedDao = db.Metadatas.Include(dao => dao.Tags).Single(dao => dao.Id ==  metadata.Id);

            var newDao = metadata.ToDao("");
            persistedDao.UpdateFromDao(newDao);

            db.SaveChanges();
            return metadata;
        }
    }
}