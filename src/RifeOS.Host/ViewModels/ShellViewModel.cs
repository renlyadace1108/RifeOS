using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RifeOS.Host.Kernel.Lifecycle;
using RifeOS.Host.Services;
using RifeOS.SDK.App;
using RifeOS.SDK.Enums;

namespace RifeOS.Host.ViewModels;

public partial class PinnedTileViewModel : ObservableObject
{
    [ObservableProperty] private string _appKey = string.Empty;
    [ObservableProperty] private string _title = string.Empty;
    [ObservableProperty] private string _icon = string.Empty;
    [ObservableProperty] private string _background = "#1D4ED8";
    [ObservableProperty] private bool _isWide = false;
    [ObservableProperty] private string _subtitle = string.Empty;
}

public partial class AppMenuItemViewModel : ObservableObject
{
    [ObservableProperty] private string _appKey = string.Empty;
    [ObservableProperty] private string _title = string.Empty;
    [ObservableProperty] private string _icon = string.Empty;
    [ObservableProperty] private string _color = "#1E3A8A";
}

public partial class RunningAppItemViewModel : ObservableObject
{
    [ObservableProperty] private string _appKey = string.Empty;
    [ObservableProperty] private string _title = string.Empty;
    [ObservableProperty] private string _icon = "📦";
    [ObservableProperty] private object? _view;
    [ObservableProperty] private bool _isActive;

    public IRifeApp? AppInstance { get; set; }
}

public partial class ShellViewModel : ObservableObject
{
    public ObservableCollection<NotificationItemViewModel> ActiveToasts => NotificationService.Instance.ActiveToasts;
    public ObservableCollection<RunningAppItemViewModel> RunningApps { get; } = new();

    // 1. 在“所有应用”列表中注册 Overview 插件
    public ObservableCollection<AppMenuItemViewModel> AllApps { get; } = new()
    {
        new AppMenuItemViewModel { AppKey = "Tasks", Title = "任务清单 (Tasks)", Icon = "📝", Color = "#1E3A8A" },
        new AppMenuItemViewModel { AppKey = "Settings", Title = "系统设置 (Settings)", Icon = "⚙", Color = "#374151" },
        new AppMenuItemViewModel { AppKey = "Overview", Title = "个人总览 (Overview)", Icon = "📊", Color = "#059669" }
    };

    // 2. 默认在 Windows 10 磁贴上固定 Overview 宽磁贴
    public ObservableCollection<PinnedTileViewModel> PinnedTiles { get; } = new()
    {
        new PinnedTileViewModel { AppKey = "Tasks", Title = "任务清单", Icon = "📝", Background = "#1D4ED8", IsWide = false },
        new PinnedTileViewModel { AppKey = "Settings", Title = "系统设置", Icon = "⚙", Background = "#4F46E5", IsWide = false },
        new PinnedTileViewModel { AppKey = "Overview", Title = "个人总览", Icon = "📊", Background = "#059669", IsWide = true, Subtitle = "今日状态与生命数据" }
    };

    [ObservableProperty] private object? _currentAppView;
    [ObservableProperty] private bool _isStartMenuOpen = false;

    [RelayCommand] public void OpenStartMenu() => IsStartMenuOpen = true;
    [RelayCommand] public void CloseStartMenu() => IsStartMenuOpen = false;
    [RelayCommand] public void ToggleStartMenu() => IsStartMenuOpen = !IsStartMenuOpen;

    [RelayCommand]
    private void UnpinFromTiles(PinnedTileViewModel tile)
    {
        PinnedTiles.Remove(tile);
        IsStartMenuOpen = true;
        NotificationService.Instance.Show("开始菜单", $"已取消固定: {tile.Title}", NotificationLevel.Info);
    }

    [RelayCommand]
    private void PinToTiles(AppMenuItemViewModel app)
    {
        IsStartMenuOpen = true;

        if (PinnedTiles.Any(t => t.AppKey == app.AppKey))
        {
            NotificationService.Instance.Show("提示", $"{app.Title} 已经固定在磁贴中了", NotificationLevel.Warning);
            return;
        }

        var newTile = new PinnedTileViewModel
        {
            AppKey = app.AppKey,
            Title = app.Title.Split(' ')[0],
            Icon = app.Icon,
            Background = app.Color,
            IsWide = false
        };

        PinnedTiles.Add(newTile);
        NotificationService.Instance.Show("开始菜单", $"已固定到磁贴: {newTile.Title}", NotificationLevel.Success);
    }

    [RelayCommand]
    private void LaunchApp(string appKey)
    {
        IsStartMenuOpen = false;

        var existing = RunningApps.FirstOrDefault(a => a.AppKey == appKey);
        if (existing != null)
        {
            SwitchToApp(existing);
            return;
        }

        // 统一通过 ALC 微内核动态装载沙箱插件
        var appInstance = AppLifecycleManager.Instance.LoadPlugin(appKey);
        if (appInstance != null)
        {
            var newItem = new RunningAppItemViewModel
            {
                AppKey = appKey,
                Title = appInstance.Metadata.Name,
                // 图标映射
                Icon = appKey switch { "Tasks" => "📝", "Settings" => "⚙", "Overview" => "📊", _ => "📦" },
                View = appInstance.CreateView(),
                AppInstance = appInstance
            };

            RunningApps.Add(newItem);
            SwitchToApp(newItem);
            NotificationService.Instance.Show("应用载入", $"已动态装载沙箱: {newItem.Title}", NotificationLevel.Info);
        }
    }

    [RelayCommand]
    private void SwitchToApp(RunningAppItemViewModel targetApp)
    {
        IsStartMenuOpen = false;

        if (CurrentAppView == targetApp.View)
        {
            MinimizeApp(targetApp);
            return;
        }

        CurrentAppView = targetApp.View;
        foreach (var app in RunningApps)
        {
            app.IsActive = (app == targetApp);
        }
    }

    [RelayCommand]
    private void MinimizeApp(RunningAppItemViewModel targetApp)
    {
        if (CurrentAppView == targetApp.View)
        {
            CurrentAppView = null;
            targetApp.IsActive = false;
        }
    }

    [RelayCommand]
    private void CloseApp(RunningAppItemViewModel targetApp)
    {
        RunningApps.Remove(targetApp);

        // 彻底释放任何 ALC 插件进程级内存
        AppLifecycleManager.Instance.UnloadPlugin(targetApp.AppKey);
        NotificationService.Instance.Show("应用释放", $"已卸载插件并回收内存: {targetApp.Title}", NotificationLevel.Info);

        if (CurrentAppView == targetApp.View)
        {
            if (RunningApps.Count > 0) SwitchToApp(RunningApps.Last());
            else CurrentAppView = null;
        }
    }

    [RelayCommand]
    private void ExitSystem()
    {
        AppLifecycleManager.Instance.UnloadAll();
        Application.Current.Shutdown();
    }
}