using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Pimp.UI.Models.CanvasModels
{
    public class MethodInfoObject : PimpCanvasObject
    {
        public MethodInfo MethodInfo { get; }

        public MethodInfoObject(MethodInfo methodInfo)
        {
            MethodInfo = methodInfo;
            _visibility = Visibility.Visible; // 기본값을 Visible로 설정
        }

        public string Name => MethodInfo.Name;

        public ParameterInfo[] MethodParameters => MethodInfo.GetParameters();

        public ParameterInfo MethodReturnParameter => MethodInfo.ReturnParameter;
    }
}
