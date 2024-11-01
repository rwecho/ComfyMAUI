using ComfyMAUI.Services;
using ComfyMAUI.Views;
using CommunityToolkit.Maui;
using Volo.Abp;
using Volo.Abp.Modularity;

namespace ComfyMAUI;

public class ComfyMAUIAppModule: AbpModule
{
    private readonly CancellationTokenSource _cts = new();
    private HostedServiceExecutor? _hostedServiceExecutor;


    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();

        context.Services.AddAntDesign();
        context.Services.AddMauiBlazorWebView();
        context.Services.AddTransientPopup<SettingsPopup, SettingsPopupViewModel>();

        context.Services.AddHostedService<Aria2ServerHosedService>();
        context.Services.AddHostedService<Aria2JobManagerHostedService>();

        this.Configure<Aria2cOptions>(configuration.GetSection("Aria2c"));
    }


    public override async Task OnApplicationInitializationAsync(ApplicationInitializationContext context)
    {
        _hostedServiceExecutor = context.ServiceProvider.GetRequiredService<HostedServiceExecutor>();
        await _hostedServiceExecutor.StartAsync(_cts.Token);

    }

    public override async Task OnApplicationShutdownAsync(ApplicationShutdownContext context)
    {
        // todo: implement the shutdown logic
        await _cts.CancelAsync();
        if (_hostedServiceExecutor != null)
        {
            await _hostedServiceExecutor.StopAsync(_cts.Token);
        }
    }
}
