using ApexDataExtracter.Config;
using ApexDataExtracter.Models.Apex;
using Flurl.Http;
using Microsoft.Extensions.Options;

namespace ApexDataExtracter.Services;

public class LoggingService
{
    private readonly SlackConfig _config;
    public LoggingService(IOptions<SlackConfig> config)
    {
        _config = config.Value;
    }
    public async Task Log(ApexData apexData)
    {
        var logMessage = ComposeLogs(apexData);
        ConsoleLog(apexData, logMessage);
        await SlackLog(apexData, logMessage);
    }

    private string ComposeLogs(ApexData apexData)
    {
       return $"PH: {apexData.Ph}\n" +
              $"Temp: {apexData.Temp}\n" +
              $"Alk: {apexData.Alk}\n" +
              $"Calcium: {apexData.Calc}\n" +
              $"Magnesium: {apexData.Mg}\n" +
              $"ATO: {apexData.ATO?.status[2]} - {apexData.ATO?.status[0]}\n" +
              $"   ATO Low Underwater: {apexData.ATOLow}\n" +
              $"   ATO High Underwater {apexData.ATOHigh}";
    }

    private void ConsoleLog(ApexData apexData, string logMesssage)
    {
        Console.WriteLine($"System: {apexData.ApexName} as at {apexData.Timestamp:f}\n" + logMesssage);
    }
    
    private async Task SlackLog(ApexData apexData, string logMesssage)
    {
        var attachement =
            (apexData.ATO?.status[2] == "OK") ? null : new
            {
                color =  "#fc2600",
                text =  "ATO IS FUCKED",
            };
        
        await _config.WebhookUrl
            .PostJsonAsync(new
            {
                blocks = new []{
                new   {
                        type = "header",
                        text = new {
                            type = "plain_text",
                            text =  $"System: {apexData.ApexName} as at {apexData.Timestamp:f}"
                        }
                },
                new {
                    type = "section",
                    text = new {
                        type = "mrkdwn",
                        text=  logMesssage
                        }
                    }
                },
                attachments = new []{attachement}
            });
    }
}