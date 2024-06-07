using Microsoft.SqlServer.Server;
using Pimp.Common.Attributes;
using Pimp.Common.Command;
using Pimp.Common.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;

namespace Pimp.Model
{
    public enum ImageFormat
    {
        jpeg,
        jpg,
        gif,
        bmp
    }

    [Serializable]
    public class CanvasResultModel : CanvasInstanceBaseModel
    {
        [NonSerialized]
        object _lock = new object();

        public string ResultPath { get; set; } = "D:\\Pimp\\Results\\";

        public ImageFormat ImageFormat { get; set; } = ImageFormat.bmp;

        [UIHidden, XmlIgnore]
        public Dictionary<string, BitmapSource> ResultList { get; set; } = new Dictionary<string, BitmapSource>();

        [UIHidden, XmlIgnore]
        public ICommand SaveImagesCommand { get; }
        public CanvasResultModel()
        {
            // ImageResult로 뺄 항목들
            SaveImagesCommand = new RelayCommand(SaveImagesBmp);
        }

        // 공통적인 메서드 하나 추가 해서 Command에 바인딩 해야 함.

        // ImageResult로 뺄 항목들
        public override void OnOutputBitmapSourceChanged(string name, BitmapSource newBitmapSource)
        {
            if (newBitmapSource == null)
            {
                return;
            }

            lock (_lock)
            {
                ResultList[name] = newBitmapSource.Clone();
            }
        }

        public void DeleteResult(string name)
        {
            lock (_lock)
            {
                if (ResultList.ContainsKey(name))
                {
                    ResultList.Remove(name);
                }
            }
        }

        // ImageResult로 뺄 항목들
        public void SaveImagesBmp()
        {
            var resultPath = $"{ResultPath}{Name}\\";

            if (Directory.Exists(resultPath) == false)
            {
                Directory.CreateDirectory(resultPath);
            }

            string imageFormat = this.ImageFormat.ToString();
            foreach (var bitmap in ResultList)
            {
                if(bitmap.Value != null)
                {
                    SaveBitmapSource(bitmap.Value, $"{resultPath}{bitmap.Key}.{imageFormat}", $"{imageFormat}");
                }
            }
        }

        // ImageResult로 뺄 항목들
        public void SaveBitmapSource(BitmapSource bitmapSource, string filePath, string format)
        {
            BitmapEncoder encoder;

            switch (format.ToLower())
            {
                case "jpeg":
                case "jpg":
                    encoder = new JpegBitmapEncoder();
                    break;
                case "gif":
                    encoder = new GifBitmapEncoder();
                    break;
                case "bmp":
                    encoder = new BmpBitmapEncoder();
                    break;
                default:
                    throw new NotSupportedException($"The specified format {format} is not supported.");
            }

            encoder.Frames.Add(BitmapFrame.Create(bitmapSource));

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                encoder.Save(fileStream);
            }
        }

        public void ParentNameChanged(string originName, ref string newName)
        {
            lock(_lock)
            {
                if(ResultList.TryGetValue(originName, out BitmapSource bitmapSource))
                {
                    while(ResultList.ContainsKey(newName))
                    {
                        newName = $"{newName}_";
                    }
                    ResultList[newName] = bitmapSource;
                    ResultList.Remove(originName);
                }
            }
        }
    }
}
