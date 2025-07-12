namespace Minfys.Models.Options;

public class SystemOptions
{
    public const string Key = "SystemOptions";

    /// <summary>
    /// Minimizes to tray when closing the window (clicking the “×” button)
    /// </summary>
    public bool EnableCloseToTray { get; set; } = false;

    /// <summary>
    /// Makes Minfys launch when computer starts
    /// </summary>

    public bool EnableAutoLaunch { get; set; } = false;
}