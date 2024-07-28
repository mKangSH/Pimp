using Pimp.Common.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Windows;
using System.ComponentModel;
using System.Windows.Data;
using Pimp.Common.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;

namespace Pimp.ViewModel
{
    public class LoggerViewModel
    {
        private ObservableCollection<LogEntry> _logs = new ObservableCollection<LogEntry>();
        private ICollectionView _logsView;

        public ICommand ClearLogCommand { get; private set; }

        public LoggerViewModel(Logger logger)
        {
            logger.LogAdded += OnLogAdded;

            _logsView = CollectionViewSource.GetDefaultView(_logs);
            _logsView.SortDescriptions.Add(new SortDescription("Timestamp", ListSortDirection.Ascending));

            ClearLogCommand = new RelayCommand(OnClearLog);
        }

        private void OnClearLog()
        {
            Application.Current.Dispatcher.Invoke(_logs.Clear);
        }

        private void OnLogAdded(string log)
        {
            // UI thread에서 실행되도록 합니다.
            Application.Current.Dispatcher.Invoke(() =>
            {
                _logs.Add(new LogEntry { Timestamp = DateTime.Now, Message = log });
            });
        }

        public ICollectionView LogsView
        {
            get { return _logsView; }
        }
    }
}
