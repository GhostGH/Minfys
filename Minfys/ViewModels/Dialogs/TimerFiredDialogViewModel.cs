using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;

namespace Minfys.ViewModels.Dialogs;

/// <summary>
/// Manages dialog window for when timer is fired.
/// </summary>
public partial class TimerFiredDialogViewModel : ViewModelBase, IRequestCloseViewModel<bool>
{
    private readonly ILogger<TimerFiredDialogViewModel> _logger;

    public TimerFiredDialogViewModel(ILogger<TimerFiredDialogViewModel> logger)
    {
        _logger = logger;

        _logger.LogInformation("{ViewModel} created", nameof(TimerFiredDialogViewModel));
    }

    /// <summary>
    /// Sends response to the caller View Model to reset the timer.
    /// </summary>
    [RelayCommand]
    private void ResetTimer()
    {
        _logger.LogInformation("In {ViewModel}, user pressed button to reset timer", nameof(TimerFiredDialogViewModel));

        RequestClose?.Invoke(this, new RequestCloseDialogEventArgs<bool>(true, true));
    }

    /// <summary>
    /// Sends response to the caller View Model to stop the timer.
    /// </summary>
    [RelayCommand]
    private void StopTimer()
    {
        _logger.LogInformation("In {ViewModel}, user pressed button to stop timer", nameof(TimerFiredDialogViewModel));

        RequestClose?.Invoke(this, new RequestCloseDialogEventArgs<bool>(false, false));
    }

    /// <summary>
    /// Executes on dialog close. Contains dialog result and new interval value.
    /// </summary>
    public event EventHandler<RequestCloseDialogEventArgs<bool>>? RequestClose;
}