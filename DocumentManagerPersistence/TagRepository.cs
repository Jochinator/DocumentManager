using System.Reflection;
using DocumentManager;
using DocumentManager.TextExtractor;
using DocumentManagerModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace DocumentManagerPersistence;

public class TagRepository
{
    private readonly string _dataRootFolder;
    private readonly string _dbPath;

    public TagRepository(IOptions<PersistenceDefinitions> definitions)
    {
        _dataRootFolder = definitions.Value.DataRootFolder;
        _dbPath = GetCompleteFilePath("Documents.db");
    }

    private string GetCompleteFilePath(string filePath)
    {
        return Path.Combine(_dataRootFolder, filePath);
    }

    public IEnumerable<TagDto> GetTags()
    {
        using (var db = new DocumentContext{ DbPath = _dbPath })
        {
            return db.Tags.Select(dao => dao.ToDto()).ToList();
        }
    }
    
    public TagDto UpdateTag(TagDto tag)
    {
        using var db = new DocumentContext { DbPath = _dbPath };
        
        var duplicate = db.Tags
            .Include(c => c.Metadatas)
            .ThenInclude(m => m.Tags)
            .FirstOrDefault(t => t.Value == tag.Value && t.Id != tag.Id);
    
        if (duplicate != null)
        {
            var tagToMerge = db.Tags
                .Include(c => c.Metadatas)
                .ThenInclude(m => m.Tags)
                .Single(t => t.Id == tag.Id);
        
            foreach (var doc in tagToMerge.Metadatas)
            {
                if (doc.Tags.All(t => t.Id != duplicate.Id))
                {
                    doc.Tags.Add(duplicate);    
                }
                
                doc.Tags.Remove(tagToMerge);
            }
        
            db.Tags.Remove(tagToMerge);
            db.SaveChanges();
            return duplicate.ToDto();
        }
        
        var persistedTag = db.Tags.Single(t => t.Id == tag.Id);
        persistedTag.IsManualOnly = tag.IsManualOnly;
        persistedTag.Value = tag.Value;
        db.SaveChanges();
        return tag;
    }

    public void DeleteTag(Guid id)
    {
        using var db = new DocumentContext { DbPath = _dbPath };
        var tag = db.Tags.Single(t => t.Id == id);
        db.Tags.Remove(tag);
        db.SaveChanges();
    }
}