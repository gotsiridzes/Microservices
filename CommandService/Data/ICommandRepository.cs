using CommandService.Models;
using System.Collections.Generic;

namespace CommandService.Data;

public interface ICommandRepository
{
    bool SaveChanges();
    
    #region Platforms

    IEnumerable<Platform> GetPlatforms();
    void CreatePlatform(Platform platform);
    bool PlatformExists(int platformId);
    bool ExternalPlatformExists(int externalPlatformId);

    #endregion

    #region Commands

    IEnumerable<Command> GetCommandsForPlatform(int platformId);
    Command GetCommand(int platformId, int commandId);
    void CreateCommand(int platformId, Command command);
    
    #endregion

}
