using Microsoft.Extensions.Options;
using System.Diagnostics;

namespace ComfyMAUI.Services;

public class Aria2ServerHosedService(ProcessService processService, IOptions<Aria2cOptions> options) : IHostedService
{
    private Process? _process;
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var arguments = $"--enable-rpc {(options.Value.ListenAll ? "--rpc-listen-all" : "")} --rpc-listen-port={options.Value.ListenPort}";
        _process = await processService.Start(options.Value.BinPath, arguments, null, null, null, false);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        if (_process != null)
        {
            _process.Kill();
        }

        return Task.CompletedTask;
    }
}
