using Microsoft.Win32;
using Pimp.Common.Command;
using Pimp.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Pimp.ViewModel
{
    public class MainWindowViewModel
    {
        public ICommand SaveCommand { get; }
        public ICommand LoadCommand { get; }

        // CanvasViewModel의 인스턴스를 추가합니다.
        public ICanvasViewModel CanvasViewModel { get; }

        public MainWindowViewModel(ICanvasViewModel canvasViewModel)
        {
            // 주입받은 CanvasViewModel을 사용합니다.
            CanvasViewModel = canvasViewModel;

            SaveCommand = new RelayCommand(() =>
            { 
                canvasViewModel.SaveInstances("D:\\Pimp\\Instance.xml");  
                canvasViewModel.SaveEdges("D:\\Pimp\\Edges.xml");
            });

            LoadCommand = new RelayCommand(() => { canvasViewModel.LoadAllInstances("D:\\Pimp\\Instance.xml", "D:\\Pimp\\Edges.xml"); });
        }
    }
}
