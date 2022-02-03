using System.Text;
using ApexDataExtracter.Config;
using ApexDataExtracter.Models.Apex;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Octokit;

namespace ApexDataExtracter.Services;

public class GitService
{
    private readonly GitHubClient _client;
    private readonly bool _enabled;
    private readonly JsonSerializerSettings _jsonSerializerSettings;

    public GitService(IOptions<GitConfig> config)
    {
        var realConfig = config.Value;
        _enabled = realConfig.Enabled;
        _client = new GitHubClient(new ProductHeaderValue("TankMate"));;
        _client.Credentials = new Credentials(realConfig.PAT);
        _jsonSerializerSettings = new JsonSerializerSettings()
        {
            NullValueHandling = NullValueHandling.Ignore
        };
    }

    public async Task<ApexData> PushDataToGithub(ApexData apexData)
    {
        if (!_enabled)
            return apexData;
        
        var (owner, repoName, filePath, branch) = ("troycornwall", "TankDetails", 
            "status.json", "main");
     
        // await _client.Repository.Content.CreateFile(
        //     owner, repoName, filePath,
        //     new CreateFileRequest($"First commit for {filePath}", "[]", branch));    
        
        var fileDetails = await _client.Repository.Content.GetAllContentsByRef(owner, repoName,
            filePath, branch);
        
        var file = fileDetails.FirstOrDefault();
        
        byte[] data = Convert.FromBase64String(file.EncodedContent);
        var content =  Encoding.UTF8.GetString(data);
        
        var statuses = JsonConvert.DeserializeObject<List<ApexData>>(content);
        
        var last3days = statuses.Where(x => x.Timestamp >= DateTimeOffset.Now.AddDays(-3)).ToList();

        var last = GetLastComplete(last3days);
        var diffd = RemoveDuplicateData(last, apexData);

        last3days.Add(diffd);
        
        
        await _client.Repository.Content.UpdateFile(owner, repoName, filePath,
            new UpdateFileRequest("BOT; Updated tank details", JsonConvert.SerializeObject(last3days, Formatting.Indented, _jsonSerializerSettings), file.Sha));

        return diffd;
    }

    private ApexData GetLastComplete(List<ApexData> statueses)
    {
        var reversed = statueses.OrderByDescending(x => x.Timestamp);
        var complete = statueses.Last();
        foreach (var status in reversed)
        {
            complete.Alk ??= status.Alk;
            complete.Ph ??= status.Ph;
            complete.Temp ??= status.Temp;
            complete.Ph ??= status.Ph;
            complete.Alk ??= status.Alk;
            complete.Calc ??= status.Calc;
            complete.Mg ??= status.Mg;
            complete.ATOStatus ??= status.ATOStatus;
            complete.ATOLow ??= status.ATOLow;
            complete.ATOHigh ??= status.ATOHigh;
        }

        return complete;
    }
    
    private ApexData RemoveDuplicateData(ApexData oldStatus, ApexData newStatus)
    {
            newStatus.Alk = GetChangedDouble(newStatus.Alk, oldStatus.Alk);
            newStatus.Ph = GetChangedDouble(newStatus.Ph, oldStatus.Ph);
            newStatus.Temp = GetChangedDouble(newStatus.Temp, oldStatus.Temp);
            newStatus.Ph = GetChangedDouble(newStatus.Ph, oldStatus.Ph);
            newStatus.Alk = GetChangedDouble(newStatus.Alk, oldStatus.Alk);
            newStatus.Calc = GetChangedDouble(newStatus.Calc, oldStatus.Calc);
            newStatus.Mg = GetChangedDouble(newStatus.Mg, oldStatus.Mg);
            newStatus.ATOLow = newStatus.ATOLow == oldStatus.ATOLow ? null : newStatus.ATOLow;
            newStatus.ATOHigh = newStatus.ATOHigh == oldStatus.ATOHigh ? null : newStatus.ATOHigh;
            newStatus.ATOStatus = oldStatus.ATOStatus != null && newStatus.ATOStatus![0].Equals(oldStatus.ATOStatus[0]) && newStatus.ATOStatus[2].Equals(oldStatus.ATOStatus[2]) ? null : newStatus.ATOStatus;

           
            return newStatus;
    }

    private double? GetChangedDouble(double? newValue, double? oldValue)
    {
        if (oldValue == null)
            return newValue;
        if (newValue == null)
            return null;
        var oldRounded =  Math.Round(oldValue!.Value, 2);
        var newRounded =  Math.Round(newValue!.Value, 2);
        // ReSharper disable once CompareOfFloatsByEqualityOperator
        return oldRounded == newRounded ? null : newRounded;
    }
    
    
}