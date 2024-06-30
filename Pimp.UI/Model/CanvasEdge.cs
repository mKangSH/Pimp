using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Pimp.Model
{
    [Serializable]
    public class CanvasEdge : INotifyPropertyChanged
    {
        private static readonly double PointOffsetY = 79.5;
        private static readonly double StartPointOffsetX = 131;
        private static readonly double EndPointOffsetX = 13;

        private CanvasInstanceBaseModel _start;
        public CanvasInstanceBaseModel Start
        {
            get { return _start; }
            set
            {
                if (_start != value)
                {
                    if (_start != null)
                    {
                        _start.PropertyChanged -= Start_PropertyChanged;
                    }

                    _start = value;

                    if (_start != null)
                    {
                        _start.PropertyChanged += Start_PropertyChanged;
                    }
                }
            }
        }

        private CanvasInstanceBaseModel _end;
        public CanvasInstanceBaseModel End
        {
            get { return _end; }
            set
            {
                if (_end != value)
                {
                    if (_end != null)
                    {
                        _end.PropertyChanged -= End_PropertyChanged;
                    }

                    _end = value;

                    if (_end != null)
                    {
                        _end.PropertyChanged += End_PropertyChanged;
                    }
                }
            }
        }

        public CanvasEdge(CanvasInstanceBaseModel start, CanvasInstanceBaseModel end)
        {
            Start = start;
            End = end;

            StartPoint = new Point(Start.X + StartPointOffsetX, Start.Y + PointOffsetY);
            EndPoint = new Point(End.X + EndPointOffsetX, End.Y + PointOffsetY);

            double offset = CalculateXDistance(StartPoint.X, EndPoint.X);

            ControlPoint1 = new Point(StartPoint.X + offset, StartPoint.Y);
            ControlPoint2 = new Point(EndPoint.X - offset, EndPoint.Y);
        }

        private Point _startPoint;
        public Point StartPoint 
        { 
            get { return _startPoint; } 
            set
            {
                if(_startPoint != value)
                {
                    _startPoint = value;
                    OnPropertyChanged(nameof(StartPoint));
                }
            }
        }

        private Point _endPoint;
        public Point EndPoint 
        { 
            get { return _endPoint; } 
            set
            {
                if(_endPoint != value)
                {
                    _endPoint = value;
                    OnPropertyChanged(nameof(EndPoint));
                }
            }
        }

        private Point _controlPoint1;
        public Point ControlPoint1 
        {
            get { return _controlPoint1; } 
            set 
            {
                _controlPoint1 = value; 
                OnPropertyChanged(nameof(ControlPoint1));
            }
        }

        private Point _controlPoint2;
        public Point ControlPoint2 
        {
            get { return _controlPoint2; }
            set
            {
                _controlPoint2 = value;
                OnPropertyChanged(nameof(ControlPoint2));
            }
        }

        private void Start_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(CanvasInstanceBaseModel.X) || e.PropertyName == nameof(CanvasInstanceBaseModel.Y))
            {
                StartPoint = new Point(Start.X + StartPointOffsetX, Start.Y + PointOffsetY);
                double offset = CalculateXDistance(StartPoint.X, EndPoint.X);
                ControlPoint1 = new Point(StartPoint.X + offset, StartPoint.Y);
                ControlPoint2 = new Point(EndPoint.X - offset, EndPoint.Y);
            }
        }

        private void End_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(CanvasInstanceBaseModel.X) || e.PropertyName == nameof(CanvasInstanceBaseModel.Y))
            {
                EndPoint = new Point(End.X + EndPointOffsetX, End.Y + PointOffsetY);
                double offset = CalculateXDistance(StartPoint.X, EndPoint.X);
                ControlPoint1 = new Point(StartPoint.X + offset, StartPoint.Y);
                ControlPoint2 = new Point(EndPoint.X - offset, EndPoint.Y);
            }
        }
        public double CalculateXDistance(double start, double end)
        {
            return Math.Abs(end - start);
        }

        // INotifyPropertyChanged 구현
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public CanvasEdge()
        {

        }
    }
}
