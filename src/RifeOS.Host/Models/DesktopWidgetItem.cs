using CommunityToolkit.Mvvm.ComponentModel;

namespace RifeOS.Host.Models;

public partial class DesktopWidgetItem : ObservableObject
{
    [ObservableProperty] private string _widgetKey = string.Empty; // "Overview" 或 "RecentFiles"
    [ObservableProperty] private double _x;
    [ObservableProperty] private double _y;
    [ObservableProperty] private bool _isPinned = true; // 是否锁定位置
}