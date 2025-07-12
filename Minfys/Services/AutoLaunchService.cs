using System.Windows.Forms;
using Microsoft.Win32;

namespace Minfys.Services;

/// <summary>
/// Manages application's auto launch functionality.
/// </summary>
public class AutoLaunchService
{
    private const string AppName = "Minfys";
    private const string RegistryPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";

    /// <summary>
    /// Enables or disables application auto launch. 
    /// </summary>
    /// <param name="enabled">Whether to enable or disable auto launch.</param>
    /// <exception cref="SecurityException">The user does not have the permissions required to create or modify registry keys.</exception>
    public void SetAutoLaunch(bool enabled)
    {
        using var key = Registry.CurrentUser.OpenSubKey(RegistryPath, true);

        if (enabled)
        {
            key?.SetValue(AppName, Application.ExecutablePath);
        }
        else
        {
            key?.DeleteValue(AppName, false);
        }
    }
}