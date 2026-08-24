using CommunityToolkit.Mvvm.ComponentModel;

namespace RifeOS.Apps.Overview.Models;

public partial class OverviewData : ObservableObject
{
    // 今日状态
    [ObservableProperty] private int _todayGoalRate = 80;
    [ObservableProperty] private double _studyHoursToday = 3.0;
    [ObservableProperty] private bool _isExerciseDone = true;
    [ObservableProperty] private int _readPages = 30;

    // 人生数据面板
    [ObservableProperty] private double _totalStudyHours = 342.5;
    [ObservableProperty] private int _habitCompletionRate = 85;
    [ObservableProperty] private int _goalProgress = 65;
    [ObservableProperty] private int _tasksCompleted = 128;
}

public class GrowthPoint
{
    public string Month { get; set; } = string.Empty;
    public double Value { get; set; }
    // 映射到 UI 柱状图的高度 (例如上限 100px)
    public double UIHeight => Value;
}