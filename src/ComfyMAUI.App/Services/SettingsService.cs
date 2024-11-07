using System.Text.Json;

namespace ComfyMAUI.Services;

public class MirrorSettings
{
    public const string Key = "mirror_settings";

    public virtual string GitHubMirror { get; set; } = "https://ghp.ci/";
    public virtual string PipMirror { get; set; } = "https://mirrors.tuna.tsinghua.edu.cn/pypi/web/simple";
    public virtual string HuggingFaceMirror { get; set; } = "https://hf-mirror.com";
}

public class ComfyUIPythonSettings
{
    public const string Key = "comfyui_settings";
    public virtual string InstallationPath { get; set; } = "bin";

    public string CustomNodesFolder
    {
        get
        {
            return Path.Combine(ComfyUIFolder, "custom_nodes");
        }
    }

    public string PythonFolder
    {
        get
        {
            return Path.Combine(InstallationPath, "python_embeded");
        }
    }

    public string ComfyUIFolder
    {
        get
        {
            return Path.Combine(InstallationPath, "ComfyUI");
        }
    }

    public string ModelsFolder
    {
        get
        {
            return Path.Combine(ComfyUIFolder, "models");
        }
    }
}

public class GpuSettings
{
    public const string Key = "gpu_settings";
    public string GpuOrCpu { get; set; } = "gpu";
}

public class SettingsService
{
    private Dictionary<string, string> _settings = [];
    private readonly Task _task;

    public SettingsService()
    {
        _task = Task.Run(Load);
    }

    private Task Load()
    {
        var settings = Preferences.Get("settings", "{}");
        _settings = JsonSerializer.Deserialize<Dictionary<string, string>>(settings) ?? [];
        return Task.CompletedTask;
    }

    public async Task<T?> Get<T>(string key)
        where T : notnull
    {
        await _task;

        return _settings.TryGetValue(key, out string? value) ? JsonSerializer.Deserialize<T>(value) : default;
    }

    public async Task Set<T>(string key, T value)
    {
        await _task;
        _settings[key] = JsonSerializer.Serialize(value);
        Preferences.Set("settings", JsonSerializer.Serialize(_settings));
    }
}