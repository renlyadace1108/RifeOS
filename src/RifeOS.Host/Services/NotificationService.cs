using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.Messaging;
using RifeOS.Host.ViewModels;
using RifeOS.SDK.Enums;
using RifeOS.SDK.Models;
using RifeOS.SDK.Services;

namespace RifeOS.Host.Services;

public sealed class NotificationService : INotificationService, IRecipient<NotificationMessage>
{
    public static NotificationService Instance { get; } = new();

    public ObservableCollection<NotificationItemViewModel> ActiveToasts { get; } = new();

    public NotificationService()
    {
        WeakReferenceMessenger.Default.Register(this);
    }

    public void Receive(NotificationMessage message)
    {
        if (Application.Current?.Dispatcher == null) return;

        Application.Current.Dispatcher.Invoke(() =>
        {
            var item = new NotificationItemViewModel(message.Title, message.Content, message.Level);
            ActiveToasts.Insert(0, item);
            _ = AutoRemoveAsync(item, message.DurationSeconds);
        });
    }

    private async Task AutoRemoveAsync(NotificationItemViewModel item, int durationSeconds)
    {
        await Task.Delay(TimeSpan.FromSeconds(durationSeconds));
        Application.Current?.Dispatcher.Invoke(() =>
        {
            ActiveToasts.Remove(item);
        });
    }

    public void Show(string title, string content, NotificationLevel level = NotificationLevel.Info)
    {
        WeakReferenceMessenger.Default.Send(new NotificationMessage(title, content, level));
    }

    public Task ShowAsync(NotificationMessage message, CancellationToken cancellationToken = default)
    {
        Receive(message);
        return Task.CompletedTask;
    }
}