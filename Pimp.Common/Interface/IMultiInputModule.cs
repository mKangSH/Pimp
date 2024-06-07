using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Pimp.Common.Interface
{
    public interface IMultiInputModule : IBaseModule
    {
        List<BitmapSource> InputImages { get; set; }

        void Run(params object[] parameters);
    }
}
