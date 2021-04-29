using System;
using System.Globalization;
using System.Windows.Data;

namespace GameHighlightClipper.Converters
{
    public sealed class IntToBoolInvertConverter : IValueConverter
    {
        public int Value { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int number)
            {
                return number != Value;
            }
            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
