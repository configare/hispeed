using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Core;
using System.IO;

namespace GeoDo.RSS.MIF.Prds.FOG
{
    public static class GetFileByDirArg
    {
        public static string GetFileBySattileSensor(IArgumentProvider argumentProvider, string dirArgName, string defDirArgName, string baseFile)
        {
            string dir;
            string filter;
            RasterIdentify rid = null;
            if (!string.IsNullOrEmpty(baseFile) && File.Exists(baseFile))
                rid = new RasterIdentify(baseFile);
            GetDirFilter(argumentProvider, dirArgName, out dir, out filter, rid);
            string serverdir = GetServerDir(dir, rid);
            if (serverdir == null)
            {
                GetDirFilter(argumentProvider, defDirArgName, out dir, out filter, rid);
            }
            else
            {
                dir = serverdir;
            }
            if (!Directory.Exists(dir))
                return null;
            string csrfile = null;
            string[] files = null;
            if (string.IsNullOrEmpty(filter))
                files = Directory.GetFiles(dir, "*.*", SearchOption.AllDirectories);
            else
                files = Directory.GetFiles(dir, filter, SearchOption.AllDirectories);
            int length = files.Length;
            if (files != null || length != 0)
            {
                if (rid == null)
                    csrfile = files[length - 1];
                else
                {
                    if (string.IsNullOrEmpty(rid.Satellite) && string.IsNullOrEmpty(rid.Sensor))
                        csrfile = files[length - 1];
                    else if (!string.IsNullOrEmpty(rid.Satellite) && !string.IsNullOrEmpty(rid.Sensor))
                        csrfile = GetFileByKey(rid.Satellite, rid.Sensor, files);
                    else if (!string.IsNullOrEmpty(rid.Satellite))
                        csrfile = GetFileByKey(rid.Satellite, files);
                    else if (!string.IsNullOrEmpty(rid.Sensor))
                        csrfile = GetFileByKey(rid.Sensor, files);
                }
            }
            csrfile = FileUsedProcessor(csrfile, files, length);
            return csrfile;
        }
        private static string GetServerDir(string currentdir, RasterIdentify rid)
        {
            string parentdir = currentdir.Remove(currentdir.Length - 8-2);//2代表前后斜杠
           //服务器只有VIRRX文件夹 将VIRR载荷类型文件指向VIRRX文件夹位置
            if (parentdir.Contains("VIRR"))
            {
                parentdir = parentdir.Replace("VIRR", "VIRRX");
            }
           
            if (Directory.Exists(parentdir))
            {
                DirectoryInfo parent = new DirectoryInfo(parentdir);
                int i=2;
                //子文件夹
                int filecount=parent.GetDirectories().Length;
                while (!Directory.Exists(currentdir))
                {
                    if (i-2 >= filecount)
                    {
                        return null;
                    }
                    else
                    {
                        //currentdir = parentdir + "\\" + rid.OrbitDateTime.AddYears(1).AddMonths(7).AddDays(-i).ToString("yyyyMMdd");//测试数据
                        currentdir = parentdir + "\\" + rid.OrbitDateTime.AddDays(-i).ToString("yyyyMMdd");
                        i++;
                    }
                }
                return currentdir;
            }
            else
            {
                return null;
            }
        }

        private static string FileUsedProcessor(string csrfile, string[] files, int length)
        {
            if (string.IsNullOrEmpty(csrfile))
            {
                csrfile = files[length - 1];
                try
                {
                    using (FileStream fs = new FileStream(csrfile, FileMode.Open, FileAccess.Read))
                    {

                    }
                }
                catch
                { }
                finally
                {
                    if(length>=2)
                    csrfile = files[length - 2];
                }
            }
            return csrfile;
        }
        private static void GetDirFilter(IArgumentProvider argumentProvider, string dirArgName, out string dir, out string filter, RasterIdentify rid)
        {
            dir = argumentProvider.GetArg(dirArgName).ToString();
            filter = null;
            int index = dir.IndexOf("filter:");
            if (index != -1)
            {
                filter = dir.Substring(index + 7);
                dir = dir.Substring(0, dir.IndexOf("filter:"));
                if (dir.IndexOf("{satellite}") != -1)
                    dir = dir.Replace("{satellite}", rid.Satellite);
                if (dir.IndexOf("{sensor}") != -1)
                    dir = dir.Replace("{sensor}", rid.Sensor);
                if (dir.IndexOf("datetimedir") != -1)
                    dir = dir.Replace("datetimedir", rid.OrbitDateTime.AddDays(-1).ToString("yyyyMMdd"));
            }
            if (dir.StartsWith("SystemData"))
                dir = AppDomain.CurrentDomain.BaseDirectory.ToUpper() + dir;
        }
        private static string GetFileByKey(string key, string[] files)
        {
            foreach (string file in files)
            {
                if (file.IndexOf(key) != -1)
                    return file;
            }
            return null;
        }
        private static string GetFileByKey(string key1, string key2, string[] files)
        {
            foreach (string file in files)
            {
                if (file.IndexOf(key1) != -1 && file.IndexOf(key2) != -1)
                    return file;
            }
            return null;
        }
    }
}
