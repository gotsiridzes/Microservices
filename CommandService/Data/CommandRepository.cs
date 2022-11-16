using CommandService.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CommandService.Data;

public class CommandRepository : ICommandRepository
{
    private readonly AppDbContext _dbContext;

    public CommandRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    #region Commands

    public void CreateCommand(int platformId, Command command)
    {
        if (command is null)
            throw new ArgumentNullException(nameof(command));
        else
        {
            command.PlatformId = platformId;
            _dbContext.Commands.Add(command);
        }
    }

    public Command GetCommand(int platformId, int commandId) =>
        _dbContext.Commands
        .Where(x => x.PlatformId == platformId && x.Id == commandId)
        .FirstOrDefault();

    public IEnumerable<Command> GetCommandsForPlatform(int platformId) => 
        _dbContext.Commands
        .Where(x => x.PlatformId == platformId);
    #endregion

    #region Platforms

    public void CreatePlatform(Platform platform)
    {
        if (platform is null)
            throw new ArgumentNullException(nameof(platform));
        else
            _dbContext.Platforms.Add(platform);
    }

    public IEnumerable<Platform> GetPlatforms() => _dbContext.Platforms.ToList();

    public bool PlatformExists(int platformId) => 
        _dbContext.Platforms
        .Any(x => x.Id == platformId);

    public bool ExternalPlatformExists(int externalPlatformId) => _dbContext.Platforms.Any(x => x.ExternalId == externalPlatformId);

    #endregion

    public bool SaveChanges() => _dbContext.SaveChanges() > 0;
}
