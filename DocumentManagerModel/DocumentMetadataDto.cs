using DocumentManagerModel;

namespace DocumentManager;

public class DocumentMetadataDto
{
    public Guid Id { get; set; }

    public string Title { get; set; } = "";

    public DateTime Date { get; set; } = DateTime.Now;

    public IEnumerable<TagDto> Tags { get; set; } = new List<TagDto>();

    public string SenderName { get; set; } = "";

    public bool Checked { get; set; } = false;

    public string FilePath { get; set; } = "";

    public string FileExtension { get; set; } = "";

    public string ContentType { get; set; } = "";
}