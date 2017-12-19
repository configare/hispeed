using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using GeoDo.RSS.MIF.Core;

namespace GeoDo.RSS.MIF.Prds.FLD
{
    public class BackWaterFileFinder:FileFinder
    {
        public Dictionary<string, string> argDic = null;
        
        public override string[] Find(string currentRasterFile, ref string extinfo, string argument)
        {
            argDic = new Dictionary<string, string>();
            if (ParseArugment(argument, out argDic))
            {
                DateTime orbitDatetime = DateTime.Now;
                if (GetOrbitDatetime(currentRasterFile, out orbitDatetime))
                {
                    string WorkDir = MifEnvironment.GetWorkspaceDir() + "\\" + "FLD" + "\\";
                    if(argDic.ContainsKey("MONTHS"))
                    {
                        string monthStr = argDic["MONTHS"];
                        int months = 0;
                        DateTime dstDatetime = DateTime.MinValue;
                        if(int.TryParse(monthStr, out months))
                            dstDatetime = orbitDatetime.AddMonths(months);
                        string searchStr = JointFindStr(dstDatetime);
                    }
                    else if(argDic.ContainsKey("DAYS"))
                    {
                        string dayStr = argDic["DAYS"];
                        int days = 0;
                        DateTime dstDatetime = DateTime.MinValue;
                        if (int.TryParse(dayStr, out days))
                            dstDatetime = orbitDatetime.AddDays(days);
                        string searchStr = JointFindStr(dstDatetime);
                    }
                }

            }
            return null;
        }

        private bool GetOrbitDatetime(string currentRasterFile, out DateTime orbitDatetime)
        {
            //无当前文件，无法获取轨道时间，无法查找基于时间的文件
            orbitDatetime = DateTime.Now;
            if (string.IsNullOrEmpty(currentRasterFile))
                return false;
            RasterIdentify ri = null;
            if (!string.IsNullOrEmpty(currentRasterFile) && File.Exists(currentRasterFile))
            {
                ri = new RasterIdentify(currentRasterFile);
                orbitDatetime = ri.OrbitDateTime;
                string searchStr = JointFindStr(orbitDatetime);
            }
            return true;
        }

        private string JointFindStr(DateTime orbitDatetime)
        {
            string result = string.Empty;
            string prdStr = string.Empty;
            prdStr = "*" + "FLD" + "*" + "FLOD";
            if (!argDic.ContainsKey("FORMAT"))
                result = prdStr + "*.*";
            else
            {
                string[] split = argDic["FORMAT"].Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                if (split == null || split.Length == 0)
                    result = prdStr + "*.*";
                else
                {
                    foreach (string format in split)
                        result += prdStr + "*." + format + ";";
                    result = result.Substring(0, result.Length - 1);
                }
            }
            return result;
        }

        private string GetFileCollection(string searchStr, string workDir)
        {
            string[] files = Directory.GetFiles(workDir, searchStr, SearchOption.AllDirectories);
            if (files == null || files.Length == 0)
                return null;
            int length = files.Length;
            string filename = string.Empty;
            filename = Path.GetFileName(files[0]);
            return filename;
        }
    }
}
