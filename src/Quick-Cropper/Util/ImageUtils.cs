using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;

namespace Kuretru.QuickCropper.Util
{
    static class ImageUtils
    {

        /// <summary>
        /// 将Image/Bitmap转换为BitmapImage
        /// </summary>
        /// <param name="src">Image/Bitmap类型对象</param>
        /// <returns>BitmapImage类型对象</returns>
        public static BitmapImage Convert(Image src)
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

        /// <summary>
        /// 将制定区块图像复制到另一区块，可作为裁图或压缩的方法
        /// </summary>
        /// <param name="source">原图像</param>
        /// <param name="sourceRectangle">原位置</param>
        /// <param name="targetRectangle">目标位置</param>
        /// <returns>目标图像</returns>
        public static Image Copy(Image source, Rectangle sourceRectangle, Rectangle targetRectangle)
        {
            Image image = new Bitmap(targetRectangle.Width, targetRectangle.Height);
            using (Graphics graphics = Graphics.FromImage(image))
            {
                graphics.DrawImage(source, targetRectangle, sourceRectangle, GraphicsUnit.Pixel);
            }
            return image;
        }

    }
}
