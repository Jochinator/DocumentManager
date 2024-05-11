using DocumentManager;

namespace DocumentManagerPersistence;

public class FilePersistence
{
    private readonly PersistenceDefinitions _definitions;

    public FilePersistence(PersistenceDefinitions definitions)
    {
        _definitions = definitions;
    }
    public string SaveFile(Guid id, DocumentFile file)
    {
        using (var fileStream = File.Create(GetStoragePath(id, file.FileExtension)))
        {
            file.Stream.Seek(0, SeekOrigin.Begin);
            file.Stream.CopyTo(fileStream);
        }

        return GetRelativePath(id, file.FileExtension);
    }

    private string GetRelativePath(Guid id, string fileExtension)
    {
        return Path.Combine(_definitions.DocumentFolder, id.ToString() + fileExtension);
    }

    public string GetFullFilePath(Guid id, string fileExtension)
    {
        return GetStoragePath(id, fileExtension);
    }

    private string GetStoragePath(Guid id, string fileExtension)
    {
        return Path.Combine(_definitions.DataRootFolder, GetRelativePath(id, fileExtension));
    }
}