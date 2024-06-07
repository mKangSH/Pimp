using Pimp.Common.Command;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Serialization;

namespace Pimp.Model
{
    [Serializable]
    public class PropertyModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        [XmlIgnore]
        public ICommand IncreaseCommand { get; private set; }
        [XmlIgnore]
        public ICommand DecreaseCommand { get; private set; }

        private void IncreaseValue()
        {
            if (Value is int intValue)
            {
                Value = intValue + 1;
            }
            else if (Value is double doubleValue)
            {
                Value = doubleValue + 1;
            }
            // Add similar blocks for other numeric types if necessary
        }

        private void DecreaseValue()
        {
            if (Value is int intValue)
            {
                Value = intValue - 1;
            }
            else if(Value is double doubleValue)
            {
                Value = doubleValue - 1;
            }
        }

        public string Name { get; set; }

        private object _value;
        public object Value
        {
            get { return _value; }
            set
            {
                if (_value != value)
                {
                    _value = value;
                    OnPropertyChanged("Value");
                }
            }
        }

        [XmlIgnore]
        public Type type { get; set; }

        [XmlIgnore]
        public PropertyInfo PropertyInfo { get; set; }

        private Array _enumValues;
        [XmlIgnore]
        public Array EnumValues
        {
            get { return _enumValues; }
            set
            {
                if (_enumValues != value)
                {
                    _enumValues = value;
                    OnPropertyChanged(nameof(EnumValues));
                }
            }
        }

        public PropertyModel() { }

        public PropertyModel(string name, object value, PropertyInfo propertyInfo)
        {
            Name = name;
            Value = value;
            this.PropertyInfo = propertyInfo;
            this.type = PropertyInfo.PropertyType;

            IncreaseCommand = new RelayCommand(IncreaseValue);
            DecreaseCommand = new RelayCommand(DecreaseValue);

            if (type.IsEnum)
            {
                EnumValues = Enum.GetValues(type);
            }
        }
    }
}

// 상하부 4장 이상 