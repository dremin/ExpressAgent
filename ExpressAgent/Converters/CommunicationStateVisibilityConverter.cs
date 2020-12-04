using System;
using System.Windows;
using System.Windows.Data;

namespace ExpressAgent.Converters
{
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class CommunicationStateVisibilityConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Visibility vis = Visibility.Collapsed;

            if (!(values[0] is string communicationState) || !(values[1] is string desiredState))
            {
                return null;
            }

            if (communicationState == desiredState)
            {
                vis = Visibility.Visible;
            }
            else if (values.Length > 2 && values[2] is string desiredState2 && communicationState == desiredState2)
            {
                vis = Visibility.Visible;
            }
            else if (values.Length > 3 && values[3] is string desiredState3 && communicationState == desiredState3)
            {
                vis = Visibility.Visible;
            }
            else if (values.Length > 4 && values[4] is string desiredState4 && communicationState == desiredState4)
            {
                vis = Visibility.Visible;
            }
            else if (values.Length > 5 && values[5] is string desiredState5 && communicationState == desiredState5)
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
