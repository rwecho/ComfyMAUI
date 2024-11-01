using Aria2NET;

namespace ComfyMAUI.Services;

public record Aria2Job(string Id, string Url, string Filename)
{
    public DownloadStatusResult? Status { get; set; }
}

