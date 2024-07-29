using Pimp.UI.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Pimp.UI.Models.CanvasModels
{
    public struct CanvasPosition
    {
        public double X { get; set; }
        public double Y { get; set; }
    }

    public class PimpCanvasObject : IVisibility
    {
        protected Visibility _visibility;
        public Visibility Visibility
        {
            get => _visibility;
            set { _visibility = value; }
        }

        public CanvasPosition CanvasPos { get; set; }
    }
}
