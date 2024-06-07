using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pimp
{
    public interface ICanvasViewModel
    {
        void SaveInstances(string path);
        void SaveEdges(string path);
        void LoadAllInstances(string instancePath, string edgePath);
    }
}
