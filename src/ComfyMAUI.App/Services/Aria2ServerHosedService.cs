using Microsoft.Extensions.Options;
using System.Diagnostics;

namespace ComfyMAUI.Services;

public class Aria2ServerHosedService(IOptions<Aria2cOptions> options) : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
#if !__IOS__
        var arguments = $"--enable-rpc {(options.Value.ListenAll ? "--rpc-listen-all" : "")} --rpc-listen-port={options.Value.ListenPort}";

        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = options.Value.BinPath,
                Arguments = arguments,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            }
        };

        process.Start();
#endif
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
