using System.Threading.Channels;

namespace Messaging;

public class MessageService: IMessageService
{
    private readonly List<Channel<Message>> _subscribers = new();
    private readonly List<Message> _buffer = new();
    private readonly Lock _lock = new();

    public void SendMessage(string text, MessageSeverity severity)
    { 
        SendMessage($"{text}", severity);
    }

    public void SendMessage(MessageTextHandler text, MessageSeverity severity)
    {
        var message = new Message 
        { 
            Segments = text.Build(),
            Severity = severity,
            CreatedAt = DateTime.Now
        };
        lock (_lock)
        {
            if (_subscribers.Count == 0)
                _buffer.Add(message);
            else
                foreach (var channel in _subscribers)
                    channel.Writer.TryWrite(message);
        }
    }

    public IAsyncEnumerable<Message> Subscribe(CancellationToken ct)
    {
        var channel = Channel.CreateUnbounded<Message>();
        lock (_lock)
        {
            foreach (var message in _buffer)
                channel.Writer.TryWrite(message);
            _buffer.Clear();
            _subscribers.Add(channel);
        }

        ct.Register(() =>
        {
            lock (_lock)
            {
                _subscribers.Remove(channel);
                channel.Writer.Complete();
            }
        });

        return channel.Reader.ReadAllAsync(ct);
    }
}