using System.Text.Json;

namespace ComfyMAUI.Services;

public record ComfyNode(string Name, string GitUrl, string GroupName, bool IsChecked);

public class ComfyNodesService
{
    public async Task<IReadOnlyList<ComfyNode>> GetNodes()
    {
        using Stream inputStream = await FileSystem.OpenAppPackageFileAsync("ComfyNodes.json");
        using StreamReader reader = new(inputStream);
        var json = await reader.ReadToEndAsync();
        return JsonSerializer.Deserialize<List<ComfyNode>>(json) ?? [];
    }

}
