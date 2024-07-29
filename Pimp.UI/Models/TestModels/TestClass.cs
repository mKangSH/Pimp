using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pimp.UI.Models.TestModels
{
    public class TestClass
    {
        public event EventHandler TestEvent;

        public string Name { get; set; }
        public string Description { get; set; }

        public void TestMethod1()
        {
        }
    }
}
