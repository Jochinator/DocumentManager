using System.Reflection;
using DocumentManager;
using DocumentManager.TextExtractor;
using DocumentManagerModel;
using Microsoft.EntityFrameworkCore;

namespace DocumentManagerPersistence;

public class TagRepository
{
    private readonly string _dataRootFolder;
    private readonly string _dbPath;

    public TagRepository(PersistenceDefinitions definitions)
    {
        _dataRootFolder = definitions.DataRootFolder;
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
}