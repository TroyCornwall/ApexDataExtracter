namespace ApexDataExtracter.Config;

public class SlackConfig
{
    public bool Enabled { get; set; } = false;
    public string? WebhookUrl { get; set; }
}