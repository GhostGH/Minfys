using System.Windows;
using Microsoft.Extensions.Logging;
using Minfys.ViewModels.Windows;

namespace Minfys.Views.Windows;

public partial class OptionsDialog : Window
{
    private readonly ILogger<OptionsDialog> _logger;

    public OptionsDialog(ILogger<OptionsDialog> logger, OptionsDialogViewModel vm)
    {
        _logger = logger;
        DataContext = vm;
        InitializeComponent();
        _logger.LogInformation("{Window} created", nameof(OptionsDialog));
    }
}