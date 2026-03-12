using DocumentManager;
using DocumentManagerModel;
using Microsoft.Extensions.Options;

namespace DocumentManagerPersistence;

public class FilePersistence
{
    private readonly PersistenceDefinitions _definitions;

    public FilePersistence(IOptions<PersistenceDefinitions> definitions)
    {
        _definitions = definitions.Value;
    }
    
    public string SaveFile(IDocumentFile file, string fileName)
    {
        var uniqueFileName = EnsureUniqueFileName(fileName, file.FileExtension);
        string storagePath = GetStoragePath(uniqueFileName, file.FileExtension);
        file.CopyTo(storagePath);

        return GetRelativePath(uniqueFileName, file.FileExtension);
    }

    private string EnsureUniqueFileName(string fileName, string fileExtension)
    {
        if (!File.Exists(GetStoragePath(fileName, fileExtension)))
        { 
            return fileName;
        }

        var counter = 1;
        while (File.Exists(GetStoragePath($"{fileName}_{counter}", fileExtension)))
        {
            counter++;
        }
        return $"{fileName}_{counter}";
    }

    public string GetRelativePath(string fileName, string fileExtension)
    {
        return Path.Combine(_definitions.DocumentFolder, fileName + fileExtension);
    }

    private string GetStoragePath(string fileName, string fileExtension)
    {
        return Path.Combine(_definitions.DataRootFolder, GetRelativePath(fileName, fileExtension));
    }

    public IEnumerable<string> GetFilesToImport()
    {
        return Directory.GetFiles(GetImportFolder());
    }

    private string GetImportFolder()
    {
        return Path.Combine(_definitions.DataRootFolder, _definitions.ImportFolder);
    }

    public void RemoveFile(string file)
    {
        File.Delete(file);
    }
    
    public string CopyToNewName(string oldRelativePath, string newFileName, string fileExtension)
    {
        var uniqueFileName = EnsureUniqueFileName(newFileName, fileExtension);
        var newRelativePath = GetRelativePath(uniqueFileName, fileExtension);
    
        File.Copy(
            Path.Combine(_definitions.DataRootFolder, oldRelativePath), 
            Path.Combine(_definitions.DataRootFolder, newRelativePath)
        );
    
        return newRelativePath;
    }

    public void DeleteManagedFile(string relativePath)
    {
        File.Delete(Path.Combine(_definitions.DataRootFolder, relativePath));
    }
}