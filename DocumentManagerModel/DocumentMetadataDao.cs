namespace DocumentManager;

public class DocumentMetadataDao
{
    public Guid Id { get; set; }

    public string Title { get; set; } = "";

    public string TextContent { get; set; }

    public DateTime Date { get; set; } = DateTime.Now;

    public List<TagDao> Tags { get; set; } = new List<TagDao>();
    
    public ContactDao? Contact { get; set; }

    public bool Checked { get; set; } = false;

    public string FilePath { get; set; } = "";

    public string FileExtension { get; set; } = "";

    public string ContentType { get; set; } = "";
    
    public string Scope { get; set; } = "default";
    public string Hash { get; set; } = "";
}