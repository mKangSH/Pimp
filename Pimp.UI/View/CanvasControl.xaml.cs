using Pimp.Model;
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

            MyListBox.Items.SortDescriptions.Add(new SortDescription("Visibility", ListSortDirection.Ascending));
            MyListBox.Items.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
        }

        private void MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MyListBox.Visibility = Visibility.Collapsed;
            SearchBox.Visibility = Visibility.Collapsed;
        }

        private void MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            // 마우스 클릭 위치 가져오기
            Point clickPosition = e.GetPosition(canvas);

            // ListBox와 TextBox 위치 설정
            Canvas.SetLeft(MyListBox, clickPosition.X);
            Canvas.SetTop(MyListBox, clickPosition.Y + 30);
            Canvas.SetLeft(SearchBox, clickPosition.X);
            Canvas.SetTop(SearchBox, clickPosition.Y); // ListBox 위에 TextBox 배치

            // ListBox와 TextBox 표시
            MyListBox.Visibility = Visibility.Visible;
            SearchBox.Visibility = Visibility.Visible;
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string filter = SearchBox.Text.ToLower();
            foreach (MethodInfoWrapper item in MyListBox.Items)
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

            MyListBox.Items.SortDescriptions.Clear();
            MyListBox.Items.SortDescriptions.Add(new SortDescription("Visibility", ListSortDirection.Ascending));
            MyListBox.Items.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));

            // 첫 번째 항목을 스크롤 뷰로 가져와서 스크롤을 맨 위로 올립니다.
            if (MyListBox.Items.Count > 0)
            {
                MyListBox.ScrollIntoView(MyListBox.Items[0]);
            }
        }
    }
}
