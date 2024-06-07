using Pimp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;

namespace Pimp.UI
{
    public class PropertyTemplateSelector : DataTemplateSelector
    {
        public DataTemplate DefaultTemplate { get; set; }
        public DataTemplate BooleanTemplate { get; set; }
        public DataTemplate NumericTemplate { get; set; }
        public DataTemplate EnumTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is PropertyModel property)
            {
                if (property.type == typeof(bool))
                {
                    return BooleanTemplate;
                }
                else if (property.type == typeof(int) || property.type == typeof(double) || property.type == typeof(float))
                {
                    return NumericTemplate;
                }
                else if (property.type.IsEnum)
                {
                    return EnumTemplate;
                }
            }
            return DefaultTemplate;
        }
    }
}
