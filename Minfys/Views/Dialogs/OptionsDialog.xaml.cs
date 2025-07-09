using System.Windows;
using Microsoft.Extensions.Logging;
using OptionsDialogViewModel = Minfys.ViewModels.Dialogs.OptionsDialogViewModel;

namespace Minfys.Views.Dialogs;

public partial class OptionsDialog : Window
{
    private readonly ILogger<OptionsDialog> _logger;

    public OptionsDialog(ILogger<OptionsDialog> logger, OptionsDialogViewModel optionsDialogViewModel)
    {
        _logger = logger;
        DataContext = optionsDialogViewModel;

        InitializeComponent();
        _logger.LogInformation("{Window} created", nameof(OptionsDialog));
    }
}