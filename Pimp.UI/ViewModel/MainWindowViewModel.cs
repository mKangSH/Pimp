using Microsoft.Win32;
using Pimp.Common.Command;
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

namespace Pimp.ViewModel
{
    public class MainWindowViewModel
    {
        public ICommand SaveCommand { get; }
        public ICommand LoadCommand { get; }
        public ICommand AssemblyUnloadCommand { get; }
        public ICommand AssemblyLoadCommand { get; }

        // CanvasViewModel의 인스턴스를 추가합니다.
        public CanvasViewModel CanvasViewModel { get; }

        public MainWindowViewModel(CanvasViewModel canvasViewModel)
        {
            // 주입받은 CanvasViewModel을 사용합니다.
            CanvasViewModel = canvasViewModel;

            SaveCommand = new RelayCommand(() =>
            { 
                canvasViewModel.SaveInstances("D:\\Pimp\\Instance.xml");  
                canvasViewModel.SaveEdges("D:\\Pimp\\Edges.xml");
            });

            LoadCommand = new RelayCommand(() => 
            { 
                canvasViewModel.LoadAllInstances("D:\\Pimp\\Instance.xml", "D:\\Pimp\\Edges.xml"); 
            });

            AssemblyUnloadCommand = new RelayCommand(() =>
            {
                WeakReference pimpWeakRef;
                DllManager.UnloadPimpCSharpAssembly(out pimpWeakRef);
                for (int i = 0; pimpWeakRef.IsAlive && (i < 10); i++)
                {
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                }

                if (pimpWeakRef.IsAlive)
                {
                    Logger.Instance.AddLog("Assembly is still alive");
                }
                else
                {
                    Logger.Instance.AddLog("Assembly is dead");
                }
            });

            AssemblyLoadCommand = new RelayCommand(() =>
            {
                DllManager.LoadPimpCSharpAssembly();
            });
        }
    }
}
