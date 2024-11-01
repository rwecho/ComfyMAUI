using Aria2NET;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
using System.Reactive.Subjects;
using Volo.Abp.DependencyInjection;

namespace ComfyMAUI.Services;

public class Aria2JobManager(IOptions<Aria2cOptions> options) : ISingletonDependency
{
    private readonly Aria2NetClient aria2NetClient = new($"http://127.0.0.1:{options.Value.ListenPort}/jsonrpc");

    private readonly ConcurrentDictionary<string, Aria2Job> Jobs = new();

    public BehaviorSubject<Aria2Job?> JobsStream = new(null);


    public async Task<Aria2Job> AddJob(string url)
    {
        var filename = Path.GetFileName(url);
        var id = await aria2NetClient.AddUriAsync([url], new Dictionary<string, object>()
            {
                 { "dir", "abc"}
            }, 0);
        var job = new Aria2Job(id, url, filename);
        Jobs.TryAdd(id, job);
        return job;
    }

    public Task<IEnumerable<Aria2Job>> GetJobs()
    {
        return Task.FromResult((IEnumerable<Aria2Job>)Jobs.Values);
    }

    internal async Task UpdateStatus()
    {
        foreach (var job in Jobs.Values)
        {
            if(job.Status == null || job.Status?.Status == "active")
            {
                var status = await aria2NetClient.TellStatusAsync(job.Id);
                job.Status = status;
            }
            JobsStream.OnNext(job);
        }
    }

    public async Task RemoveJob(string id)
    {
        await aria2NetClient.RemoveAsync(id);
        Jobs.TryRemove(id, out _);
    }
}

