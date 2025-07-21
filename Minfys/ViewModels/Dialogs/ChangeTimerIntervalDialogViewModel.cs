using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Minfys.Models.Options;
using Minfys.Services;

namespace Minfys.ViewModels.Dialogs;

public partial class ChangeTimerIntervalDialogViewModel : ViewModelBase, IRequestCloseViewModel<TimeSpan?>
{
    private readonly ILogger<ChangeTimerIntervalDialogViewModel> _logger;
    private readonly IMessageService _messageService;
    private readonly IOptionsService _optionsService;
    private readonly TimerOptions _timerOptions;

    [ObservableProperty] private TimeSpan _timerInterval;
    [ObservableProperty] private bool? _dialogResult;

    public ChangeTimerIntervalDialogViewModel(ILogger<ChangeTimerIntervalDialogViewModel> logger,
        IMessageService messageService, IOptionsService optionsService, IOptionsMonitor<TimerOptions> timerOptions)
    {
        _logger = logger;
        _messageService = messageService;
        _optionsService = optionsService;
        _timerOptions = timerOptions.CurrentValue;

        _logger.LogInformation("{ViewModel} created", nameof(ChangeTimerIntervalDialogViewModel));
    }

    [RelayCommand]
    private void AcceptChange(string? textValue)
    {
        if (TimeSpan.TryParse(textValue, out var ts))
        {
            _timerOptions.TimerInterval = ts;
            _optionsService.Save(_timerOptions, TimerOptions.Key);
            _logger.LogInformation("Interval change successful with a value {NewInterval}", textValue);
            RequestClose?.Invoke(this, new RequestCloseDialogEventArgs<TimeSpan?>(true, ts));
        }
        else
        {
            _messageService.ShowError("Invalid time interval value. Try again.");
            _logger.LogInformation("Interval change unsuccessful with the value: {NewInterval}", textValue);
        }
    }

    [RelayCommand]
    private void CancelChange()
    {
        _logger.LogInformation("Interval change was cancelled by the user");
        RequestClose?.Invoke(this, new RequestCloseDialogEventArgs<TimeSpan?>(false, null));
    }

    public event EventHandler<RequestCloseDialogEventArgs<TimeSpan?>>? RequestClose;
}