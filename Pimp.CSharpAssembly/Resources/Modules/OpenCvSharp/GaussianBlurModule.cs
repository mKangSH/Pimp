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
    class GaussianBlurModule : OneInputBaseModule
    {
        private int _kernelSize = 5;
        public int KernelSize
        {
            get { return _kernelSize; }
            set
            {
                if (_kernelSize == value)
                {
                    return;
                }

                if (value < 1)
                {
                    _kernelSize = 1;
                }
                else if (value > 61)
                {
                    _kernelSize = 61;
                }
                else
                {
                    _kernelSize = value;
                }
            }
        }

        private double _sigmaX = 0;
        public double SigmaX
        {
            get { return _sigmaX; }
            set
            {
                if (_sigmaX == value)
                {
                    return;
                }

                _sigmaX = value;
            }
        }

        private double _sigmaY = 0;
        public double SigmaY
        {
            get { return _sigmaY; }
            set
            {
                if (_sigmaY == value)
                {
                    return;
                }

                _sigmaY = value;
            }
        }

        private BorderTypes _borderTypes = BorderTypes.Default;
        public BorderTypes BorderTypes
        {
            get { return _borderTypes; }
            set
            {
                if (_borderTypes == value)
                {
                    return;
                }

                _borderTypes = value;
            }
        }

        public GaussianBlurModule()
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
            Size kSize = new Size(_kernelSize, _kernelSize);
            try
            {
                Cv2.GaussianBlur(inspectionMat, result, new Size(_kernelSize, _kernelSize), _sigmaX, _sigmaY, _borderTypes);

                OutputImage = result.ToBitmapSource();
            }

            catch(Exception ex)
            {
                var splitTrace = ex.StackTrace.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
                Logger.Instance.AddLog($"{splitTrace[splitTrace.Length - 1]}{Environment.NewLine}{ex.Message}");

                OutputImage = InputImage;
            }
        }
    }
}