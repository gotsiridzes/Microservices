using AutoMapper;
using CommandService.Data;
using CommandService.DataTransferObjects;
using CommandService.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Serilog;

namespace CommandService.Controllers;

[ApiController]
[Route("api/c/platforms/{platformId}/[controller]")]
public class CommandsController : ControllerBase
{
	private readonly ICommandRepository _repository;
	private readonly IMapper _mapper;

	public CommandsController(ICommandRepository repository, IMapper mapper)
	{
		_repository = repository;
		_mapper = mapper;
	}

	[HttpGet]
	public ActionResult<IEnumerable<CommandReadDto>> GetCommandsForPlatform(int platformId)
	{
		Log.Information("Get Commands For Platform");
		return _repository.PlatformExists(platformId) switch
		{
			true => Ok(_mapper.Map<IEnumerable<CommandReadDto>>(_repository.GetCommandsForPlatform(platformId))),
			false => NotFound()
		};
	}

	[HttpGet("{commandId}", Name = "GetCommandForPlatform")]
	public ActionResult<CommandReadDto> GetCommandForPlatform(int platformId, int commandId)
	{
		Log.Information("Get Command For Platform");

		if (!_repository.PlatformExists(platformId))
		{
			return NotFound();
		}
		else
		{
			//TODO should be refactored
			var command = _repository.GetCommand(platformId, commandId);

			return command switch
			{
				null => NotFound(),
				_ => Ok(_mapper.Map<CommandReadDto>(command))
			};
		}
	}

	[HttpPost]
	public ActionResult<CommandReadDto> CreateCommandForPlatform(int platformId, CommandCreateDto commandDto)
	{
		Log.Information("Create Command For Platform");

		if (!_repository.PlatformExists(platformId))
		{
			return NotFound();
		}

		var command = _mapper.Map<Command>(commandDto);

		_repository.CreateCommand(platformId, command);
		_repository.SaveChanges();

		var commandReadDto = _mapper.Map<CommandReadDto>(command);
		return CreatedAtRoute(nameof(GetCommandForPlatform), new
		{
			platformId = platformId,
			commandId = commandReadDto.Id,
			commandReadDto
		});
	}
}
