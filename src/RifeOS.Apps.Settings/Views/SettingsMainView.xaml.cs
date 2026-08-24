using System.Windows.Controls;
using RifeOS.Apps.Settings.ViewModels;

namespace RifeOS.Apps.Settings.Views;

public partial class SettingsMainView : UserControl
{
    public SettingsMainView()
    {
        InitializeComponent();
    }

    public SettingsMainView(SettingsViewModel viewModel) : this()
    {
        DataContext = viewModel;
    }
}