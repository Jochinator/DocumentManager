namespace DocumentManagerModel;

public class TagDto
{
    public Guid Id { get; set; }
    public string Value { get; set; }
    public bool IsManualOnly { get; set; }
}