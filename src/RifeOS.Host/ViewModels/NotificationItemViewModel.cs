using CommunityToolkit.Mvvm.ComponentModel;
using RifeOS.SDK.Enums;

namespace RifeOS.Host.ViewModels;

public partial class NotificationItemViewModel : ObservableObject
{
    [ObservableProperty] private string _id = Guid.NewGuid().ToString("N");
    [ObservableProperty] private string _title;
    [ObservableProperty] private string _content;
    [ObservableProperty] private NotificationLevel _level;

    public NotificationItemViewModel(string title, string content, NotificationLevel level = NotificationLevel.Info)
    {
        _title = title;
        _content = content;
        _level = level;
    }
}