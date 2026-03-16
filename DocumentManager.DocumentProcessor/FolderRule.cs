using DocumentManagerModel.Rule;

namespace DocumentManager.DocumentProcessor;

internal class FolderRule(RuleType ruleType, string ruleValue) : IRule
{
    public bool Apply(DocumentMetadataDto metadata, string textContent)
    {
        if (string.IsNullOrEmpty(metadata.ImportPath))
            return false;

        var parts = metadata.ImportPath.Split('/', '\\');
        var folders = parts[..^1];

        switch (ruleType)
        {
            case RuleType.DirectSuperFolderIs:
            {
                var directParent = folders.LastOrDefault();
                return directParent != null && 
                       string.Equals(directParent, ruleValue, StringComparison.CurrentCultureIgnoreCase);
            }
            case RuleType.SomeSuperFolderIs:
                return folders.Any(f => string.Equals(f, ruleValue, StringComparison.CurrentCultureIgnoreCase));
            default:
                throw new InvalidOperationException($"Rule type {ruleType} not supported");
        }
    }
}