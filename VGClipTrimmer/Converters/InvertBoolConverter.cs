using System;
using System.Globalization;
using System.Windows.Data;

namespace VGClipTrimmer.Converters
{
    public class InvertBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Convert(value, targetType);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Convert(value, targetType);
        }

        private static object Convert(object value, Type targetType)
        {
            if (value is bool)
            {
                return !(bool)value;
            }
            throw new ArgumentException();
        }
    }
}