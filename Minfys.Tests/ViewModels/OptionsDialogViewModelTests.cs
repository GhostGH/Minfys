using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Minfys.Models.Options;
using Minfys.Services;
using Minfys.ViewModels.Dialogs;
using NSubstitute;

namespace Minfys.Tests.ViewModels;

public class OptionsDialogViewModelTests
{
    private readonly OptionsDialogViewModel _viewModel;
    private ILogger<OptionsDialogViewModel> _logger;
    private AutoLaunchService _autoLaunchService;
    private IOptionsService _optionsService;
    private IOptionsMonitor<AudioOptions> _audioOptionsMonitor;
    private IOptionsMonitor<TimerOptions> _timerModesOptionsMonitor;
    private IOptionsMonitor<SystemOptions> _systemOptionsMonitor;

    public OptionsDialogViewModelTests()
    {
        _logger = Substitute.For<ILogger<OptionsDialogViewModel>>();
        _autoLaunchService = Substitute.For<AutoLaunchService>();
        _optionsService = Substitute.For<IOptionsService>();
        _audioOptionsMonitor = Substitute.For<IOptionsMonitor<AudioOptions>>();
        _timerModesOptionsMonitor = Substitute.For<IOptionsMonitor<TimerOptions>>();
        _systemOptionsMonitor = Substitute.For<IOptionsMonitor<SystemOptions>>();

        var audioOptions = new AudioOptions
        {
            FilePath = "song.mp4",
            LoopEnabled = true,
            Volume = 0.3f
        };

        var timerModesOptions = new TimerOptions
        {
            TimerInterval = TimeSpan.FromSeconds(10),
            TimerMode = TimerOptions.TimerModesEnum.Single
        };

        var systemOptions = new SystemOptions
        {
            EnableCloseToTray = true,
            EnableAutoLaunch = false
        };

        _audioOptionsMonitor.CurrentValue.Returns(audioOptions);
        _timerModesOptionsMonitor.CurrentValue.Returns(timerModesOptions);
        _systemOptionsMonitor.CurrentValue.Returns(systemOptions);

        _viewModel = new OptionsDialogViewModel(_logger, _autoLaunchService, _optionsService, _audioOptionsMonitor,
            _timerModesOptionsMonitor, _systemOptionsMonitor);
    }

    [Fact]
    public void TimerModeOptionChanged_IfNewModeIsLooping_DisableLoopOption()
    {
        // Arrange
        _viewModel.TimerMode = TimerOptions.TimerModesEnum.Looping;
        _viewModel.IsLoopOptionAvailable = true;
        _viewModel.LoopEnabled = true;

        // Act
        _viewModel.TimerModeOptionChangedCommand.Execute(null);

        // Assert
        _viewModel.IsLoopOptionAvailable.Should().Be(false);
        _viewModel.LoopEnabled.Should().Be(false);
    }

    [Fact]
    public void TimerModeOptionChanged_IfNewModeIsNotLooping_EnableLoopOption()
    {
        // Arrange
        _viewModel.TimerMode = TimerOptions.TimerModesEnum.Single;

        // Act
        _viewModel.TimerModeOptionChangedCommand.Execute(null);

        // Assert
        _viewModel.IsLoopOptionAvailable.Should().Be(true);
    }

    [Fact]
    public void SaveSettings_OnButtonPress_InvokeWindowClosingEvent()
    {
        // Arrange
        var eventRaised = false;
        _viewModel.RequestClose += (_, _) => eventRaised = true;

        // Act
        _viewModel.SaveSettingsCommand.Execute(null);

        // Assert
        eventRaised.Should().Be(true);
    }

    [Fact]
    public void CloseWindow_OnButtonPress_InvokeWindowClosingEvent()
    {
        // Arrange
        var eventRaised = false;
        _viewModel.RequestClose += (_, _) => eventRaised = true;

        // Act
        _viewModel.CloseWindowCommand.Execute(null);

        // Assert
        eventRaised.Should().Be(true);
    }
}