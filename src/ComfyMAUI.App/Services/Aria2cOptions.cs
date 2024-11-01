using Aria2NET;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
using Volo.Abp.DependencyInjection;

namespace ComfyMAUI.Services;

public class Aria2cOptions
{
    public string BinPath { get; set; } = null!;

    public int ListenPort { get; set; } = 6800;

    public bool ListenAll { get; set; }
}

