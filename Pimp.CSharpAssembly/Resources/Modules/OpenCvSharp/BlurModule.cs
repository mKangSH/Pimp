
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
using Pimp.Common.Attributes;
using Pimp.Common.Log;

namespace Pimp.CSharpAssembly.Modules
{
    class BlurModule : OneInputBaseModule
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

        // Point 구조체를 사용하여 커널의 중심을 지정합니다.
        // 미구현 상태입니다.
        private Point? _point = null;
        [UIHidden]
        public Point? Point
        {
            get { return _point; }
            set
            {
                if (_point == value)
                {
                    return;
                }

                _point = value;
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

        public BlurModule()
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

            try
            {
                // 여기에 코드를 작성하세요
                Size kSize = new Size(_kernelSize, _kernelSize);
                Cv2.Blur(inspectionMat, result, kSize, _point, _borderTypes);

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