using RifeOS.SDK.Context;
using RifeOS.SDK.Models;

namespace RifeOS.SDK.App;

public interface IRifeApp
{
    AppMetadata Metadata { get; }
    void Initialize(IRifeAppContext context);
    object CreateView();
    void Cleanup();
}