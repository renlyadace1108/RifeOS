using System.IO;
using System.Reflection;
using RifeOS.Host.Context;
using RifeOS.Host.Services;
using RifeOS.SDK.App;
using RifeOS.SDK.Enums;

namespace RifeOS.Host.Kernel.Lifecycle;

public sealed class AppLifecycleManager
{
    public static AppLifecycleManager Instance { get; } = new();

    private readonly Dictionary<string, (PluginLoadContext Alc, IRifeApp App)> _activePlugins = new();

    private AppLifecycleManager() { }

    public IRifeApp? LoadPlugin(string appKey)
    {
        if (_activePlugins.TryGetValue(appKey, out var existing))
        {
            return existing.App;
        }

        try
        {
            string pluginDll = $"RifeOS.Apps.{appKey}.dll";
            string pluginPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, pluginDll);

            if (!File.Exists(pluginPath))
            {
                NotificationService.Instance.Show("未找到插件", $"插件文件不存在: {pluginDll}", NotificationLevel.Error);
                return null;
            }

            // 1. 加载隔离程序集
            var alc = new PluginLoadContext(pluginPath);
            var assembly = alc.LoadFromAssemblyPath(pluginPath);

            // 2. 扫描实现 IRifeApp 的实体类
            var appType = assembly.GetExportedTypes().FirstOrDefault(t =>
                typeof(IRifeApp).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
                ?? assembly.GetTypes().FirstOrDefault(t =>
                    t.GetInterfaces().Any(i => i.Name == nameof(IRifeApp)) && !t.IsInterface && !t.IsAbstract);

            if (appType == null)
            {
                NotificationService.Instance.Show("载入失败", $"未在 {pluginDll} 中找到 IRifeApp 实现", NotificationLevel.Error);
                alc.Unload();
                return null;
            }

            var app = (IRifeApp)Activator.CreateInstance(appType)!;

            // 3. 构造沙箱隔离环境并注入
            string appDataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", appKey);
            Directory.CreateDirectory(appDataPath);

            var context = new RifeAppContext(
                appKey,
                appDataPath,
                UserProfileService.Instance,
                ThemeService.Instance,
                new AppStorageService(appDataPath),
                new AppConfigurationService(Path.Combine(appDataPath, "config.json")),
                NotificationService.Instance,
                EventBusService.Instance
            );

            app.Initialize(context);
            _activePlugins[appKey] = (alc, app);
            return app;
        }
        catch (Exception ex)
        {
            NotificationService.Instance.Show("加载异常", ex.Message, NotificationLevel.Error);
            return null;
        }
    }

    public void UnloadPlugin(string appKey)
    {
        if (_activePlugins.Remove(appKey, out var entry))
        {
            try { entry.App.Cleanup(); } catch { }
            entry.Alc.Unload();

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }
    }

    public void UnloadAll()
    {
        foreach (var key in _activePlugins.Keys.ToList())
        {
            UnloadPlugin(key);
        }
    }
}