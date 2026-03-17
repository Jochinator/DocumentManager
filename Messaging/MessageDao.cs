namespace Messaging;

public class MessageDao
{
    public Guid Id { get; set; }
    public List<MessageSegment> Segments { get; set; } = [];
    public MessageSeverity Severity { get; set; }
    public DateTime CreatedAt { get; set; }
}