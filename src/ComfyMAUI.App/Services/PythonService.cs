
using System.Diagnostics;
using Volo.Abp.DependencyInjection;

namespace ComfyMAUI.Services;

public class PythonService: ITransientDependency
{
    public async Task<string> GetVersion()
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "python",
                Arguments = "--version",
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
