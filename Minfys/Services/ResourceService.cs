using System.IO;
using System.Windows;

namespace Minfys.Services;

public class ResourceService : IResourceService
{
    public Stream? GetResourceStream(Uri resourceUri)
    {
        return Application.GetResourceStream(resourceUri)?.Stream;
    }
}