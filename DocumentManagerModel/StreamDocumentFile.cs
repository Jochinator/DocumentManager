namespace DocumentManager;

public class StreamDocumentFile : IDocumentFile
{
    private readonly Stream _stream;
    private readonly TextExtractor.TextExtractor _textExtractor = new TextExtractor.TextExtractor();

    public StreamDocumentFile(string fileExtension, Stream stream)
    {
        FileExtension = fileExtension;
        _stream = stream;
    }

    public string FileExtension { get; }

    public void CopyTo(string destinationPath)
    {
        _stream.Seek(0, SeekOrigin.Begin);
        using var fileStream = File.Create(destinationPath);
        _stream.CopyTo(fileStream);
    }

    public string GetTextContent()
    {
        return _textExtractor.ExtractTextFromStream(_stream);
    }

    public void Dispose() => _stream.Dispose();
}