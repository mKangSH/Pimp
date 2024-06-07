using Pimp.Common.Log;
using Pimp.Model;
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
        public static readonly string _resourcePath = @"D:\CodeProject\Pimp.CSharpAssembly\";
        public static readonly string _dllPath = @"D:\CodeProject\Pimp.CSharpAssembly\dll\";

        MainWindow _mainWindow;

        MainWindowViewModel _mainWindowViewModel;
        CanvasViewModel _canvasViewModel;
        FileViewModel _fileViewModel;
        MainTitleBarViewModel _mainTitleBarViewModel;

        InstanceDetailWindow _detailWindow;
        LoggerViewModel _loggerViewModel;
        LoadingWindow _loadingWindow;

        FileSystemWatcher _watcher;

        AddCSharpFileDialogWindow _dialog;

        private static AssemblyLoadContext _pimpCSharpAssemblyContext;
        public static Assembly _pimpCSharpAssembly;
        public static Assembly PimpCSharpAssembly
        {
            get { return _pimpCSharpAssembly; }
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // 어플리케이션 도메인에서 어셈블리 로드
            _mainWindow = new MainWindow();
            Application.Current.MainWindow = _mainWindow;

            _dialog = new AddCSharpFileDialogWindow();
            _watcher = new FileSystemWatcher();
            _detailWindow = new InstanceDetailWindow();
            _loadingWindow = new LoadingWindow();
            _fileViewModel = new FileViewModel(_resourcePath);
            _canvasViewModel = new CanvasViewModel();
            _mainTitleBarViewModel = new MainTitleBarViewModel();
            
            // dll load
            _pimpCSharpAssemblyContext = new AssemblyLoadContext($"Pimp.CSharpAssembly", isCollectible: true);
            string dllFile = $"{_dllPath}Pimp.CSharpAssembly.dll";
            if (File.Exists(dllFile))
            {
                string copyPath = $"{_dllPath}{Path.GetFileNameWithoutExtension(dllFile)}_copy.dll";
                File.Copy(dllFile, copyPath, true);
                _pimpCSharpAssembly = _pimpCSharpAssemblyContext.LoadFromAssemblyPath(copyPath);
            }

            // 파일 시스템 감시자 초기화
            InitFileSystemWatcher($"{_dllPath}");

            // Show부터 하지 않으면 ScrollViewer의 ScrollToVerticalOffset, ScrollToHorizontalOffset가 동작하지 않음.
            _mainWindow.Show();
            ConstructView();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            _fileViewModel.FolderWatcher.Changed -= _fileViewModel.FolderWatcher_Changed;
            _fileViewModel.FolderWatcher.Deleted -= _fileViewModel.FolderWatcher_Changed;
            _fileViewModel.FolderWatcher?.Dispose();

            _watcher.Changed -= OnChanged;
            _watcher?.Dispose();

            _debounceTimer?.Dispose();

            _mainWindow.Close();
        }

        private void SelectedInstanceChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedInstance")
            {
                (_detailWindow.DataContext as InstanceDetailViewModel).Instance = (_canvasViewModel).SelectedInstance;
            }
        }

        private System.Timers.Timer _debounceTimer;
        private static readonly TimeSpan DebounceTime = TimeSpan.FromMilliseconds(2000); // Adjust this as needed

        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        public void InitFileSystemWatcher(string path)
        {
            _watcher.Path = path;
            // Only watch dll files.
            _watcher.Filter = "Pimp.CSharpAssembly.dll";
            // Add event handlers.
            _watcher.Changed += OnChanged;

            // Begin watching.
            _watcher.EnableRaisingEvents = true;

            _debounceTimer = new System.Timers.Timer(DebounceTime.TotalMilliseconds);
            _debounceTimer.Elapsed += (s, args) => HandleChanged();
            _debounceTimer.AutoReset = false; // Prevent the timer from recurring
        }

        private static UInt64 _dllCount = 0;
        private string _lastChangedFile = string.Empty;
        public void OnChanged(object source, FileSystemEventArgs e)
        {
            if (e.ChangeType == WatcherChangeTypes.Changed)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    _loadingWindow.UpdateProgressBar(0, "Assembly Loading...");
                    _loadingWindow.Show();
                    // Disable the UI
                    _mainWindow.IsEnabled = false;
                    Mouse.OverrideCursor = Cursors.Wait;
                });

                _debounceTimer.Stop();
                _lastChangedFile = e.FullPath;
                _debounceTimer.Start();
            }

            e = null;
        }

        private void HandleChanged()
        {
            Application.Current.Dispatcher.Invoke(async () =>
            {
                await Task.Delay(100);

                _canvasViewModel.SaveInstances();
                _canvasViewModel.SaveEdges();

                await Task.Delay(10);

                _canvasViewModel.RemoveAllEdges();
                _canvasViewModel.RemoveAllInstances();
                _canvasViewModel.ClearProperties();

                await Task.Delay(10);

                _canvasViewModel.SelectedInstance = null;
                _pimpCSharpAssembly = null;
                
                _canvasViewModel.PropertyChanged -= SelectedInstanceChanged;
                (_detailWindow.DataContext as InstanceDetailViewModel).Instance = null;

                _loadingWindow.UpdateProgressBar(30, "Assembly Loading...");
                await Task.Delay(100);

                _pimpCSharpAssemblyContext?.Unload();
                _pimpCSharpAssemblyContext = new AssemblyLoadContext($"Pimp.CSharpAssembly{_dllCount++}", isCollectible: true);
                // Load the changed assembly.
                string copyPath = $"{_dllPath}{Path.GetFileNameWithoutExtension(_lastChangedFile)}_copy{_dllCount++}.dll";
                File.Copy(_lastChangedFile, copyPath, true);

                await Task.Delay(10);
                try
                {
                    _pimpCSharpAssembly = _pimpCSharpAssemblyContext.LoadFromAssemblyPath(copyPath);
                }
                catch (FileNotFoundException ex)
                {
                    MessageBox.Show($"어셈블리 파일을 찾을 수 없습니다: {ex.FileName}");
                    Application.Current.Shutdown();
                }
                catch (BadImageFormatException ex)
                {
                    MessageBox.Show($"어셈블리 파일이 손상되었거나 잘못된 형식입니다: {ex.FileName}");
                    Application.Current.Shutdown();
                }
                catch (FileLoadException ex)
                {
                    MessageBox.Show($"어셈블리를 로드하는 데 실패했습니다: {ex.FileName}");
                    Application.Current.Shutdown();
                }

                _loadingWindow.UpdateProgressBar(60, "Assembly Loading...");
                await Task.Delay(100);

                GC.Collect();
                GC.WaitForPendingFinalizers();

                (_detailWindow.DataContext as InstanceDetailViewModel).Instance = _canvasViewModel.SelectedInstance;
                _canvasViewModel.PropertyChanged -= SelectedInstanceChanged;
                _canvasViewModel.PropertyChanged += SelectedInstanceChanged;
                _canvasViewModel.LoadAllInstances();

                // Specify what is done when a file is changed, created, or deleted.
                var list = Directory.GetFiles(_dllPath).Where(f => f.Contains("_copy")).ToList();
                foreach (var item in list)
                {
                    try
                    {
                        File.Delete(item);
                    }
                    catch
                    {

                    }
                }

                _loadingWindow.UpdateProgressBar(80, "Assembly Loading...");
                await Task.Delay(100);

                // Enable the UI
                Mouse.OverrideCursor = null;
                _mainWindow.IsEnabled = true;

                _loadingWindow.UpdateProgressBar(100, "Assembly Loading...");
                await Task.Delay(100);

                _loadingWindow.Hide();
            });
        }

        private void ConstructView()
        {
            _mainWindowViewModel = new MainWindowViewModel(_canvasViewModel);
            _mainWindow.DataContext = _mainWindowViewModel;

            _mainWindow.CanvasControl.DataContext = _canvasViewModel;
            _mainWindow.CanvasInstanceControl.DataContext = _canvasViewModel;
            _mainWindow.FileListControl.DataContext = _fileViewModel;

            _mainWindow.ScrollViewer.ScrollToVerticalOffset(_mainWindow.ScrollViewer.ScrollableHeight / 2);
            _mainWindow.ScrollViewer.ScrollToHorizontalOffset(_mainWindow.ScrollViewer.ScrollableWidth / 2);

            _mainWindow.CanvasControl.ScrollViewer = _mainWindow.ScrollViewer;

            _detailWindow.DataContext = new InstanceDetailViewModel(_canvasViewModel.SelectedInstance, _canvasViewModel.PropertiesView);
            _canvasViewModel.PropertyChanged -= SelectedInstanceChanged;
            _canvasViewModel.PropertyChanged += SelectedInstanceChanged;
            _mainWindow.CanvasControl.DetailViewWindow = _detailWindow;
            
            _loggerViewModel = new LoggerViewModel(Logger.Instance);

            _mainWindow.LoggerControl.DataContext = _loggerViewModel;
            _mainWindow.MainTitleBar.DataContext = _mainTitleBarViewModel;

            var addCSharpFileDialogViewModel = new AddCSharpFileDialogViewModel(_fileViewModel);
            _dialog.DataContext = addCSharpFileDialogViewModel;
            _fileViewModel.AddCSharpFileRequested += (s, k) =>
            {
                _dialog.ShowDialog();
            };
            addCSharpFileDialogViewModel.HideDialogRequested += (s, k) =>
            {
                _dialog.Hide();
            };

            _mainWindow.Closed += MainWindow_Closed;
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            _detailWindow.IsProgrammaticallyClosed = true;
            _detailWindow.Close();

            _mainWindow.CanvasControl.DetailViewWindow = null;
            _detailWindow = null;

            _dialog.IsProgrammaticallyClosed = true;
            _dialog.Close();
            _dialog = null;

            Application.Current.Shutdown();
        }

        public static string GetRelativePath(string fromPath, string toPath)
        {
            var fromUri = new Uri(fromPath);
            var toUri = new Uri(toPath);

            var relativeUri = fromUri.MakeRelativeUri(toUri);
            var relativePath = Uri.UnescapeDataString(relativeUri.ToString());

            return relativePath.Replace('/', Path.DirectorySeparatorChar);
        }
    }
}
