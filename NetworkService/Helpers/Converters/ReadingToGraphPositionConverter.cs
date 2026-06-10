using System;
using System.Globalization;
using System.Windows.Data;

namespace NetworkService.Helpers.Converters
{
    public class ReadingToGraphPositionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (double)value + double.Parse((string)parameter);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
