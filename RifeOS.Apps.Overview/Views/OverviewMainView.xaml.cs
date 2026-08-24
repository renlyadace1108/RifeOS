using System.Windows.Controls;
using RifeOS.Apps.Overview.ViewModels;

namespace RifeOS.Apps.Overview.Views;

public partial class OverviewMainView : UserControl
{
    public OverviewMainView(OverviewViewModel vm)
    {
        InitializeComponent();
        DataContext = vm;
    }
}