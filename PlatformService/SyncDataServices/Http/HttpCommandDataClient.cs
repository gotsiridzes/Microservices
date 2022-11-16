using Microsoft.Extensions.Configuration;
using PlatformService.DataTransferObjects;
using Serilog;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PlatformService.SyncDataServices.Http;

public class HttpCommandDataClient : ICommandDataClient
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public HttpCommandDataClient(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task SendPlatformToCommand(PlatformReadDto platform)
    {
        var httpContent = new StringContent(
            JsonSerializer.Serialize(platform),
            Encoding.UTF8,
            "application/json");

        var response = await _httpClient.PostAsync($"{_configuration["CommandService"]}/api/c/platforms/", httpContent);
        Log.Information($"Sending data to: {_configuration["CommandService"]}/api/c/platforms/");
        if (response.IsSuccessStatusCode)
        {
            Log.Information("Sync POST to Command Service is OK");
        }
        else
        {
            Log.Error("Error: Sync POST to Command Service is NOT OK, {0}", response.ReasonPhrase);
        }
    }
}
