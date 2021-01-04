using System;
using System.Windows;
using System.Windows.Data;

namespace ExpressAgent.Converters
{
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class BooleanVisibilityConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Visibility vis = Visibility.Collapsed;

            if (!(values[0] is bool value))
            {
                return null;
            }

            if (value)
            {
                vis = Visibility.Visible;
            }

            return vis;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
