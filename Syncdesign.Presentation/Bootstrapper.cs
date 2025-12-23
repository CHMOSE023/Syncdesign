using Syncdesign.Presentation.View;
using System.Windows;


namespace Syncdesign.Presentation;

public class Bootstrapper : PrismBootstrapper
{
    protected override DependencyObject? CreateShell()
    {
        return null;
    }
      
    protected override void RegisterTypes(IContainerRegistry containerRegistry)
    {
        containerRegistry.RegisterSingleton<MainView>();

        var regionManager = Container.Resolve<IRegionManager>();

        regionManager.RegisterViewWithRegion("MainContentRegion", typeof(MainView));
    }
}
