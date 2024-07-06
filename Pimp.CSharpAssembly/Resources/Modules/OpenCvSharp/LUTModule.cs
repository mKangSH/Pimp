
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
using System.Diagnostics.Contracts;

namespace Pimp.CSharpAssembly.Modules
{
    class LUTModule : OneInputBaseModule
    {
        private byte[] _lut = new byte[256];

        private int _contrast = 100;
        public int contrast
        {
            get { return _contrast; }
            set
            {
                if (_contrast == value)
                {
                    return;
                }

                //if (value < -100)
                //{
                //    _contrast = -100;
                //}
                //else if (value > 100)
                //{
                //    _contrast = 100;
                //}
                //else
                //{
                    _contrast = value;
                //}
            }
        }

        private int _brightness = 0;
        public int brightness
        {
            get { return _brightness; }
            set
            {
                if (_brightness == value)
                {
                    return;
                }

                //if (value < -100)
                //{
                //    _brightness = -100;
                //}
                //else if (value > 100)
                //{
                //    _brightness = 100;
                //}
                //else
                //{
                    _brightness = value;
                //}
            }
        }

        public LUTModule()
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
                if (contrast > 0)
                {
                    double delta = 127 * contrast / 100;
                    double contrastResult = 255.0 / (255.0 - delta * 2);
                    double brightnessResult = contrastResult * (brightness - delta);

                    for (int i = 0; i < 256; i++)
                    {
                        double lutValue = Math.Round(contrastResult * i + brightnessResult);

                        if (lutValue < 0)
                        {
                            lutValue = 0;
                        }
                        else if (lutValue > 255)
                        {
                            lutValue = 255;
                        }

                        _lut[i] = (byte)lutValue;
                    }
                }

                Cv2.LUT(inspectionMat, _lut, result);
                // 여기에 코드를 작성하세요

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