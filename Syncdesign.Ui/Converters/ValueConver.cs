using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Shapes;

namespace Syncdesign.Ui.Converters;

/// <summary>
/// 生成线TreeViewItem 导线
/// </summary>
public class CanvasConverter : IValueConverter
{
    public double Indent { get; set; } = 20; // 必须与 TreeViewItem 的缩进一致

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var item = value as TreeViewItem;
        if (item == null) return null;

        var lines = new List<Line>();
        var grayBrush = new System.Windows.Media.SolidColorBrush(Color.FromRgb(156, 163, 175)); // gray-400
        grayBrush.Freeze(); // 提高性能

        // 1. 获取完整的祖先链条，判断每一层是否是最后一条
        var ancestorStatus = GetAncestorStatus(item);
        int depth = ancestorStatus.Count;

        if (depth == 0) return lines;

        // 2. 绘制左侧所有层级的垂直线
        for (int i = 0; i < depth; i++)
        {
            bool isLastAtDepth = ancestorStatus[i];
            bool isCurrentLevel = (i == depth - 1);

            // 如果不是当前层级，且该层级是其父级的最后一个，则不画垂直延续线
            if (!isCurrentLevel && isLastAtDepth) continue;

            double xPos = i * Indent + Indent / 2;

            lines.Add(new Line
            {
                X1 = xPos,
                X2 = xPos,
                Y1 = 0,
                // 如果是当前层级的最后一个节点，垂直线只画一半（L型）
                Y2 = (isCurrentLevel && isLastAtDepth) ? 11 : 22,
                Stroke = grayBrush,
                StrokeThickness = 0.8,
                SnapsToDevicePixels = true
            });
        }

        // 3. 绘制当前节点的水平线
        lines.Add(new Line
        {
            X1 = (depth - 1) * Indent + Indent / 2,
            X2 = depth * Indent,
            Y1 = 10,
            Y2 = 10,
            Stroke = grayBrush,
            StrokeThickness = 0.8,
            SnapsToDevicePixels = true
        });

        return lines;
    }

    // 返回一个布尔列表，表示从根到当前节点的每一层是否为该层最后一个子项
    private List<bool> GetAncestorStatus(TreeViewItem item)
    {
        var status = new List<bool>();
        var current = item;

        while (true)
        {
            var parent = GetParentTreeViewItem(current);
            if (parent == null) break;

            var itemsControl = ItemsControl.ItemsControlFromItemContainer(current);
            if (itemsControl != null)
            {
                int index = itemsControl.ItemContainerGenerator.IndexFromContainer(current);
                status.Insert(0, index == itemsControl.Items.Count - 1);
            }
            current = parent;
        }
        return status;
    }

    private TreeViewItem GetParentTreeViewItem(TreeViewItem item)
    {
        DependencyObject parent = VisualTreeHelper.GetParent(item);
        while (parent != null && !(parent is TreeViewItem) && !(parent is TreeView))
        {
            parent = VisualTreeHelper.GetParent(parent);
        }
        return parent as TreeViewItem;
    }
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }

}

public class IndentConverter : IValueConverter
{
    // 1. 添加一个可配置的属性来控制缩进大小
    public double IndentSize { get; set; } = 19.0; 
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        // 尝试将传入的值转换为UI元素
        var element = value as DependencyObject;
        if (element == null)
        {
            return 0.0; // 如果转换失败，返回0
        }

        // 2. 计算深度
        int level = GetDepth(element);

        // 3. 返回一个double类型的值 (深度 * 缩进大小)
        return level * this.IndentSize;
    }

    /// <summary>
    /// 使用VisualTreeHelper向上查找，计算TreeViewItem的深度
    /// </summary>
    private int GetDepth(DependencyObject item)
    {
        int depth = 0;
        var parent = VisualTreeHelper.GetParent(item);

        while (parent != null)
        {
            // 每找到一个作为父级的TreeViewItem，深度就加1
            if (parent is TreeViewItem)
            {
                depth++;
            }

            // 如果找到了顶层的TreeView，就停止查找
            if (parent is TreeView)
            {
                break;
            }

            // 继续向上查找
            parent = VisualTreeHelper.GetParent(parent);
        }
        return depth;
    }


    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }

}

public class BoolToVisible : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if ((bool)value)
            return Visibility.Visible;
        else
            return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class VisibleToReverse : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if ((Visibility)value == Visibility.Visible)
            return Visibility.Collapsed;
        else
            return Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}


public class DateConvertToColor : IValueConverter
{
    public object Convert(object values, Type targetType, object parameter, CultureInfo culture)
    {
        var calendarDayButton = (CalendarDayButton)values;
        var dateTime = (DateTime)calendarDayButton.DataContext;
        if (!calendarDayButton.IsMouseOver && !calendarDayButton.IsSelected && !calendarDayButton.IsBlackedOut && (dateTime.DayOfWeek == DayOfWeek.Saturday || dateTime.DayOfWeek == DayOfWeek.Sunday))
            return new SolidColorBrush(Color.FromArgb(255, 255, 47, 47));
        else
            return new SolidColorBrush(Color.FromArgb(255, 51, 51, 51));
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class BoolToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        bool boolValue = System.Convert.ToBoolean(value);

        if (parameter != null && bool.TryParse(parameter.ToString(), out bool invert) && invert)
        {
            boolValue = !boolValue;
        }

        return boolValue ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return (value is Visibility visibility && visibility == Visibility.Visible);
    }
}

public class StringToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var str = value as string;
        return string.IsNullOrWhiteSpace(str) ? Visibility.Collapsed : Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
