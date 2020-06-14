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

        static string GetSuffix(string path)
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

    }
}
