namespace DocumentManager;

public class DocumentMetadataDao
{
    public Guid Id { get; set; }

    public string Title { get; set; } = "";

    public DateTime Date { get; set; } = DateTime.Now;

    public List<TagDao> Tags { get; set; } = new List<TagDao>();

    public string SenderName { get; set; } = "";

    public bool Checked { get; set; } = false;

    public string FilePath { get; set; } = "";

    public string FileExtension { get; set; } = "";

    public string ContentType { get; set; } = "";
}