namespace Messaging;

public static class MessageMappings
{
    public static MessageDao ToDao(this Message message) => new()
    {
        Id = message.Id,
        Segments = message.Segments.Select(s => new MessageSegment { Text = s.Text, Url = s.Url }).ToList(),
        Severity = message.Severity,
        CreatedAt = message.CreatedAt
    };

    public static Message ToMessage(this MessageDao dao) => new()
    {
        Id = dao.Id,
        Segments = dao.Segments.Select(s => new Messaging.MessageSegment{Text= s.Text, Url = s.Url}).ToList(),
        Severity = dao.Severity,
        CreatedAt = dao.CreatedAt
    };
}