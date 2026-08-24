using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RifeOS.Host.Context;
using RifeOS.Host.Kernel.Lifecycle;
using RifeOS.Host.Kernel.Loader;
using RifeOS.Host.Services;
using RifeOS.Host.ViewModels;
using RifeOS.Host.Views;
using RifeOS.SDK.Context;
using RifeOS.SDK.EventBus;
using RifeOS.SDK.Services;

namespace RifeOS.Host;

public static class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        var host = CreateHostBuilder(args).Build();
        App.AppHost = host;

        var app = new App();
        app.InitializeComponent();
        app.Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, services) =>
            {
                services.AddSingleton<DynamicAppLoader>();
                services.AddSingleton<UserProfileService>();
                services.AddSingleton<IUserProfile>(sp => sp.GetRequiredService<UserProfileService>());
                services.AddSingleton<ThemeService>();
                services.AddSingleton<IThemeContext>(sp => sp.GetRequiredService<ThemeService>());
                services.AddSingleton<EventBusService>();
                services.AddSingleton<IEventBus>(sp => sp.GetRequiredService<EventBusService>());
                services.AddSingleton<NotificationService>();
                services.AddSingleton<INotificationService>(sp => sp.GetRequiredService<NotificationService>());

                services.AddTransient<Func<string, IRifeAppContext>>(sp => appId =>
                {
                    var user = sp.GetRequiredService<IUserProfile>();
                    var theme = sp.GetRequiredService<IThemeContext>();
                    var notif = sp.GetRequiredService<INotificationService>();
                    var bus = sp.GetRequiredService<IEventBus>();
                    var storage = new AppStorageService(appId);
                    var config = new AppConfigurationService(appId);
                    return new RifeAppContext(user, theme, storage, config, notif, bus);
                });

                services.AddSingleton<AppLifecycleManager>();
                services.AddSingleton<ShellViewModel>();
                services.AddSingleton<ShellWindow>();
            });
}
