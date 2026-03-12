using System.ComponentModel.DataAnnotations.Schema;

namespace DocumentManagerModel;

public class DataMigrationErrorDao
{
    public Guid Id { get; set; }
    [ForeignKey("Migration")]
    public string MigrationName { get; set; } = "";
    public Guid DocumentId { get; set; }
    public string ErrorMessage { get; set; } = "";
}