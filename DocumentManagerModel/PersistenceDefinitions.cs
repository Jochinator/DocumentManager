namespace DocumentManagerModel;

public class PersistenceDefinitions
{
    public string DataRootFolder { get; init; } = "";
    public string DocumentFolder { get; init; } = "documents";
    public string ImportFolder { get; init; } = "imports";
    public string DeletedFolder { get; init; } = "deleted";
    public string FailedFolder { get; init; } = "failed";
}