namespace DocumentManager.DocumentProcessor;

internal class EmptyRule : IRule
{
    public bool Apply(DocumentMetadataDto metadata, string textContent)
    {
        return false;
    }
}