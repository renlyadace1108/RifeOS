using System.Collections.ObjectModel;
using System.Text.Json;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RifeOS.Apps.Overview.Models;
using RifeOS.SDK.Context;
using RifeOS.SDK.Models;

namespace RifeOS.Apps.Overview.ViewModels;

public partial class OverviewViewModel : ObservableObject
{
    private readonly IRifeAppContext _context;
    private const string DataFile = "overview_data.json";

    [ObservableProperty] private OverviewData _data = new();

    public ObservableCollection<GrowthPoint> GrowthCurve { get; } = new();

    public OverviewViewModel(IRifeAppContext context)
    {
        _context = context;
        LoadMockGrowthData();
        _ = LoadDataAsync();
    }

    private void LoadMockGrowthData()
    {
        // 模拟长期成长曲线数据
        GrowthCurve.Add(new GrowthPoint { Month = "3月", Value = 20 });
        GrowthCurve.Add(new GrowthPoint { Month = "4月", Value = 35 });
        GrowthCurve.Add(new GrowthPoint { Month = "5月", Value = 50 });
        GrowthCurve.Add(new GrowthPoint { Month = "6月", Value = 45 });
        GrowthCurve.Add(new GrowthPoint { Month = "7月", Value = 70 });
        GrowthCurve.Add(new GrowthPoint { Month = "8月", Value = 85 });
    }

    // 广播方法：将最新数据发送给系统底层的事件总线
    private void BroadcastWidgetUpdate()
    {
        _context.EventBus.Publish(new WidgetUpdateMessage
        {
            TodayGoalRate = Data.TodayGoalRate,
            StudyHours = Data.StudyHoursToday,
            IsExerciseDone = Data.IsExerciseDone
        });
    }

    private async Task LoadDataAsync()
    {
        try
        {
            var json = await _context.Storage.ReadTextAsync(DataFile);
            if (!string.IsNullOrWhiteSpace(json))
            {
                var savedData = JsonSerializer.Deserialize<OverviewData>(json);
                if (savedData != null) Data = savedData;
            }
        }
        catch { }
        finally
        {
            // 数据加载完毕后，通知桌面小组件更新
            BroadcastWidgetUpdate();
        }
    }

    [RelayCommand]
    private async Task SaveDataAsync()
    {
        try
        {
            var json = JsonSerializer.Serialize(Data, new JsonSerializerOptions { WriteIndented = true });
            await _context.Storage.WriteTextAsync(DataFile, json);
            _context.Notification.Show("数据已同步", "今日成长数据已保存至独立沙箱。");

            // 数据保存修改后，通知桌面小组件更新
            BroadcastWidgetUpdate();
        }
        catch { }
    }
}