using DocumentManagerModel;

namespace DocumentManagerPersistence;

public class DeduplicateTagsMigration : IDataMigration
{
    private readonly DocumentRepository _documentRepository;
    private readonly TagRepository _tagRepository;

    public string Name => "DeduplicateTagsMigration";

    public DeduplicateTagsMigration(DocumentRepository documentRepository, TagRepository tagRepository)
    {
        _documentRepository = documentRepository;
        _tagRepository = tagRepository;
    }
    
    public void Migrate(DataMigrationDao migrationDao)
    {
        var duplicates = _tagRepository.GetTags().GroupBy(tag => tag.Value).Where(g => g.Count() > 1).ToList();
        
        foreach (var group in duplicates)
        {
            var winner = group.First();
            var losers = group.Skip(1).ToList();

            foreach (var loser in losers)
            {
                var documents 
                    = _documentRepository.GetAllDocuments()
                        .Where(dto => dto.Tags.Contains(loser))
                        .ToList();

                foreach (var doc in documents)
                {
                    var updatedTags = doc.Tags.Where(tag => tag.Id != loser.Id).ToList();

                    if (updatedTags.All(t => t.Value != winner.Value))
                    {
                        updatedTags.Add(winner);
                    }
                    
                    doc.Tags = updatedTags;
                    _documentRepository.UpdateMetadata(doc);
                }
            
                _tagRepository.DeleteTag(loser.Id);
            }
        }
    }
}