using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Minfys.Models.Options;
using Minfys.Services;

namespace Minfys.ViewModels.Dialogs;

public partial class OptionsDialogViewModel : ViewModelBase, IRequestCloseViewModel<object>
{
    private readonly ILogger<OptionsDialogViewModel> _logger;
    private readonly IOptionsService _optionsService;
    private readonly AudioOptions _audioOptions;
    private readonly TimerModesOptions _timerModesOptions;
    private readonly SystemOptions _systemOptions;

    [ObservableProperty] private string? _songPath;
    [ObservableProperty] private bool _loopEnabled;
    [ObservableProperty] private bool _isLoopOptionAvailable;
    [ObservableProperty] private float _audioVolume;
    [ObservableProperty] private bool _trayEnabled;

    public float AudioVolumePercent
    {
        get => AudioVolume * 100;
        set
        {
            AudioVolume = value / 100;
            OnPropertyChanged();
        }
    }

    [ObservableProperty] private TimerModesOptions.TimerModesEnum _timerMode;

    public OptionsDialogViewModel(ILogger<OptionsDialogViewModel> logger, IOptionsService optionsService,
        IOptionsMonitor<AudioOptions> audioOptions, IOptionsMonitor<TimerModesOptions> timerModsOptions,
        IOptionsMonitor<SystemOptions> systemOptions)
    {
        _logger = logger;
        _optionsService = optionsService;
        _audioOptions = audioOptions.CurrentValue;
        _timerModesOptions = timerModsOptions.CurrentValue;
        _systemOptions = systemOptions.CurrentValue;

        _songPath = _audioOptions.FilePath;
        _loopEnabled = _audioOptions.LoopEnabled;
        _audioVolume = _audioOptions.Volume;
        _trayEnabled = _systemOptions.EnableCloseToTray;

        _timerMode = _timerModesOptions.TimerMode;
        // lol
        _isLoopOptionAvailable = (TimerMode != TimerModesOptions.TimerModesEnum.Looping);

        _logger.LogInformation("{ViewModel} created", nameof(OptionsDialogViewModel));
    }

    [RelayCommand]
    private void TimerModeOptionChanged()
    {
        if (TimerMode == TimerModesOptions.TimerModesEnum.Looping)
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

        _timerModesOptions.TimerMode = TimerMode;
    }

    [RelayCommand]
    private void LoopOptionChanged()
    {
        _audioOptions.LoopEnabled = LoopEnabled;
    }

    [RelayCommand]
    private void TrayOptionChanged()
    {
        _systemOptions.EnableCloseToTray = TrayEnabled;
    }

    [RelayCommand]
    private void SaveSettings()
    {
        // It's here because Slider and NAudio use different variables, so no binding for this one
        _audioOptions.Volume = AudioVolume;

        _optionsService.Save(_timerModesOptions, TimerModesOptions.Key);
        _optionsService.Save(_audioOptions, AudioOptions.Key);
        _optionsService.Save(_systemOptions, SystemOptions.Key);

        CloseWindow();
    }

    [RelayCommand]
    private void CloseWindow()
    {
        _logger.LogInformation("Closing settings");
        RequestClose?.Invoke(this, new RequestCloseDialogEventArgs<object>(true));
    }

    public event EventHandler<RequestCloseDialogEventArgs<object>>? RequestClose;
}