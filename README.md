# P Image Processing Program
간단한 모듈식 형태로 비전 프로그램의 형태를 만들고자 시작한 프로그램

이미지에 따른 검사 방법을 빠르게 검토하는 것이 목적

Asset을 좌클릭 Drag And Drop으로 Canvas에 배치한 이후
우클릭으로 Asset끼리 연결하면 Image에 연결된 모듈이 실시간으로 업데이트 된다.

!!! 알려진 문제점 !!!    
Pimp.CSharpAssembly의 종속성 어셈블리를 삭제하고 다시 Pimp.Common의 dll을 참조하여 재빌드 해야함. (6/30 발견)      
Pimp.CSharpAssembly는 D:\CodeProject\ 경로에 배치해야 정상 동작함.       
     
Canvas에 AddInstance 후 Delete 할 때 Remove가 정확하게 되지 않아 메모리에 잔존하여 Assembly Unload가 불가능    
 - Develop/Refactoring Branch에서 수정 진행중 (프로그램 기본 기능에 대하여 Unload 정상 작동 확인 완료)
 - Develop/Refactoring Branch 변경 사항
     - HighLighting 기능 활성화
     - Detail View 더블클릭 기능 임시 비활성화
     - Instance Save, Load, copy, Paste 동작 기능 7/6 활성화
     - CSharpAssembly 빌드 시 파일 감지하여 자동 DLL Load, Unload 기능 7/6 활성화
     - CSharpAssembly 중 GaussianBlurModule에서 Enum 내용 삭제 (Assembly 로드 시 메모리에 잔존)

Canvas 관련 함수 수정 진행 중

### MainUI
<img src="./Document/Main UI_2024_0505.png" title="px(픽셀) 크기 설정" alt="MainUI"></img><br/>

## 사용 언어
**Language : C#**    
**Framework : WPF, .NET Framework4.8 => .NET 8.0**   
**Library : OpenCVSharp4.5.2.20210404**

## 사용 방법
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
}"
```

