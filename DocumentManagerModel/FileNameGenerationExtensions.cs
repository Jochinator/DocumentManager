using DocumentManager;

namespace DocumentManagerModel;

public static class FileNameGenerationExtensions
{
    public static string GenerateFileName(this DocumentMetadataDao dao)
        => GenerateFileName(dao.Title, dao.Contact?.Name ?? "", dao.Date);

    private static string GenerateFileName(string title, string contactName, DateTime date)
    {
        var datePart = date.ToString("yyMM");
        
        var contactPart = RemoveInvalidChars(contactName).Replace(" ", "");
        contactPart = contactPart.Length > 10 ? contactPart[..10] : contactPart;
        
        var titlePart = RemoveInvalidChars(title);
        
        var fileName = $"{datePart}_{contactPart}_{titlePart}";
        fileName = fileName.Length > 200 ? fileName[..200] : fileName;
        
        return fileName;
    }
    
    private static string RemoveInvalidChars(string input)
    {
        var invalidChars = new[] { '/', '\\', ':', '*', '?', '"', '<', '>', '|' };
        return new string(input.Where(c => !invalidChars.Contains(c)).ToArray());
    }
}