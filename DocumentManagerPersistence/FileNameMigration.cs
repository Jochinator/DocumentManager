using DocumentManager;
using DocumentManagerModel;

namespace DocumentManagerPersistence;

public class FileNameMigration: IDataMigration
{
    private readonly DocumentRepository _documentRepository;
    
    public string Name => "FileNameMigration";
    
    public FileNameMigration(DocumentRepository documentRepository)
    {
        _documentRepository = documentRepository;
    }
    
    public void Migrate(DocumentMetadataDto document)
    {
        _documentRepository.UpdateMetadata(document);
    }
}