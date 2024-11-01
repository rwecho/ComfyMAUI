namespace ComfyMAUI.Services;

public class Aria2JobManagerHostedService(Aria2JobManager aria2JobManager) : IHostedService
{
    private Task? _loopTask;
    public Task StartAsync(CancellationToken cancellationToken)
    {
        _loopTask = Task.Run(() => Loop(cancellationToken), cancellationToken);
        return Task.CompletedTask;

    }

    private async Task Loop(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                await aria2JobManager.UpdateStatus();
            }
            catch (Exception)
            {
                // ignored
            }

            await Task.Delay(200, cancellationToken);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        if (_loopTask != null)
        {
            return _loopTask;
        }
        return Task.CompletedTask;
    }
}

