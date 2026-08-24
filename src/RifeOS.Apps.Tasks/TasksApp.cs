using RifeOS.Apps.Tasks.ViewModels;
using RifeOS.Apps.Tasks.Views;
using RifeOS.SDK.App;
using RifeOS.SDK.Context;
using RifeOS.SDK.Enums;
using RifeOS.SDK.Models;

namespace RifeOS.Apps.Tasks;

public class TasksApp : IRifeApp
{
    public AppMetadata Metadata { get; } = new(
        "Tasks",
        "任务清单",
        "个人任务与日程管理",
        "1.0.0",
        "RifeOS",
        "TaskIcon",
        AppCategory.Productivity
    );

    private IRifeAppContext? _context;

    public void Initialize(IRifeAppContext context)
    {
        _context = context;
    }

    public object CreateView()
    {
        if (_context == null) throw new InvalidOperationException("TasksApp 尚未初始化。");
        var viewModel = new TasksViewModel(_context);
        return new TasksMainView(viewModel);
    }

    public void Cleanup() { }
}