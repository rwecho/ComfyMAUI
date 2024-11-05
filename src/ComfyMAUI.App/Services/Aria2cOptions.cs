using System.Reactive.Linq;

namespace ComfyMAUI.Services;

public class Aria2cOptions
{
    public string BinPath { get; set; } = null!;

    public int ListenPort { get; set; } = 6800;

    public bool ListenAll { get; set; }
}
