using ComfyMAUI.Views;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ComfyMAUI;

public partial class MainWindow : Window
{
	public MainWindow(MainPage mainPage, MainWindowViewModel viewModel)
	{
		this.Page = new NavigationPage(mainPage);
		InitializeComponent();
	}
}

public partial class MainWindowViewModel: ObservableObject
{

    [ObservableProperty]
	private string _title = "Comfy MAUI";

	[ObservableProperty]
	private string? _subtitle = "";

	[ObservableProperty]
	public bool _showTitleBar = true;
}