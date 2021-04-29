using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace GameHighlightClipper.Helpers
{
    /// <summary>
    /// Service to read texts from images through OCR Tesseract engine.
    /// </summary>
public class TesseractService
    {
        private readonly string _tesseractExePath;
        private readonly string _language;

        public TesseractService(string tesseractDir, string language = "en", string dataDir = null)
        {
            _tesseractExePath = Path.Combine(tesseractDir, "tesseract.exe");
            _language = language;

            if (string.IsNullOrEmpty(dataDir))
                dataDir = Path.Combine(tesseractDir, "tessdata");

            Environment.SetEnvironmentVariable("TESSDATA_PREFIX", dataDir);
        }

        public string GetText(params Stream[] images)
        {
            var output = string.Empty;

            if (images.Any())
            {
                var tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
                Directory.CreateDirectory(tempPath);
                var tempInputFile = NewTempFileName(tempPath);
                var tempOutputFile = NewTempFileName(tempPath);

                try
                {
                    WriteInputFiles(images, tempPath, tempInputFile);

                    var info = new ProcessStartInfo
                    {
                        FileName = _tesseractExePath,
                        Arguments = $"{tempInputFile} {tempOutputFile} -l {_language}",
                        RedirectStandardError = true,
                        RedirectStandardOutput = true,
                        CreateNoWindow = true,
                        UseShellExecute = false
                    };

                    using (var ps = Process.Start(info))
                    {
                        ps.WaitForExit();

                        var exitCode = ps.ExitCode;

                        if (exitCode == 0)
                        {
                            output = File.ReadAllText(tempOutputFile + ".txt");
                        }
                        else
                        {
                            var stderr = ps.StandardError.ReadToEnd();
                            throw new InvalidOperationException(stderr);
                        }
                    }
                }
                finally
                {
                    Directory.Delete(tempPath, true);
                }
            }

            return output;
        }

        private static void WriteInputFiles(Stream[] inputStreams, string tempPath, string tempInputFile)
        {
            // If there is more thant one image file, so build the list file using the images as input files.
            if (inputStreams.Length > 1)
            {
                var imagesListFileContent = new StringBuilder();

                foreach (var inputStream in inputStreams)
                {
                    var imageFile = NewTempFileName(tempPath);

                    using (var tempStream = File.OpenWrite(imageFile))
                    {
                        CopyStream(inputStream, tempStream);
                    }

                    imagesListFileContent.AppendLine(imageFile);
                }

                File.WriteAllText(tempInputFile, imagesListFileContent.ToString());
            }
            else
            {
                // If is only one image file, than use the image file as input file.
                using (var tempStream = File.OpenWrite(tempInputFile))
                {
                    CopyStream(inputStreams.First(), tempStream);
                }
            }
        }

        private static void CopyStream(Stream input, Stream output)
        {
            if (input.CanSeek)
                input.Seek(0, SeekOrigin.Begin);

            input.CopyTo(output);
            input.Close();
        }

        private static string NewTempFileName(string tempPath)
        {
            return Path.Combine(tempPath, Guid.NewGuid().ToString());
        }
    }
}