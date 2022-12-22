using DocumentManager;
using DocumentManagerModel;
using Microsoft.EntityFrameworkCore;

namespace DocumentManagerPersistence;

public class DocumentRepository
{
    private readonly string baseFilePath = "wwwroot";
    private readonly string documentFolder = "documents";

    public DocumentRepository()
    {
    }

    public DocumentMetadataDto CreateDocument(DocumentMetadataDto metadata, DocumentFile file)
    {
        metadata.Id = Guid.NewGuid();
        var filePath = Path.Combine(baseFilePath, documentFolder,  metadata.Id.ToString() + file.FileExtension);
        metadata.FilePath = string.Join("/", documentFolder, metadata.Id.ToString() + file.FileExtension);
        using (var db = new DocumentContext())
        {
            db.Database.EnsureCreated();
            var exisitingTags = db.Tags.Where(dao => metadata.Tags.Contains(dao.value));
            db.Metadatas.Add(metadata.ToDao(exisitingTags));
            db.SaveChanges();
        }
        
        using (var fileStream = File.Create(filePath))
        {
            file.Stream.Seek(0, SeekOrigin.Begin);
            file.Stream.CopyTo(fileStream);
        }

        return metadata;
    }

    public IEnumerable<DocumentMetadataDto> GetDocuments()
    {
        using (var db = new DocumentContext())
        {
            db.Database.EnsureCreated();
            var metadataList = db.Metadatas.Include(dao => dao.Tags).ToList();
            return metadataList.Select(dao => dao.ToDto());
        }
    }

    public DocumentMetadataDto GetDocument(Guid id)
    {
        using (var db = new DocumentContext())
        {
            db.Database.EnsureCreated();
            var metadata = db.Metadatas.Include(dao => dao.Tags).Single(data => data.Id == id);
            return metadata.ToDto();
        }
    }

    public DocumentMetadataDto UpdateMetadata(DocumentMetadataDto metadata)
    {
        using (var db = new DocumentContext())
        {
            db.Database.EnsureCreated();
            var exisitingTags = db.Tags.Where(dao => metadata.Tags.Contains(dao.value));
            db.Metadatas.Update(metadata.ToDao(exisitingTags));
            db.SaveChanges();
            return metadata;
        }
    }
}