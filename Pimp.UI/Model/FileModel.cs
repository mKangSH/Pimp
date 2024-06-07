using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;
using System.Xml.Serialization;

namespace Pimp.Model
{
    public enum FileType
    {
        Image,
        CSharp,
        Other
    }

    [Serializable]
    public class FileModel
    {
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string FileExtension { get; set; }

        [NonSerialized]
        private ImageSource _fileIcon;
        [XmlIgnore]
        public ImageSource FileIcon
        {
            get
            {
                if (_fileIcon == null)
                {
                    LoadFileIcon();
                }
                return _fileIcon;
            }
        }

        private void LoadFileIcon()
        {
            using (var icon = System.Drawing.Icon.ExtractAssociatedIcon(FilePath))
            {
                _fileIcon = Imaging.CreateBitmapSourceFromHIcon(
                    icon.Handle,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());
            }
        }

        [XmlIgnore]
        public string FileContent
        {
            get
            {
                if (FileType == FileType.CSharp)
                {
                    return System.IO.File.ReadAllText(FilePath, Encoding.UTF8);
                }
                return null;
            }
        }

        [XmlIgnore]
        public FileType FileType
        {
            get
            {
                switch (FileExtension.ToLower())
                {
                    case ".jpg":
                    case ".png":
                    case ".bmp":
                        return FileType.Image;
                    case ".cs":
                        return FileType.CSharp;
                    default:
                        return FileType.Other;
                }
            }
        }
    }
}
