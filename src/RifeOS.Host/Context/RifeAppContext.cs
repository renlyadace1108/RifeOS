using RifeOS.SDK.Context;
using RifeOS.SDK.EventBus;
using RifeOS.SDK.Services;

namespace RifeOS.Host.Context;

public sealed class RifeAppContext : IRifeAppContext
{
    public IUserProfile CurrentUser { get; }
    public IThemeContext Theme { get; }
    public IAppStorage Storage { get; }
    public IAppConfiguration Configuration { get; }
    public INotificationService Notification { get; }
    public IEventBus EventBus { get; }

    public RifeAppContext(
        IUserProfile userProfile,
        IThemeContext themeContext,
        IAppStorage storage,
        IAppConfiguration configuration,
        INotificationService notificationService,
        IEventBus eventBus)
    {
        CurrentUser = userProfile;
        Theme = themeContext;
        Storage = storage;
        Configuration = configuration;
        Notification = notificationService;
        EventBus = eventBus;
    }
}
