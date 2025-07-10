using System.Windows.Forms;
using Microsoft.Win32;

namespace Minfys.Services;

public class AutoLaunchService
{
    private readonly string _appName = "Minfys";
    private readonly string _registryPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";

    public void SetAutoLaunch(bool enabled)
    {
        using var key = Registry.CurrentUser.OpenSubKey(_registryPath, true);

        if (enabled)
        {
            key?.SetValue(_appName, Application.ExecutablePath);
        }
        else
        {
            key?.DeleteValue(_appName, false);
        }
    }
}