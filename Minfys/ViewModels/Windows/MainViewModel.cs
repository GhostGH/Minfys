using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Minfys.Services;

namespace Minfys.ViewModels.Windows;

public partial class MainViewModel : ViewModelBase
{
    private readonly ILogger<MainViewModel> _logger;
    private readonly IMessageService _messageService;
    private readonly IDialogService _dialogService;
    private DispatcherTimer _timer;
    private DateTime _endTime;

    // Used for proper time calculations
    private TimeSpan TimeRemaining { get; set; }

    // User-defined timer interval. This can only be changed inside the command
    [ObservableProperty] private TimeSpan _currentInterval = TimeSpan.FromSeconds(5);

    // Is used to display time in the UI
    [ObservableProperty] private string _displayTime;

    public MainViewModel(ILogger<MainViewModel> logger,
        IMessageService messageService, IDialogService dialogService)
    {
        _logger = logger;
        _messageService = messageService;
        _dialogService = dialogService;

        _timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(1)
        };
        TimeRemaining = CurrentInterval;
        DisplayTime = TimeRemaining.ToString(@"hh\:mm\:ss");

        _timer.Tick += TimerOnTick;
        _logger.LogInformation("{ViewModel} created", nameof(MainViewModel));
    }

    [RelayCommand]
    private void ChangeInterval()
    {
        var result = _dialogService.ShowDialog<ChangeTimerIntervalDialogViewModel, TimeSpan>();
        _logger.LogInformation("Result received: {Result}", result);
        CurrentInterval = result.Result;
        TimeRemaining = CurrentInterval;
        DisplayTime = TimeRemaining.ToString(@"hh\:mm\:ss");
    }

    [RelayCommand]
    private void StartTimer()
    {
        _endTime = DateTime.Now.Add(CurrentInterval);
        _timer.Start();
        _logger.LogInformation("Timer started with interval {Interval}", CurrentInterval);
    }

    [RelayCommand]
    private void StopTimer()
    {
        _timer.Stop();
        _logger.LogInformation("Timer has been forcefully stopped with remaining time {Time}", TimeRemaining);
        TimeRemaining = CurrentInterval;
        DisplayTime = TimeRemaining.ToString(@"hh\:mm\:ss");
    }

    private void TimerOnTick(object? sender, EventArgs e)
    {
        TimeRemaining = _endTime - DateTime.Now;
        if (TimeRemaining < TimeSpan.Zero)
        {
            TimeRemaining = TimeSpan.Zero;
            _timer.Stop();
            _logger.LogInformation("Timer fired");
            TimeRemaining = CurrentInterval;
        }

        DisplayTime = TimeRemaining.ToString(@"hh\:mm\:ss");
    }
}