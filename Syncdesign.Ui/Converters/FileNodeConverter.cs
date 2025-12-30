using System.Windows.Data;

namespace Syncdesign.Ui.Converters;

public class FileNodeToIconConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        //if (value is FileNode item)
        //{
        //    if (item.IsFolder)
        //    {
        //        return IconHelper.GetIcon(item.Name, isDirectory: true, smallIcon: true);
        //    }
        //    else
        //    {
        //        return IconHelper.GetIconByExtension(item.Name, smallIcon: true);
        //    }
        //}
        return null;
    }
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
 
public class FileNodeToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        //if (value is FileNode item)
        //{
        //    var node = value as FileNode;
        //    switch (node.Permission)
        //    {
        //        case Contracts.Enums.Permission.None:
        //            return "#6b7280"; // Gray500 
        //        case Contracts.Enums.Permission.Read:
        //            return "#16a34a"; // Green600
        //        case Contracts.Enums.Permission.Write:
        //            return "#2563eb"; // Blue600
        //        case Contracts.Enums.Permission.Admin:
        //            return "#DC2626"; //Red600
        //        default:
        //            return "#4B5563"; // Gray600
        //    }
        //}
        return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}    
