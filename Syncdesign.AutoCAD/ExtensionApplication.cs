using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.Windows;
using Serilog;
using Syncdesign.AutoCAD.Logging;
using Syncdesign.AutoCAD.View;
using System;
using System.IO;
using System.Reflection;
using System.Windows;

[assembly: ExtensionApplication(typeof(Syncdesign.AutoCAD.ExtensionApplication))]
namespace Syncdesign.AutoCAD
{  
    public class ExtensionApplication : IExtensionApplication
    {
        public ExtensionApplication()
        {
            AppDomain.CurrentDomain.AssemblyResolve += ResolveAssembly;           
        }
        private readonly static PaletteSet PaletteSet = new PaletteSet("协同设计");

        public void Initialize()
        {
            LogBootstrap.Init(); // 初始化日志程序 

            try
            { 
                Log.Information("ExtensionApplication-> Initialize -> 插件已加载 ");

                MainControl mainControl = new MainControl();
                PaletteSet.MinimumSize = new System.Drawing.Size(300, 300);
                PaletteSet.AddVisual("协同设计", mainControl, true);

                PaletteSet.Style = PaletteSetStyles.NameEditable | PaletteSetStyles.ShowCloseButton;
                PaletteSet.Dock = DockSides.Left;
                PaletteSet.Visible = true;
                PaletteSet.KeepFocus = true;  // 保持焦点  

            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }

        public void Terminate()
        {
            Log.Information("ExtensionApplication-> Terminate -> 插件已卸载");
        }

        /// <summary>
        /// 加载必要的dll
        /// </summary>
        private void LoadDll()
        {
            try
            {
                string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);  
                var dir = new DirectoryInfo(path);
                var dlls = dir.GetFiles("*.dll");
                foreach (var dll in dlls)
                {
                    Assembly.LoadFrom(dll.FullName);
                }

            }
            catch (System.Exception ex )
            {
                MessageBox.Show(ex.ToString());                
            } 
        }

        /// <summary>
        /// 订阅程序集解析失败事件,AutoCAD2020  引用不同版本dll加载失败。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        /// https://www.cnblogs.com/bigbosscyb/p/19048531

        private static Assembly ResolveAssembly(object sender, ResolveEventArgs args)
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
}
