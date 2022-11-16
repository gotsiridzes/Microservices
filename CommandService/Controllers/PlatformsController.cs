using AutoMapper;
using CommandService.Data;
using CommandService.DataTransferObjects;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.Collections.Generic;

namespace CommandService.Controllers;

[ApiController]
[Route("api/c/[controller]")]
public class PlatformsController : ControllerBase
{
    private readonly ICommandRepository _repository;
    private readonly IMapper _mapper;

    public PlatformsController(ICommandRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    [HttpGet]
    public ActionResult<IEnumerable<PlatformReadDto>> GetPlatforms()
    {
        Log.Information("Getting Platforms from Command Service");
        var platformItems = _repository.GetPlatforms();
        return Ok(_mapper.Map<IEnumerable<PlatformReadDto>>(platformItems));
    }

    [HttpPost]
    public ActionResult TestInboundConnection()
    {
        Log.Information("Inbound POST at Command Service");

        return Ok("Inbound test of Platforms Controller");
    }
}
