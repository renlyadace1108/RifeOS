using System.Windows.Controls;
using RifeOS.Apps.Settings.ViewModels;

namespace RifeOS.Apps.Settings.Views;

public partial class SettingsMainView : UserControl
{
    public SettingsMainView(SettingsViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
