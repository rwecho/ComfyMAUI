using ComfyMAUI.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Options;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace ComfyMAUI.Views;

public partial class MainPage : ContentPage
{
    public MainPage(MainPageViewModel viewModel)
    {
        InitializeComponent();
        rootComponent.Parameters = new Dictionary<string, object?>
        {
            { "HostWindowViewModel", viewModel }
        };
        BindingContext = viewModel;
    }

    protected override void OnSizeAllocated(double width, double height)
    {
        var viewModel = (MainPageViewModel)BindingContext;

        viewModel.Width = width;
        viewModel.Height = height;
    }
}


public partial class MainPageViewModel : ObservableObject, IHostWindowViewModel
{
    public MainPageViewModel(ComfyUIService comfyUIService,
        NavigationService navigationService,
        HttpClient httpClient,
        IDispatcher dispatcher,
        IOptions<ComfyUIOptions> comfyUIOptions)
    {
        ComfyUIOptions = comfyUIOptions;
        ComfyUIService = comfyUIService;
        NavigationService = navigationService;
        HttpClient = httpClient;
        Dispatcher = dispatcher;
        _url = ComfyUIOptions.Value.ListenAddress;
        Task.Run(this.Load);
    }

    private async Task Load()
    {
        CheckUrlAvailability();
        this.ComfyUIVersion = await ComfyUIService.GetComfyUIVersion();
    }

    private IDisposable? _urlAvailabilityCheck;

    private void CheckUrlAvailability()
    {
        var disposed = new Subject<bool>();
        _urlAvailabilityCheck = Observable.Interval(TimeSpan.FromMilliseconds(100))
            .TakeUntil(disposed)
            .Select(_ => Observable.FromAsync(async () =>
            {
                var response = await HttpClient.GetAsync(Url);
                return response.IsSuccessStatusCode;
            }))
            .Switch()
            .Do(isAvailable =>
            {
                if (isAvailable)
                {
                    Dispatcher.Dispatch(() =>
                    {
                        WebAvailable = true;
                    });

                    disposed.OnNext(true);
                }
            })
            .Subscribe();
    }

    [ObservableProperty]
    private string? _url, _comfyUIVersion;

    [ObservableProperty]
    private bool _webAvailable;


    private ComfyUIService ComfyUIService { get; }
    private NavigationService NavigationService { get; }
    private HttpClient HttpClient { get; }
    private IDispatcher Dispatcher { get; }
    private IOptions<ComfyUIOptions> ComfyUIOptions { get; }

    [ObservableProperty]
    private double _width, _height;
}