using DocumentManagerModel.Rule;

namespace DocumentManager;

public class TagDao
{
    public Guid Id { get; set; }
    public string Value { get; set; } = "";
    public IEnumerable<DocumentMetadataDao> Metadatas { get; set; }
    public RuleDao? Rule { get; set; }
}