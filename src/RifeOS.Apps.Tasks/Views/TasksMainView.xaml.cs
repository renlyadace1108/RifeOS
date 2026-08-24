using System.Windows.Controls;
using RifeOS.Apps.Tasks.ViewModels;

namespace RifeOS.Apps.Tasks.Views;

public partial class TasksMainView : UserControl
{
    public TasksMainView(TasksViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
