using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Minfys.Models.NAudio;
using Minfys.Models.Options;
using Minfys.Services;
using Minfys.ViewModels.Dialogs;
using NAudio.Wave;

namespace Minfys.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private readonly ILogger<MainViewModel> _logger;
    private readonly IDialogService _dialogService;
    private readonly DispatcherTimer _timer;

    private WaveOut? _waveOut;
    private LoopStream? _loopStream;
    private AudioFileReader? _audioFileReader;
    private TimerModesOptions.TimerModesEnum _timerMode;
    private string _filePath;
    private bool _loopEnabled;
    private float _audioVolume;

    // Used for time calculations
    private int _remainingSeconds;

    // User-defined timer interval
    [ObservableProperty] private TimeSpan _currentInterval = TimeSpan.FromSeconds(7);

    // Is used to display time in the UI
    [ObservableProperty] private string _displayTime;

    public MainViewModel(ILogger<MainViewModel> logger, IDialogService dialogService,
        IOptionsMonitor<AudioOptions> audioOptions, IOptionsMonitor<TimerModesOptions> timerModesOptions)
    {
        _logger = logger;
        _dialogService = dialogService;
        AudioOptions currentAudioOptions = audioOptions.CurrentValue;
        TimerModesOptions currentTimerModesOptions = timerModesOptions.CurrentValue;

        _filePath = currentAudioOptions.FilePath;
        _loopEnabled = currentAudioOptions.LoopEnabled;
        _audioVolume = currentAudioOptions.Volume;
        _timerMode = currentTimerModesOptions.TimerMode;

        _timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(1)
        };

        _remainingSeconds = (int)CurrentInterval.TotalSeconds;
        DisplayTime = TimeSpan.FromSeconds(_remainingSeconds).ToString(@"hh\:mm\:ss");

        _timer.Tick += TimerOnTick;
        audioOptions.OnChange(AudioOptionsUpdated);
        timerModesOptions.OnChange(TimerModesOptionsUpdated);

        _logger.LogInformation("{ViewModel} created", nameof(MainViewModel));
    }

    [RelayCommand]
    private void ChangeInterval()
    {
        var result = _dialogService.ShowDialog<ChangeTimerIntervalDialogViewModel, TimeSpan?>();
        _logger.LogInformation("Result received: {Result}", result);

        if (result.Result != null)
        {
            CurrentInterval = (TimeSpan)result.Result;
            _remainingSeconds = (int)CurrentInterval.TotalSeconds;
            DisplayTime = TimeSpan.FromSeconds(_remainingSeconds).ToString(@"hh\:mm\:ss");
        }
    }

    [RelayCommand]
    private void StartTimer()
    {
        _remainingSeconds = (int)CurrentInterval.TotalSeconds;
        _timer.Start();
        _logger.LogInformation("Timer started with interval {Interval}", CurrentInterval);
    }

    [RelayCommand]
    private void StopTimer()
    {
        _timer.Stop();
        _logger.LogInformation("Timer has been forcefully stopped with remaining time {Time}", _remainingSeconds);
        _remainingSeconds = (int)CurrentInterval.TotalSeconds;
        DisplayTime = TimeSpan.FromSeconds(_remainingSeconds).ToString(@"hh\:mm\:ss");
    }

    private void TimerFire()
    {
        _logger.LogInformation("Timer fired");

        _timer.Stop();
        _remainingSeconds = 0;
        _remainingSeconds = (int)CurrentInterval.TotalSeconds;
        PlaySound(_loopEnabled);

        if (_timerMode == TimerModesOptions.TimerModesEnum.Looping)
        {
            DisplayTime = TimeSpan.FromSeconds(_remainingSeconds).ToString(@"hh\:mm\:ss");
            StartTimer();
        }
        else if (_timerMode == TimerModesOptions.TimerModesEnum.Single)
        {
            var result = _dialogService.ShowDialog<TimerFiredDialogViewModel, bool>();

            if (result.Result == false)
            {
                StopSound();
                DisplayTime = TimeSpan.FromSeconds(_remainingSeconds).ToString(@"hh\:mm\:ss");
            }
            else
            {
                StopSound();
                DisplayTime = TimeSpan.FromSeconds(_remainingSeconds).ToString(@"hh\:mm\:ss");
                StartTimer();
            }
        }
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

    [RelayCommand]
    private void OpenOptions()
    {
        _dialogService.ShowDialog<OptionsDialogViewModel, object>();
    }

    private void TimerOnTick(object? sender, EventArgs e)
    {
        _remainingSeconds--;
        DisplayTime = TimeSpan.FromSeconds(_remainingSeconds).ToString(@"hh\:mm\:ss");

        if (_remainingSeconds <= 0)
        {
            TimerFire();
        }
    }

    private void AudioOptionsUpdated(AudioOptions arg1, string? arg2)
    {
        _filePath = arg1.FilePath;
        _loopEnabled = arg1.LoopEnabled;
        _audioVolume = arg1.Volume;
    }

    private void TimerModesOptionsUpdated(TimerModesOptions arg1, string? arg2)
    {
        _timerMode = arg1.TimerMode;
    }
}