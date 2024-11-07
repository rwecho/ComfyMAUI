using System.Text.Json;

namespace ComfyMAUI.Services;


public record ModelDownloadItem(string Url, string Directory, string Output);

public class DownloadModelsService
{
    public async Task<List<ModelDownloadItem>> GetPresetModels()
    {
        using Stream inputStream = await FileSystem.OpenAppPackageFileAsync("Models.json");
        using StreamReader reader = new(inputStream);
        var json = await reader.ReadToEndAsync();
        var models = JsonSerializer.Deserialize<List<ModelDownloadItem>>(json);
        return models ?? [];
    }

}