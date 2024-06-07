
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
    class WarpPolarModule : OneInputBaseModule
    {
        private int _polarImageWidth = 100;
        public int PolarImageWidth
        {
            get { return _polarImageWidth; }
            set
            {
                if (_polarImageWidth == value)
                {
                    return;
                }

                _polarImageWidth = value;
            }
        }

        private int _polarImageHeight = 100;
        public int PolarImageHeight
        {
            get { return _polarImageHeight; }
            set
            {
                if (_polarImageHeight == value)
                {
                    return;
                }

                _polarImageHeight = value;
            }
        }

        private float _centerPtX = 0;
        public float CenterPtX
        {
            get { return _centerPtX; }
            set
            {
                if (_centerPtX == value)
                {
                    return;
                }

                _centerPtX = value;
            }
        }

        private float _centerPtY = 0;
        public float CenterPtY
        {
            get { return _centerPtY; }
            set
            {
                if (_centerPtY == value)
                {
                    return;
                }

                _centerPtY = value;
            }
        }

        private double _maxRadius = 0;
        public double MaxRadius
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

        private InterpolationFlags _interpolationFlags = InterpolationFlags.Linear;
        public InterpolationFlags InterpolationFlags
        {
            get { return _interpolationFlags; }
            set
            {
                if (_interpolationFlags == value)
                {
                    return;
                }

                _interpolationFlags = value;
            }
        }

        private WarpPolarMode _warpPolarMode = WarpPolarMode.Linear;
        public WarpPolarMode WarpPolarMode
        {
            get { return _warpPolarMode; }
            set
            {
                if (_warpPolarMode == value)
                {
                    return;
                }

                _warpPolarMode = value;
            }
        }

        public WarpPolarModule()
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
                // 여기에 코드를 작성하세요
                Cv2.WarpPolar(inspectionMat, result, new Size(PolarImageWidth, PolarImageHeight), new Point2f(CenterPtX, CenterPtY), MaxRadius, InterpolationFlags.Linear, WarpPolarMode.Linear);

                OutputImage = result.ToBitmapSource();
            }
            catch (Exception ex)
            {
                var splitTrace = ex.StackTrace.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
                Logger.Instance.AddLog($"{splitTrace[splitTrace.Length - 1]}{Environment.NewLine}{ex.Message}");

                OutputImage = InputImage;
            }
        }
    }
}