namespace Minfys.Models.Options;

public class UserPreferences
{
    public AudioOptions AudioOptions { get; set; } = new();
    public SystemOptions SystemOptions { get; set; } = new();
    public TimerModesOptions TimerModesOptions { get; set; } = new();
}