using System;
using System.Windows;
using System.Windows.Markup;

namespace Syncdesign.Ui.Markup
{
    [Localizability(LocalizationCategory.Ignore)]
    [Ambient]
    [UsableDuringInitialization(true)]
    public class ControlsDictionary : ResourceDictionary
    {
        private const string DictionaryUri = "pack://application:,,,/Syncdesign.Ui;component/Resources/Syncdesign.Ui.xaml";
 
        public ControlsDictionary()
        {
            Source = new Uri(DictionaryUri, UriKind.Absolute);
        }
    }
}
