namespace ComfyMAUI.Services;

public class ComfyProcessHostedService (ProcessService processService, ComfyUIService comfyUIService,
    SettingsService settingsService
    ): IHostedService
{
    public async Task StartAsync(CancellationToken token)
    {
        var mirrorSettings = await settingsService.Get<MirrorSettings>(MirrorSettings.Key);

        var pythonVersion = await comfyUIService.GetPythonVersion();
        var comfyUIVersion = await comfyUIService.GetComfyUIVersion();

        if (pythonVersion == null || comfyUIVersion == null)
        {
            return;
        }

        var installationPath = await comfyUIService.GetInstallationPath();

        if (installationPath == null)
        {
            return;
        }


        var bat = Path.Combine(installationPath, "run_nvidia_gpu.bat");

        if (!File.Exists(bat))
        {
            return;
        }
        var environment = mirrorSettings == null ? [] : new Dictionary<string, string>
        {
            ["PIP_INDEX_URL"] = mirrorSettings.PipMirror,
            ["HF_ENDPOINT"] = mirrorSettings.HuggingFaceMirror,
        };
        await processService.Start(bat, "", installationPath, null, environment: environment);
    }

    public async Task StopAsync(CancellationToken token)
    {
        await processService.Start("taskkill", "/IM python.exe /F", "");
    }
}
