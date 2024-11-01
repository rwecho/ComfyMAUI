namespace ComfyMAUI;

public interface IHostedService
{
    Task StartAsync(CancellationToken token);

    Task StopAsync(CancellationToken token);

}

public class HostedServiceExecutor(IEnumerable<IHostedService> services)
{
    private Task? _startTask;

    public Task StartAsync(CancellationToken token)
    {
        _startTask = Task.Run(() => StartInternalAsync(token), token);

        return Task.CompletedTask;
    }

    private async Task StartInternalAsync(CancellationToken token)
    {
        // Delay for 1 second before starting the services to ensure that UI is loaded
        await Task.Delay(1000, token);
        foreach (var service in services)
        {
            await service.StartAsync(token);
        }
    }

    public async Task StopAsync(CancellationToken token)
    {
        List<Exception>? exceptions = null;

        if (_startTask != null)
        {
            await _startTask;
        }

        foreach (var service in services)
        {
            try
            {
                await service.StopAsync(token);
            }
            catch (Exception ex)
            {
                exceptions ??= [];
                exceptions.Add(ex);
            }
        }

        // Throw an aggregate exception if there were any exceptions
        if (exceptions != null)
        {
            throw new AggregateException(exceptions);
        }
    }
}