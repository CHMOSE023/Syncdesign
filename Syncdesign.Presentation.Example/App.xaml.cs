using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace Syncdesign.Presentation.Example
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static ServiceProvider? ServiceProvider;
        protected override void OnStartup(StartupEventArgs e)
        {
            ServiceProvider = ExtensionApplication.ConfigureServices();
            base.OnStartup(e);             
        }

    }

}
