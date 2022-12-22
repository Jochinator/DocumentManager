using DocumentManager;
using Microsoft.EntityFrameworkCore;

namespace DocumentManagerPersistence;

public class DocumentContext: DbContext
{
    public string DbPath { get; init; } = "Documents.db";

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => 
        optionsBuilder.UseSqlite($"Data Source={DbPath}");

    public DbSet<DocumentMetadataDao> Metadatas { get; set; }
    public DbSet<TagDao> Tags { get; set; }
}