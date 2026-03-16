namespace DocumentManagerModel.Rule;

public class RuleDto
{
    public RuleType Type { get; set; }
    public string? Value { get; set; }
    public IEnumerable<RuleDto>? Operands { get; set; }
}