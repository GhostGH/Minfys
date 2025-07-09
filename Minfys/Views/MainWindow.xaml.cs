using System.ComponentModel;
using System.Windows;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Minfys.Models.Options;
using Application = System.Windows.Application;
using MainViewModel = Minfys.ViewModels.MainViewModel;

namespace Minfys;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly ILogger<MainWindow> _logger;
    private readonly IOptionsMonitor<SystemOptions> _systemOptions;

    public MainWindow(ILogger<MainWindow> logger, IOptionsMonitor<SystemOptions> systemOptions,
        MainViewModel mainViewModel)
    {
        InitializeComponent();

        _logger = logger;
        _systemOptions = systemOptions;
        DataContext = mainViewModel;

        Closing += OnClosing;

        _logger.LogInformation("{Window} created", nameof(MainWindow));
    }

    private void OnClosing(object? sender, CancelEventArgs e)
    {
        var settings = _systemOptions.CurrentValue;

        if (settings.EnableCloseToTray)
        {
            e.Cancel = true;
            Hide();
            ShowInTaskbar = false;
            TrayIcon.Visibility = Visibility.Visible;
        }
    }

    private void MenuOpen_OnClick(object sender, RoutedEventArgs e)
    {
        Show();
        ShowInTaskbar = true;
        TrayIcon.Visibility = Visibility.Hidden;
    }

    private void MenuExit_OnClick(object sender, RoutedEventArgs e)
    {
        TrayIcon.Visibility = Visibility.Hidden;
        Application.Current.Shutdown();
    }

    private void TrayIcon_OnTrayMouseDoubleClick(object sender, RoutedEventArgs e)
    {
        Show();
        ShowInTaskbar = true;
        TrayIcon.Visibility = Visibility.Hidden;
    }
}