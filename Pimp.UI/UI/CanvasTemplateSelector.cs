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
    public class CanvasTemplateSelector : DataTemplateSelector
    {
        public DataTemplate ImageTemplate { get; set; }
        public DataTemplate ModuleTemplate { get; set; }
        public DataTemplate ResultTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is CanvasImageModel)
                return ImageTemplate;

            else if (item is CanvasModuleModel)
                return ModuleTemplate;

            else if (item is CanvasResultModel)
                return ResultTemplate;

            return base.SelectTemplate(item, container);
        }
    }
}
