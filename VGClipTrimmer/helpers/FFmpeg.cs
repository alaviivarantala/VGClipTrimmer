using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace VGClipTrimmer.helpers
{
    public class FFmpeg
    {
        public static Bitmap Snapshot(string time, string video, string width, string height, string x, string y)
        {
            string result = string.Empty;
            string errors = string.Empty;

            Process proc = new Process();
            proc.StartInfo.FileName = "./ffmpeg/ffmpeg.exe";
            proc.StartInfo.Arguments = "-ss " + time + " -i " + video + " -filter:v crop=" + width + ":" + height + ":" + x + ":" + y + " -vframes 1 -c:v png -f image2pipe -";
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.CreateNoWindow = true;
            proc.StartInfo.RedirectStandardError = true;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.Start();
            proc.BeginErrorReadLine();

            PngBitmapDecoder decoder = new PngBitmapDecoder(proc.StandardOutput.BaseStream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
            BitmapSource bitmapSource = decoder.Frames[0];

            proc.WaitForExit();

            Bitmap bitmap;
            using (var outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapSource));
                enc.Save(outStream);
                bitmap = new Bitmap(outStream);
            }

            return bitmap;
        }

        public static string Info(string video)
        {
            string result = string.Empty;
            string errors = string.Empty;

            Process proc = new Process();
            proc.StartInfo.FileName = "./ffmpeg/ffprobe.exe";
            proc.StartInfo.Arguments = "-hide_banner -show_format -show_streams -pretty " + video;
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.CreateNoWindow = true;
            proc.StartInfo.RedirectStandardError = true;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.Start();
            proc.BeginErrorReadLine();
            proc.WaitForExit();

            result = proc.StandardOutput.ReadToEnd();

            return result;
        }

        public static string Snapshots(string video, string width, string height, string x, string y)
        {
            string result = string.Empty;
            string errors = string.Empty;

            Process proc = new Process();
            proc.StartInfo.FileName = "./ffmpeg/ffmpeg.exe";
            proc.StartInfo.Arguments = "-i " + video + " -filter:v crop=" + width + ":" + height + ":" + x + ":" + y + " -vsync 0 -vf select='not(mod(n,100))' -c:v png -f image2pipe -";
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.CreateNoWindow = true;
            proc.StartInfo.RedirectStandardError = true;
            proc.StartInfo.RedirectStandardOutput = true;

            //proc.OutputDataReceived += (sender, e) => result += e.Data;
            //proc.ErrorDataReceived += (sender, e) => errors += e.Data;

            proc.OutputDataReceived += new DataReceivedEventHandler((sender, e) =>
            {
                result += e.Data;
            });

            proc.ErrorDataReceived += new DataReceivedEventHandler((sender, e) =>
            {
                errors += e.Data;
            });

            proc.Start();

            proc.BeginErrorReadLine();
            proc.BeginOutputReadLine();

            proc.WaitForExit();

            return result;
        }

    }
}
