using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using GeoDo.RSS.Core.DF;
using System.Windows.Forms;

namespace GeoDo.RSS.MIF.Core
{
    public static class MifEnvironment
    {
        public static string VECTOR_TEMPLATE = AppDomain.CurrentDomain.BaseDirectory + "数据引用";
        //public static string CACHE_DIRECTORY = AppDomain.CurrentDomain.BaseDirectory + "TEMP";
        //private static string WORKSPACE_DIR = AppDomain.CurrentDomain.BaseDirectory + "Workspace";
        public static Dictionary<string, Dictionary<string, string>> CatalogItemCNDic = null;

        private static string _workSpaceDir = null;
        private static string _cacheDir = null;
        private static string _reportDir = null;
        //private static IConfig _config = null;

        static MifEnvironment()
        {
            LoadMifEnvironment();
            //if (!Directory.Exists(CACHE_DIRECTORY))
            //    Directory.CreateDirectory(CACHE_DIRECTORY);
            //
            LoadWorkspaceChinese();
        }

        private static void LoadMifEnvironment()
        {
            MifConfig config = new MifConfig();
            _workSpaceDir = config.GetConfigValue("Workspace");
            _cacheDir = config.GetConfigValue("TEMP");
            _reportDir = config.GetConfigValue("Report");
        }

        private static void LoadWorkspaceChinese()
        {
            Dictionary<string, Dictionary<string, string>> catalogCN = CatalogCNHelper.GetDic();
            if (catalogCN != null)
                MifEnvironment.CatalogItemCNDic = catalogCN;
        }

        public static string GetWorkspaceDir()
        {
            //if (!Directory.Exists(WORKSPACE_DIR))
            //    Directory.CreateDirectory(WORKSPACE_DIR);
            //return WORKSPACE_DIR;
            return _workSpaceDir;
        }

        public static string GetFullFileName(string fname)
        {
            //return Path.Combine(CACHE_DIRECTORY, fname);
            return Path.Combine(_cacheDir, fname);
        }

        public static string GetReportDir()
        {
            return _reportDir;
        }

        public static string GetTempDir()
        {
            return _cacheDir;
        }
    }

    public class ConfigItem
    {
        string ConfigName;
        string ConfigValue;
        string Identify;
    }

}
