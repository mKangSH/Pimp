using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pimp.UI.Manager.Core
{
    public class ResourceManager
    {
        Dictionary<string, DirectoryInfo> ResourceDirectories { get; set; }

        public void Init()
        {
            ResourceDirectories = LoadAllResourceDirectories();
        }

        public Dictionary<string, DirectoryInfo> LoadAllResourceDirectories()
        {
            if (Directory.Exists(GlobalConst.ResourcePath) == false)
            {
                Directory.CreateDirectory(GlobalConst.ResourcePath);
            }

            return Directory.EnumerateDirectories(GlobalConst.ResourcePath, "*", SearchOption.AllDirectories)
                            .Select(DirectoryInfo => new DirectoryInfo(DirectoryInfo))
                            .ToDictionary(DirectoryInfo => DirectoryInfo.FullName.Substring(GlobalConst.ResourcePath.Length, DirectoryInfo.FullName.Length - GlobalConst.ResourcePath.Length));
        }

        public void Clear()
        {
            ResourceDirectories.Clear();
        }
    }
}
