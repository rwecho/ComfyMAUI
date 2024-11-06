namespace ComfyMAUI;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
    }


    protected override Window CreateWindow(IActivationState? activationState)
    {
        var window = activationState == null
            ? throw new ArgumentNullException(nameof(activationState))
            : (Window)activationState.Context.Services.GetRequiredService<MainWindow>();

        return window;
    }
}
