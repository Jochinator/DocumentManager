using DocumentManager;

namespace DocumentManagerModel;

public interface IDataMigration
{
    string Name { get; }
    string? RequiredMigration => null;
    void Migrate(DataMigrationDao migrationDao, string dbPath);
}