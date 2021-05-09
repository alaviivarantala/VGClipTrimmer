using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;

namespace GameHighlightClipper.Helpers
{
    public static class Utilities
    {
        public static void ShutdownApp()
        {
            Application.Current.Shutdown();
        }

        public static string GetPathForExe(string fileName)
        {
            string keyBase = @"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths";
            RegistryKey localMachine = Registry.LocalMachine;
            RegistryKey fileKey = localMachine.OpenSubKey(string.Format(@"{0}\{1}", keyBase, fileName));
            object result = null;
            if (fileKey != null)
            {
                result = fileKey.GetValue(string.Empty);
                fileKey.Close();
            }

            return (string)result;
        }
    }
}
