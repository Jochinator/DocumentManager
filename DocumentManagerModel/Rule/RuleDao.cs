namespace DocumentManagerModel.Rule;

public class RuleDao
{
    public Guid Id { get; set; }
    public Guid RuleId { get; set; }
    public RuleType RuleType { get; set; }
    public string? PredicateValue { get; set; }
    public ICollection<RuleDao>? Operands { get; set; }
    public RuleDao? Parent { get; set; }
}