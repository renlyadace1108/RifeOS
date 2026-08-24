using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using RifeOS.Host.ViewModels;

namespace RifeOS.Host.Views;

public partial class ShellWindow : Window
{
    private bool _isContextMenuOpen = false;

    public ShellWindow()
    {
        InitializeComponent();
    }

    private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed)
        {
            DragMove();
        }
    }

    private void StartButton_MouseEnter(object sender, MouseEventArgs e)
    {
        if (DataContext is ShellViewModel vm)
        {
            vm.OpenStartMenu();
        }
    }

    private void StartMenu_ContextMenuOpening(object sender, ContextMenuEventArgs e)
    {
        _isContextMenuOpen = true;
    }

    private void StartMenu_ContextMenuClosing(object sender, ContextMenuEventArgs e)
    {
        _isContextMenuOpen = false;
    }

    private void StartMenu_MouseLeave(object sender, MouseEventArgs e)
    {
        // 若右键菜单正在展示，绝不收起开始菜单
        if (_isContextMenuOpen) return;

        if (sender is FrameworkElement element)
        {
            var pos = e.GetPosition(element);
            // 双重校验：若光标仍处于开始菜单矩形范围内，不予关闭
            if (pos.X >= 0 && pos.X <= element.ActualWidth && pos.Y >= 0 && pos.Y <= element.ActualHeight)
            {
                return;
            }
        }

        if (DataContext is ShellViewModel vm)
        {
            vm.CloseStartMenu();
        }
    }
}