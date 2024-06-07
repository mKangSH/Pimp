using Pimp.Common.Attributes;
using Pimp.Common.Command;
using Pimp.Common.Interface;
using Pimp.Common.Log;
using Pimp.Model;
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
    public class CanvasViewModel : INotifyPropertyChanged, ICanvasViewModel
    {
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


        private CanvasInstanceBaseModel _selectedInstance;
        public CanvasInstanceBaseModel SelectedInstance
        {
            get { return _selectedInstance; }
            set
            {
                if (value == null)
                {
                    if (_selectedInstance != null)
                    {
                        _selectedInstance.PropertyChanged -= SelectedInstance_PropertyChanged;
                        if (_selectedInstance is CanvasModuleModel)
                        {
                            (_selectedInstance as CanvasModuleModel).ModuleInterface = null;
                        }
                        _selectedInstance.ZIndex = 0;
                        _selectedInstance.IsHighlighted = false;
                    }
                    _selectedInstance = null;

                    return;
                }

                if (_selectedInstance != null && _selectedInstance != value)
                {
                    _selectedInstance.ZIndex = 0;
                    _selectedInstance.IsHighlighted = false;
                    _selectedInstance.PropertyChanged -= SelectedInstance_PropertyChanged;
                }

                if (value != null && _selectedInstance != value)
                {
                    _selectedInstance = value;
                    _selectedInstance.IsHighlighted = true;
                    OnPropertyChanged("SelectedInstance");

                    // Add this condition to prevent recursive calls
                    if (_selectedInstance != null)
                    {
                        _selectedInstance.PropertyChanged += SelectedInstance_PropertyChanged;
                        UpdateProperties();
                    }
                }
            }
        }

        public ObservableCollection<CanvasInstanceBaseModel> Instances { get; private set; } = new ObservableCollection<CanvasInstanceBaseModel>();

        public ObservableCollection<CanvasEdge> Edges { get; } = new ObservableCollection<CanvasEdge>();

        public ObservableCollection<GridLine> GridLines { get; } = new ObservableCollection<GridLine>();

        private readonly static SolidColorBrush BlackBrush = new SolidColorBrush(Color.FromArgb(200, 0, 0, 0));
        private readonly static SolidColorBrush AliceBlueBrush = new SolidColorBrush(Color.FromArgb(70, 240, 248, 255));
        public CanvasViewModel()
        {
            PropertiesView = new ListCollectionView(Properties);
            for (int i = 0; i < 1000; i++)
            {
                if(i % 10 == 0)
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
            while (Instances.Any(i => i.Name == instance.Name))
            {
                instance.Name = $"{instance.Name}_{count++}";
            }

            Instances.Add(instance);
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
            else if(extension == ".cs" && file.FilePath.Contains("Modules"))
            {
                object module = null;
                try
                {
                    module = Activator.CreateInstance(App.PimpCSharpAssembly.GetType($"Pimp.CSharpAssembly.Modules.{className}"));
                }
                catch
                {
                    Logger.Instance.AddLog($"Module {className}을(를) 로드하는데 실패했습니다. \nAssembly Build를 시도해보세요!");
                    return;
                }
                
                var instance = new CanvasModuleModel
                {
                    Name = $"{Path.GetFileNameWithoutExtension(file.FileName.Replace("Module", ""))}_{(ModuleCount).ToString("000")}",
                    X = Math.Round(point.X),
                    Y = Math.Round(point.Y),
                    ZIndex = 0,
                    FileModel = file,

                    ModuleInterface = module as IModule,
                };
                AddInstance(instance);
            }
            else if (extension == ".cs" && file.FilePath.Contains("Results"))
            {
                object module = null;
                try
                {
                    module = Activator.CreateInstance(App.PimpCSharpAssembly.GetType($"Pimp.CSharpAssembly.Results.{className}"));
                }
                catch
                {
                    Logger.Instance.AddLog($"Module {className}을(를) 로드하는데 실패했습니다. \nAssembly Build를 시도해보세요!");
                    return;
                }
                
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

        public BitmapSource GetBitmapSource(string imagePath)
        {
            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(imagePath, UriKind.Absolute);
            bitmap.EndInit();
            return bitmap;
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
            if (_selectedInstance == null)
            {
                return;
            }

            // 선택한 인스턴스와 연결된 간선을 찾아 제거합니다.
            var connectedEdges = Edges.Where(edge => edge.Start == _selectedInstance || edge.End == _selectedInstance).ToList();
            foreach (var edge in connectedEdges)
            {
                if (edge.Start == _selectedInstance && edge.End is CanvasModuleModel endModule)
                {
                    edge.End.OutputBitmapSource = null;
                    (edge.End as CanvasModuleModel)?.Run();

                    endModule.CanConnect = true;
                }
                else if (edge.Start == _selectedInstance && edge.End is CanvasResultModel)
                {
                    edge.End.OutputBitmapSource = null;
                    (edge.End as CanvasResultModel)?.Run();

                    (edge.End as CanvasResultModel)?.DeleteResult(_selectedInstance.Name);
                    edge.Start.NameChanged -= (edge.End as CanvasResultModel).ParentNameChanged;
                }
                else
                {
                    // TODO : 동작 정의 필요
                }

                edge.Start.OutputBitmapSourceChanged -= edge.End.OnOutputBitmapSourceChanged;

                Edges.Remove(edge);
            }

            Instances.Remove(_selectedInstance);
            if (_selectedInstance is CanvasModuleModel module)
            {
                module.ModuleInterface = null;
            }
            if(_selectedInstance?.OutputBitmapSource != null)
            {
                _selectedInstance.OutputBitmapSource = null;
            }
            _selectedInstance = null;

            // UI를 강제로 업데이트합니다.
            OnPropertyChanged(nameof(Properties));

            // Properties 컬렉션을 Clear합니다.
            foreach (var propertyModel in Properties)
            {
                propertyModel.PropertyChanged -= PropertyModel_PropertyChanged;
                propertyModel.PropertyChanged -= PropertyModuleModel_PropertyChanged;
            }
            Properties.Clear();

            // PropertiesView를 갱신하여 UI를 업데이트합니다.
            PropertiesView.Refresh();
        }

        private void SelectedInstance_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // SelectedInstance가 null인지 확인합니다.
            if (SelectedInstance == null)
            {
                return;
            }

            var property = Properties.FirstOrDefault(p => p.Name == e.PropertyName);
            if(property == null)
            {
                return;
            }

            property.Value = _selectedInstance.GetType().GetProperty(e.PropertyName).GetValue(_selectedInstance);
        }

        private void UpdateProperties()
        {
            foreach (var propertyModel in Properties)
            {
                propertyModel.PropertyChanged -= PropertyModel_PropertyChanged;
                propertyModel.PropertyChanged -= PropertyModuleModel_PropertyChanged;
            }
            Properties.Clear();

            var properties = _selectedInstance.GetType().GetProperties().Where(p => !Attribute.IsDefined(p, typeof(UIHiddenAttribute)));

            foreach (var property in properties)
            {
                var propertyModel = new PropertyModel(property.Name, property.GetValue(_selectedInstance), property);
                propertyModel.PropertyChanged += PropertyModel_PropertyChanged;
                Properties.Add(propertyModel);
            }

            // _selectedInstance가 CanvasModuleModel 타입인 경우 ModuleInterface의 속성도 추가
            if (_selectedInstance is CanvasModuleModel canvasModuleModel && canvasModuleModel.ModuleInterface != null)
            {
                var moduleInterfaceProperties = canvasModuleModel.ModuleInterface.GetType().GetProperties().Where(p => !Attribute.IsDefined(p, typeof(UIHiddenAttribute)));
                foreach (var property in moduleInterfaceProperties)
                {
                    var propertyModel = new PropertyModel(property.Name, property.GetValue(canvasModuleModel.ModuleInterface), property);
                    propertyModel.PropertyChanged += PropertyModuleModel_PropertyChanged;
                    Properties.Add(propertyModel);
                }
            }
        }

        private void PropertyModuleModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "Value")
            {
                return;
            }

            if (_selectedInstance is not CanvasModuleModel module)
            {
                return;
            }

            var propertyModel = (PropertyModel)sender;
            var property = module.ModuleInterface.GetType().GetProperty(propertyModel.Name);

            var value = Convert.ChangeType(propertyModel.Value, property.PropertyType);
            property.SetValue(module.ModuleInterface, value);

            // X, Y, ZIndex 속성은 모듈에 적용되지 않도록 함 (캔버스 이동 관련 Property)
            if (property.Name == "X" || property.Name == "Y" || property.Name == "ZIndex")
            {
                return;
            }

            module.ModuleInterface.Run();
            module.OutputBitmapSource = module.ModuleInterface.OutputImage;
            module.Run();
        }

        private void PropertyModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Value")
            {
                var propertyModel = (PropertyModel)sender;
                var property = _selectedInstance.GetType().GetProperty(propertyModel.Name);
                var value = Convert.ChangeType(propertyModel.Value, property.PropertyType);
                property.SetValue(_selectedInstance, value);
            }
        }

        public void RemoveAllInstances()
        {
            // Instances 컬렉션을 모두 지우기
            var InstancesToRemove = Instances.ToList();
            foreach(var instance in Instances)
            {
                if (instance is CanvasModuleModel module)
                {
                    module.ModuleInterface = null;
                }
                if (instance?.OutputBitmapSource != null)
                {
                    instance.OutputBitmapSource = null;
                }

                InstancesToRemove.Add(instance);
            }

            foreach(var instance in InstancesToRemove)
            {
                Instances.Remove(instance);
            }

            SelectedInstance = null;
        }

        public void ClearProperties()
        {
            // UI를 강제로 업데이트합니다.
            OnPropertyChanged(nameof(Properties));

            // Properties 컬렉션을 Clear합니다.
            foreach (var propertyModel in Properties)
            {
                propertyModel.PropertyChanged -= PropertyModel_PropertyChanged;
                propertyModel.PropertyChanged -= PropertyModuleModel_PropertyChanged;
            }
            Properties.Clear();

            // PropertiesView를 갱신하여 UI를 업데이트합니다.
            PropertiesView.Refresh();
        }

        public void SaveInstances()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(ObservableCollection<CanvasInstanceBaseModel>));
            using (TextWriter writer = new StreamWriter("D:\\Pimp\\instance.xml"))
            {
                serializer.Serialize(writer, Instances);
            }
            SaveProperties();
        }

        public void SaveEdges()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(ObservableCollection<CanvasEdge>));
            using (TextWriter writer = new StreamWriter("D:\\Pimp\\Edges.xml"))
            {
                serializer.Serialize(writer, Edges);
            }
        }

        private SerializableDictionary<string, List<PropertyModel>> _propertiesForSerialization = new SerializableDictionary<string, List<PropertyModel>>();
        public void SaveProperties()
        {
            // TODO : 동작이 너무 느린 경우 SelectedInstance 조회를 줄이는 방법을 찾아야 합니다.
            var lastSelectedInstance = SelectedInstance;
            _propertiesForSerialization.Clear();
            
            foreach (var instance in Instances)
            {
                SelectedInstance = instance;
                _propertiesForSerialization.Add(SelectedInstance.Name, Properties.ToList());
            }
            SelectedInstance = lastSelectedInstance;

            XmlSerializer serializer = new XmlSerializer(typeof(SerializableDictionary<string, List<PropertyModel>>));
            using (TextWriter writer = new StreamWriter("D:\\Pimp\\Properties.xml"))
            {
                serializer.Serialize(writer, _propertiesForSerialization);
            }
        }

        private void LoadProperties()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(SerializableDictionary<string, List<PropertyModel>>));
            using (TextReader reader = new StreamReader("D:\\Pimp\\Properties.xml"))
            {
                var propertiesOfAllIntances = (SerializableDictionary<string, List<PropertyModel>>)serializer.Deserialize(reader);
                foreach (var instance in Instances.OfType<CanvasModuleModel>())
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

        List<CanvasInstanceBaseModel> _exceptionInstances = new List<CanvasInstanceBaseModel>();
        public void LoadAllInstances()
        {
            _exceptionInstances.Clear();

            RemoveAllEdges();
            RemoveAllInstances();
            ClearProperties();

            LoadInstances();
            LoadEdges();

            foreach (var instance in _exceptionInstances)
            {
                if (instance is CanvasModuleModel module)
                {
                    module.ModuleInterface = null;
                }
                if (instance?.OutputBitmapSource != null)
                {
                    instance.OutputBitmapSource = null;
                }

                Instances.Remove(instance);
            }

            if (_exceptionInstances.Count > 0)
            {
                SaveInstances();
                SaveEdges();

                LoadInstances();

                RemoveAllEdges();
                LoadEdges();

                MessageBox.Show($"다음 인스턴스들은 로드에 실패했습니다.\n{string.Join("\n", _exceptionInstances)}");
            }
            else
            {
                _exceptionInstances.Clear();
            }
        }

        private void LoadInstances()
        {
            // 역직렬화
            XmlSerializer serializer = new XmlSerializer(typeof(ObservableCollection<CanvasInstanceBaseModel>));
            using (TextReader reader = new StreamReader("D:\\Pimp\\instance.xml"))
            {
                Instances = (ObservableCollection<CanvasInstanceBaseModel>)serializer.Deserialize(reader);
            }

            foreach (var instance in Instances)
            {
                try
                {
                    if (instance is CanvasImageModel)
                    {
                        instance.OutputBitmapSource = GetBitmapSource(instance.FileModel.FilePath).Clone();
                    }
                    else if (instance is CanvasModuleModel)
                    {
                        var className = Path.GetFileNameWithoutExtension(instance.FileModel.FileName);

                        var module = Activator.CreateInstance(App.PimpCSharpAssembly.GetType($"Pimp.CSharpAssembly.Modules.{className}"));
                        (instance as CanvasModuleModel).ModuleInterface = module as IModule;
                    }
                }
                catch
                {
                    _exceptionInstances.Add(instance);
                }
            }
            LoadProperties();
            OnPropertyChanged(nameof(Instances));
        }

        private void LoadEdges()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(ObservableCollection<CanvasEdge>));
            using (TextReader reader = new StreamReader("D:\\Pimp\\edges.xml"))
            {
                var edges = (ObservableCollection<CanvasEdge>)serializer.Deserialize(reader);
                foreach (var edge in edges)
                {
                    var edgeStart = Instances.FirstOrDefault(instance => instance.Name == edge.Start.Name);
                    var edgeEnd = Instances.FirstOrDefault(instance => instance.Name == edge.End.Name);

                    if (_exceptionInstances.Contains(edge.Start) || _exceptionInstances.Contains(edge.End) || edgeStart == null || edgeEnd == null)
                    {
                        continue;
                    }

                    if (edgeEnd is CanvasModuleModel && edgeStart != edgeEnd)
                    {
                        // Module의 Input은 1개이므로 이미 연결된 경우 더 이상 연결할 수 없습니다.
                        if ((edgeEnd as CanvasModuleModel).CanConnect)
                        {
                            (edgeEnd as CanvasModuleModel).CanConnect = false;
                            edgeStart.OutputBitmapSourceChanged -= edgeEnd.OnOutputBitmapSourceChanged;
                            edgeStart.OutputBitmapSourceChanged += edgeEnd.OnOutputBitmapSourceChanged;
                            AddEdge(edgeStart, edgeEnd);

                            (edgeEnd as CanvasModuleModel)?.OnOutputBitmapSourceChanged(edgeStart.Name, edgeStart.OutputBitmapSource);
                        }
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

        public void RemoveAllEdges()
        {
            // 선택한 인스턴스와 연결된 간선을 찾아 제거합니다.
            var connectedEdges = Edges.ToList();
            foreach (var edge in connectedEdges)
            {
                if (edge.End is CanvasModuleModel endModule)
                {
                    edge.End.OutputBitmapSource = null;
                    
                    // (edge.End as CanvasModuleModel)?.Run();
                    endModule.CanConnect = true;
                    endModule.ModuleInterface = null;
                }
                else if (edge.End is CanvasResultModel resultModule)
                {
                    edge.End.OutputBitmapSource = null;
                    // (edge.End as CanvasResultModel)?.Run();

                    resultModule.DeleteResult(edge.End.Name);
                    edge.Start.NameChanged -= resultModule.ParentNameChanged;
                }
                else
                {
                    edge.End.OutputBitmapSource = null;
                    // TODO : 동작 정의 필요
                }

                edge.Start.OutputBitmapSourceChanged -= edge.End.OnOutputBitmapSourceChanged;

                Edges.Remove(edge);
            }
        }
    }
}
