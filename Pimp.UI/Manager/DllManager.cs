using Pimp.Common.Log;
using Pimp.UI.View;
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
        private static PimpAssemblyLoadContext _pimpCSharpAssemblyContext;
        private static Assembly _pimpCSharpAssembly;

        public static Assembly PimpCSharpAssembly
        {
            get => _pimpCSharpAssembly;
        }

        // It is important to mark this method as NoInlining, otherwise the JIT could decide
        // to inline it into the Main method. That could then prevent successful unloading
        // of the plugin because some of the MethodInfo / Type / Plugin.Interface / HostAssemblyLoadContext
        // instances may get lifetime extended beyond the point when the plugin is expected to be
        // unloaded.
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void LoadPimpCSharpAssembly()
        {
#if DEBUG
            string configName = "Debug";
#else
            string configName = "Release";
#endif
            string dllFile = $"D:\\CodeProject\\Pimp.CSharpAssembly\\bin\\{configName}\\net8.0-windows7.0\\Pimp.CSharpAssembly.dll";

            if(File.Exists(dllFile))
            {
                _pimpCSharpAssemblyContext = new PimpAssemblyLoadContext(dllFile);

                // Load the plugin assembly into the HostAssemblyLoadContext.
                // NOTE: the assemblyPath must be an absolute path.
                _pimpCSharpAssembly = _pimpCSharpAssemblyContext.LoadFromAssemblyPath(dllFile);
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
    }
}
