using RifeOS.SDK.Context;
using RifeOS.SDK.EventBus;
using RifeOS.SDK.Services;

namespace RifeOS.Host.Context;

public class RifeAppContext : IRifeAppContext
{
    public string AppId { get; }
    public string DataDirectory { get; }

    public IUserProfile CurrentUser { get; }
    public IThemeContext Theme { get; }
    public IAppStorage Storage { get; }
    public IAppConfiguration Config { get; }
    public INotificationService Notification { get; }
    public IEventBus EventBus { get; }

    public RifeAppContext(
        string appId,
        string dataDirectory,
        IUserProfile currentUser,
        IThemeContext theme,
        IAppStorage storage,
        IAppConfiguration config,
        INotificationService notification,
        IEventBus eventBus)
    {
        AppId = appId;
        DataDirectory = dataDirectory;
        CurrentUser = currentUser;
        Theme = theme;
        Storage = storage;
        Config = config;
        Notification = notification;
        EventBus = eventBus;
    }
}