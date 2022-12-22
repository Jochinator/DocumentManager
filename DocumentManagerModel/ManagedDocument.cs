namespace DocumentManager;

public class ManagedDocument
{
    public Guid Id { get; set; }
    public DocumentMetadataDao Metadata { get; }
    public DocumentFile File { get; }

    public ManagedDocument(DocumentMetadataDao metadata, DocumentFile file)
    {
        Metadata = metadata;
        File = file;
    }
}