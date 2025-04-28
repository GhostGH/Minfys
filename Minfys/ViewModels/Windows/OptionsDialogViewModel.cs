using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Minfys.Models;
using Minfys.Services;

namespace Minfys.ViewModels.Windows;

public partial class OptionsDialogViewModel : ViewModelBase, IRequestCloseViewModel<object>
{
    private readonly ILogger<OptionsDialogViewModel> _logger;
    private readonly AudioOptions _audioOptions;

    [ObservableProperty] private bool _loopEnabled;

    public OptionsDialogViewModel(ILogger<OptionsDialogViewModel> logger,
        IOptionsMonitor<AudioOptions> audioOptions)
    {
        _logger = logger;
        _audioOptions = audioOptions.CurrentValue;

        _loopEnabled = _audioOptions.LoopEnabled;

        audioOptions.OnChange(AudioOptionsUpdates);

        _logger.LogInformation("{ViewModel} created", nameof(OptionsDialogViewModel));
    }

    private void AudioOptionsUpdates(AudioOptions arg1, string? arg2)
    {
    }

    public event EventHandler<RequestCloseDialogEventArgs<object>>? RequestClose;
}