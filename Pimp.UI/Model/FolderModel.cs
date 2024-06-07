using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pimp.Model
{
    public class FolderModel
    {
        public string FolderName { get; set; }
        public string FolderPath { get; set; }

        public IEnumerable<FolderModel> SubFolders
        {
            get
            {
                var subFolderPaths = Directory.GetDirectories(FolderPath);
                foreach (var subFolderPath in subFolderPaths)
                {
                    yield return new FolderModel
                    {
                        FolderName = Path.GetFileName(subFolderPath),
                        FolderPath = subFolderPath
                    };
                }
            }
        }
    }
}
