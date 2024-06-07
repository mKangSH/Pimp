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
    class ThresholdModule : OneInputBaseModule
    {
        private ThresholdTypes _thresholdTypes = ThresholdTypes.Binary;
        public ThresholdTypes ThresholdTypes 
        { 
            get { return _thresholdTypes; }
            set 
            {
                if (_thresholdTypes == value)
                {
                    return;
                }

                _thresholdTypes = value;
            }
        }

        private double _thresholdValue = 128;
        public double ThresholdValue 
        { 
            get { return _thresholdValue; }
            set 
            {
                if(_thresholdValue == value)
                {
                    return;
                }

                if (value < 0)
                {
                    _thresholdValue = 0;
                }
                else if (value > 255)
                {
                    _thresholdValue = 255;
                }
                else
                {
                    _thresholdValue = value;
                }
            }
        }

        private double _maxThreshold = 255;
        public double MaxThreshold 
        {
            get { return _maxThreshold; }
            set 
            { 
                if(_maxThreshold == value)
                {
                    return;
                }

                if (value < 0)
                {
                    _maxThreshold = 0;
                }
                else if (value > 255)
                {
                    _maxThreshold = 255;
                }
                else
                {
                    _maxThreshold = value;
                }
            }
        }

        public ThresholdModule()
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
                Cv2.Threshold(inspectionMat, result, ThresholdValue, MaxThreshold, ThresholdTypes);

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