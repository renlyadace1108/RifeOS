using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using RifeOS.Host.Models;
using RifeOS.SDK.Models;

namespace RifeOS.Host.ViewModels;

public partial class DesktopViewModel : ObservableObject
{
    // 桌面元素集合
    public ObservableCollection<DesktopIconItem> Icons { get; } = new();
    public ObservableCollection<DesktopWidgetItem> Widgets { get; } = new();

    // 桌面布局状态
    [ObservableProperty] private bool _showIcons = true;
    [ObservableProperty] private bool _snapToGrid = true;
    [ObservableProperty] private bool _autoArrange = false;
    [ObservableProperty] private double _iconSize = 72;

    // 桌面小组件动态绑定属性 (通过 EventBus 接收沙箱数据)
    [ObservableProperty] private int _widgetGoalRate = 0;
    [ObservableProperty] private double _widgetStudyHours = 0;
    [ObservableProperty] private bool _widgetIsExerciseDone = false;

    public DesktopViewModel()
    {
        InitializeDefaultDesktop();

        // 监听来自沙箱插件的小组件更新广播
        WeakReferenceMessenger.Default.Register<WidgetUpdateMessage>(this, (recipient, msg) =>
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                WidgetGoalRate = msg.TodayGoalRate;
                WidgetStudyHours = msg.StudyHours;
                WidgetIsExerciseDone = msg.IsExerciseDone;
            });
        });
    }

    private void InitializeDefaultDesktop()
    {
        // 初始快捷方式
        Icons.Add(new DesktopIconItem { AppKey = "Tasks", Title = "任务清单", Icon = "📝", X = 20, Y = 20, Color = "#1D4ED8" });
        Icons.Add(new DesktopIconItem { AppKey = "Overview", Title = "个人总览", Icon = "📊", X = 20, Y = 120, Color = "#059669" });
        Icons.Add(new DesktopIconItem { AppKey = "Settings", Title = "系统设置", Icon = "⚙", X = 20, Y = 220, Color = "#4F46E5" });

        // 初始小组件 (默认锁定在右上方)
        Widgets.Add(new DesktopWidgetItem { WidgetKey = "Overview", X = 1200, Y = 40, IsPinned = true });
        Widgets.Add(new DesktopWidgetItem { WidgetKey = "RecentFiles", X = 1200, Y = 280, IsPinned = true });
    }

    // ================= 小组件管理命令 =================
    [RelayCommand]
    private void AddWidget(string widgetKey)
    {
        // 避免重复添加同一个组件
        if (Widgets.Any(w => w.WidgetKey == widgetKey)) return;

        // 新添加的组件默认处于中心附近，且处于非锁定（可拖拽）状态
        Widgets.Add(new DesktopWidgetItem { WidgetKey = widgetKey, X = 400, Y = 200, IsPinned = false });
    }

    [RelayCommand]
    private void RemoveWidget(DesktopWidgetItem widget)
    {
        if (widget != null) Widgets.Remove(widget);
    }

    // ================= 图标管理命令 =================
    [RelayCommand]
    private void ToggleShowIcons() => ShowIcons = !ShowIcons;

    [RelayCommand]
    private void ToggleSnapToGrid()
    {
        SnapToGrid = !SnapToGrid;
        if (SnapToGrid) ApplySnapToGrid();
    }

    [RelayCommand]
    private void ChangeIconSize(string size)
    {
        IconSize = size switch { "Large" => 96, "Small" => 56, _ => 72 };
        if (SnapToGrid) ApplySnapToGrid();
    }

    private void ApplySnapToGrid()
    {
        foreach (var icon in Icons)
        {
            icon.X = Math.Round(icon.X / IconSize) * IconSize;
            icon.Y = Math.Round(icon.Y / IconSize) * IconSize;
        }
    }
}