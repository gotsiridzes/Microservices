using AutoMapper;
using CommandService.DataTransferObjects;
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
            throw new System.NotImplementedException();
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
