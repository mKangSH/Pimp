using Pimp.Model;
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

namespace Pimp.View.TestCode
{
    /// <summary>
    /// CanvasControl_2.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class CanvasControl_2 : UserControl
    {
        public ScrollViewer ScrollViewer;

        private CanvasInstanceBaseModel _draggedInstance;
        private Point? _dragStartPoint = null;
        private Point _lastMousePosition;

        public CanvasControl_2()
        {
            InitializeComponent();
        }

        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var mousePosition = e.GetPosition(canvas);

            _draggedInstance = GetInstanceAtPosition(mousePosition);
            if (_draggedInstance != null)
            {
                _draggedInstance.ZIndex = 2; // 또는 다른 높은 값
                _draggedInstance.IsHighlighted = true;
                (DataContext as CanvasViewModel_2).SelectedInstance = _draggedInstance;
            }
            _lastMousePosition = mousePosition;
        }

        private void Canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_draggedInstance != null)
            {
                _draggedInstance.ZIndex = 1; // 또는 다른 낮은 값
            }
            _draggedInstance = null;
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (_dragStartPoint.HasValue)
            {
                var currentPosition = e.GetPosition(ScrollViewer);
                var offsetX = ScrollViewer.HorizontalOffset + ((_dragStartPoint.Value.X - currentPosition.X)) * 0.8;
                var offsetY = ScrollViewer.VerticalOffset + ((_dragStartPoint.Value.Y - currentPosition.Y)) * 0.8;
                ScrollViewer.ScrollToHorizontalOffset(offsetX);
                ScrollViewer.ScrollToVerticalOffset(offsetY);
                _dragStartPoint = currentPosition;
            }

            if (_draggedInstance != null)
            {
                var mousePosition = e.GetPosition(canvas);
                var dx = mousePosition.X - _lastMousePosition.X;
                var dy = mousePosition.Y - _lastMousePosition.Y;

                // Update the position of the dragged instance
                _draggedInstance.X += dx;
                _draggedInstance.Y += dy;

                _lastMousePosition = mousePosition;
            }
        }

        private CanvasInstanceBaseModel GetInstanceAtPosition(Point position)
        {
            var canvasViewModel = DataContext as CanvasViewModel_2;

            foreach (var instance in canvasViewModel.CanvasInstances)
            {
                if (IsPointInsideInstance(position, instance))
                {
                    return instance;
                }
            }
            return null;
        }

        private bool IsPointInsideInstance(Point point, CanvasInstanceBaseModel instance)
        {
            return point.X >= instance.X && point.X <= instance.X + 130/*instance.Width*/&&
                   point.Y >= instance.Y && point.Y <= instance.Y + 130/*instance.Height*/;
        }
    }
}
