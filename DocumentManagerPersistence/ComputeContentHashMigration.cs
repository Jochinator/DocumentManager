using System.Security.Cryptography;
using System.Text;
using DocumentManagerModel;
using Microsoft.EntityFrameworkCore;

namespace DocumentManagerPersistence;

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