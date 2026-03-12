using DocumentManager;
using DocumentManagerModel;

namespace DocumentManagerPersistence;

public class ScopeBasedFileStructureMigration : IDataMigration
{
    private readonly DocumentRepository _documentRepository;

    public string Name => "ScopeBasedFileStructureMigration";

    public ScopeBasedFileStructureMigration(DocumentRepository documentRepository)
    {
        _documentRepository = documentRepository;
    }

    public void Migrate(DocumentMetadataDto document)
    {
        _documentRepository.UpdateMetadata(document);
    }
}