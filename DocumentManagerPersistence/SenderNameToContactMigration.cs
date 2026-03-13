using DocumentManagerModel;

namespace DocumentManagerPersistence;

public class SenderNameToContactMigration : IDataMigration
{
    private readonly DocumentRepository _documentRepository;
    private readonly ContactRepository _contactRepository;

    public string Name => "SenderNameToContactMigration";

    public SenderNameToContactMigration(DocumentRepository documentRepository, ContactRepository contactRepository)
    {
        _documentRepository = documentRepository;
        _contactRepository = contactRepository;
    }

    public void Migrate(DataMigrationDao migrationDao)
    {
        
    }
}