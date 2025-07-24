using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Minfys.ExtensionMethods.Extensions;
using Minfys.Models.Options;
using Minfys.Services;

namespace Minfys.ViewModels.Dialogs;

/// <summary>
/// Manages dialog window for changing timer interval. Sends new interval value to the caller and saves it to the app settings.
/// </summary>
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

    /// <summary>
    /// Passes new interval value to the caller viewmodel and saves it to the settings.
    /// </summary>
    /// <param name="textValue">New interval value.</param>
    [RelayCommand]
    private void AcceptChange(string? textValue)
    {
        _logger.LogCommandExecution();

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
            _logger.LogError("Interval change unsuccessful with the value: {NewInterval}", textValue);
        }

        _logger.LogCommandExecuted();
    }

    /// <summary>
    /// Cancels interval change and closes the window.
    /// </summary>
    [RelayCommand]
    private void CancelChange()
    {
        _logger.LogCommandExecution();

        RequestClose?.Invoke(this, new RequestCloseDialogEventArgs<TimeSpan?>(false));

        _logger.LogCommandExecuted();
    }

    /// <summary>
    /// Executes on dialog close. Contains dialog result and new interval value.
    /// </summary>
    public event EventHandler<RequestCloseDialogEventArgs<TimeSpan?>>? RequestClose;
}