using Pimp.Common.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;

namespace Pimp.Model
{
    [Serializable]
    [XmlInclude(typeof(CanvasImageModel))]
    [XmlInclude(typeof(CanvasModuleModel))]
    [XmlInclude(typeof(CanvasResultModel))]
    public class CanvasInstanceBaseModel : INotifyPropertyChanged
    {
        public delegate void BitmapSourceChangedHandler(string name, BitmapSource newBitmapSource);
        public event BitmapSourceChangedHandler OutputBitmapSourceChanged;

        public delegate void NameChangedHandler(string originName, ref string newName);
        public event NameChangedHandler NameChanged;

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public virtual void OnOutputBitmapSourceChanged(string name, BitmapSource newBitmapSource)
        {
            
        }

        private string _name;
        [UIHidden]
        public string Name 
        {
            get { return _name; }
            set
            {
                if(_name != value)
                {
                    NameChanged?.Invoke(_name, ref value);

                    _name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        private double _x;
        public double X 
        { 
            get { return _x; }
            set
            {
                if(_x != value)
                {
                    _x = value;
                    OnPropertyChanged(nameof(X));
                }
            }
        }

        private double _y;
        public double Y 
        { 
            get { return _y; }
            set
            {
                if(_y != value)
                {
                    _y = value;
                    OnPropertyChanged(nameof(Y));
                }
            }
        }

        private int _zIndex;
        [UIHidden]
        public int ZIndex
        {
            get { return _zIndex; }
            set
            {
                if (_zIndex != value)
                {
                    _zIndex = value;
                    OnPropertyChanged(nameof(ZIndex));
                }
            }
        }

        private bool _isOverlayVisible;
        [UIHidden]
        public bool IsOverlayVisible
        {
            get { return _isOverlayVisible; }
            set
            {
                if (_isOverlayVisible != value)
                {
                    _isOverlayVisible = value;
                    OnPropertyChanged(nameof(IsOverlayVisible));
                }
            }
        }

        private bool _isHighlighted;
        [UIHidden, XmlIgnore]
        public bool IsHighlighted
        {
            get { return _isHighlighted; }
            set
            {
                if (_isHighlighted != value)
                {
                    _isHighlighted = value;
                    OnPropertyChanged(nameof(IsHighlighted));
                }
            }
        }

        private FileModel _fileModel;
        [UIHidden]
        public FileModel FileModel
        {
            get { return _fileModel; }
            set
            {
                if (_fileModel != value)
                {
                    _fileModel = value;
                    OnPropertyChanged(nameof(FileModel));
                }
            }
        }

        [NonSerialized]
        protected BitmapSource _outputBitmapSource;
        [UIHidden, XmlIgnore]
        public BitmapSource OutputBitmapSource
        {
            get { return _outputBitmapSource; }
            set
            {
                _outputBitmapSource = value;
                OnPropertyChanged(nameof(OutputBitmapSource));
            }
        }

        [NonSerialized]
        protected BitmapSource _overlayBitmapSource;
        [UIHidden, XmlIgnore]
        public BitmapSource OverlayBitmapSource
        {
            get { return _overlayBitmapSource; }
            set
            {
                _overlayBitmapSource = value;
                OnPropertyChanged(nameof(OverlayBitmapSource));
            }
        }

        public void Run()
        {
            OutputBitmapSourceChanged?.Invoke(_name, _outputBitmapSource);
        }
    }
}
