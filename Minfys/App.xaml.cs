using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Minfys;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private readonly IHost _host;

    public App()
    {
        _host = Host.CreateDefaultBuilder()
            .UseSerilog((hostContext, loggerConfiguration) =>
            {
                loggerConfiguration.ReadFrom.Configuration(hostContext.Configuration);
            })
            .ConfigureServices(service => { service.AddSingleton<MainWindow>(); }).Build();
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        Log.Information("Starting host");
        _host.Start();

        var mainWindow = _host.Services.GetRequiredService<MainWindow>();
        mainWindow.Show();

        base.OnStartup(e);
        Log.Information("Host start successful");
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        try
        {
            await _host.StopAsync();
        }
        catch (Exception exception)
        {
            Log.Fatal("Error while stopping host: " + exception.Message);
        }
        finally
        {
            Log.CloseAndFlush();
            _host.Dispose();
        }

        base.OnExit(e);
    }
}