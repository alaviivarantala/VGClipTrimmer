using GameHighlightClipper.Helpers;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace GameHighlightClipper.Converters
{
    public class DDTEnumToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((DragDropType)value)
            {
                case DragDropType.File:
                    return Application.Current.FindResource("FileIcon");

                case DragDropType.Files:
                    return Application.Current.FindResource("FilesIcon");

                case DragDropType.Folder:
                    return Application.Current.FindResource("FolderIcon");

                case DragDropType.Folders:
                    return Application.Current.FindResource("FoldersIcon");

                case DragDropType.Multiple:
                    return Application.Current.FindResource("DownloadIcon");

                case DragDropType.Invalid:
                    return Application.Current.FindResource("CrossIcon");

                default:
                    return Binding.DoNothing;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}