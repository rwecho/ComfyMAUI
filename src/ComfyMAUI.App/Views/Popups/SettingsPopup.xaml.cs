using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.AspNetCore.Components.WebView.Maui;

namespace ComfyMAUI.Views.Popups;

public partial class SettingsPopup : Popup
{
	public SettingsPopup(SettingsPopupViewModel vm)
	{
		InitializeComponent();

        vm.CloseRequested += (s, e) => CloseAsync(e);

        rootComponent.Parameters = new Dictionary<string, object?>
        {
            { "SettingsPopupViewModel", vm }
        };
        BindingContext = vm;
        
    }

    private async void OnYesButtonClicked(object sender, EventArgs e)
    {
        await CloseAsync(true);
    }

    private async void OnNoButtonClicked(object sender, EventArgs e)
    {
        await CloseAsync(false);
    }
}

public partial class SettingsPopupViewModel: ObservableObject
{
    private const int MaxWidth = 800, MaxHeight = 400;
    public Size Size => new(Math.Min(MaxWidth, ParentWidth - 200), Math.Max(MaxHeight, ParentHeight - 200));

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Size))]
    public double _parentWidth, _parentHeight;

    public event EventHandler<object?>? CloseRequested;

    public Task CloseAsync(object? result = null)
    {
        CloseRequested?.Invoke(this, result);

        return Task.CompletedTask;
    }

}