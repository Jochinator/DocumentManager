using DocumentManager;
using DocumentManagerModel;
using Microsoft.Extensions.Options;

namespace DocumentManagerPersistence;

public class FilesystemViewService
{
    private readonly PersistenceDefinitions _definitions;

    public FilesystemViewService(IOptions<PersistenceDefinitions> definitions)
    {
        _definitions = definitions.Value;
    }

    public void RegenerateView(IEnumerable<DocumentMetadataDto> documents)
    {
        var viewRoot = Path.Combine(_definitions.DataRootFolder, _definitions.ViewFolder);

        if (Directory.Exists(viewRoot))
            Directory.Delete(viewRoot, recursive: true);

        foreach (var document in documents)
        {
            AddDocumentToView(document);
        }
    }

    public void AddDocumentToView(DocumentMetadataDto document)
    {
        if (!_definitions.GenerateFilesystemView)
            return;

        var documentPath = Path.Combine(_definitions.DataRootFolder, document.FilePath);
        var fileName = Path.GetFileName(document.FilePath);

        CreateSymlink(document.Scope, "Kontakte", document.SenderName, fileName, documentPath);
        CreateSymlink(document.Scope, "Jahre", document.Date.Year.ToString(), fileName, documentPath);
        CreateSymlink(document.Scope, "Titel", null, fileName, documentPath);

        foreach (var tag in document.Tags)
        {
            CreateSymlink(document.Scope, "Tags", tag.Value, fileName, documentPath);
        }
    }

    public void UpdateDocumentInView(DocumentMetadataDto oldDocument, DocumentMetadataDto newDocument)
    {
        if (!_definitions.GenerateFilesystemView)
            return;

        RemoveDocumentFromView(oldDocument);
        AddDocumentToView(newDocument);
    }

    public void RemoveDocumentFromView(DocumentMetadataDto document)
    {
        if (!_definitions.GenerateFilesystemView)
            return;

        var fileName = Path.GetFileName(document.FilePath);

        DeleteSymlink(document.Scope, "Kontakte", document.SenderName, fileName);
        DeleteSymlink(document.Scope, "Jahre", document.Date.Year.ToString(), fileName);
        DeleteSymlink(document.Scope, "Titel", null, fileName);

        foreach (var tag in document.Tags)
        {
            DeleteSymlink(document.Scope, "Tags", tag.Value, fileName);
        }
    }

    private void CreateSymlink(string scope, string category, string? subFolder, string fileName, string documentPath)
    {
        var path = subFolder != null
            ? Path.Combine(_definitions.DataRootFolder, _definitions.ViewFolder, scope, category, subFolder)
            : Path.Combine(_definitions.DataRootFolder, _definitions.ViewFolder, scope, category);

        Directory.CreateDirectory(path);
        File.CreateSymbolicLink(Path.Combine(path, fileName), documentPath);
    }

    private void DeleteSymlink(string scope, string category, string? subFolder, string fileName)
    {
        var path = subFolder != null
            ? Path.Combine(_definitions.DataRootFolder, _definitions.ViewFolder, scope, category, subFolder)
            : Path.Combine(_definitions.DataRootFolder, _definitions.ViewFolder, scope, category);

        var symlinkPath = Path.Combine(path, fileName);
        if (File.Exists(symlinkPath))
            File.Delete(symlinkPath);
    }
}