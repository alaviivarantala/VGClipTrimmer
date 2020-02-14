using Microsoft.Win32;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Windows;

namespace VGClipTrimmer.Helpers
{
    public class General
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

        public static string SelectVideoFile()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = false;
            ofd.Filter = "Video files (*.mp4, *.avi, *.flv)|*.mp4;*.avi;*.flv";
            bool? result = ofd.ShowDialog();

            if (result == true && result.HasValue)
            {
                return ofd.FileName;
            }

            return string.Empty;
        }

        public static void ShutdownApp()
        {
            Application.Current.Shutdown();
        }
    }
}
