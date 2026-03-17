using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Messaging;

public static class MessagingExtensions
{
    public static IServiceCollection AddMessaging(this IServiceCollection services, Action<MessagingOptions> configure)
    {
        var options = new MessagingOptions();
        configure(options);
        
        services.AddDbContextFactory<MessagingContext>(o => 
            o.UseSqlite($"Data Source={options.DbPath}"));
        
        services.AddSingleton<IMessageService, MessageService>();
        return services;
    }
}