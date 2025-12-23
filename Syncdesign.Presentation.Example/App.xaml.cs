using System.Windows;

namespace Syncdesign.Presentation.Example
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public readonly static Bootstrapper Bootstrapper = new();
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);


            Bootstrapper.Run();
        }

    }

}
