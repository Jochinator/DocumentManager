using DocumentManager;
using Messaging;

namespace DocumentManagerModel;

public class FileSystemDocumentFile : IDocumentFile
{
    private readonly string _sourcePath;
    private readonly PersistenceDefinitions _definitions;
    private readonly IMessageService _messageService;
    private readonly DocumentManager.TextExtractor.TextExtractor _textExtractor = new DocumentManager.TextExtractor.TextExtractor();

    public FileSystemDocumentFile(string sourcePath, PersistenceDefinitions definitions, IMessageService messageService)
    {
        FileExtension = Path.GetExtension(sourcePath);
        _sourcePath = sourcePath;
        _definitions = definitions;
        _messageService = messageService;
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
        _messageService.SendMessage($"{fileName} konnte nicht importiert werden und wurde nach {failedFolder} verschoben.", MessageSeverity.Error);
    }

    public string GetTextContent()
    {
        return _textExtractor.ExtractTextFromFile(_sourcePath);
    }

    public void Dispose() { }
}