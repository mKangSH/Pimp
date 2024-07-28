using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Pimp.UI.Model
{
    public class MethodInfoWrapper
    {
        public MethodInfo MethodInfo { get; }

        private Visibility _visibility;
        public Visibility Visibility
        {
            get => _visibility;
            set { _visibility = value; }
        }

        public MethodInfoWrapper(MethodInfo methodInfo)
        {
            MethodInfo = methodInfo;
            _visibility = Visibility.Visible; // 기본값을 Visible로 설정
        }

        public string Name => MethodInfo.Name;
    }
}
