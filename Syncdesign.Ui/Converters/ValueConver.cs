using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Shapes;

namespace Syncdesign.Ui.Converters;

// 递归生成线
public class CanvasConverter : IValueConverter
{
    public double Indent { get; set; } = 20;  // 横向缩进
    public double VerticalSpacing { get; set; } = 10; // 节点垂直间距
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    { 
        var element = value as DependencyObject;
        if (element == null)
        {
            return new List<Line>(); // 如果转换失败，返回0
        }
        int depth = GetDepth(element); // 先计算深度
        return GetLines(depth, Indent, VerticalSpacing);
    }

    // 先计算 TreeViewItem 深度
    private int GetDepth(DependencyObject item)
    {
        int depth = 0;
        var parent = VisualTreeHelper.GetParent(item);

        while (parent != null)
        {
            if (parent is TreeViewItem)
                depth++;

            if (parent is TreeView)
                break;

            parent = VisualTreeHelper.GetParent(parent);
        }

        return depth;
    }

    private List<Line> GetLines(int depth, double indent, double vertical)
    {
        var lines = new List<Line>();
        var gray400=(SolidColorBrush)(new BrushConverter().ConvertFromString("#9ca3af"));
        for (int i = 0; i < depth ; i++)
        {
            lines.Add(new Line
            {
                X1 = indent / 2 + i * indent,
                X2 = indent / 2 + i * indent,
                Y1 = -vertical / 2,
                Y2 = vertical/2,
                Stroke = gray400,
                StrokeThickness = 0.5
            });
        } 
        // 水平线
        if (depth > 0)
        {                
            lines.Add(new Line
            {
                X1 = depth * indent - indent / 2,
                X2 = depth * indent + indent / 2 - 6,
                Y1 = vertical / 2,
                Y2 = vertical / 2,
                Stroke = gray400, 
                StrokeThickness = 0.5
            });
        }

        return lines;
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
