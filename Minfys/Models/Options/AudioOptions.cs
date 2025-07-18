namespace Minfys.Models.Options;

/// <summary>
/// Contains settings related to audio playback for timer notifications.
/// </summary>
public class AudioOptions
{
    public const string Key = "AudioOptions";

    /// <summary>
    /// Full path to the audio file that will be played as a notification.
    /// </summary>
    public string FilePath { get; set; } =
        "//Assets//Audio//default_song.mp3";

    /// <summary>
    /// Indicates whether the audio should loop when played.
    /// </summary>
    public bool LoopEnabled { get; set; } = false;

    /// <summary>
    /// Volume level of the notification sound, ranging from 0.0 (mute) to 1.0 (maximum).
    /// </summary>
    public float Volume { get; set; } = 0.3f;
}