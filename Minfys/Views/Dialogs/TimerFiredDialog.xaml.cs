using System.Windows;
using Microsoft.Extensions.Logging;
using TimerFiredDialogViewModel = Minfys.ViewModels.Dialogs.TimerFiredDialogViewModel;

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