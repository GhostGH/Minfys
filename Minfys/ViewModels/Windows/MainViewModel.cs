using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Minfys.Models;
using Minfys.Services;
using NAudio.Wave;

namespace Minfys.ViewModels.Windows;

public partial class MainViewModel : ViewModelBase
{
    private readonly ILogger<MainViewModel> _logger;
    private readonly IMessageService _messageService;
    private readonly IDialogService _dialogService;
    private readonly DispatcherTimer _timer;
    private readonly AudioOptions _audioOptions;
    private string _filePath;
    private bool _loopEnabled;
    private float _audioVolume;
    private WaveOut? _waveOut;
    private LoopStream? _loopStream;
    private AudioFileReader? _audioFileReader;

    private DateTime _endTime;

    // Used for proper time calculations
    private TimeSpan _timeRemaining;

    // User-defined timer interval. This can only be changed inside the command
    [ObservableProperty] private TimeSpan _currentInterval = TimeSpan.FromSeconds(2);

    // Is used to display time in the UI
    [ObservableProperty] private string _displayTime;

    public MainViewModel(ILogger<MainViewModel> logger,
        IMessageService messageService, IDialogService dialogService,
        IOptionsMonitor<AudioOptions> audioOptions)
    {
        _logger = logger;
        _messageService = messageService;
        _dialogService = dialogService;
        _audioOptions = audioOptions.CurrentValue;
        _filePath = _audioOptions.FilePath;
        _loopEnabled = _audioOptions.LoopEnabled;
        _audioVolume = _audioOptions.Volume;

        _timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(1)
        };
        _timeRemaining = CurrentInterval;
        DisplayTime = _timeRemaining.ToString(@"hh\:mm\:ss");

        _timer.Tick += TimerOnTick;
        audioOptions.OnChange(AudioOptionsUpdated);
        _logger.LogInformation("{ViewModel} created", nameof(MainViewModel));
    }

    [RelayCommand]
    private void ChangeInterval()
    {
        var result = _dialogService.ShowDialog<ChangeTimerIntervalDialogViewModel, TimeSpan?>();
        _logger.LogInformation("Result received: {Result}", result);

        if (result.Result == null)
        {
            return;
        }
        else
        {
            CurrentInterval = (TimeSpan)result.Result;
        }

        _timeRemaining = CurrentInterval;
        DisplayTime = _timeRemaining.ToString(@"hh\:mm\:ss");
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
        _logger.LogInformation("Timer has been forcefully stopped with remaining time {Time}", _timeRemaining);
        _timeRemaining = CurrentInterval;
        DisplayTime = _timeRemaining.ToString(@"hh\:mm\:ss");
    }

    [RelayCommand]
    private void OpenOptions()
    {
        _dialogService.ShowDialog<OptionsDialogViewModel, object>();
    }

    private void PlaySound(bool loopEnabled)
    {
        if (_waveOut != null)
        {
            StopSound();
        }

        _audioFileReader = new AudioFileReader(_filePath);
        _audioFileReader.Volume = _audioVolume;
        _loopStream = new LoopStream(_audioFileReader)
        {
            EnableLooping = loopEnabled
        };

        _waveOut = new WaveOut();
        _waveOut.Init(_loopStream);
        _waveOut.Play();
    }

    private void StopSound()
    {
        _waveOut?.Stop();
        _waveOut?.Dispose();
        _waveOut = null;

        _loopStream?.Dispose();
        _loopStream = null;

        _audioFileReader?.Dispose();
        _audioFileReader = null;
    }

    private void TimerOnTick(object? sender, EventArgs e)
    {
        _timeRemaining = _endTime - DateTime.Now;
        if (_timeRemaining < TimeSpan.Zero)
        {
            _timeRemaining = TimeSpan.Zero;
            PlaySound(_loopEnabled);
            _timer.Stop();
            _logger.LogInformation("Timer fired");
            _timeRemaining = CurrentInterval;
        }

        DisplayTime = _timeRemaining.ToString(@"hh\:mm\:ss");
    }

    private void AudioOptionsUpdated(AudioOptions arg1, string? arg2)
    {
        _loopEnabled = arg1.LoopEnabled;
        _audioVolume = arg1.Volume;
    }
}