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
using System.Security.Cryptography;
using Pimp.Common.Log;

namespace Pimp.CSharpAssembly.Modules
{
    class CannyModule : OneInputBaseModule
    {
        public enum ApertureSizeKernel
        {
            Size3 = 3,
            Size5 = 5,
            Size7 = 7
        }

        private double _threshold1 = 50;
        public double Threshold1
        {
            get { return _threshold1; }
            set
            {
                if (_threshold1 == value)
                {
                    return;
                }

                if (value < 0)
                {
                    _threshold1 = 0;
                }
                else if (value > 255)
                {
                    _threshold1 = 255;
                }
                else
                {
                    _threshold1 = value;
                }
            }
        }

        private double _threshold2 = 200;
        public double Threshold2
        {
            get { return _threshold2; }
            set
            {
                if (_threshold2 == value)
                {
                    return;
                }

                if (value < 0)
                {
                    _threshold2 = 0;
                }
                else if (value > 255)
                {
                    _threshold2 = 255;
                }
                else
                {
                    _threshold2 = value;
                }
            }
        }

        private ApertureSizeKernel _apertureSize = ApertureSizeKernel.Size5;
        public ApertureSizeKernel ApertureSize
        {
            get { return _apertureSize; }
            set
            {
                if (_apertureSize == value)
                {
                    return;
                }

                _apertureSize = value;
            }
        }

        private bool L2gradient = false;
        public bool L2Gradient
        {
            get { return L2gradient; }
            set
            {
                if (L2gradient == value)
                {
                    return;
                }

                L2gradient = value;
            }
        }

        public CannyModule()
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
            // Aperture size should be odd between 3 and 7
            try
            {
                // 여기에 코드를 작성하세요
                Cv2.Canny(inspectionMat, result, _threshold1, _threshold2, (int)_apertureSize, L2gradient);

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