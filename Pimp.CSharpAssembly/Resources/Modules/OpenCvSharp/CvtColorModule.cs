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
    class CvtColorModule : OneInputBaseModule
    {
        private ColorConversionCodes _colorConversionCodes = ColorConversionCodes.BGR2GRAY;
        public ColorConversionCodes ColorConversionCodes
        {
            get { return _colorConversionCodes; }
            set
            {
                if (_colorConversionCodes == value)
                {
                    return;
                }

                _colorConversionCodes = value;
            }
        }

        public CvtColorModule()
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
                // OpenCvException이 발생할 수 있는 코드
                Cv2.CvtColor(inspectionMat, result, _colorConversionCodes);
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