
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
    class SubMatModule : OneInputBaseModule
    {
        private int _roiX = 0;
        public int RoiX
        {
            get { return _roiX; }
            set
            {
                if (_roiX == value)
                {
                    return;
                }

                _roiX = value;
            }
        }

        private int _roiy = 0;
        public int RoiY
        {
            get { return _roiy; }
            set
            {
                if (_roiy == value)
                {
                    return;
                }

                _roiy = value;
            }
        }

        private int _width = 100;
        public int Width
        {
            get { return _width; }
            set
            {
                if (_width == value)
                {
                    return;
                }

                _width = value;
            }
        }

        private int _height = 100;
        public int Height
        {
            get { return _height; }
            set
            {
                if (_height == value)
                {
                    return;
                }

                _height = value;
            }
        }

        public SubMatModule()
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

            // 여기에 코드를 작성하세요
            try
            {
                Rect roi = new Rect(RoiX, RoiY, Width, Height);

                Mat result = inspectionMat.SubMat(roi);

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