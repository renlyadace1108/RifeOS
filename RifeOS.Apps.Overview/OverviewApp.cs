using RifeOS.Apps.Overview.ViewModels;
using RifeOS.Apps.Overview.Views;
using RifeOS.SDK.App;
using RifeOS.SDK.Context;
using RifeOS.SDK.Enums;
using RifeOS.SDK.Models;

namespace RifeOS.Apps.Overview;

public class OverviewApp : IRifeApp
{
    public AppMetadata Metadata { get; } = new(
        "Overview",
        "个人总览",
        "个人状态与生命数据看板",
        "1.0.0",
        "RifeOS",
        "OverviewIcon",
        AppCategory.Productivity
    );

    private IRifeAppContext? _context;
    private OverviewViewModel? _viewModel;

    public void Initialize(IRifeAppContext context)
    {
        _context = context;
    }

    public object CreateView()
    {
        if (_context == null) throw new InvalidOperationException("OverviewApp 尚未初始化。");
        _viewModel = new OverviewViewModel(_context);
        return new OverviewMainView(_viewModel);
    }

    public void Cleanup()
    {
        // 沙箱销毁前的资源回收钩子
        _viewModel?.SaveDataCommand.Execute(null);
    }
}