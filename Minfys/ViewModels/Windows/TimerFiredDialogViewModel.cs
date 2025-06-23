using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Minfys.Services;

namespace Minfys.ViewModels.Windows;

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
        _logger.LogInformation("User chooses to reset timer");
        RequestClose?.Invoke(this, new RequestCloseDialogEventArgs<bool>(true, true));
    }

    [RelayCommand]
    private void StopTimer()
    {
        RequestClose?.Invoke(this, new RequestCloseDialogEventArgs<bool>(false, false));
    }

    public event EventHandler<RequestCloseDialogEventArgs<bool>>? RequestClose;
}