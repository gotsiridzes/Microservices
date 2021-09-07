using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlatformService.Data
{
    public static class Initializer
    {
        public static void Seed(IApplicationBuilder builder)
        {
            using (var serviceScope = builder.ApplicationServices.CreateScope())
            {
                SeedData(serviceScope.ServiceProvider.GetService<AppDbContext>());
            }
        }

        private static void SeedData(AppDbContext dbContext)
        {
            if (!dbContext.Platforms.Any())
            {
                Console.WriteLine("Seeding Data ...");

                dbContext.Platforms.AddRange(
                    new Models.Platform() {Name = "DotNet", Cost = "Free", Publisher = "Microsoft" },
                    new Models.Platform() {Name = "YouTube", Cost = "Free", Publisher = "Google" },
                    new Models.Platform() {Name = "MacBook", Cost = "Free", Publisher = "Apple" }
                );
                dbContext.SaveChanges();
            }
            else
            {
                Console.WriteLine("In memory data is already inserted.");
            }
        }
    }
}
