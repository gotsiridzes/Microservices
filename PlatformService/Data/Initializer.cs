using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;
using System.Linq;

namespace PlatformService.Data;

public static class Initializer
{
    public static void Seed(IApplicationBuilder builder, bool isProduction)
    {
        using (var serviceScope = builder.ApplicationServices.CreateScope())
        {
            SeedData(serviceScope.ServiceProvider.GetService<AppDbContext>(), isProduction);
        }
    }

    private static void SeedData(AppDbContext dbContext, bool isProduction)
    {
        if (isProduction)
        {
            Log.Information("Attempting to apply migrations ...");
            try
            {
                dbContext.Database.Migrate();
            }
            catch (Exception ex)
            {
                Log.Error("Exception occured while applying migrations:\n {0}", ex.ToString());
            }
        }

        if (!dbContext.Platforms.Any())
        {
            Log.Information("Seeding Data ...");

            dbContext.Platforms.AddRange(
                new Models.Platform() { Name = "DotNet", Cost = "Free", Publisher = "Microsoft" },
                new Models.Platform() { Name = "YouTube", Cost = "Free", Publisher = "Google" },
                new Models.Platform() { Name = "MacBook", Cost = "Free", Publisher = "Apple" }
            );
            dbContext.SaveChanges();
        }
        else
        {
            Log.Information("In memory data is already inserted.");
        }
    }
}
