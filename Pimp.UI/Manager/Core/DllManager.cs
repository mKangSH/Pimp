using Pimp.Common.Log;
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
using System.Threading;
using System.Threading.Tasks;

namespace Pimp.UI.Manager.Core
{
    public class PimpAssemblyLoadContext : AssemblyLoadContext
    {
        public PimpAssemblyLoadContext() : base(isCollectible: true)
        {
        }

        protected override Assembly Load(AssemblyName name)
        {
            return null;
        }
    }

    public static class DllManager
    {
        private static PimpAssemblyLoadContext _pimpCSharpAssemblyContext;
        private static Assembly _pimpCSharpAssembly;

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void LoadPimpCSharpAssembly()
        {
            if(Thread.CurrentThread.Name != "UI Thread")
            {
                Logger.Instance.AddLog("LoadPimpCSharpAssembly must be called from UI thread.");
                return;
            }

            string dllFile = Path.Combine(GlobalConst.DllPath, "Pimp.CSharpAssembly.dll");
            if (File.Exists(dllFile))
            {
                string assemblyModuleFile = Path.Combine(GlobalConst.DllPath, "Pimp.CSharpAssembly_copy.dll");
                File.Copy(dllFile, assemblyModuleFile, true);

                _pimpCSharpAssemblyContext = new PimpAssemblyLoadContext();
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
            if (Thread.CurrentThread.Name != "UI Thread")
            {
                Logger.Instance.AddLog("LoadPimpCSharpAssembly must be called from UI thread.");
                weakRef = null;
                return;
            }

            // Create a weak reference to the AssemblyLoadContext that will allow us to detect
            // when the unload completes.
            weakRef = new WeakReference(_pimpCSharpAssemblyContext);

            _pimpCSharpAssemblyContext?.Unload();

            _pimpCSharpAssembly = null;
            _pimpCSharpAssemblyContext = null;

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }
    }
}
