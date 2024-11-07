using System.Diagnostics;
using System.Text;

namespace ComfyMAUI.Services;
public class ProcessService
{
    public async Task<Process> Start(string command, string arguments,
        string? workingDirectory,
        Action<string>? outputReceived = null,
        Action<string>? errorReceived = null,
        Dictionary<string,string>? environment = null,
        bool waitForExit = false)
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = command,
                Arguments = arguments,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            }
        };

        if (workingDirectory != null)
        {
            process.StartInfo.WorkingDirectory = workingDirectory;
        }

        if (environment != null)
        {
            foreach (var (key, value) in environment)
            {
                process.StartInfo.Environment[key] = value;
            }
        }

        process.OutputDataReceived += (sender, e) =>
        {
            if (e.Data != null)
            {
                outputReceived?.Invoke(e.Data);
            }
        };
        process.ErrorDataReceived += (sender, e) =>
        {
            if (e.Data != null)
            {
                errorReceived?.Invoke(e.Data);
            }
        };
        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        if (waitForExit)
        {
            await process.WaitForExitAsync();
        }

        return process;
    }

    public async Task<Process> Start(string command, string arguments,
        string? workingDirectory = null,
        Action<string>? allReceived = null,
        Dictionary<string, string>? environment = null,
        bool waitForExit = false)
    {
        return await Start(command, arguments, workingDirectory, 
            allReceived, 
            allReceived, 
            waitForExit: waitForExit);
    }


    public async Task<string> Start(string command, string arguments,
        string? workingDirectory = null)
    {
        var output = new StringBuilder();

        await Start(command, arguments, workingDirectory,
            outputReceived: data => output.AppendLine(data),
            errorReceived: data => output.AppendLine(data),
            waitForExit: true);

        return output.ToString();
    }
}
