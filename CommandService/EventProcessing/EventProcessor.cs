using AutoMapper;
using CommandService.Data;
using CommandService.DataTransferObjects;
using CommandService.Models;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System.Text.Json;

namespace CommandService.EventProcessing;

public enum EventType
{
    PlatformPublished,
    Undetermined
}

public class EventProcessor : IEventProcessor
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IMapper _mapper;

    public EventProcessor(
        IServiceScopeFactory scopeFactory,
        IMapper mapper)
    {
        _scopeFactory = scopeFactory;
        _mapper = mapper;
    }

    public void ProcessEvent(string message)
    {
        var eventType = DetermineEvent(message);
        switch (eventType)
        {
            case EventType.PlatformPublished:
                AddPlatform(message);
                break;
            default:
                break;
        }
    }

    private void AddPlatform(string platformPublishedMessage)
    {
        using (var scope = _scopeFactory.CreateScope())
        {
            var repository = scope.ServiceProvider.GetRequiredService<ICommandRepository>();

            var platformPublishedDto = JsonSerializer.Deserialize<PlatformPublishedDto>(platformPublishedMessage);

            try
            {
                var platform = _mapper.Map<Platform>(platformPublishedDto);
                if (!repository.ExternalPlatformExists(platform.Id))
                {
                    repository.CreatePlatform(platform);
                    repository.SaveChanges();
                    Log.Information("Platform Added");
                }
                else
                {
						Log.Information("Platform already exists");
                }
            }
            catch (System.Exception ex)
            {
                Log.Error(ex, "Could not add Platform object {0}");
            }
        }
    }

    private EventType DetermineEvent(string notificationMessage)
    {
        System.Console.WriteLine("Determining Event");
        var eventType = JsonSerializer.Deserialize<GenericEventDto>(notificationMessage);
        switch (eventType.Event)
        {
            case "Platform_Published":
                Log.Information("Platform_Published Event Detected");
                return EventType.PlatformPublished;
            default:
                Log.Information("Could not determine the event type");
                return EventType.Undetermined;
        }
    }
}
