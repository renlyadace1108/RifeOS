using System.Collections.ObjectModel;
using System.Text.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RifeOS.Apps.Tasks.Models;
using RifeOS.SDK.Context;

namespace RifeOS.Apps.Tasks.ViewModels;

public partial class TasksViewModel : ObservableObject
{
    private readonly IRifeAppContext _context;
    private const string DataFileName = "tasks.json";

    public ObservableCollection<TaskItem> Tasks { get; } = new();

    [ObservableProperty] private string _newTaskTitle = string.Empty;

    public TasksViewModel(IRifeAppContext context)
    {
        _context = context;
        _ = LoadTasksAsync();
    }

    private async Task LoadTasksAsync()
    {
        try
        {
            var json = await _context.Storage.ReadTextAsync(DataFileName);
            if (!string.IsNullOrWhiteSpace(json))
            {
                var list = JsonSerializer.Deserialize<List<TaskItem>>(json);
                if (list != null)
                {
                    Tasks.Clear();
                    foreach (var item in list) Tasks.Add(item);
                }
            }
        }
        catch { }
    }

    private async Task SaveTasksAsync()
    {
        try
        {
            var json = JsonSerializer.Serialize(Tasks.ToList(), new JsonSerializerOptions { WriteIndented = true });
            await _context.Storage.WriteTextAsync(DataFileName, json);
        }
        catch { }
    }

    [RelayCommand]
    private async Task AddTask()
    {
        if (string.IsNullOrWhiteSpace(NewTaskTitle)) return;

        var task = new TaskItem
        {
            Title = NewTaskTitle.Trim(),
            IsCompleted = false
        };

        Tasks.Insert(0, task);
        NewTaskTitle = string.Empty;
        await SaveTasksAsync();
        _context.Notification.Show("任务已添加", task.Title);
    }

    [RelayCommand]
    private async Task ToggleTask(TaskItem task)
    {
        await SaveTasksAsync();
    }

    [RelayCommand]
    private async Task DeleteTask(TaskItem task)
    {
        Tasks.Remove(task);
        await SaveTasksAsync();
        _context.Notification.Show("任务已删除", task.Title);
    }
}