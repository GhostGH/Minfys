using System.Windows;
using Microsoft.Extensions.Logging;
using Minfys.ViewModels.Windows;

namespace Minfys.Views.Dialogs;

public partial class TimerFiredDialog : Window
{
    private readonly ILogger<TimerFiredDialog> _logger;

    public TimerFiredDialog(ILogger<TimerFiredDialog> logger, TimerFiredDialogViewModel timerFiredDialogViewModel)
    {
        DataContext = timerFiredDialogViewModel;
        _logger = logger;

        InitializeComponent();
        logger.LogInformation("{Window} created", nameof(TimerFiredDialog));
    }
}