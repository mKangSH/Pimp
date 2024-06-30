using Pimp.Common.Log;
using Pimp.Model;
using Pimp.UI.Manager;
using Pimp.UI.View;
using Pimp.View;
using Pimp.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Security.Permissions;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using System.Xml.Linq;

namespace Pimp
{
    /// <summary>
    /// App.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class App : Application
    {
        MainWindow _mainWindow = new MainWindow();
        InstanceDetailWindow _detailWindow = new InstanceDetailWindow();

        CanvasViewModel_2 _canvasViewModel;
        
        AddCSharpFileDialogWindow _dialog = new AddCSharpFileDialogWindow();

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // 어플리케이션 도메인에서 어셈블리 로드
            Application.Current.MainWindow = _mainWindow;

            _canvasViewModel = new CanvasViewModel_2();

            // Show부터 하지 않으면 ScrollViewer의 ScrollToVerticalOffset, ScrollToHorizontalOffset가 동작하지 않음.
            _mainWindow.Show();
            ConstructView();

            ConsturctDetailWindow();
        }

        private void ConstructView()
        {
            _mainWindow.DataContext = new MainWindowViewModel(_canvasViewModel);

            _mainWindow.CanvasControl.DataContext = _canvasViewModel;
            _mainWindow.CanvasInstanceControl.DataContext = _canvasViewModel;

            // TODO : Resource Manager 클래스를 이용해야 함.
            _mainWindow.FileListControl.DataContext = new FileViewModel(@"D:\CodeProject\Pimp.CSharpAssembly\");

            _mainWindow.ScrollViewer.ScrollToVerticalOffset(_mainWindow.ScrollViewer.ScrollableHeight / 2);
            _mainWindow.ScrollViewer.ScrollToHorizontalOffset(_mainWindow.ScrollViewer.ScrollableWidth / 2);

            _mainWindow.CanvasControl.ScrollViewer = _mainWindow.ScrollViewer;
            _mainWindow.LoggerControl.DataContext = new LoggerViewModel(Logger.Instance);

            var addCSharpFileDialogViewModel = new AddCSharpFileDialogViewModel((_mainWindow.FileListControl.DataContext as FileViewModel));
            _dialog.DataContext = addCSharpFileDialogViewModel;
            (_mainWindow.FileListControl.DataContext as FileViewModel).AddCSharpFileRequested += (s, k) =>
            {
                _dialog.ShowDialog();
            };
            addCSharpFileDialogViewModel.HideDialogRequested += (s, k) =>
            {
                _dialog.Hide();
            };

            _mainWindow.Closed += MainWindow_Closed;
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            (_mainWindow.FileListControl.DataContext as FileViewModel).FolderWatcher.Changed -= (_mainWindow.FileListControl.DataContext as FileViewModel).FolderWatcher_Changed;
            (_mainWindow.FileListControl.DataContext as FileViewModel).FolderWatcher.Deleted -= (_mainWindow.FileListControl.DataContext as FileViewModel).FolderWatcher_Changed;
            (_mainWindow.FileListControl.DataContext as FileViewModel).FolderWatcher?.Dispose();
            
            _mainWindow.Close();
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            _dialog.IsProgrammaticallyClosed = true;
            _dialog.Close();
            _dialog = null;

            CloseDetailWindow();

            Application.Current.Shutdown();
        }

        private void ConsturctDetailWindow()
        {
            _detailWindow.DataContext = new InstanceDetailViewModel();

            _canvasViewModel.PropertyChanged -= SelectedInstanceChanged;
            _canvasViewModel.PropertyChanged += SelectedInstanceChanged;

            _detailWindow.Show();
        }

        private void CloseDetailWindow()
        {
            _detailWindow.IsProgrammaticallyClosed = true;
            _detailWindow.Close();

            _detailWindow = null;
        }

        private void SelectedInstanceChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedInstance")
            {
                (_detailWindow.DataContext as InstanceDetailViewModel).Instance = (_canvasViewModel).SelectedInstance;
            }
        }
    }
}
