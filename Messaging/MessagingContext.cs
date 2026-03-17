using Microsoft.EntityFrameworkCore;

namespace Messaging;

public class MessagingContext : DbContext
{
    public MessagingContext(DbContextOptions<MessagingContext> options) : base(options) { }
    
    public DbSet<MessageDao> Messages { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MessageDao>()
            .OwnsMany(m => m.Segments, b => b.ToJson());
    }
}