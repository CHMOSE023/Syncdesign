using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media;

namespace Syncdesign.Ui.Controls;

public class Segmented : ListBox
{
    static Segmented()
    {
        DefaultStyleKeyProperty.OverrideMetadata(
            typeof(Segmented),
            new FrameworkPropertyMetadata(typeof(Segmented)));
    }

    private TranslateTransform _indicatorTransform;
    private Border _indicator;

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        _indicatorTransform = GetTemplateChild("IndicatorTransform") as TranslateTransform;
        _indicator = GetTemplateChild("Indicator") as Border;

        // 控件加载或尺寸变化时更新滑块
        this.Loaded += (s, e) => UpdateIndicator(false);
        this.SizeChanged += (s, e) => UpdateIndicator(false);
    }

    protected override void OnSelectionChanged(SelectionChangedEventArgs e)
    {
        base.OnSelectionChanged(e);
        UpdateIndicator(true);
    }

    private void UpdateIndicator(bool useAnimation)
    {
        if (_indicatorTransform == null || _indicator == null || Items.Count == 0 || SelectedIndex < 0)
            return;

        // 可用宽度
        double availableWidth = ActualWidth - (Padding.Left + Padding.Right);
        double itemWidth = availableWidth / Items.Count;

        // 设置滑块宽度
        _indicator.Width = itemWidth;

        double targetX = SelectedIndex * itemWidth;

        if (useAnimation)
        {
            var anim = new DoubleAnimation
            {
                To = targetX,
                Duration = TimeSpan.FromSeconds(0.3),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };
            _indicatorTransform.BeginAnimation(TranslateTransform.XProperty, anim);
        }
        else
        {
            _indicatorTransform.BeginAnimation(TranslateTransform.XProperty, null);
            _indicatorTransform.X = targetX;
        }
    }
}
