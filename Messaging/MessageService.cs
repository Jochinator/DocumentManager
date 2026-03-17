using System.Threading.Channels;
using Microsoft.EntityFrameworkCore;

namespace Messaging;

public class MessageService : IMessageService
{
    private readonly List<Channel<Message>> _subscribers = new();
    private readonly List<Message> _buffer = new();
    private readonly Lock _lock = new();
    private readonly IDbContextFactory<MessagingContext> _dbContextFactory;

    public MessageService(IDbContextFactory<MessagingContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
        using var db = dbContextFactory.CreateDbContext();
        db.Database.Migrate();
    }

    public void SendMessage(string text, MessageSeverity severity)
    {
        SendMessage($"{text}", severity);
    }

    public void SendMessage(MessageTextHandler text, MessageSeverity severity)
    {
        var message = new Message
        {
            Id = Guid.NewGuid(),
            Segments = text.Build().ToList(),
            Severity = severity,
            CreatedAt = DateTime.Now
        };

        if (severity is MessageSeverity.Warning or MessageSeverity.Error)
        {
            using var db = _dbContextFactory.CreateDbContext();
            db.Messages.Add(message.ToDao());
            db.SaveChanges();
        }

        lock (_lock)
        {
            if (_subscribers.Count == 0 && severity is MessageSeverity.Info or MessageSeverity.Debug)
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
            using var db = _dbContextFactory.CreateDbContext();
            var unacknowledged = db.Messages
                .OrderBy(m => m.CreatedAt)
                .ToList();
            
            foreach (var dao in unacknowledged)
                channel.Writer.TryWrite(dao.ToMessage());

            // Info/Debug aus Buffer
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

    public void Acknowledge(Guid id)
    {
        using var db = _dbContextFactory.CreateDbContext();
        var message = db.Messages.Find(id);
        if (message == null) return;
        db.Messages.Remove(message);
        db.SaveChanges();
    }
}