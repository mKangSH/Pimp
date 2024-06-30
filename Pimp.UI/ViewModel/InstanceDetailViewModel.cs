﻿using Pimp.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Pimp.ViewModel
{
    public class InstanceDetailViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private CanvasInstanceBaseModel _instance;
        public CanvasInstanceBaseModel Instance
        {
            get { return _instance; }
            set
            {
                if (_instance == value)
                {
                    return;
                }

                _instance = value;
                OnPropertyChanged(nameof(Instance));
            }
        }

        private ListCollectionView _propertiesView;
        public ListCollectionView PropertiesView
        {
            get { return _propertiesView; }
            set
            {
                if (_propertiesView == value)
                {
                    return;
                }

                _propertiesView = value;
                OnPropertyChanged(nameof(PropertiesView));
            }
        }

        public InstanceDetailViewModel()
        {
        }
    }
}
