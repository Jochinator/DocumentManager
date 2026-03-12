using DocumentManagerModel;
using Microsoft.Extensions.Options;

namespace DocumentManager.DocumentProcessor;

public class FileSystemDocumentFileFactory
{
    private readonly PersistenceDefinitions _definitions;

    public FileSystemDocumentFileFactory(IOptions<PersistenceDefinitions> definitions)
    {
        _definitions = definitions.Value;
    }

    public FileSystemDocumentFile Create(string sourcePath)
    {
        return new FileSystemDocumentFile(sourcePath, _definitions);
    }
}