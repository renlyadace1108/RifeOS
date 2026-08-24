using RifeOS.Apps.Settings.ViewModels;
using RifeOS.Apps.Settings.Views;
using RifeOS.SDK.App;
using RifeOS.SDK.Context;
using RifeOS.SDK.Enums;
using RifeOS.SDK.Models;

namespace RifeOS.Apps.Settings;

public sealed class SettingsApp : IRifeApp
{
    private SettingsViewModel? _viewModel;
    private SettingsMainView? _view;

    public AppMetadata Metadata { get; } = new(
        Id: "com.rifeos.settings",
        Name: "系统设置",
        Version: "1.0.0",
        Description: "个性化主题、用户画像与系统关于配置",
        Icon: "SettingsIcon",
        Author: "Renly",
        Category: AppCategory.System
    );

    public AppLifecycleState State { get; private set; } = AppLifecycleState.Created;

    public Task InitializeAsync(IRifeAppContext context, CancellationToken cancellationToken = default)
    {
        State = AppLifecycleState.Initializing;
        _viewModel = new SettingsViewModel(context);
        _view = new SettingsMainView(_viewModel);
        State = AppLifecycleState.Running;
        return Task.CompletedTask;
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
