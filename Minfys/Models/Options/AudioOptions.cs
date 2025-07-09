namespace Minfys.Models.Options;

/// <summary>
/// Contains timer song settings
/// </summary>
public class AudioOptions
{
    public const string Key = "AudioOptions";

    public string FilePath { get; set; } =
        "C://Programming projects//C#//Projects//Minfys//Minfys//Assets//Audio//quest_ding_1.mp3";

    public bool LoopEnabled { get; set; } = false;
    public float Volume { get; set; } = 0.3f;
}