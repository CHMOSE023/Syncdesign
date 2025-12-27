using System.Windows;
using System.Windows.Controls;

namespace Syncdesign.Presentation.Resources;

public class ViewTemplateSelector : DataTemplateSelector
{
    public string? TemplateKeyPrefix { get; set; } // 可选，前缀
    public override DataTemplate? SelectTemplate(object item, DependencyObject container)
    {
        if (item == null) return null;

        if (container is FrameworkElement element)
        {
            // 动态根据类型名查找资源
            var key = item.GetType().Name + "Template"; // e.g. MessageListViewModel -> MessageListTemplate
            var resource = element.TryFindResource(key) as DataTemplate;
            return resource ?? base.SelectTemplate(item, container);
        }

        return base.SelectTemplate(item, container);
    }
}
