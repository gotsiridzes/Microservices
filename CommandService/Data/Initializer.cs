using CommandService.Models;
using CommandService.SyncDataServices.Grpc;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System.Collections.Generic;

namespace CommandService.Data;

public class Initializer
{
    public static void Seed(IApplicationBuilder applicationBuilder)
    {
        var serviceScope = applicationBuilder.ApplicationServices.CreateScope();
        var grpcClient = serviceScope.ServiceProvider.GetService<IPlatformDataClient>();

        var platforms = grpcClient.ReturnAllPlatforms();

        SeedData(serviceScope.ServiceProvider.GetService<ICommandRepository>(), platforms);
    }

    private static void SeedData(ICommandRepository repository, IEnumerable<Platform> platforms)
    {
        Log.Information("Seeding Platforms");
        //foreach (var platform in platforms)
        //{
        //    if (!repository.ExternalPlatformExists(platform.ExternalId))
        //    {
        //        repository.CreatePlatform(platform);
        //    }
        //    repository.SaveChanges();
        //}
    }
}
