using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using GameHighlightClipper.Video.Processing;

namespace GameHighlightClipper.Helpers
{
    public class FFmpeg
    {
        public static Tuple<string, string> Info(string video)
        {
            string results = string.Empty;
            string errors = string.Empty;

            using (Process process = new Process())
            {
                process.StartInfo.FileName = "./video/ffmpeg/ffprobe.exe";
                process.StartInfo.Arguments = "-hide_banner -show_format -show_streams -pretty \"" + video + "\"";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.Start();
                results = process.StandardOutput.ReadToEnd();
                errors = process.StandardError.ReadToEnd();

                process.WaitForExit();
            }

            return new Tuple<string, string>(results, errors);
        }

        public static Bitmap Snapshot(string video, TimeSpan time)
        {
            Bitmap bitmap = null;

            using (Process process = new Process())
            {
                process.StartInfo.FileName = "./video/ffmpeg/ffmpeg.exe";
                process.StartInfo.Arguments = "-ss " + time.ToString() + " -i \"" + video + "\" -vframes 1 -vcodec png -f image2pipe -";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.RedirectStandardError = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.Start();

                bitmap = new Bitmap(process.StandardOutput.BaseStream);

                process.WaitForExit();
            }

            return bitmap;
        }

        public static List<byte[]> SnapshotEverySecond(string video, string width, string height, string x, string y, int readSize)
        {
            Process proc = new Process();
            proc.StartInfo.FileName = "./video/ffmpeg/ffmpeg.exe";
            proc.StartInfo.Arguments = "-i " + video + " -vf fps=1,crop=" + width + ":" + height + ":" + x + ":" + y + " -vcodec png -f image2pipe -";
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.CreateNoWindow = true;
            proc.StartInfo.RedirectStandardError = false;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.EnableRaisingEvents = true;
            proc.Start();

            List<byte[]> resultsList = new List<byte[]>();

            using (BinaryReader reader = new BinaryReader(proc.StandardOutput.BaseStream))
            {
                byte[] readBytes = null;
                byte[] stash = new byte[0];
                byte[] image = new byte[0];
                int frameCount = 0;

                do
                {
                    readBytes = reader.ReadBytes(readSize);

                    byte[] pattern = new byte[] { 73, 69, 78, 68, 174, 66, 96, 130 };
                    int[] results = ByteProcessing.Locate(readBytes, pattern);

                    for (int i = 0; i < results.Length; i++)
                    {
                        results[i] += pattern.Length;
                    }

                    if (results.Length == 0)
                    {
                        stash = stash.ToList().Concat(readBytes.ToList()).ToArray();
                    }
                    else if (results.Length == 1)
                    {
                        image = stash.ToList().Concat(readBytes.Take(results[0]).ToList()).ToArray();
                        resultsList.Add(image);
                        frameCount++;
                        stash = readBytes.Skip(results[0]).ToArray();
                    }
                    else
                    {
                        for (int i = 0; i < results.Length; i++)
                        {
                            if (i == 0)
                            {
                                image = stash.ToList().Concat(readBytes.Take(results[0]).ToList()).ToArray();
                                resultsList.Add(image);
                                frameCount++;
                            }
                            else
                            {
                                image = readBytes.Skip(results[i - 1]).Take(results[i] - results[i - 1]).ToArray();
                                resultsList.Add(image);
                                frameCount++;

                                if (i == results.Length - 1)
                                {
                                    stash = readBytes.Skip(results[i]).ToArray();
                                }
                            }
                        }
                    }
                } while (readBytes.Length != 0);
            }

            proc.WaitForExit();

            return resultsList;
        }

        public static List<byte[]> SnapshotEverySecondProgress(string video, string width, string height, string x, string y, int readSize, IProgress<int> progress)
        {
            Process proc = new Process();
            proc.StartInfo.FileName = "./video/ffmpeg/ffmpeg.exe";
            proc.StartInfo.Arguments = "-i \"" + video + "\" -vf fps=1,crop=" + width + ":" + height + ":" + x + ":" + y + " -vcodec png -f image2pipe -";
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.CreateNoWindow = true;
            proc.StartInfo.RedirectStandardError = false;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.EnableRaisingEvents = true;
            proc.Start();

            List<byte[]> resultsList = new List<byte[]>();

            using (BinaryReader reader = new BinaryReader(proc.StandardOutput.BaseStream))
            {
                byte[] readBytes = null;
                byte[] stash = new byte[0];
                byte[] image = new byte[0];
                int frameCount = 0;

                do
                {
                    readBytes = reader.ReadBytes(readSize);

                    byte[] pattern = new byte[] { 73, 69, 78, 68, 174, 66, 96, 130 };
                    int[] results = ByteProcessing.Locate(readBytes, pattern);

                    for (int i = 0; i < results.Length; i++)
                    {
                        results[i] += pattern.Length;
                    }

                    if (results.Length == 0)
                    {
                        stash = stash.ToList().Concat(readBytes.ToList()).ToArray();
                    }
                    else if (results.Length == 1)
                    {
                        image = stash.ToList().Concat(readBytes.Take(results[0]).ToList()).ToArray();
                        resultsList.Add(image);
                        progress.Report(1);
                        frameCount++;
                        stash = readBytes.Skip(results[0]).ToArray();
                    }
                    else
                    {
                        for (int i = 0; i < results.Length; i++)
                        {
                            if (i == 0)
                            {
                                image = stash.ToList().Concat(readBytes.Take(results[0]).ToList()).ToArray();
                                resultsList.Add(image);
                                progress.Report(1);
                                frameCount++;
                            }
                            else
                            {
                                image = readBytes.Skip(results[i - 1]).Take(results[i] - results[i - 1]).ToArray();
                                resultsList.Add(image);
                                progress.Report(1);
                                frameCount++;

                                if (i == results.Length - 1)
                                {
                                    stash = readBytes.Skip(results[i]).ToArray();
                                }
                            }
                        }
                    }
                } while (readBytes.Length != 0);
            }

            proc.WaitForExit();

            return resultsList;
        }


        public static List<byte[]> SnapshotsWithFPS(string video, int fps, string width, string height, string x, string y, int readSize)
        {
            Process proc = new Process();
            proc.StartInfo.FileName = "./video/ffmpeg/ffmpeg.exe";
            proc.StartInfo.Arguments = "-i " + video + " -vf fps=1/" + fps + ", crop=" + width + ":" + height + ":" + x + ":" + y + " -vcodec png -f image2pipe -";
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.CreateNoWindow = true;
            proc.StartInfo.RedirectStandardError = false;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.EnableRaisingEvents = true;
            proc.Start();

            List<byte[]> resultsList = new List<byte[]>();

            using (BinaryReader reader = new BinaryReader(proc.StandardOutput.BaseStream))
            {
                byte[] readBytes = null;
                byte[] stash = new byte[0];
                byte[] image = new byte[0];
                int frameCount = 0;

                do
                {
                    readBytes = reader.ReadBytes(readSize);

                    byte[] pattern = new byte[] { 73, 69, 78, 68, 174, 66, 96, 130 };
                    int[] results = ByteProcessing.Locate(readBytes, pattern);

                    for (int i = 0; i < results.Length; i++)
                    {
                        results[i] += pattern.Length;
                    }

                    if (results.Length == 0)
                    {
                        stash = stash.ToList().Concat(readBytes.ToList()).ToArray();
                    }
                    else if (results.Length == 1)
                    {
                        image = stash.ToList().Concat(readBytes.Take(results[0]).ToList()).ToArray();
                        resultsList.Add(image);
                        frameCount++;
                        stash = readBytes.Skip(results[0]).ToArray();
                    }
                    else
                    {
                        for (int i = 0; i < results.Length; i++)
                        {
                            if (i == 0)
                            {
                                image = stash.ToList().Concat(readBytes.Take(results[0]).ToList()).ToArray();
                                resultsList.Add(image);
                                frameCount++;
                            }
                            else
                            {
                                image = readBytes.Skip(results[i - 1]).Take(results[i] - results[i - 1]).ToArray();
                                resultsList.Add(image);
                                frameCount++;

                                if (i == results.Length - 1)
                                {
                                    stash = readBytes.Skip(results[i]).ToArray();
                                }
                            }
                        }
                    }
                } while (readBytes.Length != 0);
            }

            proc.WaitForExit();

            return resultsList;
        }


        public static Tuple<string, string> CutVideo(string inputVideo, string outputVideo, TimeSpan from, TimeSpan to)
        {
            Tuple<string, string> result = new Tuple<string, string>(string.Empty, string.Empty);

            using (Process process = new Process())
            {
                process.StartInfo.FileName = "./video/ffmpeg/ffmpeg.exe";
                process.StartInfo.Arguments = "-ss " + from + " -i " + inputVideo + " -t " + to + " -vcodec copy -acodec copy " + outputVideo;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.Start();

                string error = process.StandardError.ReadToEnd();
                string output = process.StandardOutput.ReadToEnd();

                result = new Tuple<string, string>(error, output);

                process.WaitForExit();
            }

            return result;
        }
    }
}
