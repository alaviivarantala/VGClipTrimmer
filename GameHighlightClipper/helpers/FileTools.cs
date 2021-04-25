using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Windows;

namespace GameHighlightClipper.Helpers
{
    public class FileTools
    {
        public static string CalculateMD5(string filename)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(filename))
                {
                    var hash = md5.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }
        }

        public static string[] SelectVideoFiles()
        {
            OpenFileDialog ofd = new OpenFileDialog
            {
                Multiselect = true,
                Filter = "Video files (*.mp4, *.avi, *.flv)|*.mp4;*.avi;*.flv"
            };
            bool? result = ofd.ShowDialog();

            if (result == true && result.HasValue)
            {
                return ofd.FileNames;
            }

            return new string[] { string.Empty };
        }

        public static bool IsVideoFile(string path)
        {
            string[] mediaExtensions = { ".AVI", ".MP4", ".FLV" };

            return mediaExtensions.Contains(Path.GetExtension(path), StringComparer.OrdinalIgnoreCase);
        }

        public static long GetFileSize(string path)
        {
            return new FileInfo(path).Length; 
        }

        public static string FormatFileSize(long byteCount)
        {
            string[] suf = { "B", "KB", "MB", "GB" };
            if (byteCount == 0)
            {
                return "0" + suf[0];
            }
            long bytes = Math.Abs(byteCount);
            int place = (int)Math.Round(Math.Floor(Math.Log(bytes, 1024)));
            double num = Math.Round(bytes / Math.Pow(1024, place), 1);
            return (Math.Sign(byteCount) * num).ToString() + " " + suf[place];
        }
    }
}
