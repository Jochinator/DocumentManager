using System.Globalization;
using DocumentManagerModel.Rule;

namespace DocumentManager.DocumentProcessor;

internal class DateRule(RuleType ruleType, string ruleValue) : IRule
{
    public bool Apply(DocumentMetadataDto metadata, string textContent)
    {
        var date = DateTime.Parse(ruleValue, CultureInfo.InvariantCulture);
        if (ruleType == RuleType.DateBefore)
        {
            return metadata.Date < date.AddDays(1);
        }

        if (ruleType == RuleType.DateAfter)
        {
            return metadata.Date > date.AddDays(-1);
        }
        throw new InvalidOperationException($"Rule type {ruleType} not supported");
    }
}