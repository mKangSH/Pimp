# P Image Processing Program
간단한 모듈식 형태로 비전 프로그램의 형태를 만들고자 시작한 프로그램

이미지에 따른 검사 방법을 빠르게 검토하는 것이 목적

Asset을 좌클릭 Drag And Drop으로 Canvas에 배치한 이후
우클릭으로 Asset끼리 연결하면 Image에 연결된 모듈이 실시간으로 업데이트 된다.
### MainUI
<img src="./Document/Main UI_2024_0505.png" title="px(픽셀) 크기 설정" alt="MainUI"></img><br/>

## 사용 언어
**Language : C#**    
**Framework : WPF, .NET Framework4.8 => .NET 8.0**   
**Library : OpenCVSharp4.5.2.20210404**

## 사용 방법 
### 기본 조작 영상

### Save And Load 영상

<span style="color:white;font-size:125%">**Module NameSpace :**</span>
<span style="color:MediumPurple;font-size:125%">**namespace Pimp.CSharpAssembly.Modules**</span>    

기본적인 이미지 처리 단위인 Module을 추가하고 Main UI에 배치 하여 처리한다.     

Module Template이 필요한 경우 Main UI에서 파일이 존재하는 위치에서 우클릭으로 Add C# 파일을 선택해서 생성 가능

OpenCV를 사용하는 기본적인 Module 구조
```
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

namespace Pimp.CSharpAssembly.Modules
{
    class {FileName} : BaseModule
    {
        public {FileName}()
        {
            
        }

        public override void Run()
        {
            if (InputImage == null)
            {
                return;
            }

            base.Run();
            Mat inspectionMat = InputImage.ToMat();
            Mat result = new Mat();
            
            // 여기에 코드를 작성하세요

            OutputImage = result.ToBitmapSource();
        }
    }
}"
```

