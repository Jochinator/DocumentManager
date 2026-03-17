using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Messaging;

public class MessagingContextFactory : IDesignTimeDbContextFactory<MessagingContext>
{
    public MessagingContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<MessagingContext>()
            .UseSqlite("Data Source=messaging.db")
            .Options;
        return new MessagingContext(options);
    }
}