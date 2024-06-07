using Pimp.Common.Command;
using Pimp.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Pimp.ViewModel
{
    public class AddCSharpFileDialogViewModel : INotifyPropertyChanged
    {
        // INotifyPropertyChanged 구현
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event EventHandler HideDialogRequested;

        private FileViewModel _fileViewModel;

        public ObservableCollection<CSharpTemplate> Templates { get; private set; }

        private CSharpTemplate _selectedTemplate;

        public CSharpTemplate SelectedTemplate
        {
            get { return _selectedTemplate; }
            set
            {
                if (_selectedTemplate != value)
                {
                    _selectedTemplate = value;
                    OnPropertyChanged("SelectedTemplate");
                }
            }
        }

        public ICommand AddCSharpFileCommand { get; private set; }

        public string FileName { get; set; }

        public AddCSharpFileDialogViewModel(FileViewModel fileViewModel)
        {
            _fileViewModel = fileViewModel;

            AddCSharpFileCommand = new RelayCommand<object>(AddCSharpFile);

            Templates = new ObservableCollection<CSharpTemplate>
            {
                new CSharpTemplate{ Name = "OneInputModule", Description = "단일 입력 모듈"},
                new CSharpTemplate{ Name = "MultiInputModule", Description = "다중 입력 모듈"},
            };
        }

        private void AddCSharpFile(object obj)
        {
            string template;

            switch (SelectedTemplate.Name)
            {
                case "OneInputModule":
                    template = @"using OpenCvSharp.WpfExtensions;
using OpenCvSharp;
using Pimp.CSharpAssembly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Pimp.Common.Models;
using Pimp.Common.Log;

namespace Pimp.CSharpAssembly.Modules
{
    class " + FileName + @" : OneInputBaseModule
    {
        public " + FileName + @"()
        {
            
        }

        public override void Run()
        {
            if (InputImage == null)
            {
                OutputImage = null;
                OverlayImage = null;
                return;
            }

            try
            {
                Mat inspectionMat = InputImage.ToMat();
                Mat result = new Mat();
                // 여기에 코드를 작성하세요

                OutputImage = result.ToBitmapSource();
            }
            catch (Exception ex)
            {
                var splitTrace = ex.StackTrace.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
                Logger.Instance.AddLog($""{splitTrace[splitTrace.Length - 1]}{Environment.NewLine}{ex.Message}"");

                OutputImage = InputImage;
            }
        }
    }
}";
                    break;
                case "MultiInputModule":
                    template = @"using OpenCvSharp.WpfExtensions;
using OpenCvSharp;
using Pimp.CSharpAssembly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Pimp.Common.Models;
using Pimp.Common.Log;

namespace Pimp.CSharpAssembly.Modules
{
    class " + FileName + @" : MultiInputBaseModule
    {
        public " + FileName + @"()
        {
            
        }

        public override void Run(params object[] parameters)
        {
            if (InputImage == null)
            {
                OutputImage = null;
                OverlayImage = null;
                return;
            }

            try
            {
                Mat inspectionMat = InputImage.ToMat();
                Mat result = new Mat();
                // 여기에 코드를 작성하세요

                OutputImage = result.ToBitmapSource();
            }
            catch (Exception ex)
            {
                var splitTrace = ex.StackTrace.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
                Logger.Instance.AddLog($""{splitTrace[splitTrace.Length - 1]}{Environment.NewLine}{ex.Message}"");

                OutputImage = InputImage;
            }
        }
    }
}";
                    break;
                default:
                    throw new Exception("Unknown template");
            }

            // C# 파일을 추가하는 코드
            string path = System.IO.Path.Combine(_fileViewModel.SelectedFolder.FolderPath, FileName + ".cs");
            System.IO.File.WriteAllText(path, template, Encoding.UTF8);

            HideDialogRequested?.Invoke(this, EventArgs.Empty);
        }
    }
}
