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
    .Configure<GitConfig>(build.GetSection("Git"))
    .AddSingleton<ApexService>()
    .AddSingleton<LoggingService>()
    .AddSingleton<GitService>()
    .BuildServiceProvider();
    
    
var apexService = services.GetRequiredService<ApexService>();
var loggingService = services.GetRequiredService<LoggingService>();
var gitService = services.GetRequiredService<GitService>();
var apexData = await apexService.GetApexDataAsync();
var difference = await gitService.PushDataToGithub(apexData);
await loggingService.Log(difference);

