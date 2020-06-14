using Kuretru.QuickCropper.Entity;
using Kuretru.QuickCropper.Factory;
using Kuretru.QuickCropper.Util;
using System.Collections.Generic;
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

        private WorkProgress workProgress = new WorkProgress();
        private ImageSize targetImageSize = new ImageSize(96, 96);
        private List<string> imageFiles = null;

        public MainWindow()
        {
            InitializeComponent();
            InitializeDataContext();
            InitializeControl();
        }

        private void LoadNextImage()
        {
            BitmapImage image = new BitmapImage(new System.Uri(imageFiles[workProgress.Step]));
            cropTool.SetImage(image);
            targetImage.Source = image;
            workProgress.Step++;
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

                EnableControlPanel(true);
                LoadNextImage();
            }
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if (workProgress.Step >= workProgress.Total)
            {
                System.Windows.Forms.MessageBox.Show("所有图片已裁切完毕。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            LoadNextImage();
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

            workProgress.Step = 0;
            workProgress.Total = enabled ? imageFiles.Count : 0;
        }

    }
}
