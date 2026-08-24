using RifeOS.SDK.EventBus;
using RifeOS.SDK.Services;

namespace RifeOS.SDK.Context;

public interface IRifeAppContext
{
    IUserProfile CurrentUser { get; }
    IThemeContext Theme { get; }
    IAppStorage Storage { get; }
    IAppConfiguration Configuration { get; }
    INotificationService Notification { get; }
    IEventBus EventBus { get; }
}
