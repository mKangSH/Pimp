
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
using System.Security.Cryptography;

namespace Pimp.CSharpAssembly.Modules
{
    class SobelModule : OneInputBaseModule
    {
        private MatType _matType;
        public MatType MatType
        {
            get { return _matType; }
            set
            {
                if (_matType == value)
                {
                    return;
                }

                _matType = value;
            }
        }

        private int _xOrder = 1;
        public int XOrder
        {
            get { return _xOrder; }
            set
            {
                if (_xOrder == value)
                {
                    return;
                }

                _xOrder = value;
            }
        }

        private int _yOrder = 1;
        public int YOrder
        {
            get { return _yOrder; }
            set
            {
                if (_yOrder == value)
                {
                    return;
                }

                _yOrder = value;
            }
        }

        private int _kSize = 3;
        public int KSize
        {
            get { return _kSize; }
            set
            {
                if (_kSize == value)
                {
                    return;
                }

                _kSize = value;
            }
        }

        public SobelModule()
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
                Cv2.Sobel(inspectionMat, result, _matType, _xOrder, _yOrder, _kSize);
                result.ConvertTo(result, MatType.CV_8U);

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