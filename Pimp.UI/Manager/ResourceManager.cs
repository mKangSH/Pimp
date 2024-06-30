using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pimp.UI.Manager
{
    public class ResourceManager
    {
        public static readonly string _resourcePath = @"D:\CodeProject\Pimp.CSharpAssembly\";

        public static string GetResourcePath(string resource)
        {
            return _resourcePath + resource;
        }
    }
}
