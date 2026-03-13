using DocumentManager;
using DocumentManagerModel;
using Microsoft.EntityFrameworkCore;

namespace DocumentManagerPersistence;

public class DocumentContext: DbContext
{
    public string DbPath { get; init; } = "Documents.db";

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => 
        optionsBuilder.UseSqlite($"Data Source={DbPath}");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<TagDao>()
            .HasIndex(t => t.Value)
            .IsUnique();
    }

    public DbSet<DocumentMetadataDao> Metadatas { get; set; }
    public DbSet<TagDao> Tags { get; set; }
    public DbSet<DataMigrationDao> DataMigrations { get; set; }
    public DbSet<DataMigrationErrorDao> DataMigrationErrors { get; set; }
}