using System.Net.ServerSentEvents;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Messaging;

public static class MessageEndpoints
{
    public static void MapMessageEndpoints(this WebApplication app)
    {
        app.MapGet("/api/messages/stream", ([FromServices] IMessageService messageService, CancellationToken ct) =>
        {
            var jsonOptions = new JsonSerializerOptions 
            { 
                Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) },
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            var messages = messageService.Subscribe(ct)
                .Select(m => new SseItem<string>(JsonSerializer.Serialize(m, jsonOptions)));
            return TypedResults.ServerSentEvents(messages);
        });
        
        app.MapPost("/api/messages/{id:guid}/acknowledge", 
            ([FromRoute] Guid id, [FromServices] IMessageService messageService) =>
            {
                messageService.Acknowledge(id);
                return Results.Ok();
            });
    }
}