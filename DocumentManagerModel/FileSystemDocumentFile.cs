using DocumentManagerModel;

namespace DocumentManager;

public class FileSystemDocumentFile : IDocumentFile
{
    private readonly string _sourcePath;
    private readonly PersistenceDefinitions _definitions;
    private readonly TextExtractor.TextExtractor _textExtractor = new TextExtractor.TextExtractor();

    public FileSystemDocumentFile(string sourcePath, PersistenceDefinitions definitions)
    {
        FileExtension = Path.GetExtension(sourcePath);
        _sourcePath = sourcePath;
        _definitions = definitions;
    }

    public string FileExtension { get; }

    public void CopyTo(string destinationPath)
    {
        File.Copy(_sourcePath, destinationPath);
    }

    public void HandleError()
    {
        var fileName = Path.GetFileName(_sourcePath);
        var failedFolder = Path.Combine(_definitions.DataRootFolder, _definitions.FailedFolder);
        var destinationPath = Path.Combine(failedFolder, fileName);
        File.Move(_sourcePath, destinationPath);
    }

    public string GetTextContent()
    {
        return _textExtractor.ExtractTextFromFile(_sourcePath);
    }

    public void Dispose() { }
}