using System.IO;
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

/// <summary>
/// Manages main application window with timer.
/// </summary>
public partial class MainViewModel : ViewModelBase
{
    private const string TimeFormat = @"hh\:mm\:ss";

    private readonly ILogger<MainViewModel> _logger;
    private readonly IResourceService _resourceService;
    private readonly IMessageService _messageService;
    private readonly IDialogService _dialogService;
    private readonly DispatcherTimer _timer;
    private readonly Uri _defaultAudioFilePath;

    private WaveOut? _waveOut;
    private LoopStream? _loopStream;
    private Mp3FileReader? _audioFileReader;
    private Stream? _stream;

    private TimerOptions.TimerModesEnum _timerMode;
    private bool _loopEnabled;
    private float _audioVolume;
    private int _remainingSeconds; // Used for time calculations

    public enum StartButtonState
    {
        Start,
        Pause,
        Resume
    }

    [ObservableProperty] private StartButtonState _currentStartButtonState;
    [ObservableProperty] private bool _stopTimerButtonEnabled;
    [ObservableProperty] private TimeSpan _currentInterval; // User-defined timer interval
    [ObservableProperty] private string _displayTime; // Is used to display time in the UI

    public MainViewModel(ILogger<MainViewModel> logger, IResourceService resourceService,
        IMessageService messageService, IDialogService dialogService,
        IOptionsMonitor<AudioOptions> audioOptions, IOptionsMonitor<TimerOptions> timerModesOptions)
    {
        _logger = logger;
        _resourceService = resourceService;
        _messageService = messageService;
        _dialogService = dialogService;
        AudioOptions currentAudioOptions = audioOptions.CurrentValue;
        TimerOptions currentTimerOptions = timerModesOptions.CurrentValue;

        _currentInterval = currentTimerOptions.TimerInterval;
        _loopEnabled = currentAudioOptions.LoopEnabled;
        _audioVolume = currentAudioOptions.Volume;
        _timerMode = currentTimerOptions.TimerMode;

        CurrentStartButtonState = StartButtonState.Start;
        StopTimerButtonEnabled = false;

        _defaultAudioFilePath = new Uri("pack://application:,,,/Minfys;component/Assets/Audio/default_song.mp3",
            UriKind.Absolute);

        _timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(1)
        };

        _remainingSeconds = (int)CurrentInterval.TotalSeconds;
        DisplayTime = TimeSpan.FromSeconds(_remainingSeconds).ToString(TimeFormat);

        _timer.Tick += TimerOnTick;
        audioOptions.OnChange(AudioOptionsUpdated);
        timerModesOptions.OnChange(TimerOptionsUpdated);

        _logger.LogInformation("{ViewModel} created", nameof(MainViewModel));
    }

    /// <summary>
    /// Opens dialog for changing timer interval value. Updates the interval, if the value is new.
    /// </summary>
    [RelayCommand]
    private void ChangeInterval()
    {
        _logger.LogInformation("Command: {Command} activated", nameof(ChangeIntervalCommand));

        var result = _dialogService.ShowDialog<ChangeTimerIntervalDialogViewModel, TimeSpan?>();

        if (result.Result != null)
        {
            CurrentInterval = (TimeSpan)result.Result;

            if (!_timer.IsEnabled)
            {
                _remainingSeconds = (int)CurrentInterval.TotalSeconds;
                DisplayTime = TimeSpan.FromSeconds(_remainingSeconds).ToString(TimeFormat);
            }
        }

        _logger.LogInformation("Command: {Command} | Result: {Result}", nameof(ChangeIntervalCommand), result);
    }

    /// <summary>
    /// Starts the timer.
    /// </summary>
    [RelayCommand]
    private void StartTimer()
    {
        if (StopTimerButtonEnabled == false)
            StopTimerButtonEnabled = true;

        switch (CurrentStartButtonState)
        {
            case StartButtonState.Start:
                _remainingSeconds = (int)CurrentInterval.TotalSeconds;
                _timer.Start();
                _logger.LogInformation("Timer started with interval {Interval}", CurrentInterval);
                CurrentStartButtonState = StartButtonState.Pause;
                break;
            case StartButtonState.Pause:
                _timer.Stop();
                CurrentStartButtonState = StartButtonState.Resume;
                break;
            case StartButtonState.Resume:
                _timer.Start();
                CurrentStartButtonState = StartButtonState.Pause;
                break;
        }
    }

    /// <summary>
    /// Stops the timer.
    /// </summary>
    [RelayCommand]
    private void StopTimer()
    {
        _timer.Stop();
        _logger.LogInformation("Timer has been forcefully stopped with remaining time {Time}", _remainingSeconds);
        _remainingSeconds = (int)CurrentInterval.TotalSeconds;
        DisplayTime = TimeSpan.FromSeconds(_remainingSeconds).ToString(TimeFormat);
        CurrentStartButtonState = StartButtonState.Start;
        StopTimerButtonEnabled = false;
    }

    /// <summary>
    /// Opens settings dialog.
    /// </summary>
    [RelayCommand]
    private void OpenOptions()
    {
        _dialogService.ShowDialog<OptionsDialogViewModel, object>();
    }

    #region Event Handlers

    private void TimerOnTick(object? sender, EventArgs e)
    {
        _remainingSeconds--;
        DisplayTime = TimeSpan.FromSeconds(_remainingSeconds).ToString(TimeFormat);

        if (_remainingSeconds <= 0)
        {
            TimerFire();
        }
    }

    private void AudioOptionsUpdated(AudioOptions arg1, string? arg2)
    {
        _loopEnabled = arg1.LoopEnabled;
        _audioVolume = arg1.Volume;
    }

    private void TimerOptionsUpdated(TimerOptions arg1, string? arg2)
    {
        _timerMode = arg1.TimerMode;

        if (CurrentInterval == arg1.TimerInterval) return;
        CurrentInterval = arg1.TimerInterval;
        if (!_timer.IsEnabled)
        {
            _remainingSeconds = (int)CurrentInterval.TotalSeconds;
            DisplayTime = TimeSpan.FromSeconds(_remainingSeconds).ToString(TimeFormat);
        }
    }

    #endregion

    #region Private Methods

    private void TimerFire()
    {
        _logger.LogInformation("Timer fired");

        _timer.Stop();
        _remainingSeconds = 0;
        _remainingSeconds = (int)CurrentInterval.TotalSeconds;
        DisplayTime = TimeSpan.FromSeconds(_remainingSeconds).ToString(TimeFormat);
        CurrentStartButtonState = StartButtonState.Start;
        StopTimerButtonEnabled = false;
        PlaySound(_loopEnabled);

        if (_timerMode == TimerOptions.TimerModesEnum.Looping)
        {
            StartTimer();
        }
        else if (_timerMode == TimerOptions.TimerModesEnum.Single)
        {
            var result = _dialogService.ShowDialog<TimerFiredDialogViewModel, bool>();

            if (result.Result == false)
            {
                StopSound();
            }
            else
            {
                StopSound();
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

        try
        {
            _stream = _resourceService.GetResourceStream(_defaultAudioFilePath);

            _audioFileReader = new Mp3FileReader(_stream);

            _loopStream = new LoopStream(_audioFileReader) { EnableLooping = loopEnabled };
            var loopChanel = new WaveChannel32(_loopStream)
            {
                Volume = _audioVolume
            };

            _waveOut = new WaveOut();
            _waveOut.Init(loopChanel);
            _waveOut.Play();
        }
        catch (Exception ex)
        {
            StopSound();
            _logger.LogCritical(ex,
                "Probably failed to play audio on timer fired: the resource stream for default audio file is null somehow");
            _messageService.ShowError("An Error has occured. See application logs in ~App Data/Roaming/Minfys/logs");
        }
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

        _stream?.Dispose();
        _stream = null;
    }

    #endregion
}