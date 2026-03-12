using DocumentManager;

namespace DocumentManagerModel;

public static class FileNameGenerationExtensions
{
    public static string GenerateFileName(this DocumentMetadataDao dao)
        => GenerateFileName(dao.Title, dao.SenderName, dao.Date);
    
    public static string GenerateFileName(this DocumentMetadataDto dto)
        => GenerateFileName(dto.Title, dto.SenderName, dto.Date);
    
    private static string GenerateFileName(string title, string senderName, DateTime date)
    {
        var datePart = date.ToString("yyMM");
        
        var contactPart = RemoveInvalidChars(senderName).Replace(" ", "");
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