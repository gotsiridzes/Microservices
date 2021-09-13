using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.Data;
using PlatformService.DataTransferObjects;
using PlatformService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlatformService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlatformsController : ControllerBase
    {
        private readonly IPlatformRepository _platformRepository;
        private readonly IMapper _mapper;

        public PlatformsController(IPlatformRepository platformRepository, IMapper mapper)
        {
            _platformRepository = platformRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public ActionResult<IEnumerable<PlatformReadDto>> GetPlatforms()
        {
            var platformItems = _platformRepository.GetPlatforms().ToList();
            return Ok(_mapper.Map<IEnumerable<PlatformReadDto>>(platformItems));
        }

        [HttpGet("{id}", Name = "GetPlatform")]
        public ActionResult<PlatformReadDto> GetPlatform(int id)
        {
            var platformItem = _platformRepository.GetPlatform(id);
            
            if (platformItem != null)
                return Ok(_mapper.Map<PlatformReadDto>(platformItem));
            
            return NotFound();
        }

        [HttpPost]
        public ActionResult<PlatformReadDto> CreatePlatform(PlatformCreateDto platformCreateDto)
        {
            var platformModel = _mapper.Map<Platform>(platformCreateDto);
            
            _platformRepository.CreatePlatform(platformModel);
            _platformRepository.SaveChanges();

            var platformReadDto =_mapper.Map<PlatformReadDto>(platformModel);

            return CreatedAtRoute(nameof(GetPlatform), new { Id = platformReadDto.Id }, platformReadDto);
        }
    }
}
