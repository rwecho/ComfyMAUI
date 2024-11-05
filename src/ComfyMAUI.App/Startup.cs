using ComfyMAUI.Components.Pages.InstallWizard;
using ComfyMAUI.Services;
using ComfyMAUI.Views;
using ComfyMAUI.Views.Popups;
using CommunityToolkit.Maui;
using Microsoft.Extensions.Configuration;

namespace ComfyMAUI;

public class Startup
{
    private readonly CancellationTokenSource _cts = new();

    private HostedServiceExecutor? _serviceExecutor;

    public void ConfigureServices(IConfiguration configuration, IServiceCollection services)
    {
        services.AddSingleton<Aria2JobManager>();
        services.AddTransient<ComfyUIService>();
        services.AddTransient<GitService>();
        services.AddTransient<NavigationService>();
        services.AddTransient<NvidiaService>();
        services.AddTransient<PythonService>();
        services.AddSingleton<SettingsService>();
        services.AddTransient<HostedServiceExecutor>();
        services.AddTransient<ComfyNodesService>();
        services.AddTransient<ProcessService>();
        
        
        services.AddTransient<InstallWizardViewModel>();


        services.AddAntDesign();
        services.AddMauiBlazorWebView();
        services.AddTransientPopup<SettingsPopup, SettingsPopupViewModel>();

        services.AddTransient<MainWindow>();
        services.AddTransient<MainWindowViewModel>();

        services.AddTransient<MainPage>();
        services.AddTransient<MainPageViewModel>();

        services.AddHostedService<Aria2ServerHosedService>();
        services.AddHostedService<Aria2JobManagerHostedService>();
        services.AddHostedService<ComfyProcessHostedService>();

        services.Configure<Aria2cOptions>(configuration.GetSection("Aria2c").Bind);
    }

    public void OnApplicationInitialization(IServiceProvider services)
    {
        _serviceExecutor = services.GetRequiredService<HostedServiceExecutor>();
        _serviceExecutor.StartAsync(_cts.Token).Wait();
    }

    public async Task OnApplicationShutdown()
    {
        if (_serviceExecutor == null) return;

        await _serviceExecutor.StopAsync(_cts.Token);
    }
}

