using ComfyMAUI.Services;
using CommunityToolkit.Mvvm.ComponentModel;

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
    public MainPageViewModel(ComfyUIService comfyUIService, NavigationService navigationService)
    {
        ComfyUIService = comfyUIService;
        NavigationService = navigationService;
        Task.Run(this.Load);
    }

    private async Task Load()
    {
        this.ComfyUIVersion = await ComfyUIService.GetComfyUIVersion();
    }

    [ObservableProperty]
    private string? _url = "http://127.0.0.1:8188", _comfyUIVersion;


    public ComfyUIService ComfyUIService { get; }
    public NavigationService NavigationService { get; }

    [ObservableProperty]
    private double _width, _height;
}