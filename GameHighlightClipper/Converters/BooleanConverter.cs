using System;
using System.Globalization;
using System.Windows.Data;

namespace GameHighlightClipper.Converters
{
    public class BooleanConverter<T> : IValueConverter
    {
        public T False { get; set; }
        public T True { get; set; }

        public virtual object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is bool))
                return False;
            else
                return (bool)value ? True : False;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}