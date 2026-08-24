using CommunityToolkit.Mvvm.ComponentModel;

namespace RifeOS.Host.Models;

public partial class DesktopIconItem : ObservableObject
{
    [ObservableProperty] private string _appKey = string.Empty;
    [ObservableProperty] private string _title = string.Empty;
    [ObservableProperty] private string _icon = string.Empty;
    [ObservableProperty] private double _x;
    [ObservableProperty] private double _y;
    [ObservableProperty] private string _color = "#3B82F6";
}