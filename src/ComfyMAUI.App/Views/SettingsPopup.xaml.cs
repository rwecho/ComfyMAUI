using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ComfyMAUI.Views;

public partial class SettingsPopup : Popup
{
	public SettingsPopup(SettingsPopupViewModel vm)
	{
		InitializeComponent();
        BindingContext = vm;
    }

    private void OnYesButtonClicked(object sender, EventArgs e)
    {

    }

    private void OnNoButtonClicked(object sender, EventArgs e)
    {

    }
}

public partial class SettingsPopupViewModel: ObservableObject
{
}