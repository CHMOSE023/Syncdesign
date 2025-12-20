using Syncdesign.Ui.Appearance;
using System;
using System.Windows;
using System.Windows.Markup;

namespace Syncdesign.Ui.Markup
{
    [Localizability(LocalizationCategory.Ignore)]
    [Ambient]
    [UsableDuringInitialization(true)]
    public class ThemesDictionary : ResourceDictionary
    {   
        private const string ThemesDictionaryPath = "pack://application:,,,/Syncdesign.Ui;component/Resources/Theme/Light.xaml";
        public ThemesDictionary()
        {
            Source = new Uri(ThemesDictionaryPath, UriKind.Absolute);
        }
       

    }
}
