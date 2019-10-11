using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;

namespace VGClipTrimmer.helpers
{
    public class FFmpeg
    {
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

        public static void SnapshotsToMemory(string video, string width, string height, string x, string y)
        {
            Process proc = new Process();
            proc.StartInfo.FileName = "./ffmpeg/ffmpeg.exe";
            proc.StartInfo.Arguments = "-i " + video + " -vf fps=1,crop=" + width + ":" + height + ":" + x + ":" + y + " -vcodec png -f image2pipe -";
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.CreateNoWindow = true;
            proc.StartInfo.RedirectStandardError = true;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.EnableRaisingEvents = true;
            proc.Start();
            proc.BeginErrorReadLine();

            List<Bitmap> images = new List<Bitmap>();

            using (BinaryReader reader = new BinaryReader(proc.StandardOutput.BaseStream))
            {
                byte[] readBytes = null;
                byte[] stash = new byte[0];
                bool firstPass = true;
                do
                {
                    readBytes = reader.ReadBytes(1000);
                    byte[] pattern = new byte[] { 137, 80, 78, 71 };
                    int[] results = ByteProcessing.Locate(readBytes, pattern);

                    if (firstPass && results.Length == 1)
                    {
                        stash = stash.ToList().Concat(readBytes.ToList()).ToArray();
                        firstPass = false;
                    }
                    else
                    {
                        if (results.Length > 0)
                        {
                            for (int i = 0; i < results.Length; i++)
                            {
                                byte[] image = new byte[0];
                                if (i == 0)
                                {
                                    image = stash.ToList().Concat(readBytes.Take(results[i]).ToList()).ToArray();
                                }
                                else if (i == results.Length - 1)
                                {
                                    stash = readBytes.Skip(results[i]).ToArray();
                                }
                                else
                                {
                                    image = readBytes.Skip(results[i]).Take(results[i + 1]).ToArray();
                                }
                                if (image.Length > 0)
                                {
                                    string testiPath = Path.Combine(@"c:\", "clips", "testit");
                                    int count = Directory.EnumerateFiles(testiPath).Count();
                                    //File.WriteAllBytes(Path.Combine(testiPath, count + ".png"), image);

                                    using (MemoryStream ms = new MemoryStream(image.ToArray()))
                                    {
                                        images.Add(new Bitmap(ms));
                                    }

                                }
                            }
                        }
                        else
                        {
                            stash = stash.ToList().Concat(readBytes.ToList()).ToArray();
                        }
                    }
                } while (readBytes.Length != 0);

            }

            proc.WaitForExit();
        }

        public static Process SnapshotsToFileProcess(string video, string width, string height, string x, string y)
        {
            string filePath = new FileInfo(video).DirectoryName;
            string tempPath = Path.Combine(filePath, "temp");

            Process process = new Process();
            process.StartInfo.FileName = "./ffmpeg/ffmpeg.exe";
            process.StartInfo.Arguments = "-i " + video + " -vf fps=1,crop=" + width + ":" + height + ":" + x + ":" + y + " -vcodec png " + tempPath + "\\%d.png";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.RedirectStandardError = false;
            process.StartInfo.RedirectStandardOutput = false;

            return process;
        }

        public static void SnapshotsToFileVoid(string video, string width, string height, string x, string y, string tempPath)
        {
            Process process = new Process();
            process.StartInfo.FileName = "./ffmpeg/ffmpeg.exe";
            process.StartInfo.Arguments = "-i " + video + " -vf fps=1,crop=" + width + ":" + height + ":" + x + ":" + y + " -vcodec png " + tempPath + "\\%d.png";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.RedirectStandardError = false;
            process.StartInfo.RedirectStandardOutput = false;

            process.Start();
            process.WaitForExit();
        }

    }
}
