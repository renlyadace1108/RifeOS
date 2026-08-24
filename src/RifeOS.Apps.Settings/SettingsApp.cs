using RifeOS.Apps.Settings.ViewModels;
using RifeOS.Apps.Settings.Views;
using RifeOS.SDK.App;
using RifeOS.SDK.Context;
using RifeOS.SDK.Enums;
using RifeOS.SDK.Models;

namespace RifeOS.Apps.Settings;

public class SettingsApp : IRifeApp
{
    public AppMetadata Metadata { get; } = new(
        "Settings",
        "系统设置",
        "系统级通用配置与个性化",
        "1.0.0",
        "RifeOS",
        "SettingsIcon",
        AppCategory.System
    );

    private IRifeAppContext? _context;

    public void Initialize(IRifeAppContext context)
    {
        _context = context;
    }

    public object CreateView()
    {
        if (_context == null) throw new InvalidOperationException("SettingsApp 尚未初始化。");
        var viewModel = new SettingsViewModel(_context);
        return new SettingsMainView(viewModel);
    }

    public void Cleanup() { }
}