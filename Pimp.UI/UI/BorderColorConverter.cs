using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace Pimp.UI
{
    public class BorderColorConverter : IValueConverter
    {
        private static SolidColorBrush _highlightedBrush = new SolidColorBrush(Colors.MediumPurple);
        private static SolidColorBrush _defaultBrush = new SolidColorBrush(Colors.Black);

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isHighlighted && isHighlighted)
            {
                return _highlightedBrush;
            }
            return _defaultBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
