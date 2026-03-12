using DocumentManager;
using DocumentManagerModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DocumentManagerPersistence;

public class DataMigrationService : IHostedService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public DataMigrationService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _ = Task.Run(() =>
        {
            using var scope = _scopeFactory.CreateScope();
            var migrations = scope.ServiceProvider.GetRequiredService<IEnumerable<IDataMigration>>();
            var documentRepository = scope.ServiceProvider.GetRequiredService<DocumentRepository>();

            foreach (var migration in migrations)
            {
                if (documentRepository.IsMigrationCompleted(migration.Name))
                    continue;

                var migrationDao = new DataMigrationDao
                {
                    Name = migration.Name,
                    StartedAt = DateTime.Now
                };

                var documents = documentRepository.GetAllDocuments();
                foreach (var doc in documents)
                {
                    try
                    {
                        migration.Migrate(doc);
                    }
                    catch (Exception e)
                    {
                        migrationDao.Errors.Add(new DataMigrationErrorDao
                        {
                            Id = Guid.NewGuid(),
                            MigrationName = migration.Name,
                            DocumentId = doc.Id,
                            ErrorMessage = e.Message
                        });
                    }
                }

                migrationDao.Completed = true;
                migrationDao.CompletedAt = DateTime.Now;
                documentRepository.SaveMigration(migrationDao);
            }
        }, cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}