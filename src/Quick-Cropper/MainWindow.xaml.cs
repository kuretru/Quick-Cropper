using CroppingImageLibrary.Services;
using Kuretru.QuickCropper.Entity;
using Kuretru.QuickCropper.Factory;
using Kuretru.QuickCropper.Util;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media.Imaging;

namespace Kuretru.QuickCropper
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {

        private readonly WorkProgress workProgress = new WorkProgress();
        private readonly ImageSize targetImageSize = new ImageSize(96, 96);
        private List<string> imageFiles = null;
        private Image currentBitmap;
        private Image targetBitmap;
        private string saveFolder = "";

        public MainWindow()
        {
            InitializeComponent();
            InitializeDataContext();
            InitializeControl();
        }

        /// <summary>
        /// 载入下一张图片
        /// </summary>
        private void LoadNextImage()
        {
            string imageFile = imageFiles[workProgress.Step];
            BitmapImage image = new BitmapImage(new Uri(imageFile));

            cropTool.SetImage(image);
            imageNameLabel.Content = FileUtils.GetFileName(imageFile);
            currentBitmap = Image.FromFile(imageFile);

            workProgress.Step++;
            //SetDefaultCropPosition();
        }

        /// <summary>
        /// 保存当前图片
        /// </summary>
        private void SaveImage()
        {

            string path = saveFolder + imageNameLabel.Content;
            targetBitmap.Save(path, ImageFormat.Png);
        }

        /// <summary>
        /// 设置裁切框默认位置
        /// </summary>
        /// <returns></returns>
        private void SetDefaultCropPosition()
        {
            double width, height;

            var imageSize = cropTool.GetImageRenderSize();
            double imageAspectRatio = imageSize.Width / imageSize.Height;
            double targetAspectRatio = targetImageSize.Width / targetImageSize.Height;
            if (targetAspectRatio < imageAspectRatio)
            {
                // 先计算高度，再根据比例计算宽度
                height = imageSize.Height;
                width = height * targetAspectRatio;
            }
            else
            {
                // 先计算宽度，在根据比例计算高度
                width = imageSize.Width;
                height = width / targetAspectRatio;
            }

            double left = (cropTool.RenderSize.Width - width) / 2;
            double top = (cropTool.RenderSize.Height - height) / 2;

            cropTool.CropService.SetCropArea(left, top, width, height);
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            using (FolderBrowserDialog folderBrowserDialog = FolderBrowserDialogFactory.Default())
            {
                DialogResult dialogResult = folderBrowserDialog.ShowDialog();
                if (dialogResult != System.Windows.Forms.DialogResult.OK)
                {
                    return;
                }

                string imageFolderPath = folderBrowserDialog.SelectedPath;
                string[] imageFolderFiles = Directory.GetFiles(imageFolderPath);
                imageFiles = new List<string>();
                foreach (string imageFile in imageFolderFiles)
                {
                    if (FileUtils.IsImageFile(imageFile))
                    {
                        imageFiles.Add(imageFile);
                    }
                }

                if (imageFiles.Count == 0)
                {
                    System.Windows.Forms.MessageBox.Show("在当前目录下没有找到图片文件，请重新选择文件夹，或更改图片后缀名。",
                        "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                saveFolder = imageFolderPath + "\\cropped\\";
                if (!Directory.Exists(saveFolder))
                {
                    Directory.CreateDirectory(saveFolder);
                }

                EnableControlPanel(true);
                LoadNextImage();
            }
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            SaveImage();
            if (workProgress.Step >= workProgress.Total)
            {
                System.Windows.Forms.MessageBox.Show("所有图片已裁切完毕。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            LoadNextImage();
        }

        private void ComputeButton_Click(object sender, RoutedEventArgs e)
        {
            SetDefaultCropPosition();
        }

        private void CropTool_CroppedAreaChanged(object sender)
        {
            if (currentBitmap == null)
            { return; }

            CropArea cropArea = cropTool.CropService.GetCroppedArea();

            if (cropArea.CroppedRectAbsolute.Width == 0 || cropArea.CroppedRectAbsolute.Height == 0)
            { return; }

            var renderSize = cropTool.GetImageRenderSize();
            double proportion = (renderSize.Width / currentBitmap.Width + renderSize.Height / currentBitmap.Height) / 2;

            int x = (int)((cropArea.CroppedRectAbsolute.X - (cropTool.ActualWidth - renderSize.Width) / 2) / proportion);
            int y = (int)((cropArea.CroppedRectAbsolute.Y - (cropTool.ActualHeight - renderSize.Height) / 2) / proportion);
            int width = (int)(cropArea.CroppedRectAbsolute.Width / proportion);
            int height = (int)(cropArea.CroppedRectAbsolute.Height / proportion);

            Rectangle cropRectangle = new Rectangle(x, y, width, height);
            Rectangle targetRectangle = new Rectangle(0, 0, width, height);
            targetBitmap = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(targetBitmap))
            {
                g.DrawImage(currentBitmap, targetRectangle, cropRectangle, GraphicsUnit.Pixel);
            }

            targetImage.Source = ImageUtils.Convert(targetBitmap);
        }

        /// <summary>
        /// 初始化数据绑定
        /// </summary>
        private void InitializeDataContext()
        {
            progressStackPanel.DataContext = workProgress;
            targetImageSizeStackPanel.DataContext = targetImageSize;
        }

        /// <summary>
        /// 初始化控件
        /// </summary>
        private void InitializeControl()
        {
            cropTool.CroppedAreaChangedEvent += CropTool_CroppedAreaChanged;
            imageNameLabel.Content = "";
            EnableControlPanel(false);
        }

        /// <summary>
        /// 初始化控制面板
        /// </summary>
        /// <param name="enabled">是否启用</param>
        private void EnableControlPanel(bool enabled)
        {
            targetImageWidthTextBox.IsEnabled = enabled;
            targetImageHeightTextBox.IsEnabled = enabled;
            nextButton.IsEnabled = enabled;
            computeButton.IsEnabled = enabled;

            workProgress.Step = 0;
            workProgress.Total = enabled ? imageFiles.Count : 0;
        }
    }
}
