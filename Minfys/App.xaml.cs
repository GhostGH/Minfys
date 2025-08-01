﻿using System.IO;
using System.Text.Json;
using System.Windows;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Minfys.Models.Options;
using Minfys.Services;
using Serilog;
using ChangeTimerIntervalDialog = Minfys.Views.Dialogs.ChangeTimerIntervalDialog;
using ChangeTimerIntervalDialogViewModel = Minfys.ViewModels.Dialogs.ChangeTimerIntervalDialogViewModel;
using MainViewModel = Minfys.ViewModels.MainViewModel;
using OptionsDialog = Minfys.Views.Dialogs.OptionsDialog;
using OptionsDialogViewModel = Minfys.ViewModels.Dialogs.OptionsDialogViewModel;
using TimerFiredDialog = Minfys.Views.Dialogs.TimerFiredDialog;
using TimerFiredDialogViewModel = Minfys.ViewModels.Dialogs.TimerFiredDialogViewModel;

namespace Minfys;

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
                var uri = new Uri("pack://application:,,,/Minfys;component/appsettings.json", UriKind.Absolute);
                var resourceInfo = GetResourceStream(uri);
                if (resourceInfo != null)
                {
                    config.AddJsonStream(resourceInfo.Stream);
                }
                else
                {
                    throw new FileNotFoundException("Resource not found: appsettings.json", uri.ToString());
                }

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


                service.AddSingleton(provider => _userConfigPath);
                service.AddSingleton<IMessageService, MessageService>();
                service.AddSingleton<IDialogService, DialogService>();
                service.AddSingleton<IOptionsService, OptionsService>();
                service.AddSingleton<IResourceService, ResourceService>();
                service.AddSingleton<AutoLaunchService>();

                service.AddOptions<AudioOptions>()
                    .Bind(hostContext.Configuration.GetSection(AudioOptions.Key));
                service.AddOptions<SystemOptions>()
                    .Bind(hostContext.Configuration.GetSection(SystemOptions.Key));
                service.AddOptions<TimerOptions>()
                    .Bind(hostContext.Configuration.GetSection(TimerOptions.Key));
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
            await Log.CloseAndFlushAsync();
            _host.Dispose();
        }

        base.OnExit(e);
    }
}