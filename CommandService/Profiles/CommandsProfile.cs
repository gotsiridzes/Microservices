using AutoMapper;
using CommandService.DataTransferObjects;
using CommandService.Models;

namespace CommandService.Profiles
{
    public class CommandsProfile : Profile
    {
        public CommandsProfile()
        {
            CreateMap<Platform, PlatformReadDto>();
            CreateMap<Command, CommandReadDto>();
            CreateMap<CommandCreateDto, Command>();
            CreateMap<PlatformPublishedDto, Platform>()
                .ForMember(
                    dest => dest.ExternalId, 
                    ops => ops.MapFrom(src => src.Id));
        }
    }
}
