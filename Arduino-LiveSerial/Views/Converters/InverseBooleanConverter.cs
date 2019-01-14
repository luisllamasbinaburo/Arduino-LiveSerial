using System;
using System.Windows.Data;


namespace Arduino_LiveSerial.View.Converters
{
    [ValueConversion(typeof(object), typeof(bool))]
    public class InverseBooleanConverter : IValueConverter
    {
        //<converters:InverseBooleanConverter x:Key="InverseBooleanConverter" />

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!(value is bool)) throw new InvalidOperationException("The target must be a boolean");
            return !(bool)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!(value is bool)) throw new InvalidOperationException("The target must be a boolean");
            return !(bool)value;
        }
    }
}
