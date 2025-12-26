using System.Collections;
using System.Windows;
using System.Windows.Controls;

namespace Syncdesign.Ui.Controls;

public class Messages : Control
{
    static Messages()
    {
        DefaultStyleKeyProperty.OverrideMetadata(
            typeof(Messages),
            new FrameworkPropertyMetadata(typeof(Messages)));
    }

    #region ItemsSource

    public IEnumerable ItemsSource
    {
        get => (IEnumerable)GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    public static readonly DependencyProperty ItemsSourceProperty =
        DependencyProperty.Register(
            nameof(ItemsSource),
            typeof(IEnumerable),
            typeof(Messages),
            new PropertyMetadata(null));

    #endregion

    #region SelectedItem

    public object SelectedItem
    {
        get => GetValue(SelectedItemProperty);
        set => SetValue(SelectedItemProperty, value);
    }

    public static readonly DependencyProperty SelectedItemProperty =
        DependencyProperty.Register(
            nameof(SelectedItem),
            typeof(object),
            typeof(Messages),
            new FrameworkPropertyMetadata(
                null,
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    #endregion
}
