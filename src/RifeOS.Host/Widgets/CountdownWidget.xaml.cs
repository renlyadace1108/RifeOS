using System.Windows.Controls;

namespace RifeOS.Host.Widgets;

public partial class CountdownWidget : UserControl
{
    public CountdownWidget()
    {
        InitializeComponent();
        var targetDate = new DateTime(2026, 12, 26);
        var remainingDays = (targetDate - DateTime.Today).Days;
        TxtDays.Text = remainingDays > 0 ? remainingDays.ToString() : "0";
    }
}