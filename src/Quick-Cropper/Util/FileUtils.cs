using System.Collections.Generic;
using System.Configuration;

namespace Kuretru.QuickCropper.Util
{
    static class FileUtils
    {

        static readonly List<string> SUFFIXS = null;

        static FileUtils()
        {
            string suffix = ConfigurationManager.AppSettings["ImageFileSuffix"];
            string[] suffixs = suffix.Split('|');
            SUFFIXS = new List<string>(suffixs);
        }

        /// <summary>
        /// 根据后缀名判断文件是否是图片
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <returns>是否是图片</returns>
        public static bool IsImageFile(string path)
        {
            string suffix = GetSuffix(path);
            if (suffix == "")
            {
                return false;
            }
            if (SUFFIXS.Contains(suffix))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 获取文件后缀名
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <returns>文件后缀名</returns>
        public static string GetSuffix(string path)
        {
            if (path == null || path.Length == 0)
            {
                return "";
            }
            int index = path.LastIndexOf(".");
            if (index == -1)
            {
                return "";
            }
            return path.Substring(index).ToLower();
        }

        /// <summary>
        /// 获取文件名
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <returns>文件名</returns>
        public static string GetFileName(string path)
        {
            if (path == null || path.Length == 0)
            {
                return "";
            }
            int index = path.LastIndexOf("\\");
            if (index == -1)
            {
                return "";
            }
            return path.Substring(index + 1).ToLower();
        }

    }
}
