using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using RifeOS.Apps.Tasks.Data;
using RifeOS.SDK.Context;
using RifeOS.SDK.Models;

namespace RifeOS.Apps.Tasks.ViewModels;

public sealed partial class TasksViewModel : ObservableObject
{
    private readonly IRifeAppContext _context;
    private readonly string _dbPath;

    [ObservableProperty]
    private string _newTaskTitle = string.Empty;

    [ObservableProperty]
    private int _newTaskPriority = 2;

    public ObservableCollection<TaskItem> Tasks { get; } = new();

    public TasksViewModel(IRifeAppContext context)
    {
        _context = context;
        _dbPath = _context.Storage.GetDatabaseFilePath("tasks.db");
    }

    public async Task InitializeDatabaseAsync()
    {
        using var db = new TasksDbContext(_dbPath);
        await db.Database.EnsureCreatedAsync();
        await LoadTasksAsync();
    }

    [RelayCommand]
    public async Task LoadTasksAsync()
    {
        using var db = new TasksDbContext(_dbPath);
        var items = await db.Tasks.OrderByDescending(t => t.CreatedAt).ToListAsync();

        Tasks.Clear();
        foreach (var item in items)
        {
            Tasks.Add(item);
        }
    }

    [RelayCommand]
    public async Task AddTaskAsync()
    {
        if (string.IsNullOrWhiteSpace(NewTaskTitle)) return;

        var task = new TaskItem
        {
            Title = NewTaskTitle.Trim(),
            Priority = NewTaskPriority,
            IsCompleted = false,
            CreatedAt = DateTime.UtcNow
        };

        using (var db = new TasksDbContext(_dbPath))
        {
            db.Tasks.Add(task);
            await db.SaveChangesAsync();
        }

        Tasks.Insert(0, task);
        NewTaskTitle = string.Empty;

        await _context.Notification.ShowAsync(new NotificationMessage(
            Title: "任务已创建",
            Content: $"「{task.Title}」已添加到清单",
            Type: SDK.Enums.NotificationType.Success,
            SourceAppId: "com.rifeos.tasks"
        ));
    }

    [RelayCommand]
    public async Task ToggleTaskAsync(TaskItem task)
    {
        using var db = new TasksDbContext(_dbPath);
        var entity = await db.Tasks.FindAsync(task.Id);
        if (entity != null)
        {
            entity.IsCompleted = task.IsCompleted;
            await db.SaveChangesAsync();
        }
    }

    [RelayCommand]
    public async Task DeleteTaskAsync(TaskItem task)
    {
        using var db = new TasksDbContext(_dbPath);
        var entity = await db.Tasks.FindAsync(task.Id);
        if (entity != null)
        {
            db.Tasks.Remove(entity);
            await db.SaveChangesAsync();
        }

        Tasks.Remove(task);
    }
}
