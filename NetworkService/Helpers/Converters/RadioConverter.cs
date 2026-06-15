using NetworkService.Helpers.Filters;
using System;
using System.Globalization;
using System.Windows.Data;

namespace NetworkService.Helpers.Converters
{
    public class IDFilterRadioConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((IDFilter?)value == null)
            {
                return false;
            }
            return ((IDFilter)value).Equals((IDFilter)Enum.Parse(typeof(IDFilter), (string)parameter));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? parameter : Binding.DoNothing;
        }
    }
    public class ValueRadioConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((ValueFilter?)value == null)
            {
                return false;
            }
            return ((ValueFilter)value).Equals((ValueFilter)Enum.Parse(typeof(ValueFilter), (string)parameter));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? parameter : Binding.DoNothing;
        }
    }
}
