using Pimp.Model;
using Pimp.ViewModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Pimp.View
{
    /// <summary>
    /// CanvasInstanceControl.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class CanvasInstanceControl : UserControl
    {
        private static Thickness BorderHide = new Thickness(0);
        private static Thickness BorderShow = new Thickness(1);

        public CanvasInstanceControl()
        {
            InitializeComponent();
        }

        private void ListBoxItem_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F2 && sender is ListBoxItem listBoxItem)
            {
                // Find the TextBox in the ListBoxItem.
                var textBox = FindVisualChild<TextBox>(listBoxItem);

                if (textBox != null)
                {
                    textBox.IsReadOnly = false;
                    textBox.IsHitTestVisible = true;
                    textBox.CaretBrush = SystemColors.WindowTextBrush; // Show the cursor
                    textBox.BorderThickness = BorderShow; // Show the border
                    textBox.Background = Brushes.White; // Show the border

                    textBox.Focus();
                    textBox.SelectAll();

                    // Add the event handlers.
                    textBox.KeyDown += TextBox_KeyDown;
                    textBox.LostFocus += TextBox_LostFocus;
                }
            }
        }

        private T FindVisualChild<T>(DependencyObject obj) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                var child = VisualTreeHelper.GetChild(obj, i);

                if (child is T t)
                {
                    return t;
                }

                var childOfChild = FindVisualChild<T>(child);

                if (childOfChild != null)
                {
                    return childOfChild;
                }
            }

            return null;
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && sender is TextBox textBox)
            {
                ResetTextBox(textBox);
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                ResetTextBox(textBox);
            }
        }

        private void ResetTextBox(TextBox textBox)
        {
            textBox.IsReadOnly = true;
            textBox.IsHitTestVisible = false;
            textBox.CaretBrush = Brushes.Transparent; // Hide the cursor
            textBox.BorderThickness = BorderHide; // Hide the border
            textBox.Background = Brushes.Transparent; // Hide the border

            // Unsubscribe the event handlers.
            textBox.KeyDown -= TextBox_KeyDown;
            textBox.LostFocus -= TextBox_LostFocus;
        }
    }
}
