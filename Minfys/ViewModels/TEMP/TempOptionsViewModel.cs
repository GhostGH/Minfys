using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Minfys.Models;
using Minfys.Services;

namespace Minfys.ViewModels.Windows;

public partial class TempOptionsViewModel : ObservableObject
{
    private readonly ISettingsService _settings;

    public TempOptionsViewModel(ISettingsService settings)
    {
        _settings = settings;
        FilePath = settings.Options.FilePath;
        LoopEnabled = settings.Options.LoopEnabled;
    }

    [ObservableProperty] private string _filePath;
    [ObservableProperty] private bool _loopEnabled;

    partial void OnFilePathChanged(string value) => Save();
    partial void OnLoopEnabledChanged(bool value) => Save();

    private void Save()
    {
        _settings.Save(new AudioOptions
        {
            FilePath = FilePath,
            LoopEnabled = LoopEnabled
        });
    }

    [RelayCommand]
    private void RestoreDefault()
    {
        // Удалить override-файл и извлечь дефолт заново
        var defaultPath = (_settings as SettingsService)!.ExtractDefaultAudio();
        FilePath = defaultPath;
    }

    [RelayCommand]
    private void Browse()
    {
        // Открыть диалог и присвоить FilePath
    }
}