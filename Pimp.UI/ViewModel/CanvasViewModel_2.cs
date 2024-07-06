﻿using Pimp.Common.Attributes;
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
    /// <summary>
    /// CanvasViewModel Version 2
    /// DLL Load, Unload 테스트용 클래스
    /// 현재 SelectedInstance 및 CanvasIntances에서 인스턴스 삭제 후 DLL 언로드 테스트 정상 수행 확인 완료
    /// TODO : Resource Manager 클래스를 이용해야 함.
    /// </summary>
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

        public ObservableCollection<PropertyModel> Properties { get; } = new ObservableCollection<PropertyModel>();

        // Properties 컬렉션을 ListCollectionView로 감쌉니다.
        private ListCollectionView _propertiesView;
        public ListCollectionView PropertiesView
        {
            get { return _propertiesView; }
            set
            {
                _propertiesView = value;
                OnPropertyChanged(nameof(PropertiesView));
            }
        }

        public ObservableCollection<CanvasInstanceBaseModel> CanvasInstances { get; private set; } = new ObservableCollection<CanvasInstanceBaseModel>();

        public ObservableCollection<CanvasEdge> Edges { get; } = new ObservableCollection<CanvasEdge>();

        public ObservableCollection<GridLine> GridLines { get; } = new ObservableCollection<GridLine>();

        private CanvasInstanceBaseModel _selectedInstance;
        public CanvasInstanceBaseModel SelectedInstance
        {
            get => _selectedInstance;
            set
            {
                if (_selectedInstance == value)
                {
                    return;
                }
                
                if (value == null)
                {
                    _selectedInstance.PropertyChanged -= SelectedInstance_PropertyChanged;
                }

                _selectedInstance = value;
                if(_selectedInstance != null)
                {
                    UpdateProperties();
                    _selectedInstance.PropertyChanged += SelectedInstance_PropertyChanged;
                }
                
                OnPropertyChanged(nameof(SelectedInstance));
            }
        }

        private void SelectedInstance_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // SelectedInstance가 null인지 확인합니다.
            if (SelectedInstance == null)
            {
                return;
            }

            var property = Properties.FirstOrDefault(p => p.Name == e.PropertyName);
            if (property == null)
            {
                return;
            }

            property.Value = _selectedInstance.GetType().GetProperty(e.PropertyName).GetValue(_selectedInstance);
        }

        private void UpdateProperties()
        {
            ClearProperties();

            var properties = _selectedInstance.GetType().GetProperties().Where(p => !Attribute.IsDefined(p, typeof(UIHiddenAttribute)));

            foreach (var property in properties)
            {
                var propertyModel = new PropertyModel(property.Name, property.GetValue(_selectedInstance), property);
                propertyModel.PropertyChanged += PropertyModel_PropertyChanged;
                Properties.Add(propertyModel);
            }

            // _selectedInstance가 CanvasModuleModel 타입인 경우 ModuleInterface의 속성도 추가
            if (_selectedInstance is CanvasOneInputModuleModel canvasOneInputModuleModel && canvasOneInputModuleModel.ModuleInterface != null)
            {
                var moduleInterfaceProperties = canvasOneInputModuleModel.ModuleInterface.GetType().GetProperties().Where(p => (Attribute.IsDefined(p, typeof(UIHiddenAttribute)) == false));
                foreach (var property in moduleInterfaceProperties)
                {
                    var propertyModel = new PropertyModel(property.Name, property.GetValue(canvasOneInputModuleModel.ModuleInterface), property);
                    propertyModel.PropertyChanged += PropertyModuleModel_PropertyChanged;
                    Properties.Add(propertyModel);
                }
            }
            else if (_selectedInstance is CanvasMultiInputModuleModel canvasMultiInputModuleModel && canvasMultiInputModuleModel.ModuleInterface != null)
            {
                var moduleInterfaceProperties = canvasMultiInputModuleModel.ModuleInterface.GetType().GetProperties().Where(p => (Attribute.IsDefined(p, typeof(UIHiddenAttribute)) == false));
                foreach (var property in moduleInterfaceProperties)
                {
                    var propertyModel = new PropertyModel(property.Name, property.GetValue(canvasMultiInputModuleModel.ModuleInterface), property);
                    propertyModel.PropertyChanged += PropertyModuleModel_PropertyChanged;
                    Properties.Add(propertyModel);
                }
            }
        }

        private void PropertyModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "Value")
            {
                return;
            }

            var propertyModel = (PropertyModel)sender;
            var property = _selectedInstance.GetType().GetProperty(propertyModel.Name);
            var value = Convert.ChangeType(propertyModel.Value, property.PropertyType);
            property.SetValue(_selectedInstance, value);
        }

        private void PropertyModuleModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "Value")
            {
                return;
            }

            if (_selectedInstance is CanvasOneInputModuleModel oneInputModule)
            {
                var propertyModel = (PropertyModel)sender;
                var property = oneInputModule.ModuleInterface.GetType().GetProperty(propertyModel.Name);

                var value = Convert.ChangeType(propertyModel.Value, property.PropertyType);
                property.SetValue(oneInputModule.ModuleInterface, value);

                // X, Y, ZIndex 속성은 모듈에 적용되지 않도록 함 (캔버스 이동 관련 Property)
                if (property.Name == "X" || property.Name == "Y" || property.Name == "ZIndex")
                {
                    return;
                }

                oneInputModule.ModuleInterface.Run();
                oneInputModule.OutputBitmapSource = oneInputModule.ModuleInterface.OutputImage;
                if (oneInputModule.ModuleInterface.OverlayImage != null && oneInputModule.ModuleInterface.OverlayImage.Width > 0 && oneInputModule.ModuleInterface.OverlayImage.Height > 0)
                {
                    oneInputModule.OverlayBitmapSource = oneInputModule.ModuleInterface.OverlayImage.Clone();
                }
                else
                {
                    oneInputModule.OverlayBitmapSource = null;
                }
                oneInputModule.Run();
            }
            else if (_selectedInstance is CanvasMultiInputModuleModel multiInputModule)
            {
                var propertyModel = (PropertyModel)sender;
                var property = multiInputModule.ModuleInterface.GetType().GetProperty(propertyModel.Name);

                var value = Convert.ChangeType(propertyModel.Value, property.PropertyType);
                property.SetValue(multiInputModule.ModuleInterface, value);

                // X, Y, ZIndex 속성은 모듈에 적용되지 않도록 함 (캔버스 이동 관련 Property)
                if (property.Name == "X" || property.Name == "Y" || property.Name == "ZIndex")
                {
                    return;
                }

                multiInputModule.ModuleInterface.Run();
                multiInputModule.OutputBitmapSource = multiInputModule.ModuleInterface.OutputImage;
                if (multiInputModule.ModuleInterface.OverlayImage != null && multiInputModule.ModuleInterface.OverlayImage.Width > 0 && multiInputModule.ModuleInterface.OverlayImage.Height > 0)
                {
                    multiInputModule.OverlayBitmapSource = multiInputModule.ModuleInterface.OverlayImage.Clone();
                }
                else
                {
                    multiInputModule.OverlayBitmapSource = null;
                }
                multiInputModule.Run();
            }
        }

        public CanvasViewModel_2()
        {
            PropertiesView = new ListCollectionView(Properties);
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
            var className = Path.GetFileNameWithoutExtension(file.FileName);

            if (file.FileType == FileType.Image)
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
            else if (file.FileType == FileType.CSharp && file.FilePath.Contains("Modules"))
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
            else if (file.FileType == FileType.CSharp && file.FilePath.Contains("Results"))
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

        public void RemoveSelectedInstance()
        {
            if (SelectedInstance == null)
            {
                return;
            }

            // 선택한 인스턴스와 연결된 간선을 찾아 제거합니다.
            var connectedEdges = Edges.Where(edge => edge.Start == _selectedInstance || edge.End == _selectedInstance).ToList();
            foreach (var edge in connectedEdges)
            {
                RemoveEdge(_selectedInstance, edge);
            }

            if (SelectedInstance is CanvasOneInputModuleModel oneInputModuleModel)
            {
                oneInputModuleModel.ModuleInterface = null;
            }
            else if(SelectedInstance is CanvasMultiInputModuleModel multiInputModuleModel)
            {
                multiInputModuleModel.ModuleInterface = null;
            }

            CanvasInstances.Remove(SelectedInstance);

            ClearProperties();
        }

        public void RemoveAllInstances()
        {
            foreach (var instance in CanvasInstances)
            {
                _selectedInstance = instance;
                RemoveSelectedInstance();
            }
            
            SelectedInstance = null;
        }

        public void SaveInstances(string path)
        {
            if (Directory.Exists(Directory.GetParent(path).FullName) == false)
            {
                Directory.CreateDirectory(Directory.GetParent(path).FullName);
            }

            XmlSerializer serializer = new XmlSerializer(typeof(ObservableCollection<CanvasInstanceBaseModel>));
            using (TextWriter writer = new StreamWriter(path))
            {
                serializer.Serialize(writer, CanvasInstances);
            }

            if (path.Contains("_temp"))
            {
                SaveProperties($"{Directory.GetParent(path).FullName}\\Properties_temp.xml");
            }
            else
            {
                SaveProperties($"{Directory.GetParent(path).FullName}\\Properties.xml");
            }
        }

        List<CanvasInstanceBaseModel> _exceptionInstances = new List<CanvasInstanceBaseModel>();
        public void LoadInstances(string path)
        {
            // 역직렬화
            XmlSerializer serializer = new XmlSerializer(typeof(ObservableCollection<CanvasInstanceBaseModel>));
            using (TextReader reader = new StreamReader(path))
            {
                CanvasInstances = (ObservableCollection<CanvasInstanceBaseModel>)serializer.Deserialize(reader);
            }

            foreach (var instance in CanvasInstances)
            {
                try
                {
                    if (instance is CanvasImageModel)
                    {
                        instance.OutputBitmapSource = GetBitmapSource(instance.FileModel.FilePath).Clone();
                    }
                    else if (instance is CanvasOneInputModuleModel)
                    {
                        var className = Path.GetFileNameWithoutExtension(instance.FileModel.FileName);

                        var module = Activator.CreateInstance(DllManager.PimpCSharpAssembly.GetType($"Pimp.CSharpAssembly.Modules.{className}"));
                        (instance as CanvasOneInputModuleModel).ModuleInterface = module as IOneInputModule;
                    }
                    else if (instance is CanvasMultiInputModuleModel)
                    {
                        var className = Path.GetFileNameWithoutExtension(instance.FileModel.FileName);

                        var module = Activator.CreateInstance(DllManager.PimpCSharpAssembly.GetType($"Pimp.CSharpAssembly.Modules.{className}"));
                        (instance as CanvasMultiInputModuleModel).ModuleInterface = module as IMultiInputModule;
                    }
                }
                catch
                {
                    _exceptionInstances.Add(instance);
                }
            }

            if (path.Contains("_temp"))
            {
                LoadProperties($"{Directory.GetParent(path).FullName}\\Properties_temp.xml");
            }
            else
            {
                LoadProperties($"{Directory.GetParent(path).FullName}\\Properties.xml");
            }

            OnPropertyChanged(nameof(CanvasInstances));
        }

        public void AddEdge(CanvasInstanceBaseModel start, CanvasInstanceBaseModel end)
        {
            var edge = new CanvasEdge(start, end);
            Edges.Add(edge);
        }

        public void RemoveEdge(CanvasInstanceBaseModel instance, CanvasEdge edge)
        { 
            if (edge.Start == instance)
            {
                edge.End.OutputBitmapSource = null;
                edge.End.Run();
            }

            if (edge.End is CanvasOneInputModuleModel endModule)
            {
                endModule.CanConnect = true;
            }

            else if (edge.End is CanvasResultModel resultModule)
            {
                resultModule?.DeleteResult(instance.Name);
                edge.Start.NameChanged -= resultModule.ParentNameChanged;
            }

            edge.Start.OutputBitmapSourceChanged -= edge.End.OnOutputBitmapSourceChanged;
            Edges.Remove(edge);
        }

        public void SaveEdges(string path)
        {
            if (Directory.Exists(Directory.GetParent(path).FullName) == false)
            {
                Directory.CreateDirectory(Directory.GetParent(path).FullName);
            }

            XmlSerializer serializer = new XmlSerializer(typeof(ObservableCollection<CanvasEdge>));
            using (TextWriter writer = new StreamWriter(path))
            {
                serializer.Serialize(writer, Edges);
            }
        }

        public void LoadEdges(string path)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(ObservableCollection<CanvasEdge>));
            using (TextReader reader = new StreamReader(path))
            {
                var edges = (ObservableCollection<CanvasEdge>)serializer.Deserialize(reader);
                foreach (var edge in edges)
                {
                    var edgeStart = CanvasInstances.FirstOrDefault(instance => instance.Name == edge.Start.Name);
                    var edgeEnd = CanvasInstances.FirstOrDefault(instance => instance.Name == edge.End.Name);

                    if (_exceptionInstances.Contains(edge.Start) || _exceptionInstances.Contains(edge.End) || edgeStart == null || edgeEnd == null)
                    {
                        continue;
                    }

                    if (edgeEnd is CanvasOneInputModuleModel && edgeStart != edgeEnd)
                    {
                        // Module의 Input은 1개이므로 이미 연결된 경우 더 이상 연결할 수 없습니다.
                        if ((edgeEnd as CanvasOneInputModuleModel).CanConnect)
                        {
                            (edgeEnd as CanvasOneInputModuleModel).CanConnect = false;
                            edgeStart.OutputBitmapSourceChanged -= edgeEnd.OnOutputBitmapSourceChanged;
                            edgeStart.OutputBitmapSourceChanged += edgeEnd.OnOutputBitmapSourceChanged;
                            AddEdge(edgeStart, edgeEnd);

                            (edgeEnd as CanvasOneInputModuleModel)?.OnOutputBitmapSourceChanged(edgeStart.Name, edgeStart.OutputBitmapSource);
                        }
                    }
                    else if (edgeEnd is CanvasMultiInputModuleModel && edgeStart != edgeEnd)
                    {
                        // TODO : MultiInputModule의 동작 정의 필요
                        edgeStart.OutputBitmapSourceChanged -= edgeEnd.OnOutputBitmapSourceChanged;
                        edgeStart.OutputBitmapSourceChanged += edgeEnd.OnOutputBitmapSourceChanged;
                        AddEdge(edgeStart, edgeEnd);

                        (edgeEnd as CanvasMultiInputModuleModel)?.OnOutputBitmapSourceChanged(edgeStart.Name, edgeStart.OutputBitmapSource);
                    }
                    else if (edgeEnd is CanvasResultModel && edgeStart != edgeEnd)
                    {
                        edgeStart.OutputBitmapSourceChanged -= edgeEnd.OnOutputBitmapSourceChanged;
                        edgeStart.OutputBitmapSourceChanged += edgeEnd.OnOutputBitmapSourceChanged;
                        edgeStart.NameChanged -= (edgeEnd as CanvasResultModel).ParentNameChanged;
                        edgeStart.NameChanged += (edgeEnd as CanvasResultModel).ParentNameChanged;
                        AddEdge(edgeStart, edgeEnd);

                        (edgeEnd as CanvasResultModel)?.OnOutputBitmapSourceChanged(edgeStart.Name, edgeStart.OutputBitmapSource);
                    }
                }
            }
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

        private SerializableDictionary<string, List<PropertyModel>> _propertiesForSerialization = new SerializableDictionary<string, List<PropertyModel>>();
        public void SaveProperties(string path)
        {
            // TODO : 동작이 너무 느린 경우 SelectedInstance 조회를 줄이는 방법을 찾아야 합니다.
            var lastSelectedInstance = SelectedInstance;
            
            foreach (var instance in CanvasInstances)
            {
                SelectedInstance = instance;
                _propertiesForSerialization.Add(SelectedInstance.Name, Properties.ToList());
            }
            SelectedInstance = lastSelectedInstance;

            XmlSerializer serializer = new XmlSerializer(typeof(SerializableDictionary<string, List<PropertyModel>>));
            using (TextWriter writer = new StreamWriter(path))
            {
                serializer.Serialize(writer, _propertiesForSerialization);
            }

            _propertiesForSerialization.Clear();
        }

        private void LoadProperties(string path)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(SerializableDictionary<string, List<PropertyModel>>));
            using (TextReader reader = new StreamReader(path))
            {
                var propertiesOfAllIntances = (SerializableDictionary<string, List<PropertyModel>>)serializer.Deserialize(reader);

                // TODO : CanvasMultiInputModuleModel의 동작 정의 필요
                foreach (var instance in CanvasInstances.OfType<CanvasOneInputModuleModel>())
                {
                    if (propertiesOfAllIntances.TryGetValue(instance.Name, out var properties) == false)
                    {
                        continue;
                    }

                    foreach (var property in properties)
                    {
                        if (property.Name == "X" || property.Name == "Y")
                        {
                            continue;
                        }

                        var instanceProperty = instance.ModuleInterface.GetType().GetProperty(property.Name);
                        object value;

                        if (instanceProperty.PropertyType.IsEnum)
                        {
                            value = Enum.ToObject(instanceProperty.PropertyType, property.Value);
                        }
                        else
                        {
                            value = Convert.ChangeType(property.Value, instanceProperty.PropertyType);
                        }

                        instanceProperty.SetValue(instance.ModuleInterface, value);
                    }
                }
            }
        }

        public void ClearProperties()
        {
            // Properties 컬렉션을 Clear합니다.
            foreach (var propertyModel in Properties)
            {
                propertyModel.PropertyChanged -= PropertyModel_PropertyChanged;
                propertyModel.PropertyChanged -= PropertyModuleModel_PropertyChanged;
            }

            Properties.Clear();
            PropertiesView.Refresh();
        }
    }
}