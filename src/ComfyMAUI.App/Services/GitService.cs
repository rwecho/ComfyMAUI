using System.Diagnostics;
using Volo.Abp.DependencyInjection;

namespace ComfyMAUI.Services;

public class GitService: ITransientDependency
{
    public async Task<string> GetVersion()
    {
#if WINDOWS
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "git",
                Arguments = "version",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };
        process.Start();
        string result = await process.StandardOutput.ReadToEndAsync();
        process.WaitForExit();
        return result;
#else
        throw new NotSupportedException();
#endif
    }

    public async Task<string> GetInstallationPath()
    {
#if WINDOWS
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "powershell",
                Arguments = "-Command \"(Get-Command git).Source\"",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };
        process.Start();
        string result = await process.StandardOutput.ReadToEndAsync();
        process.WaitForExit();
        return result.Trim();
#else
    throw new NotSupportedException();
#endif
    }
}
