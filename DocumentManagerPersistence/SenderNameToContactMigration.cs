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
        var groups = _documentRepository.GetAllDocuments().GroupBy(dto => dto.SenderName.Trim());
        
        foreach (var group in groups)
        {
            var contactName = group.Key;
            var contactDto = _contactRepository.GetOrCreate(contactName);
            
            foreach (var document in group)
            {
                document.Contact = contactDto;
                _documentRepository.UpdateMetadata(document);
            }
        }
    }
}