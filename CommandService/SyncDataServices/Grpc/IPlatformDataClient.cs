using CommandService.Models;
using System.Collections.Generic;

namespace CommandService.SyncDataServices.Grpc
{
    public interface IPlatformDataClient
    {
        IEnumerable<Platform> ReturnAllPlatforms();
    }
}
