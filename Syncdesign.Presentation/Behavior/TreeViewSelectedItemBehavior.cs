using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Controls;

namespace Syncdesign.Presentation.Behavior;

public class TreeViewSelectedItemBehavior : Behavior<TreeView>
{
    // 定义依赖属性，用于绑定 ViewModel 中的属性
    public object SelectedItem
    {
        get => GetValue(SelectedItemProperty);
        set => SetValue(SelectedItemProperty, value);
    }

    public static readonly DependencyProperty SelectedItemProperty =
        DependencyProperty.Register(nameof(SelectedItem), typeof(object),
            typeof(TreeViewSelectedItemBehavior),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    protected override void OnAttached()
    {
        base.OnAttached();
        // 监听 TreeView 的选中改变事件
        AssociatedObject.SelectedItemChanged += OnTreeViewSelectedItemChanged;
    }

    protected override void OnDetaching()
    {
        base.OnDetaching();
        if (AssociatedObject != null)
        {
            AssociatedObject.SelectedItemChanged -= OnTreeViewSelectedItemChanged;
        }
    }

    private void OnTreeViewSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        // 当 UI 选中项改变时，更新依赖属性（从而更新 ViewModel）
        SelectedItem = e.NewValue;
    }
}
