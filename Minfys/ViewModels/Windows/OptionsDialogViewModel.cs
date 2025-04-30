using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Minfys.Models;
using Minfys.Services;

namespace Minfys.ViewModels.Windows;

public partial class OptionsDialogViewModel : ViewModelBase, IRequestCloseViewModel<object>
{
    private readonly ILogger<OptionsDialogViewModel> _logger;
    private readonly IOptionsService _optionsService;
    private readonly AudioOptions _audioOptions;

    [ObservableProperty] private string? _songPath;
    [ObservableProperty] private bool _loopEnabled;

    public OptionsDialogViewModel(ILogger<OptionsDialogViewModel> logger, IOptionsService optionsService,
        IOptionsMonitor<AudioOptions> audioOptions)
    {
        _logger = logger;
        _optionsService = optionsService;
        _audioOptions = audioOptions.CurrentValue;

        _songPath = _audioOptions.FilePath;
        _loopEnabled = _audioOptions.LoopEnabled;

        audioOptions.OnChange(AudioOptionsUpdates);

        _logger.LogInformation("{ViewModel} created", nameof(OptionsDialogViewModel));
    }

    [RelayCommand]
    private void LoopOptionChanged()
    {
        _audioOptions.LoopEnabled = LoopEnabled;
        _optionsService.Save(_audioOptions, AudioOptions.Key);
    }

    private void AudioOptionsUpdates(AudioOptions arg1, string? arg2)
    {
    }

    public event EventHandler<RequestCloseDialogEventArgs<object>>? RequestClose;
}