namespace DocumentManagerPersistence;

public class PersistenceDefinitions
{
    public PersistenceDefinitions()
    {
        DataRootFolder = Path.Combine("/home", "jochen", "documentManagerData");
        DocumentFolder = "documents";
        Directory.CreateDirectory(DataRootFolder);
        Directory.CreateDirectory(Path.Combine(DataRootFolder, "documents"));
        Directory.CreateDirectory(Path.Combine("wwwroot", "documents"));
    }
    public string DataRootFolder { get;}
    public string DocumentFolder { get;  }
}