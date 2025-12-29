using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Syncdesign.Presentation.Behavior;

public class ScrollHeaderHideBehavior : Behavior<FrameworkElement>
{
    private double _currentTopMargin = 0;
    private ScrollViewer? _cachedScrollViewer;

    // 动画配置
    public double ScrollSpeed { get; set; } = 300; // 稍微调大一点，配合动画更顺滑
    public Duration Duration { get; set; } = new Duration(TimeSpan.FromMilliseconds(300)); // 动画时长

    protected override void OnAttached()
    {
        base.OnAttached();
        AssociatedObject.Loaded += OnAssociatedObjectLoaded;
    }

    private void OnAssociatedObjectLoaded(object sender, RoutedEventArgs e)
    {
        var parentGrid = GetParentObject(AssociatedObject);
        if (parentGrid != null)
        {
            parentGrid.PreviewMouseWheel += OnPreviewMouseWheel;
            _cachedScrollViewer = FindChild<ScrollViewer>(parentGrid);
        }
    }

    private void OnPreviewMouseWheel(object sender, MouseWheelEventArgs e)
    {
        double maxHide = AssociatedObject.ActualHeight;
        if (maxHide <= 0) return;

        // 1. 判定方向与状态
        bool isHiding = e.Delta < 0 && _currentTopMargin > -maxHide;
        bool isShowing = e.Delta > 0 && _currentTopMargin < 0;

        // 2. 向上滚动的额外判定（是否在列表顶部）
        if (isShowing)
        {
            if (_cachedScrollViewer != null && _cachedScrollViewer.VerticalOffset > 0)
            {
                isShowing = false;
            }
        }

        // 3. 执行平滑动画
        if (isHiding || isShowing)
        {
            double targetMargin = _currentTopMargin + (e.Delta > 0 ? ScrollSpeed : -ScrollSpeed);

            // 范围锁定
            if (targetMargin < -maxHide) targetMargin = -maxHide;
            if (targetMargin > 0) targetMargin = 0;

            if (Math.Abs(targetMargin - _currentTopMargin) > 0.1)
            {
                AnimateMargin(targetMargin);
                _currentTopMargin = targetMargin;
            }

            e.Handled = true;
        }
    }

    private void AnimateMargin(double targetTop)
    {
        // 创建厚度动画
        ThicknessAnimation animation = new ThicknessAnimation
        {
            To = new Thickness(0, targetTop, 0, 0),
            Duration = Duration,
            // 使用 QuadraticEase 让动画开头和结尾更柔和
            EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
        };

        // 如果旧动画没播完，新动画会直接覆盖它，保证操作连贯性
        AssociatedObject.BeginAnimation(FrameworkElement.MarginProperty, animation);
    }

    // --- 辅助方法保持不变 ---
    private UIElement? GetParentObject(DependencyObject obj)
    {
        var parent = VisualTreeHelper.GetParent(obj);
        while (parent != null && !(parent is Grid)) parent = VisualTreeHelper.GetParent(parent);
        return parent as UIElement;
    }

    private T? FindChild<T>(DependencyObject parent) where T : DependencyObject
    {
        if (parent == null) return null;
        int count = VisualTreeHelper.GetChildrenCount(parent);
        for (int i = 0; i < count; i++)
        {
            var child = VisualTreeHelper.GetChild(parent, i);
            if (child is T t) return t;
            var res = FindChild<T>(child);
            if (res != null) return res;
        }
        return null;
    }

    protected override void OnDetaching()
    {
        var parentGrid = GetParentObject(this.AssociatedObject);
        if (parentGrid != null) parentGrid.PreviewMouseWheel -= OnPreviewMouseWheel;
        base.OnDetaching();
    }
}