using Pimp.Common.Command;
using Pimp.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Pimp.ViewModel
{
    public class FileViewModel : INotifyPropertyChanged
    {
        enum FolderChangeType
        {
            Created,
            Renamed,
            Deleted
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event EventHandler AddCSharpFileRequested;

        private FileSystemWatcher _resourceWatcher;
        public FileSystemWatcher ResourceWatcher
        {
            get { return _resourceWatcher; }
        }

        private FileSystemWatcher _folderWatcher;
        public FileSystemWatcher FolderWatcher
        {
            get { return _folderWatcher; }
        }

        public ObservableCollection<FolderModel> Folders { get; private set; }
        public ObservableCollection<FileModel> Files { get; set; }
        
        private FolderModel _selectedFolder;
        public FolderModel SelectedFolder
        {
            get { return _selectedFolder; }
            set
            {
                if (_selectedFolder != value)
                {
                    _selectedFolder = value;

                    // 기존 FileSystemWatcher를 해제합니다.
                    if (_folderWatcher != null)
                    {
                        _folderWatcher.Changed -= FolderWatcher_Changed;
                        _folderWatcher.Deleted -= FolderWatcher_Changed;  // 파일이 삭제되었을 때도 이벤트 핸들러를 호출합니다.
                        _folderWatcher.Dispose();
                    }

                    if(value == null)
                    {
                        _selectedFolder = null;
                        return;
                    }

                    // 새 FileSystemWatcher를 설정합니다.
                    _folderWatcher = new FileSystemWatcher(_selectedFolder.FolderPath)
                    {
                        IncludeSubdirectories = false,  // 하위 폴더의 변경을 감지하지 않습니다.
                    };

                    _folderWatcher.Changed += FolderWatcher_Changed;
                    _folderWatcher.Deleted += FolderWatcher_Changed;
                    _folderWatcher.EnableRaisingEvents = true;

                    UpdateFiles();

                    OnPropertyChanged("SelectedFolder");
                }
            }
        }

        public void FolderWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            // UI 스레드에서 UpdateFiles 메서드를 호출합니다.
            Application.Current.Dispatcher.Invoke(UpdateFiles);
        }

        private FileModel _selectedFile;
        public FileModel SelectedFile 
        { 
            get { return _selectedFile; }
            set
            {
                if(_selectedFile != value)
                {
                    _selectedFile = value;
                    OnPropertyChanged("SelectedFile");
                }
            }
        } // 선택한 파일을 추적하는 속성

        public ICommand ShowAddCSharpFileDialogCommand { get; private set; }
        public ICommand DeleteSelectedFileCommand { get; private set; }

        private static readonly HashSet<string> _allowedExtensions = new HashSet<string> { ".cs", ".jpg", ".png", ".gif", ".bmp" };

        private FolderModel _rootFolder;

        public FileViewModel(string projectPath)
        {
            ShowAddCSharpFileDialogCommand = new RelayCommand<object>(ShowAddCSharpFileDialog);
            DeleteSelectedFileCommand = new RelayCommand<object>(DeleteSelectedFile, CanDeleteSelectedFile);

            Folders = new ObservableCollection<FolderModel>();
            Files = new ObservableCollection<FileModel>();

            // Create the root folder
            _rootFolder = new FolderModel
            {
                FolderPath = $"{projectPath}Resources\\",
                FolderName = "Resources"
            };

            foreach (var folderPath in Directory.GetDirectories(_rootFolder.FolderPath))
            {
                var folder = new FolderModel
                {
                    FolderName = Path.GetFileName(folderPath),
                    FolderPath = folderPath
                };

                Folders.Add(folder);

                if (SelectedFolder == null)
                {
                    SelectedFolder = folder;
                }
            }

            _resourceWatcher = new FileSystemWatcher(_rootFolder.FolderPath)
            {
                IncludeSubdirectories = true,
                NotifyFilter = NotifyFilters.DirectoryName,
                EnableRaisingEvents = true
            }; 

            _resourceWatcher.Created += ResourceWatcher_Created;
            _resourceWatcher.Renamed += ResourceWatcher_Renamed;
            _resourceWatcher.Deleted += ResourceWatcher_Deleted;

            UpdateFiles();
        }

        private void ResourceWatcher_Created(object sender, FileSystemEventArgs e)
        {
            // UI 스레드에서 UpdateFiles 메서드를 호출합니다.
            Application.Current.Dispatcher.Invoke(() => { UpdateFolders(e.FullPath, FolderChangeType.Created); });
        }

        private void ResourceWatcher_Renamed(object sender, RenamedEventArgs e)
        {
            // UI 스레드에서 UpdateFiles 메서드를 호출합니다.
            Application.Current.Dispatcher.Invoke(() => { UpdateFolders(e.FullPath, FolderChangeType.Renamed, e.OldFullPath); });
        }

        private void ResourceWatcher_Deleted(object sender, FileSystemEventArgs e)
        {
            // UI 스레드에서 UpdateFiles 메서드를 호출합니다.
            Application.Current.Dispatcher.Invoke(() => { UpdateFolders(e.FullPath, FolderChangeType.Deleted); });
        }

        private void UpdateFolders(string path, FolderChangeType changeType, string oldPath = null)
        {
            if(SelectedFolder != null)
            {
                SelectedFolder = null;
            }

            Folders.Clear();
            foreach (var folderPath in Directory.GetDirectories(_rootFolder.FolderPath))
            {
                var folder = new FolderModel
                {
                    FolderName = Path.GetFileName(folderPath),
                    FolderPath = folderPath
                };

                Folders.Add(folder);

                if (SelectedFolder == null)
                {
                    SelectedFolder = folder;
                }
            }
        }

        public void UpdateFiles()
        {
            if (SelectedFolder == null)
            {
                return;
            }

            SelectedFile = null;
            Files.Clear();

            // 선택된 폴더 내의 모든 하위 폴더를 가져옵니다.
            var folderPaths = Directory.EnumerateDirectories(SelectedFolder.FolderPath);

            foreach (var folderPath in folderPaths)
            {
                var folderInfo = new DirectoryInfo(folderPath);

                Files.Add(new FileModel
                {
                    FileName = folderInfo.Name,
                    FilePath = folderInfo.FullName,
                    FileExtension = ""  // 폴더는 확장자가 없습니다.
                });
            }

            var filePaths = Directory.EnumerateFiles(SelectedFolder.FolderPath)
                                     .Where(filePath => _allowedExtensions.Contains(Path.GetExtension(filePath).ToLower()));

            foreach (var filePath in filePaths)
            {
                var fileInfo = new FileInfo(filePath);
                var extension = fileInfo.Extension.ToLower();

                Files.Add(new FileModel
                {
                    FileName = fileInfo.Name,
                    FilePath = fileInfo.FullName,
                    FileExtension = extension
                });
            }
        }

        private void ShowAddCSharpFileDialog(object obj)
        {
            AddCSharpFileRequested?.Invoke(this, EventArgs.Empty);
        }

        private bool CanDeleteSelectedFile(object obj)
        {
            return SelectedFile != null;
        }

        private void DeleteSelectedFile(object obj)
        {
            var result = MessageBox.Show("Are you sure you want to delete this file?", "Confirmation", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                File.Delete(SelectedFile.FilePath);
                UpdateFiles();
            }
        }
    }
}
