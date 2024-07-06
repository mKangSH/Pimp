using Pimp.Common.Log;
using Pimp.UI.View;
using Pimp.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Loader;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace Pimp.UI.Manager
{
    class PimpAssemblyLoadContext : AssemblyLoadContext
    {
        // Resolver of the locations of the assemblies that are dependencies of the
        // main plugin assembly.
        private AssemblyDependencyResolver _resolver;

        public PimpAssemblyLoadContext(string pluginPath) : base(isCollectible: true)
        {
            _resolver = new AssemblyDependencyResolver(pluginPath);
        }

        // The Load method override causes all the dependencies present in the plugin's binary directory to get loaded
        // into the HostAssemblyLoadContext together with the plugin assembly itself.
        // NOTE: The Interface assembly must not be present in the plugin's binary directory, otherwise we would
        // end up with the assembly being loaded twice. Once in the default context and once in the HostAssemblyLoadContext.
        // The types present on the host and plugin side would then not match even though they would have the same names.
        protected override Assembly Load(AssemblyName name)
        {
            string assemblyPath = _resolver.ResolveAssemblyToPath(name);
            if (assemblyPath != null)
            {
                Console.WriteLine($"Loading assembly {assemblyPath} into the HostAssemblyLoadContext");
                return LoadFromAssemblyPath(assemblyPath);
            }

            return null;
        }
    }

    public static class DllManager
    {
        private static FileSystemWatcher _watcher = new FileSystemWatcher();
        private static System.Timers.Timer _debounceTimer;
        private static readonly TimeSpan DebounceTime = TimeSpan.FromMilliseconds(2000); // Adjust this as needed

        private static PimpAssemblyLoadContext _pimpCSharpAssemblyContext;
        private static Assembly _pimpCSharpAssembly;

        public static Assembly PimpCSharpAssembly
        {
            get => _pimpCSharpAssembly;
        }

        private static CanvasViewModel_2 _canvasViewModel;
        public static CanvasViewModel_2 CanvasViewModel
        {
            get => _canvasViewModel;
            set => _canvasViewModel = value;
        }

        // It is important to mark this method as NoInlining, otherwise the JIT could decide
        // to inline it into the Main method. That could then prevent successful unloading
        // of the plugin because some of the MethodInfo / Type / Plugin.Interface / HostAssemblyLoadContext
        // instances may get lifetime extended beyond the point when the plugin is expected to be
        // unloaded.
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void LoadPimpCSharpAssembly()
        {
            string dllFile = Path.Combine(GlobalConst.dllPath, "Pimp.CSharpAssembly.dll");
            if(File.Exists(dllFile))
            {
                string assemblyModuleFile = Path.Combine(GlobalConst.dllPath, "Pimp.CSharpAssembly_copy.dll");
                File.Copy(dllFile, assemblyModuleFile, true);

                _pimpCSharpAssemblyContext = new PimpAssemblyLoadContext(assemblyModuleFile);

                // Load the plugin assembly into the HostAssemblyLoadContext.
                // NOTE: the assemblyPath must be an absolute path.
                _pimpCSharpAssembly = _pimpCSharpAssemblyContext.LoadFromAssemblyPath(assemblyModuleFile);
            }
            else
            {
                Logger.Instance.AddLog(dllFile + " is not exist.");
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void UnloadPimpCSharpAssembly(out WeakReference weakRef)
        {
            // Create a weak reference to the AssemblyLoadContext that will allow us to detect
            // when the unload completes.
            if (_pimpCSharpAssemblyContext == null)
            {
                weakRef = null;
                return;
            }

            weakRef = new WeakReference(_pimpCSharpAssemblyContext);

            _pimpCSharpAssemblyContext.Unload();

            _pimpCSharpAssembly = null;
            _pimpCSharpAssemblyContext = null;
        }

        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        public static void InitFileSystemWatcher(string path)
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
        private static string _lastChangedFile = string.Empty;
        private static void OnChanged(object source, FileSystemEventArgs e)
        {
            if (e.ChangeType == WatcherChangeTypes.Changed)
            {
                _debounceTimer.Stop();
                _lastChangedFile = e.FullPath;
                _debounceTimer.Start();
            }

            e = null;
        }

        private static void HandleChanged()
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                _canvasViewModel.SaveInstances("D:\\Pimp\\Instance_temp.xml");
                _canvasViewModel.SaveEdges("D:\\Pimp\\Edges_temp.xml");

                _canvasViewModel.RemoveAllInstances();

                WeakReference pimpWeakRef;
                UnloadPimpCSharpAssembly(out pimpWeakRef);
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

                LoadPimpCSharpAssembly();

                _canvasViewModel.LoadInstances("D:\\Pimp\\Instance_temp.xml");
                _canvasViewModel.LoadEdges("D:\\Pimp\\Edges_temp.xml");

                File.Delete("D:\\Pimp\\Instance_temp.xml");
                File.Delete("D:\\Pimp\\Edges_temp.xml");
                File.Delete("D:\\Pimp\\Properties_temp.xml");
            });
        }
    }
}
