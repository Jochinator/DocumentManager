namespace DocumentManager;

public interface IDocumentFile : IDisposable
{
    string FileExtension { get; }
    void CopyTo(string destinationPath);
    string GetTextContent();
}