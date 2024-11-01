
using System.Diagnostics;
using Volo.Abp.DependencyInjection;

namespace ComfyMAUI.Services;

public class NavigationService:ITransientDependency
{
    public async Task PushAsync(Page page)
    {
        var navigation = Application.Current?.Windows[0].Page?.Navigation;

        if (navigation != null)
        {
            await navigation.PushAsync(page);
        }
    }

    public async Task PopAsync()
    {
        var navigation = Application.Current?.Windows[0].Page?.Navigation;
        if (navigation != null)
        {
            await navigation.PopAsync();
        }
    }

    public async Task PopModalAsync()
    {
        var navigation = Application.Current?.Windows[0].Page?.Navigation;
        if (navigation != null)
        {
            await navigation.PopModalAsync();
        }
    }

    public async Task PopToRootAsync()
    {
        var navigation = Application.Current?.Windows[0].Page?.Navigation;
        if (navigation != null)
        {
            await navigation.PopToRootAsync();
        }
    }

    public async Task PushModalAsync(Page page)
    {
        var navigation = Application.Current?.Windows[0].Page?.Navigation;
        if (navigation != null)
        {
            await navigation.PushModalAsync(page);
        }
    }
}
