using PlatformService.Models;
using System.Collections.Generic;

namespace PlatformService.Data;

public interface IPlatformRepository
{
    bool SaveChanges();
    IEnumerable<Platform> GetPlatforms();
    Platform GetPlatform(int id);
    void CreatePlatform(Platform platform);
}
