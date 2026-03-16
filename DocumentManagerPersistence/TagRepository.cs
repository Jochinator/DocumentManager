using DocumentManager;
using DocumentManagerModel;
using DocumentManagerModel.Rule;
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
        _dbPath = GetCompleteFilePath(definitions.Value.DbName);
    }

    private string GetCompleteFilePath(string filePath)
    {
        return Path.Combine(_dataRootFolder, filePath);
    }

    public IEnumerable<TagDto> GetTags()
    {
        using var db = new DocumentContext{ DbPath = _dbPath };
        var ruleRepository = new RuleRepository(db);
        return db.Tags.Include(t => t.Rule)
            .Select(t => IncludeRule(t, ruleRepository))
            .Select(dao => dao.ToDto())
            .ToList();
    }
    
    public TagDto UpdateTag(TagDto tag)
    {
        using var db = new DocumentContext { DbPath = _dbPath };
        var ruleRepository = new RuleRepository(db);
        var duplicate = db.Tags
            .Include(c => c.Metadatas)
            .ThenInclude(m => m.Tags)
            .FirstOrDefault(t => t.Value == tag.Value && t.Id != tag.Id);
    
        if (duplicate != null)
        {
            if (tag.Rule != null)
            {
                throw new InvalidOperationException("Merging tags is not supported, if the removed tag contains a rule.");
            }
            
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
        
        var persistedTag = db.Tags.Include(t => t.Rule)
            .Where(t => t.Id == tag.Id)
            .Select(t => IncludeRule(t, ruleRepository)).Single();
        persistedTag.Value = tag.Value;
        ruleRepository.Delete(persistedTag.Rule);
        persistedTag.Rule = ruleRepository.Save(tag.Rule.ToRuleDao());
        db.SaveChanges();
        return tag;
    }

    public void DeleteTag(Guid id)
    {
        using var db = new DocumentContext { DbPath = _dbPath };
        var ruleRepository = new RuleRepository(db);
        var tag = db.Tags.Include(t => t.Rule)
            .Where(t => t.Id == id).Select(t => IncludeRule(t, ruleRepository)).Single();
        ruleRepository.Delete(tag.Rule);
        db.Tags.Remove(tag);
        db.SaveChanges();
    }
    
    private static TagDao IncludeRule(TagDao t, RuleRepository ruleRepository)
    {
        t.Rule = ruleRepository.Load(t.Rule);
        return t;
    }
}