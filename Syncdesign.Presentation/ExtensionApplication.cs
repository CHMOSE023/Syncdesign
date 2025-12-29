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
    public ExtensionApplication()
    { 
       AppDomain.CurrentDomain.AssemblyResolve += ResolveAssembly;
    }
    public void Initialize()
    { 
        try
        {
            Bootstrapper.Instance.Init();

            var mainView = Bootstrapper.Instance.ServiceProvider.GetService<MainView>();
            var paletteSet = Bootstrapper.Instance.ServiceProvider.GetService<PaletteSet>();

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
