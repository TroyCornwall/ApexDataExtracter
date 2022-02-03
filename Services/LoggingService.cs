using System.Text;
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
        var sb = new StringBuilder();
        if (apexData.Ph != null)
            sb.Append($"PH: {apexData.Ph}\n");
        if (apexData.Temp != null)
            sb.Append($"Temp: {apexData.Temp}\n");
        if (apexData.Alk != null)
            sb.Append($"Alk: {apexData.Alk}\n");
        if (apexData.Calc != null)
            sb.Append($"Calcium: {apexData.Calc}\n");
        if (apexData.Mg != null)
            sb.Append($"Magnesium: {apexData.Mg}\n");
        if (apexData.ATOStatus != null)
            sb.Append($"ATO: {apexData.ATOStatus[2]} - {apexData.ATOStatus[0]}\n");
        if (apexData.ATOLow != null)
            sb.Append($"   ATO Low Underwater: {apexData.ATOLow}\n");
        if (apexData.ATOHigh != null)
            sb.Append( $"   ATO High Underwater {apexData.ATOHigh}");

        // if (sb.Length == 0)
        //     sb.Append("No Changes");
        
        return sb.ToString();
    }

    private void ConsoleLog(ApexData apexData, string logMesssage)
    {
        Console.WriteLine($"System: {apexData.ApexName} as at {apexData.Timestamp:f}\n" + logMesssage);
    }
    
    private async Task SlackLog(ApexData apexData, string logMesssage)
    {       
        if (!_config.Enabled)
            return;

        if (string.IsNullOrWhiteSpace(logMesssage))
        {
            logMesssage = "No Changes..";
            if (apexData.Timestamp.Minute > 10)
            {
                return;
            }
        }
        
        var attachement =
            (apexData.ATOStatus != null && apexData.ATOStatus?[2] == "OK") ? null : new
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