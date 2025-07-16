using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;

namespace Minfys.ViewModels.Dialogs;

public partial class TimerFiredDialogViewModel : ViewModelBase, IRequestCloseViewModel<bool>
{
    private readonly ILogger<TimerFiredDialogViewModel> _logger;

    public TimerFiredDialogViewModel(ILogger<TimerFiredDialogViewModel> logger)
    {
        _logger = logger;

        _logger.LogInformation("{ViewModel} created", nameof(TimerFiredDialogViewModel));
    }

    [RelayCommand]
    private void ResetTimer()
    {
        _logger.LogInformation("In {ViewModel}, user pressed button to reset timer", nameof(TimerFiredDialogViewModel));

        RequestClose?.Invoke(this, new RequestCloseDialogEventArgs<bool>(true, true));
    }

    [RelayCommand]
    private void StopTimer()
    {
        _logger.LogInformation("In {ViewModel}, user pressed button to stop timer", nameof(TimerFiredDialogViewModel));

        RequestClose?.Invoke(this, new RequestCloseDialogEventArgs<bool>(false, false));
    }

    public event EventHandler<RequestCloseDialogEventArgs<bool>>? RequestClose;
}