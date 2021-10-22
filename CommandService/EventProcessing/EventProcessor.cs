using AutoMapper;
using CommandService.Data;
using CommandService.DataTransferObjects;
using CommandService.Models;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;

namespace CommandService.EventProcessing
{
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
                        System.Console.WriteLine("Platform Added");
                    }
                    else
                    {
                        System.Console.WriteLine("Platform already exists");
                    }
                }
                catch (System.Exception ex)
                {
                    System.Console.WriteLine("Could not add Platform object {0}", ex);
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
                    System.Console.WriteLine("Platform_Published Event Detected");
                    return EventType.PlatformPublished;
                default:
                    System.Console.WriteLine("Could not determine the event type");
                    return EventType.Undetermined;
            }
        }
    }
}
