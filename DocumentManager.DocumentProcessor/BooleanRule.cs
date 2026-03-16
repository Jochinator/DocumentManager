using DocumentManagerModel.Rule;

namespace DocumentManager.DocumentProcessor;

public class BooleanRule(RuleType ruleType, IEnumerable<IRule> ruleOperands) : IRule
{
    public bool Apply(DocumentMetadataDto metadata, string textContent) =>
        ruleType switch
        {
            RuleType.And => ruleOperands.All(rule => rule.Apply(metadata, textContent)),
            RuleType.Or => ruleOperands.Any(rule => rule.Apply(metadata, textContent)),
            RuleType.Not => ruleOperands.All(rule => !rule.Apply(metadata, textContent)),
            _ => throw new InvalidOperationException($"Rule type {ruleType} not supported")
        };
}