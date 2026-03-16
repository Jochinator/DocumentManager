using DocumentManagerModel.Rule;

namespace DocumentManagerModel;

public class TagDto
{
    public Guid Id { get; set; }
    public string Value { get; set; }
    public RuleDto? Rule { get; set; }
}