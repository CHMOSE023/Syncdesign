using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace Syncdesign.Presentation.Example
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
      
        protected override void OnStartup(StartupEventArgs e)
        {
            Bootstrapper.Instance.Init(); 
            base.OnStartup(e);             
        }

    }

}
