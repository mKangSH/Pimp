using Pimp.Common.Log;
using Pimp.UI.Manager.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Pimp.UI.Manager
{
    public class Managers
    {
        private Managers() { }

        private static Managers s_instance;
        private static Managers Instance 
        { 
            get 
            { 
                Init();
                return s_instance; 
            } 
        }

        #region Core
        private ResourceManager _resource = new ResourceManager();

        public static ResourceManager Resource { get { return Instance._resource; } }
        #endregion

        internal static void Init()
        {
            if(s_instance == null)
            {
                s_instance = new Managers();
                s_instance._resource.Init();
            }
        }

        public static void Clear()
        {
            s_instance._resource.Clear();
        }
    }
}
