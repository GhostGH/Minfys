namespace Minfys.Models.Options;

/// <summary>
/// Contains system-level settings related to application behavior and startup.
/// </summary>
public class SystemOptions
{
    public const string Key = "SystemOptions";

    /// <summary>
    /// Indicates whether to minimize to tray when closing the window (clicking the “×” button).
    /// </summary>
    public bool EnableCloseToTray { get; set; } = false;

    /// <summary>
    /// Indicates whether to make Minfys launch when computer starts.
    /// </summary>
    public bool EnableAutoLaunch { get; set; } = false;
}