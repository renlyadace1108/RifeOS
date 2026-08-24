using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using RifeOS.Host.Models;
using RifeOS.Host.ViewModels;

namespace RifeOS.Host.Views;

public partial class DesktopView : UserControl
{
    // 图标拖拽字段
    private bool _isIconDragging = false;
    private Point _iconClickOffset;
    private DesktopIconItem? _draggedIcon;

    // 小组件拖拽字段
    private bool _isWidgetDragging = false;
    private Point _widgetClickOffset;
    private DesktopWidgetItem? _draggedWidget;

    public DesktopView()
    {
        InitializeComponent();
    }

    // ================= 桌面快捷方式拖拽逻辑 =================
    private void Canvas_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ClickCount == 2) return;
        if (e.OriginalSource is DependencyObject source)
        {
            var element = FindVisualParent<ContentPresenter>(source);
            if (element != null && element.Content is DesktopIconItem icon)
            {
                _draggedIcon = icon;
                _isIconDragging = true;
                _iconClickOffset = e.GetPosition(element);
                if (sender is Canvas canvas) canvas.CaptureMouse();
            }
        }
    }

    private void Canvas_PreviewMouseMove(object sender, MouseEventArgs e)
    {
        if (_isIconDragging && _draggedIcon != null && sender is Canvas canvas)
        {
            var mousePos = e.GetPosition(canvas);
            _draggedIcon.X = mousePos.X - _iconClickOffset.X;
            _draggedIcon.Y = mousePos.Y - _iconClickOffset.Y;
        }
    }

    private void Canvas_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        if (_isIconDragging && _draggedIcon != null && sender is Canvas canvas)
        {
            _isIconDragging = false;
            canvas.ReleaseMouseCapture();
            if (DataContext is DesktopViewModel vm && vm.SnapToGrid)
            {
                _draggedIcon.X = Math.Round(_draggedIcon.X / vm.IconSize) * vm.IconSize;
                _draggedIcon.Y = Math.Round(_draggedIcon.Y / vm.IconSize) * vm.IconSize;
            }
            _draggedIcon = null;
        }
    }

    private void DesktopIcon_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (sender is Button btn && btn.DataContext is DesktopIconItem iconItem)
        {
            if (Application.Current.MainWindow?.DataContext is ShellViewModel shellVm)
            {
                if (shellVm.LaunchAppCommand.CanExecute(iconItem.AppKey))
                    shellVm.LaunchAppCommand.Execute(iconItem.AppKey);
            }
        }
    }

    // ================= 桌面小组件拖拽逻辑 =================
    private void Widget_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.OriginalSource is DependencyObject source)
        {
            var element = FindVisualParent<ContentPresenter>(source);
            // 只有处于“非锁定”状态的小组件才允许被鼠标捕获拖拽
            if (element != null && element.Content is DesktopWidgetItem widget && !widget.IsPinned)
            {
                _draggedWidget = widget;
                _isWidgetDragging = true;
                _widgetClickOffset = e.GetPosition(element);

                if (sender is Canvas canvas) canvas.CaptureMouse();
                e.Handled = true; // 拦截事件，防止穿透到底层
            }
        }
    }

    private void Widget_PreviewMouseMove(object sender, MouseEventArgs e)
    {
        if (_isWidgetDragging && _draggedWidget != null && sender is Canvas canvas)
        {
            var mousePos = e.GetPosition(canvas);
            _draggedWidget.X = mousePos.X - _widgetClickOffset.X;
            _draggedWidget.Y = mousePos.Y - _widgetClickOffset.Y;
        }
    }

    private void Widget_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        if (_isWidgetDragging && _draggedWidget != null && sender is Canvas canvas)
        {
            _isWidgetDragging = false;
            canvas.ReleaseMouseCapture();
            _draggedWidget = null;
        }
    }

    private static T? FindVisualParent<T>(DependencyObject child) where T : DependencyObject
    {
        DependencyObject parentObject = System.Windows.Media.VisualTreeHelper.GetParent(child);
        if (parentObject == null) return null;
        if (parentObject is T parent) return parent;
        return FindVisualParent<T>(parentObject);
    }
}