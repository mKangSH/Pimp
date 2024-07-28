using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Pimp.UI
{
    public static class GlobalConst
    {
        public static string ResourcePath { get; } = $"D:\\CodeProject\\Pimp.CSharpAssembly\\Resources\\";

        public static string DllPath { get; } = $"D:\\CodeProject\\Pimp.CSharpAssembly\\dll\\";

        public readonly static SolidColorBrush BlackBrush = new SolidColorBrush(Color.FromArgb(200, 0, 0, 0));

        public readonly static SolidColorBrush AliceBlueBrush = new SolidColorBrush(Color.FromArgb(70, 240, 248, 255));
    }
}
