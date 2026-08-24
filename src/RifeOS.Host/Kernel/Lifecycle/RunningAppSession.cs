using RifeOS.Host.Kernel.Loader;
using RifeOS.SDK.App;
using RifeOS.SDK.Models;

namespace RifeOS.Host.Kernel.Lifecycle;

public sealed class RunningAppSession
{
    public required AppMetadata Metadata { get; init; }
    public required IRifeApp Instance { get; init; }
    public required AppAssemblyLoadContext LoadContext { get; init; }
    public required object View { get; init; }
    public DateTime LaunchedAt { get; init; } = DateTime.UtcNow;
}
