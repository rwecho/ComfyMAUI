using AntDesign;
using ChangeTracking;
using ComfyMAUI.Services;
using System.Reactive.Subjects;
using static ComfyMAUI.Services.ComfyUIPythonSettings;

namespace ComfyMAUI.Components.Pages.InstallWizard;

public class InstallWizardViewModel(SettingsService settingsService,
    INotificationService notice,
    IMessageService message,
    GitService gitService,
    Aria2JobManager aria2JobManager) : IDisposable
{
    private MirrorSettings _mirrorSettings = new();

    private ComfyUIPythonSettings _pythonSettings = new();

    public string GitHubMirror
    {
        get => _mirrorSettings.GitHubMirror;
        set => _mirrorSettings.GitHubMirror = value;
    }

    public string PipMirror
    {
        get => _mirrorSettings.PipMirror;
        set => _mirrorSettings.PipMirror = value;
    }

    public string HuggingFaceMirror
    {
        get => _mirrorSettings.HuggingFaceMirror;
        set => _mirrorSettings.HuggingFaceMirror = value;
    }

    public string ComfyUIInstallationPath
    {
        get => _pythonSettings.InstallationPath;
        set => _pythonSettings.InstallationPath = value;
    }


    public BehaviorSubject<int> CurrentSubject { get; } = new(0);

    public BehaviorSubject<Aria2Job?> Aria2Job => aria2JobManager.JobsStream;


    public bool CanNext
    {
        get
        {
            return CurrentSubject.Value switch
            {
                0 => !string.IsNullOrEmpty(GitHubMirror) && !string.IsNullOrEmpty(PipMirror) && !string.IsNullOrEmpty(HuggingFaceMirror),
                1 => true,
                2 => !string.IsNullOrEmpty(ComfyUIInstallationPath) && Directory.Exists(ComfyUIInstallationPath),
                _ => true
            };
        }
    }

    public async Task SaveComfyUISetting()
    {
        var trackable = _pythonSettings.CastToIChangeTrackable();
        if (trackable.IsChanged)
        {
            await settingsService.Set(Key, _pythonSettings);
            await message.Success("保存成功");
        }
    }

    public bool CanDone
    {
        get
        {
            return true;
        }
    }

    public bool CanBack
    {
        get
        {
            return false;
        }
    }

    public async Task OnNext()
    {
        try
        {
            if (CurrentSubject.Value == 0)
            {
                var trackable = _mirrorSettings.CastToIChangeTrackable();
                if (trackable.IsChanged)
                {
                    await settingsService.Set(Key, _mirrorSettings);
                    await message.Success("保存成功");
                }
            }

        }
        catch (Exception exception)
        {
            await notice.Error(new NotificationConfig
            {
                Message = "Failed to save settings",
                Description = exception.Message
            });
        }
        CurrentSubject.OnNext(CurrentSubject.Value + 1);
    }


    public Task OnBack()
    {
        return Task.CompletedTask;
    }


    public void Dispose()
    {
    }

    public async Task LoadMirrors()
    {
        _mirrorSettings = (await settingsService.Get<MirrorSettings>(Key) ?? new()).AsTrackable();
        _pythonSettings = (await settingsService.Get<ComfyUIPythonSettings>(Key) ?? new()).AsTrackable();
    }



    public async Task<Aria2Job> SetupComfyUI()
    {
        var downloadUrl = "http://myhome.rwecho.top:9000/comfy-maui/ComfyUI_windows_portable_nvidia.1.7z?Content-Disposition=attachment%3B%20filename%3D%22ComfyUI_windows_portable_nvidia.1.7z%22&X-Amz-Algorithm=AWS4-HMAC-SHA256&X-Amz-Credential=echo%2F20241104%2F%2Fs3%2Faws4_request&X-Amz-Date=20241104T140318Z&X-Amz-Expires=432000&X-Amz-SignedHeaders=host&X-Amz-Signature=8e0ce03ff6cfdd517105dd8fce0f6a432af8dbb969b0810b0ff88eea680674ac";
        //var url = $"{GitHubMirror.TrimEnd('/')}/https://github.com/comfyanonymous/ComfyUI/releases/download/v0.2.6/ComfyUI_windows_portable_nvidia.7z";

        return await aria2JobManager.AddJob(downloadUrl);
    }


    public Task<BehaviorSubject<string?>> SetupComfyUI(string sevenZipPath)
    {
        BehaviorSubject<string?> stream = new(null);

        _ = Task.Run(() =>
        {
            using var archive = new SevenZipExtractor.ArchiveFile(sevenZipPath);
            archive.Extract(entry =>
            {
                stream.OnNext(entry.FileName);
                return Path.Combine(ComfyUIInstallationPath, entry.FileName.Replace("ComfyUI_windows_portable\\", ""));
            });
            stream.OnCompleted();
        });
        return Task.FromResult(stream);
    }

    public async Task<ISubject<string>> PullComfyNodes(IReadOnlyList<ComfyNode> nodes)
    {
        var comfyUIPath = Path.Combine(ComfyUIInstallationPath, "ComfyUI\\custom_nodes");

        var pullSubject = new BehaviorSubject<string>("");

        if (!Directory.Exists(comfyUIPath))
        {
            return new BehaviorSubject<string>("ComfyUI not found");
        }

        _ = Task.Run(async () =>
        {
            try
            {
                foreach (var node in nodes)
                {
                    var nodePath = Path.Combine(comfyUIPath, node.Name);
                    if (Directory.Exists(nodePath))
                    {
                        pullSubject.OnNext($"{node.Name} 已经存在，跳过");
                        continue;
                    }

                    pullSubject.OnNext($"=========== 正在拉取 {node.Name} ===========");

                    var url = $"{GitHubMirror.TrimEnd('/')}/{node.GitUrl}";

                    await gitService.Clone(url, comfyUIPath, (line) =>
                    {
                        if (line != null)
                            pullSubject.OnNext(line);
                    });

                    pullSubject.OnNext($"=========== {node.Name} 拉取完成 ===========");
                }

                pullSubject.OnNext("拉取完成");

                pullSubject.OnCompleted();
            }
            catch (Exception exception)
            {
                pullSubject.OnError(exception);
            }
        });
        return pullSubject;
    }

    public Task Initialize()
    {
        return Task.CompletedTask;
    }
}
