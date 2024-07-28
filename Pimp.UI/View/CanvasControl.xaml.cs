using Pimp.Model;
using Pimp.UI.Model;
using Pimp.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
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
    /// CanvasControl.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class CanvasControl : UserControl
    {
        public CanvasControl()
        {
            InitializeComponent();

            MethodListBox.Items.SortDescriptions.Add(new SortDescription("Visibility", ListSortDirection.Ascending));
            MethodListBox.Items.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
        }

        private void MouseLeftButtonDown_Canvas(object sender, MouseButtonEventArgs e)
        {
            MethodListStackPanel.Visibility = Visibility.Collapsed;
        }

        private void MouseRightButtonDown_Canvas(object sender, MouseButtonEventArgs e)
        {
            // 마우스 클릭 위치 가져오기
            Point clickPosition = e.GetPosition(canvas);

            // ListBox와 TextBox 위치 설정
            Canvas.SetLeft(MethodListStackPanel, clickPosition.X);
            Canvas.SetTop(MethodListStackPanel, clickPosition.Y);

            // ListBox와 TextBox 표시
            MethodListStackPanel.Visibility = Visibility.Visible;
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string filter = SearchBox.Text.ToLower();
            foreach (MethodInfoWrapper item in MethodListBox.Items)
            {
                if (item.Name.ToLower().Contains(filter))
                {
                    item.Visibility = Visibility.Visible;
                }
                else
                {
                    item.Visibility = Visibility.Collapsed;
                }
            }

            MethodListBox.Items.SortDescriptions.Clear();
            MethodListBox.Items.SortDescriptions.Add(new SortDescription("Visibility", ListSortDirection.Ascending));
            MethodListBox.Items.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));

            // 첫 번째 항목을 스크롤 뷰로 가져와서 스크롤을 맨 위로 올립니다.
            if (MethodListBox.Items.Count > 0)
            {
                MethodListBox.ScrollIntoView(MethodListBox.Items[0]);
            }
        }

        private void MouseDoubleClick_MethodListBox(object sender, MouseButtonEventArgs e)
        {
            // 마우스 클릭 위치 가져오기
            Point clickPosition = e.GetPosition(canvas);

            MethodListStackPanel.Visibility = Visibility.Collapsed;

            // Test용 코드
            (DataContext as CanvasViewModel)?.PimpObjects.Add(new PimpObject() { CanvasPos = new CanvasPosition() { X = clickPosition.X, Y = clickPosition.Y} });
        }
    }
}
