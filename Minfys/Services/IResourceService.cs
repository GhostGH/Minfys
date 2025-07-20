using System.IO;

namespace Minfys.Services;

/// <summary>
/// Provides contract for getting resource streams.
/// </summary>
public interface IResourceService
{
    /// <summary>
    /// Gets a resource stream for the resource.
    /// </summary>
    /// <param name="resourceUri">Uri of the resource.</param>
    /// <returns></returns>
    Stream? GetResourceStream(Uri resourceUri);
}