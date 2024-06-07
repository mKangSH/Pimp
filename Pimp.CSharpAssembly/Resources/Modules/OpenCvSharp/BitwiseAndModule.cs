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
    class BitwiseAndModule : MultiInputBaseModule
    {
        public BitwiseAndModule()
        {
            
        }

        public override void Run(params object[] parameters)
        {
            //if (InputImages == null)
            //{
            //    OutputImage = null;
            //    OverlayImage = null;
            //    return;
            //}

            try
            {
                //Mat inspectionMat = InputImage.ToMat();
                //Mat result = new Mat();
                //// 여기에 코드를 작성하세요

                //OutputImage = result.ToBitmapSource();
            }
            catch (Exception ex)
            {
                //var splitTrace = ex.StackTrace.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
                //Logger.Instance.AddLog($"{splitTrace[splitTrace.Length - 1]}{Environment.NewLine}{ex.Message}");

                //OutputImage = InputImage;
            }
        }
    }
}