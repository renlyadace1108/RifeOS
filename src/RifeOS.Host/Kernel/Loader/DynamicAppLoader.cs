using System.IO;
using System.Reflection;
using RifeOS.SDK.App;
using RifeOS.SDK.Models;

namespace RifeOS.Host.Kernel.Loader;

public sealed class DynamicAppLoader
{
    private readonly string _appsDirectory;

    public DynamicAppLoader()
    {
        _appsDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Apps");
        if (!Directory.Exists(_appsDirectory))
        {
            Directory.CreateDirectory(_appsDirectory);
        }
    }

    public IReadOnlyList<AppMetadata> DiscoverAvailableApps()
    {
        var result = new List<AppMetadata>();
        var appDirs = Directory.GetDirectories(_appsDirectory);

        foreach (var dir in appDirs)
        {
            var dllFiles = Directory.GetFiles(dir, "RifeOS.Apps.*.dll");
            foreach (var dll in dllFiles)
            {
                try
                {
                    var context = new AppAssemblyLoadContext(dll);
                    var assembly = context.LoadFromAssemblyPath(dll);
                    var appType = assembly.GetExportedTypes().FirstOrDefault(t => typeof(IRifeApp).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

                    if (appType != null && Activator.CreateInstance(appType) is IRifeApp instance)
                    {
                        result.Add(instance.Metadata);
                    }
                    context.Unload();
                }
                catch
                {
                }
            }
        }
        return result;
    }

    public (IRifeApp AppInstance, AppAssemblyLoadContext LoadContext) LoadApp(string appId)
    {
        var appDirs = Directory.GetDirectories(_appsDirectory);
        foreach (var dir in appDirs)
        {
            var dllFiles = Directory.GetFiles(dir, "RifeOS.Apps.*.dll");
            foreach (var dll in dllFiles)
            {
                var context = new AppAssemblyLoadContext(dll);
                var assembly = context.LoadFromAssemblyPath(dll);
                var appType = assembly.GetExportedTypes().FirstOrDefault(t => typeof(IRifeApp).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

                if (appType != null && Activator.CreateInstance(appType) is IRifeApp instance)
                {
                    if (instance.Metadata.Id.Equals(appId, StringComparison.OrdinalIgnoreCase))
                    {
                        return (instance, context);
                    }
                }
                context.Unload();
            }
        }

        throw new FileNotFoundException($"未能找到 Id 为 [{appId}] 的应用插件包。");
    }
}
