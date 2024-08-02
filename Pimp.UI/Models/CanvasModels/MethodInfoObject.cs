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

        public string Name => MethodInfo.Name;

        public List<ParameterInfo> MethodParameters => MethodInfo.GetParameters().Where(p => p.Attributes != ParameterAttributes.Out).ToList();

        private List<ParameterInfo> _methodReturnParameters;

        public List<ParameterInfo> MethodReturnParameters => _methodReturnParameters;

        public MethodInfoObject(MethodInfo methodInfo)
        {
            MethodInfo = methodInfo;
            _visibility = Visibility.Visible; // 기본값을 Visible로 설정

            _methodReturnParameters = new List<ParameterInfo>((MethodInfo.GetParameters().Where(p => p.Attributes == ParameterAttributes.Out).ToList()));
            if(MethodInfo.ReturnParameter.ParameterType.Name != "Void")
            {
                _methodReturnParameters.Add(MethodInfo.ReturnParameter);
            }
        }
    }
}
