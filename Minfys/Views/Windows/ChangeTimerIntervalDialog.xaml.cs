using System.Windows;
using Microsoft.Extensions.Logging;
using Minfys.ViewModels.Windows;

namespace Minfys.Views.Windows;

public partial class ChangeTimerIntervalDialog : Window
{
    private readonly ILogger<ChangeTimerIntervalDialog> _logger;

    public ChangeTimerIntervalDialog(ILogger<ChangeTimerIntervalDialog> logger,
        ChangeTimerIntervalDialogViewModel changeTimerIntervalDialogViewModel)
    {
        _logger = logger;
        DataContext = changeTimerIntervalDialogViewModel;

        InitializeComponent();
        _logger.LogInformation("ChangeTimerIntervalDialog created");
    }
}