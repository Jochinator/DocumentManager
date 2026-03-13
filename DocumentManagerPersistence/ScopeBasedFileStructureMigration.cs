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

    public void Migrate(DataMigrationDao migrationDao)
    {
        var documents = _documentRepository.GetAllDocuments();
        foreach (var doc in documents)
        {
            
            try
            {
                _documentRepository.UpdateMetadata(doc);
            }
            catch (Exception e)
            {
                migrationDao.Errors.Add(new DataMigrationErrorDao
                {
                    Id = Guid.NewGuid(),
                    MigrationName = Name,
                    DocumentId = doc.Id,
                    ErrorMessage = e.Message
                });
            }
        }
        
    }
}