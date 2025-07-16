using FluentAssertions;
using Microsoft.Extensions.Logging;
using Minfys.ViewModels.Dialogs;
using NSubstitute;

namespace Minfys.Tests.ViewModels;

public class TimerFiredDialogViewModelTests
{
    private readonly ILogger<TimerFiredDialogViewModel> _logger;
    private readonly TimerFiredDialogViewModel _viewModel;

    public TimerFiredDialogViewModelTests()
    {
        _logger = Substitute.For<ILogger<TimerFiredDialogViewModel>>();

        _viewModel = new TimerFiredDialogViewModel(_logger);
    }

    [Fact]
    public void ResetTimer_AfterPressingResetTimerButton_ShouldInvokeWindowClosingEvent()
    {
        // Arrange
        var eventRaised = false;
        _viewModel.RequestClose += (_, _) => eventRaised = true;

        // Act
        _viewModel.ResetTimerCommand.Execute(null);

        // Assert
        eventRaised.Should().Be(true);
    }

    [Fact]
    public void StopTimer_AfterPressingStopTimerButton_ShouldInvokeWindowClosingEvent()
    {
        // Arrange
        var eventRaised = false;
        _viewModel.RequestClose += (_, _) => eventRaised = true;

        // Act
        _viewModel.StopTimerCommand.Execute(null);

        // Assert
        eventRaised.Should().Be(true);
    }
}