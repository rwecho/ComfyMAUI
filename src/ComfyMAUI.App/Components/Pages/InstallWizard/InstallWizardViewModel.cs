using System.Reactive.Subjects;

namespace ComfyMAUI.Components.Pages.InstallWizard;


public class InstallWizardViewModel()
{
    private readonly BehaviorSubject<int> _currentSubject = new(0);

    public IObservable<int> CurrentSubject => _currentSubject;

    public Task OnNext()
    {
        if (_currentSubject.Value == 4)
        {
            return Task.CompletedTask;
        }

        _currentSubject.OnNext(_currentSubject.Value + 1);

        return Task.CompletedTask;
    }
}
