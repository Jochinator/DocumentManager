namespace Messaging;

public class Message
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public IReadOnlyList<MessageSegment> Segments { get; set; } = [];
    public MessageSeverity Severity { get; set; }
    public DateTime CreatedAt { get; set; }
}