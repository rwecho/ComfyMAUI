using CommunityToolkit.Maui;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.LifecycleEvents;
using Nito.AsyncEx;
using Serilog;
using Serilog.Events;
using System;
using System.Reflection;

namespace ComfyMAUI;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        SetupSerilog();

        var startUp = new Startup();

        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp(serviceProvider =>
            {
                return new App();
            })
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            })
            .ConfigureLifecycleEvents(events =>
            {
#if WINDOWS
                events.AddWindows(windows => windows
                    .OnWindowCreated((window) =>
                    {
                        var handle = WinRT.Interop.WindowNative.GetWindowHandle(window);
                        var id = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(handle);
                        var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(id);

                        switch (appWindow.Presenter)
                        {
                            case Microsoft.UI.Windowing.OverlappedPresenter overlappedPresenter:
                                ////disable the max button
                                //overlappedPresenter.IsMaximizable = false;
                                break;
                        }

                        //When user execute the closing method, we can make the window do not close by   e.Cancel = true;.
                        appWindow.Closing += (s, e) =>
                        {
                            startUp.OnApplicationShutdown().Wait();
                        };
                    })
                    .OnActivated ((window, args)=> LogEvent(nameof(WindowsLifecycle.OnActivated)))
                    .OnClosed((window, args) => 
                    {
                        LogEvent(nameof(WindowsLifecycle.OnClosed));
                    })
                    .OnLaunched((window, args) => LogEvent(nameof(WindowsLifecycle.OnLaunched)))
                    .OnLaunching((window, args) => LogEvent(nameof(WindowsLifecycle.OnLaunching)))
                    .OnVisibilityChanged((window, args) => LogEvent(nameof(WindowsLifecycle.OnVisibilityChanged)))
                    .OnPlatformMessage((window, args) =>
                    {
                        if (args.MessageId == Convert.ToUInt32("031A", 16))
                        {
                            // System theme has changed.
                        }
                    })
                );
#endif
            });
        ConfigureConfiguration(builder);
#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif
        builder.Logging.AddSerilog(dispose: true);

        startUp.ConfigureServices(builder.Configuration, builder.Services);
        var app = builder.Build();
        startUp.OnApplicationInitialization(app.Services);

        return app;
    }

    private static void LogEvent(string eventName, string? type = null)
    {
        Log.Logger.Information($"Event: {eventName} {type}");
    }

    private static void ConfigureConfiguration(MauiAppBuilder builder)
    {
        var assembly = typeof(App).GetTypeInfo().Assembly;
        builder.Configuration.AddJsonFile("appsettings.json", optional: false, false);
    }

    private static void SetupSerilog()
    {
        var flushInterval = new TimeSpan(0, 0, 1);
        var file = Path.Combine(FileSystem.AppDataDirectory, "ComfyMAUI.log");

        Log.Logger = new LoggerConfiguration()
        .MinimumLevel.Verbose()
        .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
        .Enrich.FromLogContext()
        .WriteTo.File(file, flushToDiskInterval: flushInterval, encoding: System.Text.Encoding.UTF8, rollingInterval: RollingInterval.Day, retainedFileCountLimit: 22)
        .CreateLogger();
    }
}

