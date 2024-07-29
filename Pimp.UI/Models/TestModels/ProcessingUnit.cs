using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pimp.UI.Models.TestModels
{
    public partial class ProcessingUnit
    {
        public event EventHandler<int> ConditionMet;
        public event EventHandler OnStartup;

        public int ID1 { get; set; }
        public int ID2 { get; set; }
        public int ID3 { get; set; }


        public void test1()
        {
        }

        public void test2(int a, int b, double c)
        {
        }

        public void test3(List<int> a)
        {
        }

        public T test4<T>(T a)
        {
            T b = a;

            return b;
        }
    }
}
