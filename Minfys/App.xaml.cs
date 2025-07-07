using System.IO;
using System.Text.Json;
using System.Windows;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Minfys.Models;
using Minfys.Services;
using Minfys.ViewModels.Windows;
using Serilog;
using ChangeTimerIntervalDialog = Minfys.Views.Dialogs.ChangeTimerIntervalDialog;
using MainViewModel = Minfys.ViewModels.Windows.MainViewModel;
using OptionsDialog = Minfys.Views.Dialogs.OptionsDialog;
using TimerFiredDialog = Minfys.Views.Dialogs.TimerFiredDialog;

namespace Minfys;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private readonly IHost _host;
    private readonly string _userConfigPath;

    public App()
    {
        string appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        string userDir = Path.Combine(appdata, "Minfys");
        Directory.CreateDirectory(userDir);
        _userConfigPath = Path.Combine(userDir, "userPreferences.json");

        if (!File.Exists(_userConfigPath))
        {
            var defaultPrefs = new UserPreferences();
            var json = JsonSerializer.Serialize(defaultPrefs,
                new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_userConfigPath, json);
        }

        _host = Host.CreateDefaultBuilder()
            .UseSerilog((hostContext, loggerConfiguration) =>
            {
                loggerConfiguration.ReadFrom.Configuration(hostContext.Configuration);
            })
            .ConfigureAppConfiguration((hostContext, config) =>
            {
                config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                config.AddJsonFile(_userConfigPath, optional: true, reloadOnChange: true);
            })
            .ConfigureServices((hostContext, service) =>
            {
                service.AddSingleton<MainWindow>();
                service.AddTransient<ChangeTimerIntervalDialog>();
                service.AddTransient<OptionsDialog>();
                service.AddTransient<TimerFiredDialog>();

                service.AddTransient<MainViewModel>();
                service.AddTransient<ChangeTimerIntervalDialogViewModel>();
                service.AddTransient<OptionsDialogViewModel>();
                service.AddTransient<TimerFiredDialogViewModel>();


                service.AddSingleton<IMessageService, MessageService>();
                service.AddSingleton<IDialogService, DialogService>();
                service.AddSingleton(provider => _userConfigPath);
                service.AddSingleton<IOptionsService, OptionsService>();

                service.AddOptions<AudioOptions>()
                    .Bind(hostContext.Configuration.GetSection(AudioOptions.Key));
                service.AddOptions<SystemOptions>()
                    .Bind(hostContext.Configuration.GetSection(SystemOptions.Key));
                service.AddOptions<TimerModesOptions>()
                    .Bind(hostContext.Configuration.GetSection(TimerModesOptions.Key));
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
            Log.Fatal(exception, "Error while stopping host: {Message}", exception.Message);
        }
        finally
        {
            Log.CloseAndFlush();
            _host.Dispose();
        }

        base.OnExit(e);
    }
}