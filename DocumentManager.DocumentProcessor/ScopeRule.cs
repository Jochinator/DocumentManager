namespace DocumentManager.DocumentProcessor;

internal class ScopeRule(string ruleValue) : IRule
{
    public bool Apply(DocumentMetadataDto metadata, string textContent)
    {
        return string.Equals(metadata.Scope, ruleValue, StringComparison.CurrentCultureIgnoreCase);
    }
}