using System.Windows;
using System.Windows.Media;
using RifeOS.SDK.Services;

namespace RifeOS.Host.Services;

public sealed class ThemeService : IThemeContext
{
    public static ThemeService Instance { get; } = new();

    public string CurrentTheme { get; private set; } = "Dark (默认深色)";

    public event Action<string>? ThemeChanged;

    private ThemeService() { }

    public void SwitchTheme(string themeName)
    {
        CurrentTheme = themeName;
        ApplyTheme(themeName);
        ThemeChanged?.Invoke(themeName);
    }

    private void ApplyTheme(string themeName)
    {
        if (Application.Current?.Resources == null) return;

        Application.Current.Dispatcher.Invoke(() =>
        {
            var res = Application.Current.Resources;

            if (themeName.Contains("Light"))
            {
                // 浅色主题：清爽柔和色系
                res["AppBackgroundBrush"] = new SolidColorBrush(Color.FromRgb(243, 244, 246));
                res["SurfaceBackgroundBrush"] = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                res["SubSurfaceBackgroundBrush"] = new SolidColorBrush(Color.FromRgb(235, 238, 242));
                res["BorderBrush"] = new SolidColorBrush(Color.FromRgb(215, 220, 228));
                res["TextPrimaryBrush"] = new SolidColorBrush(Color.FromRgb(17, 24, 39));
                res["TextSecondaryBrush"] = new SolidColorBrush(Color.FromRgb(100, 116, 139));
                res["WatermarkOpacity"] = 0.08;

                // 浅色磁贴颜色（明亮饱满）
                res["TileTasksBrush"] = new SolidColorBrush(Color.FromRgb(37, 99, 235));
                res["TileSettingsBrush"] = new SolidColorBrush(Color.FromRgb(99, 102, 241));
                res["TileDashboardBrush"] = new SolidColorBrush(Color.FromRgb(16, 185, 129));
                res["TileDefaultBrush"] = new SolidColorBrush(Color.FromRgb(59, 130, 246));
                res["TileTextBrush"] = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            }
            else if (themeName.Contains("OLED"))
            {
                // OLED 纯黑：高对比度、暗雅重彩
                res["AppBackgroundBrush"] = new SolidColorBrush(Color.FromRgb(0, 0, 0));
                res["SurfaceBackgroundBrush"] = new SolidColorBrush(Color.FromRgb(8, 8, 10));
                res["SubSurfaceBackgroundBrush"] = new SolidColorBrush(Color.FromRgb(16, 16, 18));
                res["BorderBrush"] = new SolidColorBrush(Color.FromRgb(32, 32, 36));
                res["TextPrimaryBrush"] = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                res["TextSecondaryBrush"] = new SolidColorBrush(Color.FromRgb(120, 120, 120));
                res["WatermarkOpacity"] = 0.16;

                // OLED 磁贴颜色（深邃对比）
                res["TileTasksBrush"] = new SolidColorBrush(Color.FromRgb(30, 58, 138));
                res["TileSettingsBrush"] = new SolidColorBrush(Color.FromRgb(67, 56, 202));
                res["TileDashboardBrush"] = new SolidColorBrush(Color.FromRgb(6, 78, 59));
                res["TileDefaultBrush"] = new SolidColorBrush(Color.FromRgb(30, 58, 138));
                res["TileTextBrush"] = new SolidColorBrush(Color.FromRgb(240, 240, 240));
            }
            else
            {
                // 默认 Dark：专业深色质感
                res["AppBackgroundBrush"] = new SolidColorBrush(Color.FromRgb(15, 15, 17));
                res["SurfaceBackgroundBrush"] = new SolidColorBrush(Color.FromRgb(22, 22, 25));
                res["SubSurfaceBackgroundBrush"] = new SolidColorBrush(Color.FromRgb(30, 30, 34));
                res["BorderBrush"] = new SolidColorBrush(Color.FromRgb(40, 40, 45));
                res["TextPrimaryBrush"] = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                res["TextSecondaryBrush"] = new SolidColorBrush(Color.FromRgb(150, 150, 150));
                res["WatermarkOpacity"] = 0.12;

                // 默认深色磁贴颜色
                res["TileTasksBrush"] = new SolidColorBrush(Color.FromRgb(29, 78, 216));
                res["TileSettingsBrush"] = new SolidColorBrush(Color.FromRgb(79, 70, 229));
                res["TileDashboardBrush"] = new SolidColorBrush(Color.FromRgb(4, 120, 87));
                res["TileDefaultBrush"] = new SolidColorBrush(Color.FromRgb(37, 99, 235));
                res["TileTextBrush"] = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            }
        });
    }
}