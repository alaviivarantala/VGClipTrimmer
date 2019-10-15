using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VGClipTrimmer.helpers
{
    public class OldOCR
    {
        /*
        private string OCR3(Bitmap img)
        {
            string res = "";
            using (var engine = new TesseractEngine(@"tessdata", "eng", EngineMode.Default))
            {
                // Whitelist of chars to recognize
                engine.SetVariable("tessedit_char_whitelist", "ACDEIKLMNOTW");
                //engine.SetVariable("tessedit_char_whitelist", "1234567890ABCDEFGHIJKLMNOPQRSTUVWXYZ");
                // Dont bother with word plausibility
                engine.SetVariable("tessedit_unrej_any_wd", true);

                // Look for one line
                Page page = engine.Process(img, PageSegMode.Auto);
                res = page.GetText();
            }
            return res;
        }
        */
        /*
string directoryPath = new FileInfo(video).DirectoryName;
string tempPath = Path.Combine(directoryPath, "temp");

if (!Directory.Exists(tempPath))
{
    Directory.CreateDirectory(tempPath);
}

FFmpeg.SnapshotsToFileVoid(video, width.ToString(), height.ToString(), startingPointX.ToString(), startingPointY.ToString(), tempPath);

int fileCount = Directory.GetFiles(tempPath).Length;

int maxDegreeOfParallelism = Environment.ProcessorCount;
Parallel.For(0, fileCount, new ParallelOptions { MaxDegreeOfParallelism = maxDegreeOfParallelism }, (i) =>
{
    string filePath = Path.Combine(tempPath, (i + 1).ToString() + ".png");
    TimeSpan time = TimeSpan.FromSeconds(i);
    Bitmap image = (Bitmap)Image.FromFile(filePath);
    string resultOCR = TestOCRImage(image);
    if (resultOCR.ToLower().Contains("eliminated") || resultOCR.ToLower().Contains("knocked"))
    {
        results.Add(time);
    }
    image.Dispose();
    File.Delete(filePath);
});
*/
        /*
        using (FileSystemWatcher watcher = new FileSystemWatcher())
        {
            string filePath = new FileInfo(video).DirectoryName;
            string tempPath = Path.Combine(filePath, "temp");

            if (!Directory.Exists(tempPath))
            {
                Directory.CreateDirectory(tempPath);
            }

            watcher.Path = tempPath;

            watcher.NotifyFilter = NotifyFilters.LastAccess
                                 | NotifyFilters.LastWrite
                                 | NotifyFilters.FileName
                                 | NotifyFilters.DirectoryName;

            watcher.Filter = "*.png";

            watcher.Created += new FileSystemEventHandler((sender, e) =>
            {
                TimeSpan time = TimeSpan.FromSeconds(int.Parse(e.Name.Split('.')[0]));
                Bitmap image = (Bitmap)Image.FromFile(e.FullPath);
                string resultOCR = TestOCRImage(image);
                if (resultOCR.ToLower().Contains("eliminated") || resultOCR.ToLower().Contains("knocked"))
                {
                    results.Add(time);
                }
                image.Dispose();
                File.Delete(e.FullPath);
            });

            watcher.EnableRaisingEvents = true;

            Process ffmpeg = FFmpeg.SnapshotsToFile(video, width.ToString(), height.ToString(), startingPointX.ToString(), startingPointY.ToString());

            ffmpeg.Start();
            ffmpeg.WaitForExit();
        }
        */

        /*
        int maxDegreeOfParallelism = Environment.ProcessorCount;
        Parallel.For(0, length, new ParallelOptions { MaxDegreeOfParallelism = maxDegreeOfParallelism }, (i) =>
        {
            TimeSpan time = TimeSpan.FromSeconds(i);
            Bitmap resultImage = FFmpeg.Snapshot(time.ToString(), video, width.ToString(), height.ToString(), startingPointX.ToString(), startingPointY.ToString());
            SaveImage(resultImage);
            string resultOCR = TestOCRImage(resultImage);
            if (resultOCR.ToLower().Contains("eliminated") || resultOCR.ToLower().Contains("knocked"))
            {
                results.Add(time);
            }
        });
        */
        /*
        List<string> images = new List<string>();
        string data = string.Empty;
        string received = string.Empty;
        string dd = string.Empty;
        DataReceivedEventHandler outputHandler = new DataReceivedEventHandler((sender, e) =>
        {
            received = e.Data;
            if (!string.IsNullOrWhiteSpace(data) && !string.IsNullOrWhiteSpace(received) && received.Contains("PNG"))
            {
                int index = received.IndexOf("‰PNG");
                data += received.Substring(0, index);
                images.Add(data);
                data = received.Substring(index, received.Length - index);
            }
            else
            {
                data += received;
            }
        });

        FFmpeg.SnapshotsToMemory(video, width.ToString(), height.ToString(), startingPointX.ToString(), startingPointY.ToString(), outputHandler);

        for (int i = 0; i < images.Count; i++)
        {
            Bitmap bitmap = new Bitmap(new MemoryStream(Encoding.UTF8.GetBytes(images[i])));
            SaveImage(bitmap);
            string resultOCR = TestOCRImage(bitmap);
            if (resultOCR.ToLower().Contains("eliminated") || resultOCR.ToLower().Contains("knocked"))
            {
                TimeSpan time = TimeSpan.FromSeconds(i);
                results.Add(time);
            }
        }
        */
        /*
BinaryReader reader = new BinaryReader(new FileStream(clips + "1.png", FileMode.Open));

byte[] bytes = reader.ReadBytes(50000);

byte[] pattern = new byte[] { 73, 69, 78, 68, 174, 66, 96, 130 };
int[] results = ByteProcessing.Locate(bytes, pattern);

bytes = bytes.Take(results[0] + pattern.Length).ToArray();
*/
    }
}
