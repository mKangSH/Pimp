using Pimp.Common.Log;
using Pimp.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Pimp.View
{
    /// <summary>
    /// LoggerControl.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class LoggerControl : UserControl
    {
        public LoggerControl()
        {
            InitializeComponent();

            DataContext = new LoggerViewModel(Logger.Instance);
        }

        private void ListView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double width = listView.ActualWidth - ((GridView)listView.View).Columns[0].ActualWidth - SystemParameters.VerticalScrollBarWidth;
            if (width >= 0)
            {
                ((GridView)listView.View).Columns[1].Width = width;
            }
        }
    }
}
