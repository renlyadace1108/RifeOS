using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RifeOS.Host.Kernel.Lifecycle;
using RifeOS.Host.Kernel.Loader;
using RifeOS.Host.Services;
using RifeOS.SDK.Enums;
using RifeOS.SDK.Models;

namespace RifeOS.Host.ViewModels;

public sealed partial class ShellViewModel : ObservableObject
{
    private readonly DynamicAppLoader _loader;
    private readonly AppLifecycleManager _lifecycleManager;

    [ObservableProperty]
    private UserProfileService _user;

    [ObservableProperty]
    private ThemeService _theme;

    [ObservableProperty]
    private bool _isOobeActive;

    [ObservableProperty]
    private bool _isStartMenuOpen;

    [ObservableProperty]
    private object? _currentView;

    [ObservableProperty]
    private string _activeTitle = "RifeOS";

    [ObservableProperty]
    private string _oobeNickname = "Renly";

    [ObservableProperty]
    private string _oobePassword = string.Empty;

    [ObservableProperty]
    private string _oobeSignature = "Stay focused. Build the future.";

    [ObservableProperty]
    private string _oobeSelectedTheme = "Dark";

    public ObservableCollection<AppMetadata> InstalledApps { get; } = new();
    public ObservableCollection<RunningAppSession> ActiveSessions => _lifecycleManager.ActiveSessions;

    public ShellViewModel(
        DynamicAppLoader loader,
        AppLifecycleManager lifecycleManager,
        UserProfileService user,
        ThemeService theme)
    {
        _loader = loader;
        _lifecycleManager = lifecycleManager;
        _user = user;
        _theme = theme;

        _isOobeActive = !_user.IsInitialized;

        _lifecycleManager.ActiveAppSwitched += session =>
        {
            if (session != null)
            {
                CurrentView = session.View;
                ActiveTitle = session.Metadata.Name;
            }
            else
            {
                // 当所有 App 关闭时，必须将当前视图强行置 null 断开引用链
                CurrentView = null;
                ActiveTitle = "RifeOS";
            }
        };

        RefreshInstalledApps();
    }

    [RelayCommand]
    public void CompleteOobe()
    {
        if (Enum.TryParse<ThemeMode>(OobeSelectedTheme, true, out var mode))
        {
            Theme.SetTheme(mode);
        }

        User.InitializeUser(OobeNickname, OobePassword, OobeSignature);
        IsOobeActive = false;
        RefreshInstalledApps();
    }

    [RelayCommand]
    public void RefreshInstalledApps()
    {
        InstalledApps.Clear();
        foreach (var app in _loader.DiscoverAvailableApps())
        {
            InstalledApps.Add(app);
        }
    }

    [RelayCommand]
    public void ToggleStartMenu()
    {
        IsStartMenuOpen = !IsStartMenuOpen;
    }

    [RelayCommand]
    public async Task LaunchAppAsync(string appId)
    {
        IsStartMenuOpen = false;
        await _lifecycleManager.LaunchOrActivateAsync(appId);
    }

    [RelayCommand]
    public void SwitchToApp(string appId)
    {
        _lifecycleManager.SwitchTo(appId);
    }

    [RelayCommand]
    public async Task CloseAppAsync(string appId)
    {
        await _lifecycleManager.CloseAppAsync(appId);
    }
}