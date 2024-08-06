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
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
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

            MethodDoubleClickCommand = new RelayCommand<MouseButtonEventArgs>(OnMethodDoubleClick);
            LeftButtonDownCommand_MethodInstance = new RelayCommand<MouseButtonEventArgs>(OnLeftButtonDown_MethodInstance);
            LeftButtonDownCommand_ParameterInfo = new RelayCommand<MouseButtonEventArgs>(OnLeftButtonDown_ParameterInfo);

            LeftButtonUpCommand_MethodInstance = new RelayCommand<MouseButtonEventArgs>(OnLeftButtonUp_MethodInstance);
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

        public ICommand MethodDoubleClickCommand { get; }
        private void OnMethodDoubleClick(object parameter)
        {
            if (parameter is MouseButtonEventArgs e)
            {
                // 마우스 클릭 위치 가져오기
                Point clickPosition = e.GetPosition((((e.Source as ListBox).Parent as StackPanel).Parent as Canvas));

                MethodInfoObject methodInfoObject = ((e.OriginalSource as FrameworkElement)?.DataContext as MethodInfoObject);
                if (methodInfoObject == null || methodInfoObject.Visibility == Visibility.Collapsed)
                {
                    return;
                }

                ((e.Source as ListBox)?.Parent as StackPanel).Visibility = Visibility.Collapsed;

                methodInfoObject.CanvasPos = new CanvasPosition { X = clickPosition.X, Y = clickPosition.Y };
                CanvasMethodInfoInstances.Add(methodInfoObject);
            }
        }

        public ICommand LeftButtonDownCommand_MethodInstance { get; }
        private void OnLeftButtonDown_MethodInstance(object parameter)
        {
            // Grid || TextBlock || Polygon
            if (parameter is MouseButtonEventArgs e)
            {
                if(e.OriginalSource is Polygon polygon)
                {
                    
                }
                else if (e.OriginalSource is TextBlock textBlock)
                {

                }
            }
        }

        public ICommand LeftButtonUpCommand_MethodInstance { get; }
        private void OnLeftButtonUp_MethodInstance(object parameter)
        {
            // Grid || TextBlock || Polygon
            if (parameter is MouseButtonEventArgs e)
            {
                if (e.OriginalSource is Polygon polygon)
                {

                }
                else if (e.OriginalSource is TextBlock textBlock)
                {

                }
            }
        }

        public ICommand LeftButtonDownCommand_ParameterInfo { get; }
        public void OnLeftButtonDown_ParameterInfo(object parameter)
        {
            if (parameter is MouseButtonEventArgs e)
            {
                if ((e.OriginalSource is TextBlock || e.OriginalSource is Ellipse) == false)
                {
                    return;
                }

                ParameterInfo parameterInfo = (e.OriginalSource as FrameworkElement)?.DataContext as ParameterInfo;
            }
        }
    }
}