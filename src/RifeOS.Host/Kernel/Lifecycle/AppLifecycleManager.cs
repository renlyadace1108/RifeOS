using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using RifeOS.Host.Kernel.Loader;
using RifeOS.SDK.Context;

namespace RifeOS.Host.Kernel.Lifecycle;

public sealed class AppLifecycleManager
{
    [DllImport("psapi.dll")]
    private static extern int EmptyWorkingSet(IntPtr hwProc);

    private readonly DynamicAppLoader _loader;
    private readonly Func<string, IRifeAppContext> _contextFactory;
    private readonly ConcurrentDictionary<string, RunningAppSession> _runningSessions = new();

    public ObservableCollection<RunningAppSession> ActiveSessions { get; } = new();

    public event Action<RunningAppSession>? AppLaunched;
    public event Action<string>? AppTerminated;
    public event Action<RunningAppSession?>? ActiveAppSwitched;

    public RunningAppSession? CurrentActiveSession { get; private set; }

    public AppLifecycleManager(DynamicAppLoader loader, Func<string, IRifeAppContext> contextFactory)
    {
        _loader = loader;
        _contextFactory = contextFactory;
    }

    public async Task<RunningAppSession> LaunchOrActivateAsync(string appId)
    {
        if (_runningSessions.TryGetValue(appId, out var existingSession))
        {
            SwitchTo(appId);
            return existingSession;
        }

        var (appInstance, loadContext) = _loader.LoadApp(appId);
        var appContext = _contextFactory(appId);

        await appInstance.InitializeAsync(appContext);
        var view = appInstance.CreateView();

        var session = new RunningAppSession
        {
            Metadata = appInstance.Metadata,
            Instance = appInstance,
            LoadContext = loadContext,
            View = view
        };

        _runningSessions[appId] = session;

        Application.Current.Dispatcher.Invoke(() =>
        {
            ActiveSessions.Add(session);
        });

        SwitchTo(appId);
        AppLaunched?.Invoke(session);

        return session;
    }

    public void SwitchTo(string appId)
    {
        if (_runningSessions.TryGetValue(appId, out var session))
        {
            CurrentActiveSession = session;
            ActiveAppSwitched?.Invoke(session);
        }
    }

    public async Task CloseAppAsync(string appId)
    {
        if (!_runningSessions.TryRemove(appId, out var session))
        {
            return;
        }

        // 1. 异步触发 App 的终止清理
        await session.Instance.OnTerminateAsync();

        // 2. 从活跃集合移除并切换视图焦点
        Application.Current.Dispatcher.Invoke(() =>
        {
            ActiveSessions.Remove(session);
            if (CurrentActiveSession?.Metadata.Id == appId)
            {
                CurrentActiveSession = ActiveSessions.LastOrDefault();
                ActiveAppSwitched?.Invoke(CurrentActiveSession);
            }
        });

        AppTerminated?.Invoke(appId);

        // 3. 将卸载逻辑隔绝在独立非内联方法中，彻底斩断强引用
        var alcWeakRef = UnloadSessionCore(session);

        // 4. 多轮分代回收并压缩 LOH/POH 堆
        TriggerDeepGarbageCollection();

        // 5. 将无用物理内存释放回 Windows 操作系统
        TrimProcessWorkingSet();
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static WeakReference UnloadSessionCore(RunningAppSession session)
    {
        var weakRef = new WeakReference(session.LoadContext, trackResurrection: false);

        // 深度解绑 WPF UI 元素所有依赖项与上下文
        if (session.View is FrameworkElement fe)
        {
            fe.DataContext = null;
        }
        if (session.View is ContentControl cc)
        {
            cc.Content = null;
        }

        session.LoadContext.Unload();
        return weakRef;
    }

    private static void TriggerDeepGarbageCollection()
    {
        for (var i = 0; i < 2; i++)
        {
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, blocking: true, compacting: true);
            GC.WaitForPendingFinalizers();
        }
    }

    private static void TrimProcessWorkingSet()
    {
        try
        {
            using var proc = System.Diagnostics.Process.GetCurrentProcess();
            EmptyWorkingSet(proc.Handle);
        }
        catch
        {
            // 忽略非核心权限异常
        }
    }
}