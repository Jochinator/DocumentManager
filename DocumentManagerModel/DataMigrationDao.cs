using System.ComponentModel.DataAnnotations;

namespace DocumentManagerModel;

public class DataMigrationDao
{
    [Key]
    public string Name { get; set; } = "";
    public bool Completed { get; set; } = false;
    public DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public List<DataMigrationErrorDao> Errors { get; set; } = new();
}