using ComfyMAUI.Services;
using Microsoft.Extensions.Logging;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace ComfyMAUI.Components.Pages.InstallWizard;

public enum DownloadSetupStatus
{
    Pending,
    Downloading,
    Extracting,
    Completed
}

public class DownloadAndSetupViewModel(
    Aria2JobManager aria2JobManager,
    ILogger<DownloadAndSetupViewModel> logger)
{
    private readonly BehaviorSubject<Aria2Job?> _jobObservable = new(null);
    private readonly Subject<string?> _extractFileNameObservable = new();

    public IObservable<Aria2Job?> JobObservable => _jobObservable;
    public IObservable<string?> ExtractFileNameObservable => _extractFileNameObservable;

    public Subject<DownloadSetupStatus> StatusObservable { get; } = new();

    public void DownloadAndSetup(string url, string installationPath)
    {
        StatusObservable.OnNext(DownloadSetupStatus.Pending);

        var downloadObservable = Observable.FromAsync(async () =>
        {
            var job = await aria2JobManager.AddJob(url, installationPath);
            _jobObservable.OnNext(job);
            StatusObservable.OnNext(DownloadSetupStatus.Downloading);
            return job;
        }).Finally(() => logger.LogInformation("已经添加的下载任务队列"));

        var downloadStatusObservable = Observable.Interval(TimeSpan.FromSeconds(1))
            .TakeUntil(_jobObservable
                .Where(job => !(job?.Status?.Status == "active" || job?.Status?.Status == "waiting" || job?.Status?.Status == null))
            )
            .Select((t) => Observable.FromAsync(async () =>
            {
                var job = _jobObservable.Value!;
                var status = await aria2JobManager.GetStatus(job.Id);
                job.Status = status;
                _jobObservable.OnNext(job);
                return job;
            }))
            .Switch()
            .Finally(() => logger.LogInformation("下载任务已经完成"));

        Observable.Defer(() => _jobObservable)
            .Where(job => job?.Status?.Status == "active" || job?.Status?.Status == "waiting")
            .Select(job => job!.Status!.TotalLength)
            .DistinctUntilChanged()
            .Subscribe(totalLength =>
            {
                logger.LogInformation($"下载任务总长度: {totalLength}");
            });

        var extractObservable = Observable.Create<SevenZipExtractor.Entry>(observable =>
        {
            StatusObservable.OnNext(DownloadSetupStatus.Extracting);
            var filePath = _jobObservable.Value?.Status?.Files[0].Path!;
            using var archive = new SevenZipExtractor.ArchiveFile(filePath);
            archive.Extract(entry =>
            {
                observable.OnNext(entry);
                _extractFileNameObservable.OnNext(entry.FileName);
                return Path.Combine(installationPath, entry.FileName.Replace("ComfyUI_windows_portable\\", ""));
            });
            observable.OnCompleted();

            _extractFileNameObservable.OnNext("安装完成");
            return () => { };
        })
        .Finally(() =>
        {
            var filePath = _jobObservable.Value?.Status?.Files[0].Path!;
            _extractFileNameObservable.OnNext("安装完成, 删除压缩包");
            File.Delete(filePath);
            logger.LogInformation("已经解压文件");
        });

        downloadObservable
            .Concat(downloadStatusObservable)
            .Select(o => Unit.Default)
            .Concat(extractObservable.Select(o => Unit.Default))
            .Finally(() => StatusObservable.OnNext(DownloadSetupStatus.Completed))
            .Subscribe();
    }
}
