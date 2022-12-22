namespace DocumentManager;

public class DocumentFile : IDisposable
{
    public DocumentFile(string fileExtension, Stream content)
    {
        FileExtension = fileExtension;
        Stream = content;
    }

    public Guid Id { get; set; }

    public string FileExtension { get; }
    public Stream Stream { get; }

    public void Dispose()
    {
        Stream.Dispose();
    }
}