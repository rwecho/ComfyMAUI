using Volo.Abp.DependencyInjection;

namespace ComfyMAUI.Services;

public class ComfyUIService: ITransientDependency
{

    public async Task<string> GetVersion()
    {
        return "";
    }


}