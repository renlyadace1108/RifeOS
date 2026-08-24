using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Threading;

namespace RifeOS.Host.Widgets;

public partial class SystemMonitorWidget : UserControl
{
    private readonly DispatcherTimer _timer;
    private readonly Process _currentProcess = Process.GetCurrentProcess();

    public SystemMonitorWidget()
    {
        InitializeComponent();
        _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(2) };
        _timer.Tick += (s, e) => UpdateMetrics();
        _timer.Start();
        UpdateMetrics();
    }

    private void UpdateMetrics()
    {
        _currentProcess.Refresh();
        long workingSetMb = _currentProcess.WorkingSet64 / (1024 * 1024);
        TxtHostRam.Text = $"{workingSetMb} MB";

        var uptime = TimeSpan.FromMilliseconds(Environment.TickCount64);
        TxtUptime.Text = $"{(int)uptime.TotalHours}h {uptime.Minutes}m";
    }
}