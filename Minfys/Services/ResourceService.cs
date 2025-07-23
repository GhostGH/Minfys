using System.IO;
using System.Windows;

namespace Minfys.Services;

/// <summary>
/// Manages access to embedded resources.
/// </summary>
public class ResourceService : IResourceService
{
    public Stream? GetResourceStream(Uri resourceUri)
    {
        return Application.GetResourceStream(resourceUri)?.Stream;
    }
}