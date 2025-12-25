using Autodesk.AutoCAD.Windows;
using Microsoft.Extensions.DependencyInjection;
using Syncdesign.Presentation.View;
using Syncdesign.Presentation.ViewModel;


namespace Syncdesign.Presentation;

/// <summary>
/// 提供配置和初始化应用程序服务及依赖项的方法。
/// </summary>
/// <remarks>Bootstrapper 类通常在应用程序启动时使用，
/// 用于向依赖注入容器注册服务、视图模型和其他依赖项。
/// 这使得可以在整个应用程序中集中管理服务的生命周期和依赖关系。</remarks>
public sealed class Bootstrapper  
{
    private static readonly Lazy<Bootstrapper> _instance =  new(() => new Bootstrapper());
    public static Bootstrapper Instance => _instance.Value;

    public ServiceProvider ServiceProvider { get; private set; } = null!;

    private Bootstrapper() { } // 私有构造函数

    public void Init()
    {
        if (ServiceProvider != null) return; // 已初始化
        ServiceProvider = ConfigureServices();
    }


    public ServiceProvider ConfigureServices()
    {
        PaletteSet paletteSet = new("协同设计");
        paletteSet.MinimumSize = new System.Drawing.Size(300, 300);
        paletteSet.Style = PaletteSetStyles.NameEditable | PaletteSetStyles.ShowCloseButton;
        paletteSet.Dock = DockSides.Left;
        paletteSet.Visible = true;
        paletteSet.KeepFocus = true;

        var services = new ServiceCollection();

        // 注册外部资源
        services.AddSingleton(paletteSet);

        // 注册 Views
        services.AddSingleton<MainView>();

        // 注册 ViewModels
        services.AddSingleton<MainViewModel>();
        services.AddSingleton<ContainerViewModel>();
        services.AddSingleton<SidebarViewModel>();
        services.AddSingleton<HeaderViewModel>();
        services.AddSingleton<UserListViewModel>();

        return services.BuildServiceProvider();
    }
}
