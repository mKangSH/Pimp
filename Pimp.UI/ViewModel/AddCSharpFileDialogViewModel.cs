using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Pimp.ViewModel
{
    public class AddCSharpFileDialogViewModel
    {
        public AddCSharpFileDialogViewModel()
        {
            
        }

        private string GetTemplate(string name, string fileName)
        {
            string template;

            switch (name)
            {
                case "OneInputModule":
                    template = @"using OpenCvSharp.WpfExtensions;
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
    class " + fileName + @" : OneInputBaseModule
    {
        public " + fileName + @"()
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

            try
            {
                Mat inspectionMat = InputImage.ToMat();
                Mat result = new Mat();
                // 여기에 코드를 작성하세요

                OutputImage = result.ToBitmapSource();
            }
            catch (Exception ex)
            {
                var splitTrace = ex.StackTrace.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
                Logger.Instance.AddLog($""{splitTrace[splitTrace.Length - 1]}{Environment.NewLine}{ex.Message}"");

                OutputImage = InputImage;
            }
        }
    }
}";
                    break;
                case "MultiInputModule":
                    template = @"using OpenCvSharp.WpfExtensions;
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
    class " + fileName + @" : MultiInputBaseModule
    {
        public " + fileName + @"()
        {
            
        }

        public override void Run(params object[] parameters)
        {
            if (InputImage == null)
            {
                OutputImage = null;
                OverlayImage = null;
                return;
            }

            try
            {
                Mat inspectionMat = InputImage.ToMat();
                Mat result = new Mat();
                // 여기에 코드를 작성하세요

                OutputImage = result.ToBitmapSource();
            }
            catch (Exception ex)
            {
                var splitTrace = ex.StackTrace.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
                Logger.Instance.AddLog($""{splitTrace[splitTrace.Length - 1]}{Environment.NewLine}{ex.Message}"");

                OutputImage = InputImage;
            }
        }
    }
}";
                    break;
                default:
                    throw new Exception("Unknown template");
            }

            return template;
        }
    }
}
