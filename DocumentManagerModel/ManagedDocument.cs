namespace DocumentManager;

public class ManagedDocument
{
    public Guid Id { get; set; }
    public DocumentMetadataDao Metadata { get; }
    public IDocumentFile File { get; }

    public ManagedDocument(DocumentMetadataDao metadata, IDocumentFile file)
    {
        Metadata = metadata;
        File = file;
    }
}