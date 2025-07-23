namespace Minfys.Services;

/// <summary>
/// Provides contract for implementing app's auto launch functionality.
/// </summary>
public interface IAutoLaunchService
{
    void SetAutoLaunch(bool enabled);
}