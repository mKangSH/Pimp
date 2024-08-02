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

        public void TestMethod2(int a, out int b)
        {
            b = a;
        }

        public void TestMethod3(int a, int b, int c, int d, int e)
        {
            b = a;
        }

        public void BranchMethod(bool condition)
        {
            if (condition)
            {
                Console.WriteLine("True");
            }
            else
            {
                Console.WriteLine("False");
            }
        }
    }
}
