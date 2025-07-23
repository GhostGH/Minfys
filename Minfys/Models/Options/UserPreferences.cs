namespace Minfys.Models.Options;

/// <summary>
/// Contains all settings for userPreferences.json.
/// </summary>
public class UserPreferences
{
    public AudioOptions AudioOptions { get; set; } = new();
    public SystemOptions SystemOptions { get; set; } = new();
    public TimerOptions TimerOptions { get; set; } = new();
}