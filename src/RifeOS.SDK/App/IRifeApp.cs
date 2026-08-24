using RifeOS.SDK.Context;
using RifeOS.SDK.Enums;
using RifeOS.SDK.Models;

namespace RifeOS.SDK.App;

public interface IRifeApp
{
    AppMetadata Metadata { get; }
    AppLifecycleState State { get; }
    Task InitializeAsync(IRifeAppContext context, CancellationToken cancellationToken = default);
    object CreateView();
    Task OnSuspendAsync(CancellationToken cancellationToken = default);
    Task OnTerminateAsync(CancellationToken cancellationToken = default);
}
