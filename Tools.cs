using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Screenshoter {
    public static class Tools {
        public static Bitmap? TakeScreenshot() {
            var left = SystemInformation.VirtualScreen.Left;
            var top = SystemInformation.VirtualScreen.Top;
            var num = SystemInformation.VirtualScreen.Width;
            var num2 = SystemInformation.VirtualScreen.Height;
            Bitmap bitmap = new Bitmap(num, num2);
            try {
                using Graphics graphics = Graphics.FromImage(bitmap);
                graphics.CopyFromScreen(left, top, 0, 0, bitmap.Size);
                return bitmap;
            } catch {
                bitmap.Dispose();
                return null;
            }
        }

        public static void SaveScreenshot(string path, Bitmap screenshot) {
            var directory = Path.GetDirectoryName(path);
            if (directory == null) {
                throw new Exception($"Не удалось открыть папку для файла {path}");
            }

            Directory.CreateDirectory(directory);

            var tempFile = Path.GetTempFileName();
            var encoder = ImageCodecInfo.GetImageEncoders()
                .First(c => c.FormatID == ImageFormat.Jpeg.Guid);
            EncoderParameters encoderParameters =
                new EncoderParameters(1) {Param = {[0] = new EncoderParameter(Encoder.Quality, 30L)}};
            screenshot.Save(tempFile, encoder, encoderParameters);

            File.Copy(tempFile, path);
        }

        [DllImport("user32.dll")]
        public static extern bool SetProcessDPIAware();

        public static void RemoveFoldersOlder(string path, int days) {
            var folders = Directory.GetDirectories(path, "*", SearchOption.TopDirectoryOnly);
            foreach (var folder in folders) {
                if (!DateTime.TryParseExact(new DirectoryInfo(folder).Name, "dd.MM.yyyy",
                    CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out var folderTime)) {
                    continue;
                }

                if ((DateTime.Now - folderTime).TotalDays > days) {
                    Directory.Delete(folder, true);
                }
            }
        }
    }
}