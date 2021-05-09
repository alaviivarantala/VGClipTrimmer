using System;
using System.Globalization;
using System.Windows.Data;

namespace GameHighlightClipper.Converters
{
    public class SecondsTimeSpanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int seconds)
            {
                return new TimeSpan(0, 0, seconds);
            }
            return new TimeSpan(0, 0, 0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}