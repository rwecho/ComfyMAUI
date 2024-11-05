namespace ComfyMAUI.Services;

public class ComfyProcessHostedService (ProcessService processService, ComfyUIService comfyUIService): IHostedService
{
    public async Task StartAsync(CancellationToken token)
    {
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
        await processService.Start(bat, "", installationPath);
    }

    public async Task StopAsync(CancellationToken token)
    {
        await processService.Start("taskkill", "/IM python.exe /F", "");
    }
}
