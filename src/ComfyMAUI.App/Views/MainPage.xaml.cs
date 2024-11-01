﻿using ComfyMAUI.Services;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ComfyMAUI.Views;

public partial class MainPage : ContentPage
{
    public MainPage(MainPageViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}


public partial class MainPageViewModel : ObservableObject
{
    public MainPageViewModel(ComfyUIService comfyUIService, NavigationService navigationService)
    {
        ComfyUIService = comfyUIService;
        NavigationService = navigationService;
        Task.Run(this.Load);
    }

    private async Task Load()
    {
        this.ComfyUIVersion = await ComfyUIService.GetVersion();
    }

    [ObservableProperty]
    private string? _url, _comfyUIVersion;


    public ComfyUIService ComfyUIService { get; }
    public NavigationService NavigationService { get; }
}