using System.Reactive.Linq;

namespace ComfyMAUI.Services;

public static class ObservableExtensions
{
    public static IObservable<T?> FinallyAsync<T>(this IObservable<T?> observable, Func<Task> finalFunc)
    {
        var finallyObservable = Observable.Return<T?>(default)
            .Select(t => Observable.FromAsync(async () =>
            {
                await finalFunc();
                return t;
            }))
            .Switch();
        return observable.Concat(finallyObservable);
    }
}