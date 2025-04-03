using System.Configuration;
using System.Data;
using System.Windows;
using System.Windows.Documents;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Minfys.ViewModels;

namespace Minfys;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private IHost _host;

    public App()
    {
        Host.CreateDefaultBuilder().ConfigureServices(service =>
        {
            service.AddSingleton<MainWindow>();
            

        }).Build();
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        _host.Start();

        var mainWindow = _host.Services.GetRequiredService<MainWindow>();
        mainWindow.Show();
        
        base.OnStartup(e);
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        try
        {
            await _host.StopAsync();
        }
        catch (Exception exception)
        {
            Console.Error.WriteLine("Error while stopping host: " + exception.Message);
        }
        finally
        {
            _host.Dispose();
        }
        
        base.OnExit(e);
    }
}