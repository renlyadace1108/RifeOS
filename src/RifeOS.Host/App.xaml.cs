using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RifeOS.Host.Services;
using RifeOS.Host.Views;

namespace RifeOS.Host;

/// <summary>
/// The git is very difficult to use!@Renly
/// </summary>
public partial class App : Application
{
    public static IHost AppHost { get; set; } = null!;

    public App()
    {
    }

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        await AppHost.StartAsync();

        var themeService = AppHost.Services.GetRequiredService<ThemeService>();
        themeService.SetTheme(SDK.Enums.ThemeMode.Dark);

        var mainWindow = AppHost.Services.GetRequiredService<ShellWindow>();
        mainWindow.Show();
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        using (AppHost)
        {
            await AppHost.StopAsync();
        }
        base.OnExit(e);
    }
}
