namespace ComfyMAUI.Views;

public interface IHostWindowViewModel
{
    public double Width { get; }
    public double Height { get;  }
}


public interface IPopupHostWindowViewModel : IHostWindowViewModel
{
    public Task CloseAsync(object? result = null);
}