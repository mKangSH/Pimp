using Pimp.Common.Attributes;
using Pimp.Common.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Pimp.Common.Log;

namespace Pimp.Common.Models
{
    public abstract class OneInputBaseModule : IOneInputModule
    {
        [UIHidden]
        public BitmapSource InputImage { get; set; }

        [UIHidden]
        public BitmapSource OutputImage { get; set; }

        [UIHidden]
        public BitmapSource OverlayImage { get; set; }

        public OneInputBaseModule()
        {

        }

        public abstract void Run();
    }
}
