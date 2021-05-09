using GameHighlightClipper.Helpers;
using System;
using System.Globalization;
using System.Windows.Data;

namespace GameHighlightClipper.Converters
{
    public class TimelineEventStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TimelineEvent timelineEvent)
            {
                return timelineEvent.Start + "-" + (timelineEvent.Start + timelineEvent.Duration);
            }
            return "Error";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}