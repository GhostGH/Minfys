namespace Minfys.Models;

/// <summary>
/// Contains timer song settings
/// </summary>
public class AudioOptions
{
    public const string Key = "AudioOptions";
    public string FilePath { get; set; } = string.Empty;
    public bool LoopEnabled { get; set; }
}