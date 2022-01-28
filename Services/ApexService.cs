using ApexDataExtracter.Config;
using ApexDataExtracter.Models.Apex;
using Flurl.Http;
using Microsoft.Extensions.Options;

namespace ApexDataExtracter.Services;

public class ApexService
{
    private readonly ApexConfig _config;

    public ApexService(IOptions<ApexConfig> config)
    {
        _config = config.Value;
    }
    
    public async Task<ApexData> GetApexDataAsync()
    {
        var url = $"http://{_config.Hostname}/cgi-bin/status.json";
        var json  = await url.GetJsonAsync<Status>();
        return new ApexData(json);
    }
}