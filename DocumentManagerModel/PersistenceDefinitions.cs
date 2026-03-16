namespace DocumentManagerModel;

public class PersistenceDefinitions
{
    public string DataRootFolder { get; init; } = "";
    public string DocumentFolder { get; init; } = "documents";
    public string ImportFolder { get; init; } = "imports";
    public string DeletedFolder { get; init; } = "deleted";
    public string FailedFolder { get; init; } = "failed";
    public string ViewFolder { get; init; } = "view";
    public string DbName { get; init; } = "Documents.db";
    public bool GenerateFilesystemView { get; init; } = false;
}