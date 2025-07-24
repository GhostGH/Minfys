using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Minfys.ExtensionMethods.Extensions;

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
        _logger.LogCommandExecution();

        RequestClose?.Invoke(this, new RequestCloseDialogEventArgs<bool>(true, true));

        _logger.LogCommandExecuted();
    }

    /// <summary>
    /// Sends response to the caller View Model to stop the timer.
    /// </summary>
    [RelayCommand]
    private void StopTimer()
    {
        _logger.LogCommandExecution();

        RequestClose?.Invoke(this, new RequestCloseDialogEventArgs<bool>(false, false));

        _logger.LogCommandExecuted();
    }

    /// <summary>
    /// Executes on dialog close. Contains dialog result and new interval value.
    /// </summary>
    public event EventHandler<RequestCloseDialogEventArgs<bool>>? RequestClose;
}