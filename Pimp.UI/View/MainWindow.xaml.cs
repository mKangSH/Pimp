using Pimp.Model;
using Pimp.View;
using Pimp.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Pimp
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.PreviewKeyDown += MainWindow_PreviewKeyDown;
        }

        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            e.Handled = true;
        }

        private void CanvasControl_Drop(object sender, DragEventArgs e)
        {
            var file = e.Data.GetData(typeof(FileModel)) as FileModel;
            if (file != null)
            {
                // ViewModel에 파일을 추가하는 메서드를 호출합니다.
                // 이 메서드는 ViewModel에서 구현해야 합니다.
                Point dropPosition = e.GetPosition(sender as IInputElement);

                (this.CanvasControl.DataContext as CanvasViewModel)?.AddInstanceToCanvas(file, dropPosition);
            }
        }

        private void MainWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                var originalSourceType = (e.OriginalSource as Control)?.GetType();

                if (originalSourceType == null)
                {
                    return;
                }

                // Now you can check the type of the original source
                if (originalSourceType == typeof(TextBox))
                {
                    // The event was raised by a TextBox
                }
                else if (originalSourceType == typeof(UserControl))
                {
                    // The event was raised by a UserControl
                }
                else if (originalSourceType == typeof(ListBoxItem) || originalSourceType == typeof(ScrollViewer))
                {
                    (this.CanvasControl.DataContext as CanvasViewModel)?.RemoveSelectedInstance();
                    this.CanvasControl.DetailViewWindow.Hide();
                }
                // Add more checks as needed
            }
            else if (e.Key == Key.C && Keyboard.Modifiers == ModifierKeys.Control)
            {
                (this.CanvasControl.DataContext as CanvasViewModel)?.CopySelectedInstance();
            }
            else if (e.Key == Key.V && Keyboard.Modifiers == ModifierKeys.Control)
            {
                (this.CanvasControl.DataContext as CanvasViewModel)?.PasteCopiedInstance();
            }
        }
    }
}
