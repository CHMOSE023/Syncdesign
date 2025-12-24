using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.Windows;
using Microsoft.Extensions.DependencyInjection;
using Syncdesign.Presentation;
using Syncdesign.Presentation.View;
using System.IO;
using System.Reflection;
using System.Windows;

[assembly: ExtensionApplication(typeof(ExtensionApplication))]
namespace Syncdesign.Presentation;

public class ExtensionApplication : IExtensionApplication
{   
    private ServiceProvider ServiceProvider;
    public ExtensionApplication()
    { 
       AppDomain.CurrentDomain.AssemblyResolve += ResolveAssembly;
    }
    public void Initialize()
    { 
        try
        {
            ServiceProvider = ConfigureServices();

            var mainView = ServiceProvider.GetService<MainView>();
            var paletteSet = ServiceProvider.GetService<PaletteSet>();

            if (mainView != null && paletteSet != null)
            {
                paletteSet.AddVisual("协同设计", mainView, true);
            }
        }
        catch (System.Exception ex)
        {
            MessageBox.Show(ex.ToString());
        }

    }

    public void Terminate()
    {

    }

    public static ServiceProvider ConfigureServices()
    {
        PaletteSet paletteSet = new("协同设计");
        paletteSet.MinimumSize = new System.Drawing.Size(300, 300);
        paletteSet.Style = PaletteSetStyles.NameEditable | PaletteSetStyles.ShowCloseButton;
        paletteSet.Dock = DockSides.Left;
        paletteSet.Visible = true;
        paletteSet.KeepFocus = true;  // 保持焦点  


        var services = new ServiceCollection();

        services.AddSingleton(paletteSet);
        services.AddSingleton<MainView>();

        return services.BuildServiceProvider();
    }

    /// <summary>
    /// 订阅程序集解析失败事件,AutoCAD2020  引用不同版本dll加载失败。
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    /// https://www.cnblogs.com/bigbosscyb/p/19048531
    private static Assembly? ResolveAssembly(object sender, ResolveEventArgs args)
    {
        var requestedAssemblyName = new AssemblyName(args.Name).Name;

        var currentDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        var dllPath = Path.Combine(currentDir, requestedAssemblyName + ".dll");

        if (File.Exists(dllPath))
        {
            return Assembly.LoadFrom(dllPath);
        }            
        return null;
    }
}
