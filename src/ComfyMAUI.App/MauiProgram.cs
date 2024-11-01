using ComfyMAUI.Services;
using ComfyMAUI.Views;
using CommunityToolkit.Maui;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Serilog;
using System.Reflection;
using Volo.Abp;
using Volo.Abp.Autofac;

namespace ComfyMAUI;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        Log.Information("Starting app.");
        Log.Information("Current directory: {CurrentDirectory}", AppContext.BaseDirectory);

        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            })
            .ConfigureContainer(new AbpAutofacServiceProviderFactory(new Autofac.ContainerBuilder()));

        ConfigureConfiguration(builder);

        builder.Services.AddApplication<ComfyMAUIAppModule>(options =>
        {
            options.Services.ReplaceConfiguration(builder.Configuration);
        });

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

        var app = builder.Build();
        app.Services.GetRequiredService<IAbpApplicationWithExternalServiceProvider>().InitializeAsync(app.Services).Wait();
        return app;
    }

    private static void ConfigureConfiguration(MauiAppBuilder builder)
    {
        var assembly = typeof(App).GetTypeInfo().Assembly;
        builder.Configuration.AddJsonFile("appsettings.json", optional: false, false);
    }
}

