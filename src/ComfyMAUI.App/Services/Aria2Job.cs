using Aria2NET;

namespace ComfyMAUI.Services;

public record Aria2Job(string Id, string Url, string Filename, string DownloadFilePath)
{
    public DownloadStatusResult? Status { get; set; }
}

