using CommunityToolkit.Mvvm.ComponentModel;

namespace RifeOS.Apps.Tasks.Models;

public partial class TaskItem : ObservableObject
{
    [ObservableProperty] private string _id = Guid.NewGuid().ToString("N");
    [ObservableProperty] private string _title = string.Empty;
    [ObservableProperty] private bool _isCompleted;
    [ObservableProperty] private DateTime _createdAt = DateTime.Now;
}