using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace NetworkService.Helpers.Converters
{
    internal class GraphVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Visibility ret = ((int[])value).Any(i => i == 0) ? Visibility.Visible : Visibility.Hidden;
            if(parameter != null)
            {
                ret = bool.Parse((string)parameter) ? ret == Visibility.Visible ? Visibility.Hidden : Visibility.Visible : ret;  
            }
            return ret;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
