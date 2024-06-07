using Pimp.Helper;
using Pimp.Model;
using Pimp.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace Pimp.View
{
    /// <summary>
    /// FileListControl.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class FileListControl : UserControl
    {
        public FileListControl()
        {
            InitializeComponent();
        }

        private void ListView_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var listView = sender as ListView;
                var selectedItem = listView.SelectedItem;
                if (selectedItem != null)
                {
                    DragDrop.DoDragDrop(listView, selectedItem, DragDropEffects.Copy);
                }
            }
        }

        private void ListView_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            var listView = sender as ListView;
            var selectedItem = listView.SelectedItem;
            if (selectedItem is FileModel file)
            {
                if(file.FileExtension == "" && Directory.Exists(file.FilePath))
                {
                    var folder = new FolderModel
                    {
                        FolderName = Path.GetFileName(file.FilePath),
                        FolderPath = file.FilePath
                    };
                    // ViewModel에 파일을 추가하는 메서드를 호출합니다.
                    // 이 메서드는 ViewModel에서 구현해야 합니다.
                    (this.DataContext as FileViewModel).SelectedFolder = folder;
                }   
            }
        }
    }
}
