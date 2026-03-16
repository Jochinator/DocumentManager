using System.Security.Cryptography;
using System.Text;
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

        using var db = new DocumentContext();
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

public class ComputeContentHashMigration: IDataMigration
{
    public string RequiredMigration => "20260316154441_AddContentHash";
    public string Name => "ComputeContentHash"; 

    public void Migrate(DataMigrationDao migrationDao, string dbPath)
    {
        using (var db = new MigrationContext20260316154441(dbPath))
        {
            foreach (var metadata in db.Metadatas)
            {
                metadata.Hash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(metadata.TextContent)));
            }
            db.SaveChanges();
        }
    }
    
    internal class MigrationContext20260316154441 : DbContext
    {
        private readonly string _dbPath;

        public MigrationContext20260316154441(string dbPath)
        {
            _dbPath = dbPath;
        }
        
        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite($"Data Source={_dbPath}");
        
        public DbSet<DocumentMetadata20260316154441> Metadatas { get; set; }
    
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DocumentMetadata20260316154441>()
                .ToTable("Metadatas");
        }
    }

    internal class DocumentMetadata20260316154441
    {
        public Guid Id { get; set; }
        public string TextContent { get; set; } = "";
        public string? Hash { get; set; }
    }
}