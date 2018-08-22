using System;
using System.Globalization;
using Xamarin.Forms;

namespace Timeline.Converters
{
    class InverseDefaultStringToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is string))
            {
                throw new InvalidOperationException("Value must be string");
            }

            if (String.Equals((String)value, "Default")) return false;
            return true; ;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }

}
