using System.IO;
using System.Reflection;
using System.Text.Json;
using Minfys.Models;

namespace Minfys.Services;

// SettingsService.cs

public class SettingsService : ISettingsService
{
    private readonly string _settingsPath;
    private readonly string _appDataDir;
    private const string DefaultResourceName = "Minfys.Assets.Audio.default_song.mp3";

    public AudioOptions Options { get; private set; }

    public SettingsService()
    {
        _appDataDir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Minfys");
        Directory.CreateDirectory(_appDataDir);

        _settingsPath = Path.Combine(_appDataDir, "userSettings.json");
        if (File.Exists(_settingsPath))
        {
            Options = JsonSerializer.Deserialize<AudioOptions>(
                File.ReadAllText(_settingsPath))!;
        }
        else
        {
            // инициализация дефолтных настроек
            var defaultPath = ExtractDefaultAudio();
            Options = new AudioOptions
            {
                FilePath = defaultPath,
                LoopEnabled = false
            };
            Save(Options);
        }
    }

    public void Save(AudioOptions options)
    {
        Options = options;
        File.WriteAllText(_settingsPath,
            JsonSerializer.Serialize(options, new JsonSerializerOptions { WriteIndented = true }));
    }

    /// <summary>
    /// wef
    /// </summary>
    /// <returns></returns>
    public string ExtractDefaultAudio()
    {
        // копирует ресурс из сборки в %APPDATA%\Minfys\default_song.mp3
        string dest = Path.Combine(_appDataDir, "default_song.mp3");
        if (!File.Exists(dest))
        {
            using var src = Assembly.GetExecutingAssembly()
                .GetManifestResourceStream(DefaultResourceName)!;
            using var fs = File.Create(dest);
            src.CopyTo(fs);
        }

        return dest;
    }
}