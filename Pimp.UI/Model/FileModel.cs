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
using System.Runtime.InteropServices;
using System.IO;
using System.Drawing;

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
        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbSizeFileInfo, uint uFlags);

        [StructLayout(LayoutKind.Sequential)]
        public struct SHFILEINFO
        {
            public IntPtr hIcon;
            public IntPtr iIcon;
            public uint dwAttributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        };


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

        private void LoadFileIcon()
        {
            if (string.IsNullOrEmpty(FilePath))
            {
                return;
            }

            if (File.Exists(FilePath))
            {
                using (var icon = System.Drawing.Icon.ExtractAssociatedIcon(FilePath))
                {
                    _fileIcon = Imaging.CreateBitmapSourceFromHIcon(
                        icon.Handle,
                        Int32Rect.Empty,
                        BitmapSizeOptions.FromEmptyOptions());
                }
            }
            else if (Directory.Exists(FilePath))
            {
                SHFILEINFO shinfo = new SHFILEINFO();
                IntPtr hImgSmall = SHGetFileInfo(FilePath, 0, ref shinfo, (uint)Marshal.SizeOf(shinfo), (uint)(0x100 | 0x0));

                Icon icon = Icon.FromHandle(shinfo.hIcon);

                _fileIcon = Imaging.CreateBitmapSourceFromHIcon(
                    icon.Handle,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());
            }
        }
    }
}
