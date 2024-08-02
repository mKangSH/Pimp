using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Pimp.Common.Attributes;
using Pimp.Common.Log;
using Pimp.UI;
using Pimp.UI.Manager;
using Pimp.UI.Models;
using Pimp.UI.Models.CanvasModels;
using Pimp.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;

namespace Pimp.ViewModel
{
    public class CanvasViewModel : ObservableObject
    {
        public ObservableCollection<MethodInfoObject> ProcessingUnitMethods { get; }
        public ObservableCollection<GridLine> GridLines { get; } = new ObservableCollection<GridLine>();

        public ObservableCollection<MethodInfoObject> CanvasMethodInfoInstances { get; } = new ObservableCollection<MethodInfoObject>();

        public CanvasViewModel()
        {
            InitGridLines();

            Type type = typeof(OpenCvSharp.Cv2);
            MethodInfo[] methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Static);
            ProcessingUnitMethods = new ObservableCollection<MethodInfoObject>(methods.Select(m => new MethodInfoObject(m)));
        }
        
        private void InitGridLines()
        {
            for (int i = 0; i < 1000; i++)
            {
                if (i % 10 == 0)
                {
                    GridLines.Add(new GridLine { StartPoint = new Point(i * 10, 0), EndPoint = new Point(i * 10, 10000), GridBrush = GlobalConst.BlackBrush });
                    GridLines.Add(new GridLine { StartPoint = new Point(0, i * 10), EndPoint = new Point(10000, i * 10), GridBrush = GlobalConst.BlackBrush });
                }
                else
                {
                    GridLines.Add(new GridLine { StartPoint = new Point(i * 10, 0), EndPoint = new Point(i * 10, 10000), GridBrush = GlobalConst.AliceBlueBrush });
                    GridLines.Add(new GridLine { StartPoint = new Point(0, i * 10), EndPoint = new Point(10000, i * 10), GridBrush = GlobalConst.AliceBlueBrush });
                }
            }
        }
    }
}