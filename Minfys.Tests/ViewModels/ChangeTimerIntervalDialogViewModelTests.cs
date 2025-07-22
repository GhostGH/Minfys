using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Minfys.Models.Options;
using Minfys.Services;
using Minfys.ViewModels.Dialogs;
using NSubstitute;

namespace Minfys.Tests.ViewModels;

public class ChangeTimerIntervalDialogViewModelTests
{
    private readonly ILogger<ChangeTimerIntervalDialogViewModel> _logger;
    private readonly IMessageService _messageService;
    private readonly IOptionsService _optionsService;
    private readonly IOptionsMonitor<TimerOptions> _timerOptions;
    private readonly ChangeTimerIntervalDialogViewModel _viewModel;

    public ChangeTimerIntervalDialogViewModelTests()
    {
        _logger = Substitute.For<ILogger<ChangeTimerIntervalDialogViewModel>>();
        _messageService = Substitute.For<IMessageService>();
        _optionsService = Substitute.For<IOptionsService>();
        _timerOptions = Substitute.For<IOptionsMonitor<TimerOptions>>();

        var timerOptions = new TimerOptions
        {
            TimerInterval = TimeSpan.FromSeconds(10),
            TimerMode = TimerOptions.TimerModesEnum.Single
        };

        _timerOptions.CurrentValue.Returns(timerOptions);

        _viewModel = new ChangeTimerIntervalDialogViewModel(_logger, _messageService, _optionsService, _timerOptions);
    }

    [Fact]
    public void AcceptChange_AfterSubmittingCorrectInterval_ShouldInvokeWindowClosingEvent()
    {
        // Arrange
        string interval = TimeSpan.FromMinutes(5).ToString();
        var eventRaised = false;
        _viewModel.RequestClose += (_, _) => eventRaised = true;

        // Act
        _viewModel.AcceptChangeCommand.Execute(interval);

        // Assert
        eventRaised.Should().Be(true);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("hello")]
    [InlineData("1.3")]
    [InlineData("0.01")]
    [InlineData("11f")]
    public void AcceptChange_AfterSubmittingIncorrectInterval_ShouldNotInvokeWindowClosingEvent(string? interval)
    {
        // Arrange
        var eventRaised = false;
        _viewModel.RequestClose += (_, _) => eventRaised = true;

        // Act
        _viewModel.AcceptChangeCommand.Execute(interval);

        // Assert
        eventRaised.Should().Be(false);
    }

    [Fact]
    public void CancelChange_AfterChangeCanceled_ShouldInvokeWindowClosingEvent()
    {
        // Arrange
        var eventRaised = false;
        _viewModel.RequestClose += (_, _) => eventRaised = true;

        // Act
        _viewModel.CancelChangeCommand.Execute(null);

        // Assert
        eventRaised.Should().Be(true);
    }
}