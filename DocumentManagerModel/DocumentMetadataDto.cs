namespace DocumentManager;

public class DocumentMetadataDto
{
    public Guid Id { get; set; }

    public string Title { get; set; } = "";

    public DateTime Date { get; set; } = DateTime.Now;

    public IEnumerable<string> Tags { get; set; } = new List<string>();

    public string SenderName { get; set; } = "";

    public bool Checked { get; set; } = false;

    public string FilePath { get; set; } = "";

    public string FileExtension { get; set; } = "";

    public string ContentType { get; set; } = "";
}