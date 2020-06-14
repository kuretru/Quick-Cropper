using System;
using System.Windows.Forms;

namespace Kuretru.QuickCropper.Factory
{
    static class FolderBrowserDialogFactory
    {

        public static FolderBrowserDialog Default()
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog
            {
                RootFolder = Environment.SpecialFolder.MyComputer,
                Description = "请选择图片数据集存放的文件夹。",
                ShowNewFolderButton = false
            };
            return folderBrowserDialog;
        }

    }
}
