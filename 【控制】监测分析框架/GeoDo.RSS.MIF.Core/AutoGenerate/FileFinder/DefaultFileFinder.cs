using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

namespace GeoDo.RSS.MIF.Core
{
    public class DefaultFileFinder : FileFinder
    {
        public Dictionary<string, string> argDic = null;
        private const string regexStr = @"(?<key>\S+)\s*=\s*(?<value>\S*)";
        private Regex _reg = new Regex(regexStr);
        private Dictionary<DateTime, List<string>> fileDic = null;

        /* Prd=BAG,SubPrd=DBLV,Days=10,Format=dat|ldf, */
        public override string[] Find(string currentRasterFile, ref string extinfo, string argument)
        {
            if (!ParseArugment(argument, out argDic))
                return null;
            //无产品、子产品定义，无法定位工作空间
            if (!argDic.ContainsKey("PRD") || !argDic.ContainsKey("SUBPRD"))
                return null;
            DateTime orbitDatetime = DateTime.Now;
            if (!GetOrbitDatetime(currentRasterFile, out orbitDatetime))
                return null;
            string WorkDir = GetWorkspaceDir(argDic["PRD"], argDic["SUBPRD"]);
            string searchStr = JointFindStr(orbitDatetime);
            if (!GetFileCollection(searchStr, WorkDir))
                return null;
            return SiftFiles(orbitDatetime, ref extinfo);
        }

        private string[] SiftFiles(DateTime orbitDatetime, ref string extinfo)
        {
            bool naturalDays = true;
            if (argDic.ContainsKey("NATURALDAYS"))
                naturalDays = bool.Parse(argDic["NATURALDAYS"]);
            DateTime[] region = CalcDatetimeRegion(orbitDatetime, naturalDays, ref extinfo);
            if (region == null)
                return null;
            List<string> result = new List<string>();
            foreach (DateTime dt in fileDic.Keys)
            {
                if (dt < region[0] || dt >= region[1])
                    continue;
                result.AddRange(fileDic[dt]);
            }
            return result.Count == 0 ? null : result.ToArray(); ;
        }

        private DateTime[] CalcDatetimeRegion(DateTime orbitDatetime, bool naturalDays, ref string extinfo)
        {
            DateTime[] region = new DateTime[2];
            if (!argDic.ContainsKey("DAYS"))
                return null;
            int days = int.Parse(argDic["DAYS"]);
            if (naturalDays)
            {
                region[0] = orbitDatetime.AddDays(0 - days).Date;
                region[1] = orbitDatetime;
                extinfo += "_" + region[0].ToString("yyyyMMdd") + "_" + region[1].ToString("yyyyMMdd");
            }
            else
            {
                switch (days)
                {
                    case 7:
                        region[0] = orbitDatetime.AddDays(0 - orbitDatetime.DayOfWeek).Date;
                        region[1] = orbitDatetime.AddDays(7 - orbitDatetime.Day).Date;
                        extinfo += "_" + region[0].ToString("yyyyMMdd") + "_" + region[1].ToString("yyyyMMdd");
                        break;
                    case 10:
                        if (orbitDatetime.Day < 11)
                        {
                            region[0] = orbitDatetime.AddDays(0 - orbitDatetime.Day).Date;
                            region[1] = orbitDatetime.AddDays(11 - orbitDatetime.Day).Date;
                            extinfo += "_FTDS";
                        }
                        else if (orbitDatetime.Day < 21)
                        {
                            region[0] = orbitDatetime.AddDays(11 - orbitDatetime.Day).Date;
                            region[1] = orbitDatetime.AddDays(21 - orbitDatetime.Day).Date;
                            extinfo += "_MTDS";
                        }
                        else
                        {
                            region[0] = orbitDatetime.AddDays(21 - orbitDatetime.Day).Date;
                            region[1] = DateTime.Parse(orbitDatetime.AddMonths(1).ToString("yyyy/MM") + "/01 00:00:00");
                            extinfo += "_LTDS";
                        }
                        break;
                    case 30:
                        region[0] = DateTime.Parse(orbitDatetime.ToString("yyyy/MM/") + "01 00:00:00");
                        region[1] = DateTime.Parse(orbitDatetime.AddMonths(1).ToString("yyyy/MM") + "/01 00:00:00");
                        extinfo += "_" + orbitDatetime.ToString("MM") + "OY";
                        break;
                    case 90:
                        region[0] = DateTime.Parse(orbitDatetime.AddMonths(-2).ToString("yyyy/MM") + "/01 00:00:00");
                        region[1] = DateTime.Parse(orbitDatetime.AddMonths(1).ToString("yyyy/MM") + "/01 00:00:00");
                        if (orbitDatetime.Month <= 2)
                            extinfo += "_SPRI";
                        else if (orbitDatetime.Month <= 5)
                            extinfo += "_SUMM";
                        else if (orbitDatetime.Month <= 8)
                            extinfo += "_AUTU";
                        else if (orbitDatetime.Month <= 11)
                            extinfo += "_WINT";
                        break;
                    case 365:
                        region[0] = DateTime.Parse(orbitDatetime.AddYears(-1).ToString("yyyy") + "/01/01 00:00:00");
                        region[1] = DateTime.Parse(orbitDatetime.AddYears(1).ToString("yyyy") + "/01/01 00:00:00");
                        extinfo += "_" + orbitDatetime.ToString("yyyy");
                        break;
                }
            }
            return region;
        }

        private bool GetFileCollection(string searchStr, string workDir)
        {
            if (fileDic == null)
                fileDic = new Dictionary<DateTime, List<string>>();
            else
                fileDic.Clear();
            string[] files = Directory.GetFiles(workDir, searchStr, SearchOption.AllDirectories);
            if (files == null || files.Length == 0)
                return false;
            int length = files.Length;
            string filename = string.Empty;
            RasterIdentify ri = null;
            for (int i = 0; i < length; i++)
            {
                filename = Path.GetFileName(files[i]);
                ri = new RasterIdentify(files[i]);
                if (!fileDic.ContainsKey(ri.OrbitDateTime))
                    fileDic.Add(ri.OrbitDateTime, new List<string>());
                fileDic[ri.OrbitDateTime].Add(files[i]);
            }
            return true;
        }

        private string JointFindStr(DateTime orbitDatetime)
        {
            string result = string.Empty;
            string prdStr = string.Empty;
            string baseOn = string.Empty;
            if (argDic.ContainsKey("PRD") && argDic.ContainsKey("SUBPRD"))
                prdStr = "*" + argDic["PRD"] + "*" + argDic["SUBPRD"];
            if (argDic.ContainsKey("BASEON"))
                baseOn = "*" + argDic["BASEON"];
            if (!argDic.ContainsKey("FORMAT"))
                result = prdStr + baseOn + "*.*";
            else
            {
                string[] split = argDic["FORMAT"].Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                if (split == null || split.Length == 0)
                    result = prdStr + baseOn + "*.*";
                else
                {
                    foreach (string format in split)
                        result += prdStr + baseOn + "*." + format + ";";
                    result = result.Substring(0, result.Length - 1);
                }
            }
            return result;
        }

        private bool GetOrbitDatetime(string currentRasterFile, out DateTime orbitDatetime)
        {
            //无当前文件，无法获取轨道时间，无法查找基于时间的文件
            orbitDatetime = DateTime.Now;
            if (!argDic.ContainsKey("DAYS") && string.IsNullOrEmpty(currentRasterFile))
                return false;
            RasterIdentify ri = null;
            if (!string.IsNullOrEmpty(currentRasterFile) && File.Exists(currentRasterFile))
            {
                ri = new RasterIdentify(currentRasterFile);
                orbitDatetime = ri.OrbitDateTime;
            }
            return true;
        }
    }
}
