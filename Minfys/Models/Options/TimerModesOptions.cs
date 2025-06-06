namespace Minfys.Models;

public class TimerModesOptions
{
    public const string Key = "ModeOptions";

    public enum TimerModesEnum
    {
        Single,
        Persistent
    }

    public static Array AvailableModes => Enum.GetValues(typeof(TimerModesEnum));
    public TimerModesEnum TimerMode { get; set; } = TimerModesEnum.Single;
}