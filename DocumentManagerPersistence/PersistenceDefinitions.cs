namespace DocumentManagerPersistence;

public class PersistenceDefinitions
{
    public PersistenceDefinitions()
    {
        DataRootFolder = Path.Combine("/home", "jochen", "documentManagerData");
        DocumentFolder = "documents";
        ImportFolder = "imports";
        DeletedFolder = "deleted";
        Directory.CreateDirectory(DataRootFolder);
        Directory.CreateDirectory(Path.Combine(DataRootFolder, DocumentFolder));
        Directory.CreateDirectory(Path.Combine(DataRootFolder, ImportFolder));
        Directory.CreateDirectory(Path.Combine(DataRootFolder, DeletedFolder));
        Directory.CreateDirectory(Path.Combine("wwwroot", "documents"));
    }

    public string ImportFolder { get; set; }

    public string DataRootFolder { get;}
    public string DocumentFolder { get;  }
    public string DeletedFolder { get; set; }
}