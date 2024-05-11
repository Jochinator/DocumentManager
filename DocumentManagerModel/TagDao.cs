namespace DocumentManager;

public class TagDao
{
    public Guid Id { get; set; }
    public string Value { get; set; } = "";

    public bool IsManualOnly { get; set; } = false;

    public IEnumerable<DocumentMetadataDao> Metadatas { get; set; }
}