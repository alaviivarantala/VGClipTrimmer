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
    public class FFmpegPipe
    {
        public static Bitmap Video(string time, string video, string width, string height, string x, string y)
        {
            string result = string.Empty;
            string errors = string.Empty;

            Process proc = new Process();
            proc.StartInfo.FileName = "./ffmpeg/ffmpeg.exe";
            //proc.StartInfo.Arguments = string.Format("-f image2pipe -i pipe:.bmp -maxrate {0}k -r {1} -an -y {2}", bitrate, fps, outputfilename);
            //ffmpeg -ss {time} -i {input}.mp4 -filter:v "crop={w}:{h}:{x}:{y}" -vframes 1 {output}.png
            //proc.StartInfo.Arguments = string.Format("-ss {0} -i {1}.mp4 -filter:v \"crop={2}:{3}:{4}:{5}\" -vframes 1 pipe:1", time, video, width, height, x, y);
            proc.StartInfo.Arguments = "-ss " + time + " -i " + video + " -filter:v crop=" + width + ":" + height + ":" + x + ":" + y + " -vframes 1 -c:v png -f image2pipe -";
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.CreateNoWindow = true;
            proc.StartInfo.RedirectStandardError = true;
            proc.StartInfo.RedirectStandardOutput = true;
            //proc.ErrorDataReceived += (sender, args) => errors += args.Data;
            //proc.OutputDataReceived += (sender, args) => result += args.Data;
            proc.Start();
            //result = proc.StandardOutput.ReadToEnd();
            //errors = proc.StandardError.ReadToEnd();
            proc.BeginErrorReadLine();

            PngBitmapDecoder decoder = new PngBitmapDecoder(proc.StandardOutput.BaseStream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
            BitmapSource bitmapSource = decoder.Frames[0];

            //proc.BeginOutputReadLine();
            //proc.BeginErrorReadLine();
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
        /*
        public static void Video2()
        {
            Process process = new Process();
            StringBuilder outputStringBuilder = new StringBuilder();

            try
            {
                process.StartInfo.FileName = exeFileName;
                process.StartInfo.WorkingDirectory = args.ExeDirectory;
                process.StartInfo.Arguments = args;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.UseShellExecute = false;
                process.EnableRaisingEvents = false;
                process.OutputDataReceived += (sender, eventArgs) => outputStringBuilder.AppendLine(eventArgs.Data);
                process.ErrorDataReceived += (sender, eventArgs) => outputStringBuilder.AppendLine(eventArgs.Data);
                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                var processExited = process.WaitForExit(PROCESS_TIMEOUT);

                if (processExited == false) // we timed out...
                {
                    process.Kill();
                    throw new Exception("ERROR: Process took too long to finish");
                }
                else if (process.ExitCode != 0)
                {
                    var output = outputStringBuilder.ToString();
                    var prefixMessage = "";

                    throw new Exception("Process exited with non-zero exit code of: " + process.ExitCode + Environment.NewLine +
                    "Output from process: " + outputStringBuilder.ToString());
                }
            }
            finally
            {
                process.Close();
            }
        }
        */
    }
}
