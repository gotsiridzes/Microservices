using AutoMapper;
using PlatformService.DataTransferObjects;
using PlatformService.Models;
using PlatformService.Protos;

namespace PlatformService.Profiles;

public class PlatformsProfile : Profile
{
    public PlatformsProfile()
    {
        CreateMap<Platform, PlatformReadDto>();
        CreateMap<PlatformCreateDto, Platform>();
        CreateMap<PlatformReadDto, PlatformPublishedDto>();
        CreateMap<Platform, GrpcPlatformModel>()
            .ForMember(
                dest => dest.PlatformId, 
                ops => ops.MapFrom(src => src.Id))
            .ForMember(
                dest => dest.Name,
                ops => ops.MapFrom(src => src.Name))
            .ForMember(
                dest => dest.Publisher,
                ops => ops.MapFrom(src => src.Publisher));
    }
}
