using DocumentManagerModel.Rule;

namespace DocumentManager.DocumentProcessor;

internal static class RuleExtensions
{
    public static IRule ToRule(this RuleDto? rule)=> rule?.Type switch
    {
        null => new EmptyRule(),
        RuleType.ContentContains => new ContentContainsRule(rule.Value!),
        RuleType.DateBefore or RuleType.DateAfter => new DateRule(rule.Type, rule.Value!),
        RuleType.And or RuleType.Or or RuleType.Not => new BooleanRule(rule.Type, rule.Operands!.Select(dto => dto.ToRule())),
        RuleType.DirectSuperFolderIs or RuleType.SomeSuperFolderIs => new FolderRule(rule.Type, rule.Value!),
        RuleType.ScopeIs             => new ScopeRule(rule.Value!),
        _ => throw new InvalidOperationException($"Rule type {rule.Type} not supported")
    };
}