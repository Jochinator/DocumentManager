using DocumentManagerModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace DocumentManagerPersistence;

public class DataMigrationService
{
    private readonly DocumentRepository _documentRepository;
    private readonly FilesystemViewService _filesystemViewService;
    private readonly PersistenceDefinitions _definitions;
    private List<IDataMigration> _migrations = [new ComputeContentHashMigration()];
    private readonly string _dbPath;

    public DataMigrationService(DocumentRepository documentRepository, IOptions<PersistenceDefinitions> definitions, FilesystemViewService filesystemViewService)
    {
        _documentRepository = documentRepository;
        _filesystemViewService = filesystemViewService;
        _definitions = definitions.Value;
        _dbPath = Path.Combine(_definitions.DataRootFolder, _definitions.DbName);
    }
    
    public void Init()
    {
        Migrate();
        if (!_definitions.GenerateFilesystemView)
            return;
        var documents = _documentRepository.GetAllDocuments();
        _filesystemViewService.RegenerateView(documents);
    }

    private void Migrate()
    {
        foreach (var migration in _migrations)
        {
            if (_documentRepository.IsMigrationCompleted(migration.Name))
                continue;
            
            PerformRequiredDatabaseMigrations(migration);

            var migrationDao = new DataMigrationDao
            {
                Name = migration.Name,
                StartedAt = DateTime.Now
            };

            migration.Migrate(migrationDao, _dbPath);
            migrationDao.Completed = true;
            migrationDao.CompletedAt = DateTime.Now;
            _documentRepository.SaveMigration(migrationDao);
        }

        using var db = new DocumentContext{ DbPath = _dbPath };
        db.Database.Migrate();
    }

    private void PerformRequiredDatabaseMigrations(IDataMigration migration)
    {
        using var db = new DocumentContext
            { DbPath = _dbPath };
        if (migration.RequiredMigration == null) return;
        if (db.Database.GetPendingMigrations().Contains(migration.RequiredMigration))
        {
            db.Database.Migrate(migration.RequiredMigration);
        }
    }
}