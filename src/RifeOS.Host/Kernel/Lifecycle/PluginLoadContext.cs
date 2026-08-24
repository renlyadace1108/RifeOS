using System.Reflection;
using System.Runtime.Loader;
using RifeOS.SDK.App;

namespace RifeOS.Host.Kernel.Lifecycle;

public sealed class PluginLoadContext : AssemblyLoadContext
{
    private readonly AssemblyDependencyResolver _resolver;

    public PluginLoadContext(string pluginPath) : base(isCollectible: true)
    {
        _resolver = new AssemblyDependencyResolver(pluginPath);
    }

    protected override Assembly? Load(AssemblyName assemblyName)
    {
        // 关键核心：无条件将 SDK 程序集共享给插件，保证 IRifeApp 接口类型 100% 相同
        if (assemblyName.Name == typeof(IRifeApp).Assembly.GetName().Name)
        {
            return typeof(IRifeApp).Assembly;
        }

        string? assemblyPath = _resolver.ResolveAssemblyToPath(assemblyName);
        return assemblyPath != null ? LoadFromAssemblyPath(assemblyPath) : null;
    }
}