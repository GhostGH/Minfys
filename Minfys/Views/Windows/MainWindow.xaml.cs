using System.Windows;
using Microsoft.Extensions.Logging;
using MainViewModel = Minfys.ViewModels.Windows.MainViewModel;

namespace Minfys;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly ILogger<MainWindow> _logger;

    public MainWindow(ILogger<MainWindow> logger, MainViewModel mainViewModel)
    {
        _logger = logger;
        DataContext = mainViewModel;
        InitializeComponent();
        _logger.LogInformation("MainWindow created");
    }
}