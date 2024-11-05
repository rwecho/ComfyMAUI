using ComfyMAUI.Views;
using ComfyMAUI.Views.Popups;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;

namespace ComfyMAUI;

public partial class MainWindow : Window
{
	public MainWindow(MainPage mainPage, MainWindowViewModel viewModel)
	{
		this.Page = new NavigationPage(mainPage);
		InitializeComponent();
		this.BindingContext = viewModel;
	}
}

public partial class MainWindowViewModel(IPopupService popupService) : ObservableObject
{

	[ObservableProperty]
	private string _title = "Comfy MAUI";

	[ObservableProperty]
	private string? _subtitle = "";

	[ObservableProperty]
	public bool _showTitleBar = true;

	[ObservableProperty]
	private double _width, _height;


    public ICommand GoSettingsCommand => new AsyncRelayCommand(async () =>
	{
        await popupService.ShowPopupAsync<SettingsPopupViewModel>(viewModel=>
		{
			viewModel.ParentWidth = this.Width;
            viewModel.ParentHeight = this.Height;
        });
    });
}