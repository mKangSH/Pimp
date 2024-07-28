using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Pimp.UI.Model
{
    public struct CanvasPosition
    {
        public double X { get; set; }
        public double Y { get; set; }
    }

    public class PimpObject
    {
        public CanvasPosition CanvasPos { get; set; }

        public Vector3 Position { get; set; }

        public Vector3 Rotation { get; set; }

        public Vector3 Scale { get; set; }
    }
}
