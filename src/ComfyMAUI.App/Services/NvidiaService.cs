
using System.Diagnostics;
using Volo.Abp.DependencyInjection;

namespace ComfyMAUI.Services;

public class NvidiaService: ITransientDependency
{
    public async Task<string> GetVersion()
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "nvidia-smi",
                Arguments = "--query-gpu=driver_version --format=csv,noheader",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };
        process.Start();
        string result = await process.StandardOutput.ReadToEndAsync();
        process.WaitForExit();
        return result;
    }

}