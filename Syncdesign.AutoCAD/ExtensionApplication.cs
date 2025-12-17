using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.Windows;
using Syncdesign.AutoCAD.View;
using System.IO;
using System.Reflection;
using System.Windows;

[assembly: ExtensionApplication(typeof(Syncdesign.AutoCAD.ExtensionApplication))]
namespace Syncdesign.AutoCAD
{
    public class ExtensionApplication : IExtensionApplication
    {

        private readonly static PaletteSet PaletteSet = new PaletteSet("协同设计");

        public void Initialize()
        {
            try
            {
                LoadDll();

                MainControl mainControl = new MainControl();
                PaletteSet.MinimumSize = new System.Drawing.Size(300, 300);
                PaletteSet.AddVisual("工作管理", mainControl, true);

                PaletteSet.Style = PaletteSetStyles.NameEditable | PaletteSetStyles.ShowCloseButton;
                PaletteSet.Dock = DockSides.Left;
                PaletteSet.Visible = true; 

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
                throw;
            } 
        }
    }
}
