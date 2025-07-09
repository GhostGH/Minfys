namespace Minfys.Models;

public class SystemOptions
{
    public const string Key = "SystemOptions";

    /// <summary>
    /// Minimize to tray when closing the window (clicking the “×” button)
    /// </summary>
    public bool EnableCloseToTray { get; set; } = false;

    /// <summary>
    /// Make Minfys launch when computer starts
    /// </summary>

    public bool EnableAutoLaunch { get; set; } = false;
}