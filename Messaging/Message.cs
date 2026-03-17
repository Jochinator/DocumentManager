namespace Messaging;

public class Message
{
    public Guid Id { get; set; } 
    public IReadOnlyList<MessageSegment> Segments { get; set; } = [];
    public MessageSeverity Severity { get; set; }
    public DateTime CreatedAt { get; set; }
}