using Aria2NET;
using Microsoft.Extensions.Options;

namespace ComfyMAUI.Services;

public class Aria2JobManager(IOptions<Aria2cOptions> options) 
{
    private readonly Aria2NetClient aria2NetClient = new($"http://127.0.0.1:{options.Value.ListenPort}/jsonrpc");

    public async Task<Aria2Job> AddJob(string url, string folder)
    {
        var id = await aria2NetClient.AddUriAsync([url], new Dictionary<string, object>()
            {
                 { "dir", folder}
            }, 0);
        var job = new Aria2Job(id, url);
        return job;
    }

    public async Task<DownloadStatusResult> GetStatus(string jobId)
    {
        return await aria2NetClient.TellStatusAsync(jobId);
    }
}

