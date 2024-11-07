using Aria2NET;

namespace ComfyMAUI.Services;

public record Aria2Job(string Id, string Url)
{
    public DownloadStatusResult? Status { get; set; }

    public string? DownloadFilePath => Status?.Files.FirstOrDefault()?.Path;
}
