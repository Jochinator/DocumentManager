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
    
    public string SaveFile(IDocumentFile file, string fileName, string scope)
    {
        Directory.CreateDirectory(Path.Combine(_definitions.DataRootFolder, _definitions.DocumentFolder, scope));
        var uniqueFileName = EnsureUniqueFileName(fileName, file.FileExtension, scope);
        string storagePath = GetStoragePath(uniqueFileName, file.FileExtension, scope);
        file.CopyTo(storagePath);

        return GetRelativePath(scope, uniqueFileName, file.FileExtension);
    }

    private string EnsureUniqueFileName(string fileName, string fileExtension, string scope)
    {
        if (!File.Exists(GetStoragePath(fileName, fileExtension, scope)))
        { 
            return fileName;
        }

        var counter = 1;
        while (File.Exists(GetStoragePath($"{fileName}_{counter}", fileExtension, scope)))
        {
            counter++;
        }
        return $"{fileName}_{counter}";
    }

    internal string GetRelativePath(string scope, string fileName, string fileExtension)
    {
        return Path.Combine(_definitions.DocumentFolder, scope, fileName + fileExtension);
    }

    private string GetStoragePath(string fileName, string fileExtension, string scope)
    {
        return Path.Combine(_definitions.DataRootFolder, GetRelativePath(scope, fileName, fileExtension));
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
    
    public string CopyToNewName(string scope, string oldRelativePath, string newFileName, string fileExtension)
    {
        Directory.CreateDirectory(Path.Combine(_definitions.DataRootFolder, _definitions.DocumentFolder, scope));
        var uniqueFileName = EnsureUniqueFileName(newFileName, fileExtension, scope);
        var newRelativePath = GetRelativePath(scope, uniqueFileName, fileExtension);
    
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