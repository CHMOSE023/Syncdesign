using System.Windows;
using System.Windows.Controls;

namespace Syncdesign.Ui.Controls;

public class SectionExpander : Expander
{
    static SectionExpander()
    {
        DefaultStyleKeyProperty.OverrideMetadata(
            typeof(SectionExpander),
            new FrameworkPropertyMetadata(typeof(SectionExpander)));
    }
}