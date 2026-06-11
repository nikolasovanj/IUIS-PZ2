using NetworkService.ViewModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace NetworkService.Helpers.Converters
{
    public class TypeCountToGraphWidthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int count = MainWindowViewModel.Entities.Count;
            double ret;
            if (parameter != null && bool.Parse((string)parameter))
            {
                ret = count - (double)value;
            }
            else
            {
                ret = (double)value;
            }
                return ret / count * 240;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
