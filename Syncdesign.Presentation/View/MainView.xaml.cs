using Syncdesign.Presentation.ViewModel;
using System.Windows;
using System.Windows.Controls;

namespace Syncdesign.Presentation.View
{
    /// <summary>
    /// MainView.xaml 的交互逻辑
    /// </summary>
    public partial class MainView : UserControl
    {
        public MainView(MainViewModel mainViewModel)
        {
            // 全局可见只需要加载一次          
            InitializeComponent();

            var dict = new ResourceDictionary
            {
                Source = new Uri("pack://application:,,,/Syncdesign.Presentation;component/Resources/ViewTemplates.xaml", UriKind.Absolute)
            };
            Resources.MergedDictionaries.Add(dict);

            DataContext = mainViewModel;
        }
    }
}
