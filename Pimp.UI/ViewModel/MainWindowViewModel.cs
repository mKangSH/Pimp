using Microsoft.Win32;
using CommunityToolkit.Mvvm.Input;
using Pimp.Common.Log;
using Pimp.UI.Manager;
using Pimp.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Pimp.UI.Manager.Core;

namespace Pimp.ViewModel
{
    public class MainWindowViewModel
    {
        public ICommand SaveCommand { get; }
        public ICommand LoadCommand { get; }

        public MainWindowViewModel()
        {
            SaveCommand = new RelayCommand(() =>
            {
            });

            LoadCommand = new RelayCommand(() =>
            {
                
            });
        }
    }
}
