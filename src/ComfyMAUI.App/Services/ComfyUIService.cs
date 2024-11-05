namespace ComfyMAUI.Services;

public class ComfyUIService(SettingsService settingsService, ProcessService processService)
{

    public async Task<string?> GetInstallationPath()
    {
        var comfyUIPythonSettings = await settingsService.Get<ComfyUIPythonSettings>(ComfyUIPythonSettings.Key);

        if (comfyUIPythonSettings == null)
            return null;

        if (!Directory.Exists(comfyUIPythonSettings.InstallationPath))
        {
            return null;
        }

        return comfyUIPythonSettings.InstallationPath;
    }


    public async Task<string?> GetPythonVersion()
    {
        var comfyUIPythonSettings = await settingsService.Get<ComfyUIPythonSettings>(ComfyUIPythonSettings.Key);

        if (comfyUIPythonSettings == null)
            return null;


        if (!Directory.Exists(comfyUIPythonSettings.PythonFolder))
        {
            return null;
        }
        var pythonPath = Path.Combine(comfyUIPythonSettings.PythonFolder, "python.exe");
        if (!File.Exists(pythonPath))
        {
            return null;
        }

        return await processService.Start(pythonPath, "--version", comfyUIPythonSettings.PythonFolder);
    }


    public async Task<string?> GetComfyUIVersion()
    {
        var comfyUIPythonSettings = await settingsService.Get<ComfyUIPythonSettings>(ComfyUIPythonSettings.Key);

        if (comfyUIPythonSettings == null)
            return null;

        if (!Directory.Exists(comfyUIPythonSettings.ComfyUIFolder))
        {
            return null;
        }

        return await processService.Start("git", "tag --points-at HEAD", comfyUIPythonSettings.ComfyUIFolder);
    }
}