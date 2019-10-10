using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Media.Imaging;

namespace VGClipTrimmer.helpers
{
    public class FFmpeg
    {
        public static Bitmap Snapshot(string time, string video, string width, string height, string x, string y)
        {
            Bitmap bitmap = null;

            using (Process process = new Process())
            {
                process.StartInfo.FileName = "./ffmpeg/ffmpeg.exe";
                process.StartInfo.Arguments = "-ss " + time + " -i " + video + " -filter:v crop=" + width + ":" + height + ":" + x + ":" + y + " -vframes 1 -c:v png -f image2pipe -";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.Start();
                process.BeginErrorReadLine();

                bitmap = new Bitmap(process.StandardOutput.BaseStream);

                process.WaitForExit();
            }

            return bitmap;
        }

        public static string Info(string video)
        {
            string result = string.Empty;

            using (Process process = new Process())
            {
                process.StartInfo.FileName = "./ffmpeg/ffprobe.exe";
                process.StartInfo.Arguments = "-hide_banner -show_format -show_streams -pretty " + video;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.Start();
                process.BeginErrorReadLine();

                result = process.StandardOutput.ReadToEnd();

                process.WaitForExit();
            }

            return result;
        }

        public static void Snapshots(string video, string width, string height, string x, string y, DataReceivedEventHandler output)
        {
            Process proc = new Process();
            proc.StartInfo.FileName = "./ffmpeg/ffmpeg.exe";
            proc.StartInfo.Arguments = "-i " + video + " -vf fps=1,crop=" + width + ":" + height + ":" + x + ":" + y + " -c:v png -f image2pipe -";
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.CreateNoWindow = true;
            proc.StartInfo.RedirectStandardError = true;
            proc.StartInfo.RedirectStandardOutput = true;

            proc.OutputDataReceived += output;

            proc.Start();

            proc.BeginErrorReadLine();
            proc.BeginOutputReadLine();

            proc.WaitForExit();
        }
    }
}
