using DocumentManager;

namespace DocumentManagerModel;

public interface IDataMigration
{
    string Name { get; }
    void Migrate(DataMigrationDao migrationDao);
}