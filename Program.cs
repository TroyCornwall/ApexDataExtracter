using ApexDataExtracter.Config;
using ApexDataExtracter.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var configBuilder = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: true)
    .AddUserSecrets<Program>(true); //TODO: add env variables...

var build =  configBuilder.Build();
var services = new ServiceCollection()
    .Configure<ApexConfig>(build.GetSection("Apex"))
    .Configure<SlackConfig>(build.GetSection("Slack"))
    .AddSingleton<ApexService>()
    .AddSingleton<LoggingService>()
    .BuildServiceProvider();
    
    
var apexService = services.GetRequiredService<ApexService>();
var loggingService = services.GetRequiredService<LoggingService>();
var apexData = await apexService.GetApexDataAsync();
await loggingService.Log(apexData);


