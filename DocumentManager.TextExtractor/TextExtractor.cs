using UglyToad.PdfPig;

namespace DocumentManager.TextExtractor;

public class TextExtractor
{
    public string ExtractTextFromFile(string pathToFile)
    {
        using var document = PdfDocument.Open(pathToFile);
        return string.Join("", document.GetPages().Select(page => page.Text));
    }
}