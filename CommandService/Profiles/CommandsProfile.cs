using AutoMapper;
using CommandService.DataTransferObjects;
using CommandService.Models;
using PlatformService.Protos;

namespace CommandService.Profiles;

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
        CreateMap<GrpcPlatformModel, Platform>()
            .ForMember(
                dest => dest.ExternalId,
                ops => ops.MapFrom(src => src.PlatformId))
            .ForMember(
                dest => dest.Name,
                ops => ops.MapFrom(src => src.Name))
            .ForMember(
                dest => dest.Commands,
                ops => ops.Ignore());
    }
}
