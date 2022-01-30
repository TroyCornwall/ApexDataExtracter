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

    public GitService(IOptions<GitConfig> config)
    {
        var realConfig = config.Value;
        _enabled = realConfig.Enabled;
        _client = new GitHubClient(new ProductHeaderValue("TankMate"));;
        _client.Credentials = new Credentials(realConfig.PAT);
    }

    public async Task PushDataToGithub(ApexData apexData)
    {
        if (!_enabled)
            return;
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
        statuses.Add(apexData);
        
        var updateResult = await _client.Repository.Content.UpdateFile(owner, repoName, filePath,
            new UpdateFileRequest("BOT; Updated tank details", JsonConvert.SerializeObject(statuses, Formatting.Indented), file.Sha));
        
        await Task.CompletedTask;
    }
}