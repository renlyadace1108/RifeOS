using RifeOS.SDK.EventBus;
using RifeOS.SDK.Services;

namespace RifeOS.SDK.Context;

public interface IRifeAppContext
{
    string AppId { get; }
    string DataDirectory { get; }
    string AppDataDirectory => DataDirectory;

    IUserProfile CurrentUser { get; }
    IThemeContext Theme { get; }
    IAppStorage Storage { get; }
    IAppConfiguration Config { get; }
    INotificationService Notification { get; }
    IEventBus EventBus { get; }
}