using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RifeOS.SDK.Context;

namespace RifeOS.Apps.Settings.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    private readonly IRifeAppContext _context;

    [ObservableProperty] private string _selectedTheme = "Dark (默认深色)";
    [ObservableProperty] private string _storageUsage = "计算中...";

    public ObservableCollection<string> AvailableThemes { get; } = new()
    {
        "Dark (默认深色)",
        "OLED Midnight (极黑)",
        "Light (经典浅色)"
    };

    public SettingsViewModel(IRifeAppContext context)
    {
        _context = context;
        LoadSettings();
    }

    private void LoadSettings()
    {
        SelectedTheme = _context.Theme.CurrentTheme;
        StorageUsage = $"沙箱根路径: {_context.AppDataDirectory}";
    }

    [RelayCommand]
    private void ChangeTheme(string newTheme)
    {
        SelectedTheme = newTheme;
        _context.Theme.SwitchTheme(newTheme);
        _context.Notification.Show("主题已应用", $"当前系统色彩方案已切换至 {newTheme}");
    }
}