using Pimp.Common.Log;
using Pimp.UI.Manager;
using Pimp.UI.Manager.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pimp.UI.Watcher
{
    public class DllWatcher
    {
        private static FileSystemWatcher _watcher = new FileSystemWatcher();
        private static System.Timers.Timer _debounceTimer;
        private static readonly TimeSpan _debounceTime = TimeSpan.FromMilliseconds(2000); // Adjust this as needed

        public DllWatcher(string path)
        {
            _watcher.Path = path;
            // Only watch dll files.
            _watcher.Filter = "Pimp.CSharpAssembly.dll";
            // Add event handlers.
            _watcher.Changed += OnChanged;

            // Begin watching.
            _watcher.EnableRaisingEvents = true;

            _debounceTimer = new System.Timers.Timer(_debounceTime.TotalMilliseconds);
            _debounceTimer.Elapsed += (s, args) => HandleChanged();
            _debounceTimer.AutoReset = false; // Prevent the timer from recurring
        }

        private void OnChanged(object source, FileSystemEventArgs e)
        {
            if (e.ChangeType == WatcherChangeTypes.Changed)
            {
                _debounceTimer.Stop();
                _debounceTimer.Start();
            }

            e = null;
        }

        private void HandleChanged()
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                WeakReference pimpWeakRef;
                DllManager.UnloadPimpCSharpAssembly(out pimpWeakRef);
                if(pimpWeakRef == null)
                {
                    Logger.Instance.AddLog("Assembly is not loaded");
                    return;
                }

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
                    DllManager.LoadPimpCSharpAssembly();
                }
            });
        }
    }
}