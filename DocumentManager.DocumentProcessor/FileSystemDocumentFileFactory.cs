using DocumentManagerModel;
using Messaging;
using Microsoft.Extensions.Options;

namespace DocumentManager.DocumentProcessor;

public class FileSystemDocumentFileFactory
{
    private readonly PersistenceDefinitions _definitions;
    private readonly IMessageService _messageService;

    public FileSystemDocumentFileFactory(IOptions<PersistenceDefinitions> definitions, IMessageService messageService)
    {
        _definitions = definitions.Value;
    }

    public FileSystemDocumentFile Create(string sourcePath)
    {
        return new FileSystemDocumentFile(sourcePath, _definitions, _messageService);
    }
}