using System;
using System.Globalization;
using System.Windows.Data;

namespace NetworkService.Helpers.Converters
{
    public class ReadingToGraphPositionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return 250.0 - ((int)value - int.Parse((string)parameter)) * 5 / 6;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
