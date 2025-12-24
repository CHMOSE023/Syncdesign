using Microsoft.Extensions.DependencyInjection;
using Syncdesign.Presentation.View;
using System.Windows;

namespace Syncdesign.Presentation.Example
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent(); 
            var mainView = App.ServiceProvider?.GetService<MainView>();
            Content = mainView;
        }
    }
}