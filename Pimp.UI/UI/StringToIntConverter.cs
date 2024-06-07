using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows;

namespace Pimp.UI
{
    public class StringToIntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int intValue)
                return intValue.ToString();

            // TODO : Double, Float 등 다른 숫자 타입 Convert 추가 필요. (새로운 Converter 추가)
            else if (value is double doubleValue)
                return doubleValue.ToString();

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string strValue && int.TryParse(strValue, out int intValue))
                return intValue;

            // TODO : Double, Float 등 다른 숫자 타입 Convert 추가 필요. (새로운 Converter 추가)

            return DependencyProperty.UnsetValue;
        }
    }
}
