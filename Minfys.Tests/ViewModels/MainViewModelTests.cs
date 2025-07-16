using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Minfys.Models.Options;
using Minfys.Services;
using Minfys.ViewModels;
using Minfys.ViewModels.Dialogs;
using NSubstitute;

namespace Minfys.Tests.ViewModels;

public class MainViewModelTests
{
    private readonly IOptionsMonitor<AudioOptions> _audioOptions;
    private readonly IDialogService _dialogService;
    private readonly ILogger<MainViewModel> _logger;
    private readonly IOptionsMonitor<TimerModesOptions> _timerModesOptions;
    private readonly MainViewModel _viewModel;

    public MainViewModelTests()
    {
        _logger = Substitute.For<ILogger<MainViewModel>>();
        _dialogService = Substitute.For<IDialogService>();
        _audioOptions = Substitute.For<IOptionsMonitor<AudioOptions>>();
        _timerModesOptions = Substitute.For<IOptionsMonitor<TimerModesOptions>>();


        var audioOptions = new AudioOptions
        {
            FilePath = "song.mp4",
            LoopEnabled = false,
            Volume = 0.3f
        };

        var timerModesOptions = new TimerModesOptions
        {
            TimerMode = TimerModesOptions.TimerModesEnum.Single
        };

        _audioOptions.CurrentValue.Returns(audioOptions);
        _timerModesOptions.CurrentValue.Returns(timerModesOptions);

        _viewModel = new MainViewModel(_logger, _dialogService, _audioOptions, _timerModesOptions);
    }

    [Fact]
    public void ChangeIntervalCommand_ShouldUpdateCurrentIntervalAndDisplayTime_WhenDialogReturnsTrue()
    {
        // Arrange
        bool? dialogResult = true;
        TimeSpan newInterval = TimeSpan.FromMinutes(5);

        _dialogService.ShowDialog<ChangeTimerIntervalDialogViewModel, TimeSpan?>()
            .Returns(callInfo => (dialogResult, newInterval));

        // Act
        _viewModel.ChangeIntervalCommand.Execute(null);

        // Assert
        _dialogService.Received(1).ShowDialog<ChangeTimerIntervalDialogViewModel, TimeSpan?>();
        _viewModel.CurrentInterval.Should().Be(newInterval);
        _viewModel.DisplayTime.Should().Be("00:05:00");
    }

    [Fact]
    public void ChangeIntervalCommand_ShouldNotUpdateCurrentIntervalAndDisplayTime_WhenDialogReturnsFalse()
    {
        // Arrange
        var originalInterval = _viewModel.CurrentInterval;
        var originalDisplayTime = _viewModel.DisplayTime;

        bool? dialogResult = false;
        TimeSpan? newInterval = null;

        _dialogService.ShowDialog<ChangeTimerIntervalDialogViewModel, TimeSpan?>()
            .Returns(callInfo => (dialogResult, newInterval));

        // Act
        _viewModel.ChangeIntervalCommand.Execute(null);

        // Assert
        _dialogService.Received(1).ShowDialog<ChangeTimerIntervalDialogViewModel, TimeSpan?>();
        _viewModel.CurrentInterval.Should().Be(originalInterval);
        _viewModel.DisplayTime.Should().Be(originalDisplayTime);
    }

    [Fact]
    public void ChangeIntervalCommand_ShouldNotUpdateCurrentIntervalAndDisplayTime_WhenDialogReturnsNullInterval()
    {
        // Arrange
        var originalInterval = _viewModel.CurrentInterval;
        var originalDisplayTime = _viewModel.DisplayTime;

        bool? dialogResult = true;
        TimeSpan? newInterval = null;

        _dialogService.ShowDialog<ChangeTimerIntervalDialogViewModel, TimeSpan?>()
            .Returns(callInfo => (dialogResult, newInterval));

        // Act
        _viewModel.ChangeIntervalCommand.Execute(null);

        // Assert
        _dialogService.Received(1).ShowDialog<ChangeTimerIntervalDialogViewModel, TimeSpan?>();
        _viewModel.CurrentInterval.Should().Be(originalInterval);
        _viewModel.DisplayTime.Should().Be(originalDisplayTime);
    }
}