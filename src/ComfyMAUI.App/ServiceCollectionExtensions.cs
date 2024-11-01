using CommunityToolkit.Maui;

namespace ComfyMAUI;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddHostedService<TService>(this IServiceCollection services)
        where TService : class, IHostedService
    {
        services.AddSingleton<IHostedService, TService>();
        return services;
    }
}

