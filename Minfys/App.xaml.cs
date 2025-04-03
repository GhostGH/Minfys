using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Minfys.Services;
using Minfys.ViewModels.Windows;
using Minfys.Views.Windows;
using Serilog;
using MainViewModel = Minfys.ViewModels.Windows.MainViewModel;

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
            .ConfigureServices(service =>
            {
                service.AddSingleton<MainWindow>();
                service.AddTransient<ChangeTimerIntervalDialog>();

                service.AddTransient<MainViewModel>();
                service.AddTransient<ChangeTimerIntervalDialogViewModel>();


                service.AddSingleton<IMessageService, MessageService>();
                service.AddSingleton<IDialogService, DialogService>();
            }).Build();
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        Log.Information("Starting host");
        _host.Start();

        var mainWindow = _host.Services.GetRequiredService<MainWindow>();
        mainWindow.Show();

        base.OnStartup(e);
        Log.Information("Host started");
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        try
        {
            await _host.StopAsync();
        }
        catch (Exception exception)
        {
            Log.Fatal(exception, "Error while stopping host: {ErrorMessage}", exception.Message);
        }
        finally
        {
            Log.CloseAndFlush();
            _host.Dispose();
        }

        base.OnExit(e);
    }
}