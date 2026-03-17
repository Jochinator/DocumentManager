using DocumentManagerModel;
using Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace DocumentManagerPersistence;

public class DataMigrationService
{
    private readonly DocumentRepository _documentRepository;
    private readonly FilesystemViewService _filesystemViewService;
    private readonly IMessageService _messageService;
    private readonly PersistenceDefinitions _definitions;
    private readonly List<IDataMigration> _migrations = [new ComputeContentHashMigration()];
    private readonly string _dbPath;

    public DataMigrationService(DocumentRepository documentRepository, IOptions<PersistenceDefinitions> definitions, FilesystemViewService filesystemViewService, IMessageService messageService)
    {
        _documentRepository = documentRepository;
        _filesystemViewService = filesystemViewService;
        _messageService = messageService;
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
        using (var dbForInitialMigration = new DocumentContext{ DbPath = _dbPath })
        {
            MakeSureMigrationIsAppliedBeforeContinue("20260312105307_AddDataMigrationTables");
        }
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
        var requiredMigration = migration.RequiredMigration;
        MakeSureMigrationIsAppliedBeforeContinue(requiredMigration);
    }

    private void MakeSureMigrationIsAppliedBeforeContinue(string? requiredMigration)
    {
        using var db = new DocumentContext
            { DbPath = _dbPath };
        if (requiredMigration == null) return;
        if (db.Database.GetPendingMigrations().Contains(requiredMigration))
        {   
            _messageService.SendMessage($"Migration {requiredMigration} wird ausgeführt...", MessageSeverity.Debug);
            db.Database.Migrate(requiredMigration);
            _messageService.SendMessage($"Migration {requiredMigration} abgeschlossen.", MessageSeverity.Debug);
        }
    }
}