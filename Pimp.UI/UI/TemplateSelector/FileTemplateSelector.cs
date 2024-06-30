using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using Pimp.Model;

namespace Pimp.UI
{
    public class FileTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var element = container as FrameworkElement;

            if (element != null && item != null && item is FileModel)
            {
                var file = item as FileModel;
                switch (file.FileType)
                {
                    case FileType.Image:
                        return element.FindResource("ImageFileTemplate") as DataTemplate;
                    case FileType.CSharp:
                        return element.FindResource("CSharpFileTemplate") as DataTemplate;
                    default:
                        return null;
                }
            }

            return null;
        }
    }
}
