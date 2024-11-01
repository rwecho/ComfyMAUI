using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ComfyMAUI.Views;

public partial class BlazorPage : ContentPage
{
	public BlazorPage(string startPath)
	{
		InitializeComponent();
		blazorWebView.StartPath = startPath;
    }
}
