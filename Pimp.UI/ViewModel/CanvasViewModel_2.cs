using Pimp.Common.Attributes;
using Pimp.Common.Command;
using Pimp.Common.Interface;
using Pimp.Common.Log;
using Pimp.Model;
using Pimp.UI.Manager;
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
    public class CanvasViewModel_2 : INotifyPropertyChanged
    {
        private readonly static SolidColorBrush BlackBrush = new SolidColorBrush(Color.FromArgb(200, 0, 0, 0));
        private readonly static SolidColorBrush AliceBlueBrush = new SolidColorBrush(Color.FromArgb(70, 240, 248, 255));

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        UInt64 _imageCount = 1;
        public UInt64 ImageCount
        {
            get
            {
                if (_imageCount == UInt64.MaxValue)
                {
                    _imageCount = 1;
                }

                return _imageCount++;
            }
        }

        UInt64 _moduleCount = 1;
        public UInt64 ModuleCount
        {
            get
            {
                if (_moduleCount == UInt64.MaxValue)
                {
                    _moduleCount = 1;
                }

                return _moduleCount++;
            }
        }

        UInt64 _resultModuleCount = 1;
        public UInt64 ResultModuleCount
        {
            get
            {
                if (_resultModuleCount == UInt64.MaxValue)
                {
                    _resultModuleCount = 1;
                }

                return _resultModuleCount++;
            }
        }

        public ObservableCollection<CanvasInstanceBaseModel> CanvasInstances { get; private set; } = new ObservableCollection<CanvasInstanceBaseModel>();

        public ObservableCollection<CanvasEdge> Edges { get; } = new ObservableCollection<CanvasEdge>();

        public ObservableCollection<GridLine> GridLines { get; } = new ObservableCollection<GridLine>();

        public CanvasViewModel_2()
        {
            for (int i = 0; i < 1000; i++)
            {
                if (i % 10 == 0)
                {
                    GridLines.Add(new GridLine { StartPoint = new Point(i * 10, 0), EndPoint = new Point(i * 10, 10000), GridBrush = BlackBrush });
                    GridLines.Add(new GridLine { StartPoint = new Point(0, i * 10), EndPoint = new Point(10000, i * 10), GridBrush = BlackBrush });
                }
                else
                {
                    GridLines.Add(new GridLine { StartPoint = new Point(i * 10, 0), EndPoint = new Point(i * 10, 10000), GridBrush = AliceBlueBrush });
                    GridLines.Add(new GridLine { StartPoint = new Point(0, i * 10), EndPoint = new Point(10000, i * 10), GridBrush = AliceBlueBrush });
                }
            }
        }

        public void AddInstance(CanvasInstanceBaseModel instance)
        {
            // 인스턴스의 이름이 중복되지 않도록 처리합니다.
            var count = 1;
            while (CanvasInstances.Any(i => i.Name == instance.Name))
            {
                instance.Name = $"{instance.Name}_{count++}";
            }

            CanvasInstances.Add(instance);
        }

        public void AddInstanceToCanvas(FileModel file, Point point)
        {
            var extension = file.FileExtension.ToLower();
            var className = Path.GetFileNameWithoutExtension(file.FileName);

            if (extension == ".jpg" || extension == ".png" || extension == ".gif" || extension == ".bmp")
            {
                var instance = new CanvasImageModel
                {
                    Name = $"{className}_{(ImageCount).ToString("000")}",
                    X = Math.Round(point.X),
                    Y = Math.Round(point.Y),
                    ZIndex = 0,
                    FileModel = file,

                    OutputBitmapSource = GetBitmapSource(file.FilePath).Clone(),
                };
                AddInstance(instance);
            }
            else if (extension == ".cs" && file.FilePath.Contains("Modules"))
            {
                object module = null;
                try
                {
                    module = Activator.CreateInstance(DllManager.PimpCSharpAssembly.GetType($"Pimp.CSharpAssembly.Modules.{className}"));
                }
                catch
                {
                    Logger.Instance.AddLog($"Module {className}을(를) 로드하는데 실패했습니다. \nAssembly Build를 시도해보세요!");
                    return;
                }

                if (module is IOneInputModule oneInputModule)
                {
                    var instance = new CanvasOneInputModuleModel
                    {
                        Name = $"{Path.GetFileNameWithoutExtension(file.FileName.Replace("Module", ""))}_{(ModuleCount).ToString("000")}",
                        X = Math.Round(point.X),
                        Y = Math.Round(point.Y),
                        ZIndex = 0,
                        FileModel = file,

                        ModuleInterface = oneInputModule,
                    };
                    AddInstance(instance);
                }
                else if (module is IMultiInputModule multiInputModule)
                {
                    var instance = new CanvasMultiInputModuleModel
                    {
                        Name = $"{Path.GetFileNameWithoutExtension(file.FileName.Replace("Module", ""))}_{(ModuleCount).ToString("000")}",
                        X = Math.Round(point.X),
                        Y = Math.Round(point.Y),
                        ZIndex = 0,
                        FileModel = file,

                        ModuleInterface = multiInputModule,
                    };
                    AddInstance(instance);
                }
            }
            else if (extension == ".cs" && file.FilePath.Contains("Results"))
            {
                var instance = new CanvasResultModel
                {
                    Name = $"{Path.GetFileNameWithoutExtension(file.FileName)}_{(ResultModuleCount).ToString("000")}",
                    X = Math.Round(point.X),
                    Y = Math.Round(point.Y),
                    ZIndex = 0,
                    FileModel = file,
                };
                AddInstance(instance);
            }
        }

        public void AddEdge(CanvasInstanceBaseModel start, CanvasInstanceBaseModel end)
        {
            var edge = new CanvasEdge(start, end);
            Edges.Add(edge);
        }

        public void RemoveEdge(CanvasEdge edge)
        {
            Edges.Remove(edge);
        }

        public void RemoveSelectedInstance()
        {
            CanvasInstances.RemoveAt(0);
        }

        private void SelectedInstance_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            
        }

        private void PropertyModuleModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            
        }

        private void PropertyModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            
        }

        public void RemoveAllInstances()
        {
            
        }

        public void ClearProperties()
        {

        }

        public void SaveInstances(string path)
        {
        }

        public void SaveEdges(string path)
        {
            
        }

        private SerializableDictionary<string, List<PropertyModel>> _propertiesForSerialization = new SerializableDictionary<string, List<PropertyModel>>();
        public void SaveProperties(string path)
        {
            
        }

        private void LoadProperties(string path)
        {
            
        }

        List<CanvasInstanceBaseModel> _exceptionInstances = new List<CanvasInstanceBaseModel>();
        public void LoadAllInstances(string instancePath, string edgePath)
        {
            
        }

        private void LoadInstances(string path)
        {
            
        }

        private void LoadEdges(string path)
        {
            
        }

        public void RemoveAllEdges()
        {
            
        }


        private CanvasInstanceBaseModel _copiedInstance;

        public void CopySelectedInstance()
        {

        }

        public void PasteCopiedInstance()
        {

        }

        public BitmapSource GetBitmapSource(string imagePath)
        {
            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(imagePath, UriKind.Absolute);
            bitmap.EndInit();
            return bitmap;
        }
    }
}
