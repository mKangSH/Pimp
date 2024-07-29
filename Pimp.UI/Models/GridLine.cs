using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Pimp.UI.Models
{
    public class GridLine
    {
        public SolidColorBrush GridBrush { get; set; }

        public Point StartPoint { get; set; }
        public Point EndPoint { get; set; }
    }
}
