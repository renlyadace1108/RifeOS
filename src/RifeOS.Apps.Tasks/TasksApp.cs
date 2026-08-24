using RifeOS.Apps.Tasks.ViewModels;
using RifeOS.Apps.Tasks.Views;
using RifeOS.SDK.App;
using RifeOS.SDK.Context;
using RifeOS.SDK.Enums;
using RifeOS.SDK.Models;

namespace RifeOS.Apps.Tasks;

public sealed class TasksApp : IRifeApp
{
    private TasksViewModel? _viewModel;
    private TasksMainView? _view;

    public AppMetadata Metadata { get; } = new(
        Id: "com.rifeos.tasks",
        Name: "任务清单",
        Version: "1.0.0",
        Description: "聚焦高效的任务管理与执行跟踪",
        Icon: "TaskIcon",
        Author: "Renly",
        Category: AppCategory.Productivity
    );

    public AppLifecycleState State { get; private set; } = AppLifecycleState.Created;

    public async Task InitializeAsync(IRifeAppContext context, CancellationToken cancellationToken = default)
    {
        State = AppLifecycleState.Initializing;
        _viewModel = new TasksViewModel(context);
        await _viewModel.InitializeDatabaseAsync();
        _view = new TasksMainView(_viewModel);
        State = AppLifecycleState.Running;
    }

    public object CreateView()
    {
        return _view ?? throw new InvalidOperationException("视图尚未初始化。");
    }

    public Task OnSuspendAsync(CancellationToken cancellationToken = default)
    {
        State = AppLifecycleState.Suspended;
        return Task.CompletedTask;
    }

    public Task OnTerminateAsync(CancellationToken cancellationToken = default)
    {
        State = AppLifecycleState.Terminated;
        _view = null;
        _viewModel = null;
        return Task.CompletedTask;
    }
}
