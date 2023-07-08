using CommandService.AsyncDataServices;
using CommandService.Data;
using CommandService.EventProcessing;
using CommandService.SyncDataServices.Grpc;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Extensions.Logging;
using System;

namespace CommandService;

public class Startup
{
	public Startup(IConfiguration configuration)
	{
		Configuration = configuration;
	}

	public IConfiguration Configuration { get; }

	public void ConfigureServices(IServiceCollection services)
	{
		services.AddControllers();
		services.AddHostedService<MessageBusSubscriber>();
		services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
		services.AddDbContext<AppDbContext>(ops => ops.UseInMemoryDatabase("CommandInMemoryDb"));
		services.AddScoped<ICommandRepository, CommandRepository>();
		services.AddSingleton<IEventProcessor, EventProcessor>();
		services.AddScoped<IPlatformDataClient, PlatformDataClient>();

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

		services.AddSwaggerGen(c =>
		{
			c.SwaggerDoc("v1", new OpenApiInfo { Title = "CommandService", Version = "v1" });
		});
	}

	public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
	{
		if (env.IsDevelopment())
		{
			app.UseDeveloperExceptionPage();
			app.UseSwagger();
			app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "CommandService v1"));
		}

		//app.UseHttpsRedirection();

		app.UseRouting();

		app.UseAuthorization();

		app.UseEndpoints(endpoints =>
		{
			endpoints.MapControllers();
		});

		Initializer.Seed(app);
	}
}
