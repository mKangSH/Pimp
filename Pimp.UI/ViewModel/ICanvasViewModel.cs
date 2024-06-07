using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pimp
{
    public interface ICanvasViewModel
    {
        void SaveInstances();
        void SaveEdges();
        void LoadAllInstances();
    }
}
