namespace Minfys.Models.Options;

/// <summary>
/// Contains configuration for available timer modes.
/// </summary>
public class TimerModesOptions
{
    public const string Key = "ModeOptions";

    /// <summary>
    /// Represents the available timer behavior modes.
    /// </summary>
    public enum TimerModesEnum
    {
        /// <summary>
        /// The timer automatically restarts after each completion.
        /// </summary>
        Looping,

        /// <summary>
        /// The timer runs only once and then stops.
        /// </summary>
        Single
    }

    /// <summary>
    /// List of all available timer modes.
    /// </summary>
    public static Array AvailableModes => Enum.GetValues(typeof(TimerModesEnum));

    /// <summary>
    /// Currently selected timer mode.
    /// </summary>
    public TimerModesEnum TimerMode { get; set; } = TimerModesEnum.Looping;
}