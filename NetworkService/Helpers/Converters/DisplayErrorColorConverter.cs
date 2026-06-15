using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace NetworkService.Helpers.Converters
{
    public class DisplayErrorColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((int)value < 350 && (int)value > 250);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
