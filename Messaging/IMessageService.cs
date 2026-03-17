namespace Messaging;

public interface IMessageService
{
    void SendMessage(string text, MessageSeverity severity);
    void SendMessage(MessageTextHandler text, MessageSeverity severity);
    IAsyncEnumerable<Message> Subscribe(CancellationToken ct);
    void Acknowledge(Guid id);
}