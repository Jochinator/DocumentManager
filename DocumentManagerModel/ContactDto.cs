using DocumentManagerModel.Rule;

namespace DocumentManager;

public class ContactDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
    public RuleDto? Rule { get; set; }
}