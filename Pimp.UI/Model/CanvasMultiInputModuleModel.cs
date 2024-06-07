using Pimp.Common.Attributes;
using Pimp.Common.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;

namespace Pimp.Model
{
    [Serializable]
    public class CanvasMultiInputModuleModel : CanvasInstanceBaseModel
    {
        [UIHidden, XmlIgnore]
        public IMultiInputModule ModuleInterface { get; set; }

        [NonSerialized]
        object _lock = new object();

        [UIHidden, XmlIgnore]
        public Dictionary<string, BitmapSource> ImageList { get; set; } = new Dictionary<string, BitmapSource>();

        public CanvasMultiInputModuleModel()
        {

        }

        public override void OnOutputBitmapSourceChanged(string name, BitmapSource newBitmapSource)
        {
            lock (_lock)
            {
                ImageList[name] = newBitmapSource.Clone();
            }
        }

        public void DeleteResult(string name)
        {
            lock (_lock)
            {
                if (ImageList.ContainsKey(name))
                {
                    ImageList.Remove(name);
                }
            }
        }
    }
}
