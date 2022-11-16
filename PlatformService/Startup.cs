using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using PlatformService.AsyncDataServices;
using PlatformService.Data;
using PlatformService.SyncDataServices.Grpc;
using PlatformService.SyncDataServices.Http;
using Serilog;
using Serilog.Extensions.Logging;
using System;

namespace PlatformService;

public class Startup
{
    public IConfiguration Configuration { get; }
    public IWebHostEnvironment Environment { get; }

    public Startup(IConfiguration configuration, IWebHostEnvironment environment)
    {
        Configuration = configuration;
        Environment = environment;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        if (Environment.IsProduction())
        {
            Console.WriteLine("Using SQL Server Database");
            services.AddDbContext<AppDbContext>(ops => ops.UseSqlServer(Configuration.GetConnectionString("Platform")));
        }
        else
        {
            Console.WriteLine("Using InMemory Database");
            services.AddDbContext<AppDbContext>(ops => ops.UseInMemoryDatabase("Platform"));
        }

        services.AddScoped<IPlatformRepository, PlatformRepository>();
        services.AddHttpClient<ICommandDataClient, HttpCommandDataClient>();
        services.AddSingleton<IMessageBusClient,  MessageBusClient>();
        services.AddGrpc();
        services.AddControllers();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "PlatformService", Version = "v1" });
        });
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        Console.WriteLine("Command Service Endpoint: {0}", Configuration["CommandService"]);

		var loggerConfig = new LoggerConfiguration()
			.Enrich.FromLogContext()
			.WriteTo.Console()
			//.WriteTo.Async(a => a.RollingFile("logs/{Date}.txt", fileSizeLimitBytes: 10485760))
			.WriteTo.Async(a => a.File("logs/.log", rollingInterval: RollingInterval.Day))
			.MinimumLevel.Information()
			.ReadFrom.Configuration(Configuration);

			Log.Logger = loggerConfig.CreateLogger();

			services.AddLogging(v => v.AddSerilog(Log.Logger));

			// Passing a `null` logger to `SerilogLoggerFactory` results in disposal via
			// `Log.CloseAndFlush()`, which additionally replaces the static logger with a no-op.
			var loggerFactory = new SerilogLoggerFactory(null, true);

			services.AddSingleton<ILoggerFactory>(loggerFactory);
			services.AddLogging(v => v.AddSerilog(Log.Logger));
		}

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "PlatformService v1"));
        }

        //app.UseHttpsRedirection();

        app.UseRouting();

        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapGrpcService<GrpcPlatformService>();
            
            endpoints.MapGet("/protos/platforms.proto", async context =>
            {
                await context.Response.WriteAsync(System.IO.File.ReadAllText("Protos/platforms.proto")); // serving contract
            });
        });

        Initializer.Seed(app, env.IsProduction());
    }
}
