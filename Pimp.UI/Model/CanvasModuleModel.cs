using Pimp.Common.Attributes;
using Pimp.Common.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;

namespace Pimp.Model
{
    [Serializable]
    public class CanvasModuleModel : CanvasInstanceBaseModel
    {
        [UIHidden, XmlIgnore]
        public IModule ModuleInterface { get; set; }
        
        [UIHidden, XmlIgnore]
        public bool CanConnect { get; set; } = true;

        public CanvasModuleModel()
        {
        }

        public override void OnOutputBitmapSourceChanged(string name, BitmapSource newBitmapSource)
        {
            if (ModuleInterface == null)
            {
                return;
            }

            ModuleInterface.InputImage = (newBitmapSource == null) ? null : newBitmapSource.Clone();
            ModuleInterface.Run();

            OutputBitmapSource = (ModuleInterface.OutputImage == null) ? null : ModuleInterface.OutputImage;
            OverlayBitmapSource = (ModuleInterface.OverlayImage == null) ? null : ModuleInterface.OverlayImage;

            this.Run();
        }
    }
}
