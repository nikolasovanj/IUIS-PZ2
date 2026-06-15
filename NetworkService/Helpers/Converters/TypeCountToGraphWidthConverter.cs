using NetworkService.ViewModel;
using System;
using System.Globalization;
using System.Windows.Data;

namespace NetworkService.Helpers.Converters
{
    public class TypeCountToGraphWidthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int count = MainWindowViewModel.Entities.Count;
            return (double)value / count * 240;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
