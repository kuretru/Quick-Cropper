using System.IO;
using System.Windows.Media.Imaging;

namespace Kuretru.QuickCropper.Util
{
    static class ImageUtils
    {

        public static BitmapImage Convert(System.Drawing.Image src)
        {
            MemoryStream memoryStream = new MemoryStream();
            src.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            memoryStream.Seek(0, SeekOrigin.Begin);
            image.StreamSource = memoryStream;
            image.EndInit();
            return image;
        }

    }
}
