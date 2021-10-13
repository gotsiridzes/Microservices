using AutoMapper;
using CommandService.Data;
using CommandService.DataTransferObjects;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CommandService.Controllers
{
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
        public ActionResult<IEnumerable<CommandReadDto>> GetCommandsForPlatform(int platformId) =>
            _repository.PlatformExists(platformId) switch
            {
                true => Ok(_mapper.Map<IEnumerable<CommandReadDto>>(_repository.GetCommandsForPlatform(platformId))),
                false => NotFound()
            };

    }
}
