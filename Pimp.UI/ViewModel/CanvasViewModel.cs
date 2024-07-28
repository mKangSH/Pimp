﻿using CommunityToolkit.Mvvm.ComponentModel;
using Pimp.Common.Attributes;
using Pimp.Common.Log;
using Pimp.Model;
using Pimp.UI;
using Pimp.UI.Manager;
using Pimp.UI.Model;
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
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;

namespace Pimp.ViewModel
{
    public class MethodInfoWrapper : ObservableObject
    {
        public MethodInfo MethodInfo { get; }

        private Visibility _visibility;
        public Visibility Visibility
        {
            get => _visibility;
            set => SetProperty(ref _visibility, value);
        }

        public MethodInfoWrapper(MethodInfo methodInfo)
        {
            MethodInfo = methodInfo;
            _visibility = Visibility.Visible; // 기본값을 Visible로 설정
        }

        public string Name => MethodInfo.Name;
    }

    public class CanvasViewModel : ObservableObject
    {
        public ObservableCollection<MethodInfoWrapper> ProcessingUnitMethods { get; }
        public ObservableCollection<GridLine> GridLines { get; }

        public CanvasViewModel()
        {
            Type type = typeof(OpenCvSharp.Cv2);
            MethodInfo[] methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Static);
            ProcessingUnitMethods = new ObservableCollection<MethodInfoWrapper>(methods.Select(m => new MethodInfoWrapper(m)));

            GridLines = new ObservableCollection<GridLine>();
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











