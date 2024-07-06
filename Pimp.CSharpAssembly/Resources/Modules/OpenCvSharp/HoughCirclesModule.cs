
using OpenCvSharp.WpfExtensions;
using OpenCvSharp;
using Pimp.CSharpAssembly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Pimp.Common.Models;
using Pimp.Common.Log;

namespace Pimp.CSharpAssembly.Modules
{
    class HoughCirclesModule : OneInputBaseModule
    {
        private static readonly Scalar _alphaRed = new Scalar(0, 0, 255, 128);

        private HoughModes _houghMode = HoughModes.Gradient;
        public HoughModes HoughMode
        {
            get { return _houghMode; }
            set
            {
                if (_houghMode == value)
                {
                    return;
                }

                _houghMode = value;
            }
        }

        private double _dp = 1;
        public double Dp
        {
            get { return _dp; }
            set
            {
                if (_dp == value)
                {
                    return;
                }

                _dp = value;
            }
        }

        private double _minDist = 200;
        public double MinDist
        {
            get { return _minDist; }
            set
            {
                if (_minDist == value)
                {
                    return;
                }

                _minDist = value;
            }
        }

        private double _param1 = 100;
        public double Param1
        {
            get { return _param1; }
            set
            {
                if (_param1 == value)
                {
                    return;
                }

                _param1 = value;
            }
        }

        private double _param2 = 150;
        public double Param2
        {
            get { return _param2; }
            set
            {
                if (_param2 == value)
                {
                    return;
                }

                _param2 = value;
            }
        }

        private int _minRadius = 100;
        public int MinRadius
        {
            get { return _minRadius; }
            set
            {
                if (_minRadius == value)
                {
                    return;
                }

                _minRadius = value;
            }
        }

        private int _maxRadius = 500;
        public int MaxRadius
        {
            get { return _maxRadius; }
            set
            {
                if (_maxRadius == value)
                {
                    return;
                }

                _maxRadius = value;
            }
        }

        public HoughCirclesModule()
        {
            
        }

        public override void Run()
        {
            if (InputImage == null)
            {
                OutputImage = null;
                OverlayImage = null;
                return;
            }

            Mat inspectionMat = InputImage.ToMat();
            Mat result = new Mat();

            // 여기에 코드를 작성하세요
            try
            {
                Mat overlay = new Mat(inspectionMat.Height, inspectionMat.Width, MatType.CV_8UC4);

                // Cv2.HoughCircles(검출 이미지, 검출 방법, 해상도 비율, 최소 거리, 캐니 엣지 임곗값, 중심 임곗값, 최소 반지름, 최대 반지름)
                CircleSegment[] circles = Cv2.HoughCircles(inspectionMat, HoughMode, Dp, MinDist, Param1, Param2, MinRadius, MaxRadius);

                // 찾은 원을 그립니다.
                foreach (CircleSegment circle in circles)
                {
                    Cv2.Circle(overlay, (int)circle.Center.X, (int)circle.Center.Y, (int)circle.Radius, _alphaRed, 3);
                }

                // 여기에 코드를 작성하세요
                OverlayImage = overlay.ToBitmapSource();
                OutputImage = result.ToBitmapSource();
            }
            catch (Exception ex)
            {
                Logger.Instance.AddLog($"{ex.Message}");

                OutputImage = InputImage;
            }
        }
    }
}