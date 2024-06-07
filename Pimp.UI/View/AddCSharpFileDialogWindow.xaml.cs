using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Shapes;

namespace Pimp.View
{
    /// <summary>
    /// AddCSharpFileDialogWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class AddCSharpFileDialogWindow : Window
    {
        public bool IsProgrammaticallyClosed { get; set; } = false;

        public AddCSharpFileDialogWindow()
        {
            InitializeComponent();
            this.Closing += InstanceDetailWindow_Closing;
        }

        private void InstanceDetailWindow_Closing(object sender, CancelEventArgs e)
        {
            if (IsProgrammaticallyClosed == false)
            {
                e.Cancel = true;  // 닫기 동작 취소
                this.Hide();      // Window 숨기기
            }
        }
    }
}
