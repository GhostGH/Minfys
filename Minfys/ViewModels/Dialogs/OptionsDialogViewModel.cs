using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Minfys.Models.Options;
using Minfys.Services;

namespace Minfys.ViewModels.Dialogs;

/// <summary>
/// Manages dialog window for changing app settings using OptionsService.
/// </summary>
public partial class OptionsDialogViewModel : ViewModelBase, IRequestCloseViewModel<object>
{
    private readonly ILogger<OptionsDialogViewModel> _logger;
    private readonly AutoLaunchService _autoLaunchService;
    private readonly IOptionsService _optionsService;
    private readonly AudioOptions _audioOptions;
    private readonly TimerOptions _timerOptions;
    private readonly SystemOptions _systemOptions;

    [ObservableProperty] private string? _songPath;

    /// <summary>
    /// Indicates whether song loop option is enabled.
    /// </summary>
    [ObservableProperty] private bool _loopEnabled;

    /// <summary>
    /// Indicates whether to enable in UI checkbox for song loop.
    /// </summary>
    [ObservableProperty] private bool _isLoopOptionAvailable;

    [ObservableProperty] private float _audioVolume;
    [ObservableProperty] private bool _trayEnabled;
    [ObservableProperty] private bool _autoLaunchEnabled;

    public float AudioVolumePercent
    {
        get => AudioVolume * 100;
        set
        {
            AudioVolume = value / 100;
            OnPropertyChanged();
        }
    }

    [ObservableProperty] private TimerOptions.TimerModesEnum _timerMode;

    public OptionsDialogViewModel(ILogger<OptionsDialogViewModel> logger, AutoLaunchService autoLaunchService,
        IOptionsService optionsService, IOptionsMonitor<AudioOptions> audioOptions,
        IOptionsMonitor<TimerOptions> timerModsOptions, IOptionsMonitor<SystemOptions> systemOptions)
    {
        _logger = logger;
        _autoLaunchService = autoLaunchService;
        _optionsService = optionsService;
        _audioOptions = audioOptions.CurrentValue;
        _timerOptions = timerModsOptions.CurrentValue;
        _systemOptions = systemOptions.CurrentValue;

        _songPath = _audioOptions.FilePath;
        _loopEnabled = _audioOptions.LoopEnabled;
        _audioVolume = _audioOptions.Volume;
        _trayEnabled = _systemOptions.EnableCloseToTray;
        _autoLaunchEnabled = _systemOptions.EnableAutoLaunch;

        _timerMode = _timerOptions.TimerMode;
        // lol
        _isLoopOptionAvailable = (TimerMode != TimerOptions.TimerModesEnum.Looping);

        _logger.LogInformation("{ViewModel} created", nameof(OptionsDialogViewModel));
    }

    /// <summary>
    /// Processes "Timer Mode" option change — updates value in configuration.
    /// Manages availability of audio loop option in the UI, depending on timer mode value.
    /// </summary>
    [RelayCommand]
    private void TimerModeOptionChanged()
    {
        _logger.LogInformation("Timer mode changed | ViewModel: {ViewModel}, NewValue: {TimerMode}",
            nameof(OptionsDialogViewModel), TimerMode);

        if (TimerMode == TimerOptions.TimerModesEnum.Looping)
        {
            IsLoopOptionAvailable = false;
            LoopEnabled = false;
            _audioOptions.LoopEnabled = LoopEnabled;
        }
        else
        {
            IsLoopOptionAvailable = true;
            _audioOptions.LoopEnabled = LoopEnabled;
        }

        _timerOptions.TimerMode = TimerMode;
    }

    /// <summary>
    /// Processes "Loop" option change — updates value in configuration.
    /// </summary>
    [RelayCommand]
    private void LoopOptionChanged()
    {
        _logger.LogInformation("Loop option changed | ViewModel: {ViewModel}, NewValue: {LoopEnabled}",
            nameof(OptionsDialogViewModel), LoopEnabled);

        _audioOptions.LoopEnabled = LoopEnabled;
    }

    /// <summary>
    /// Processes "Close to Tray" option change — updates value in configuration.
    /// </summary>
    [RelayCommand]
    private void TrayOptionChanged()
    {
        _logger.LogInformation("Tray option changed | ViewModel: {ViewModel}, NewValue: {TrayEnabled}",
            nameof(OptionsDialogViewModel), TrayEnabled);

        _systemOptions.EnableCloseToTray = TrayEnabled;
    }

    /// <summary>
    /// Processes "Auto Launch" option change — updates value in configuration.
    /// </summary>
    [RelayCommand]
    private void AutoLaunchOptionChanged()
    {
        _logger.LogInformation("Auto launch option changed | ViewModel: {ViewModel}, NewValue: {AutoLaunchEnabled}",
            nameof(OptionsDialogViewModel), AutoLaunchEnabled);

        _systemOptions.EnableAutoLaunch = AutoLaunchEnabled;
    }

    /// <summary>
    /// Saves all settings to userPreferences.json.
    /// </summary>
    [RelayCommand]
    private void SaveSettings()
    {
        _logger.LogInformation("User pressed button to save settings | ViewModel: {ViewModel}",
            nameof(OptionsDialogViewModel));

        // It's here because Slider and NAudio use different variables, so no binding property for this one
        _audioOptions.Volume = AudioVolume;

        _autoLaunchService.SetAutoLaunch(AutoLaunchEnabled);

        _optionsService.Save(_timerOptions, TimerOptions.Key);
        _optionsService.Save(_audioOptions, AudioOptions.Key);
        _optionsService.Save(_systemOptions, SystemOptions.Key);

        CloseWindow();
    }

    /// <summary>
    /// Closes the dialog window.
    /// </summary>
    [RelayCommand]
    private void CloseWindow()
    {
        _logger.LogInformation("Closing settings | ViewModel: {ViewModel}", nameof(OptionsDialogViewModel));

        RequestClose?.Invoke(this, new RequestCloseDialogEventArgs<object>(true));
    }

    /// <summary>
    /// Executes on dialog close. Contains dialog result and new interval value.
    /// </summary>
    public event EventHandler<RequestCloseDialogEventArgs<object>>? RequestClose;
}