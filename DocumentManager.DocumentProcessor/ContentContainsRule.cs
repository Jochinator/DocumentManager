namespace DocumentManager.DocumentProcessor;

internal class ContentContainsRule(string query) : IRule
{
    public bool Apply(DocumentMetadataDto metadata, string textContent)
    {
        return textContent.Contains(query, StringComparison.CurrentCultureIgnoreCase);
    }
}