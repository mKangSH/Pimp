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

        //public ICommand AssemblyUnloadCommand { get; }
        //public ICommand AssemblyLoadCommand { get; }

        public MainWindowViewModel()
        {
            SaveCommand = new RelayCommand(() =>
            {
                //canvasViewModel.SaveInstances("D:\\Pimp\\Instance.xml");
                //canvasViewModel.SaveEdges("D:\\Pimp\\Edges.xml");
            });

            LoadCommand = new RelayCommand(() =>
            {
                //canvasViewModel.LoadInstances("D:\\Pimp\\Instance.xml");
                //canvasViewModel.LoadEdges("D:\\Pimp\\Edges.xml");
            });

            //AssemblyUnloadCommand = new RelayCommand(() =>
            //{
            //    WeakReference pimpWeakRef;
            //    DllManager.UnloadPimpCSharpAssembly(out pimpWeakRef);
            //    for (int i = 0; pimpWeakRef.IsAlive && (i < 10); i++)
            //    {
            //        GC.Collect();
            //        GC.WaitForPendingFinalizers();
            //    }

            //    if (pimpWeakRef.IsAlive)
            //    {
            //        Logger.Instance.AddLog("Assembly is still alive");
            //    }
            //    else
            //    {
            //        Logger.Instance.AddLog("Assembly is dead");
            //    }
            //});

            //AssemblyLoadCommand = new RelayCommand(() =>
            //{
            //    DllManager.LoadPimpCSharpAssembly();
            //});
        }
    }
}
