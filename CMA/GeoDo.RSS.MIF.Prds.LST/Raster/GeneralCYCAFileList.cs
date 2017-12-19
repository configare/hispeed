using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.Core.DF;

namespace GeoDo.RSS.MIF.Prds.LST
{
    public class GeneralCYCAFileList
    {
        /*
        *分卫星 传感器
        *日信息：卫星_传感器_年_季_月_旬_日_时间起_时间止
        *旬信息: 卫星_传感器_年_季_月_旬_时间起_时间止
        *月信息：卫星_传感器_年_季_月_时间起_时间止
        *季信息：卫星_传感器_年_季_时间起_时间止
        *年信息：卫星_传感器_年_时间起_时间止
        *
        *周信息：卫星_传感器_年_周_时间起_时间止
        */
        public static CYCAFileListInfo GeneralFileList(string[] files, RegionArg args, SubProductDef subPro, string outSubProIdentify)
        {
            CYCAFileListInfo result = new CYCAFileListInfo();
            GetMaxCyctime(args, ref result);
            if (result.Days001Files == null)
                return null;
            DataIdentify di = null;
            string fileInfo = string.Empty;
            RasterIdentify ri = null;
            foreach (string file in files)
            {
                di = DataIdentifyMatcher.Match(file);
                string days001file = AddDays001FilesInfo(ref result, di, args, subPro, file, out ri, out fileInfo, outSubProIdentify);
                if (result.Days007Files != null && !string.IsNullOrEmpty(days001file))
                    AddDays007FilesInfo(ref result, ri, args, subPro, days001file, fileInfo);
                string days010file = string.Empty;
                if (result.Days010Files != null && !string.IsNullOrEmpty(days001file))
                    days010file = AddDays010FilesInfo(ref result, ri, args, subPro, days001file, ref fileInfo);
                string days030file = string.Empty;
                if (result.Days030Files != null && !string.IsNullOrEmpty(days010file))
                    days030file = AddDays030FilesInfo(ref result, ri, args, subPro, days010file, ref fileInfo);
                string days090file = string.Empty;
                if (result.Days090Files != null && !string.IsNullOrEmpty(days030file))
                    days090file = AddDays090FilesInfo(ref result, ri, args, subPro, days030file, ref fileInfo);
                string days365file = string.Empty;
                if (result.Days365Files != null && !string.IsNullOrEmpty(days090file))
                    days365file = AddDays365FilesInfo(ref result, ri, args, subPro, days090file, ref fileInfo);
            }
            return result;
        }

        private static string AddDays007FilesInfo(ref CYCAFileListInfo result, RasterIdentify ri, RegionArg args, SubProductDef subPro, string days001file, string fileInfo)
        {
            string days10FLag = GetDays010(ri.OrbitDateTime.AddHours(8).Day) + "_";
            string dayFlag = ri.OrbitDateTime.AddHours(8).Day + "_";
            fileInfo = fileInfo.Replace("_" + days10FLag + dayFlag, "_" + WeekOfYear(ri.OrbitDateTime.AddHours(8)) + "_");
            if (result.Days007Files.ContainsKey(fileInfo))
                result.Days007Files[fileInfo].Add(days001file);
            else
            {
                List<string> files = new List<string>();
                files.Add(days001file);
                result.Days007Files.Add(fileInfo, files);
                ri.CYCFlag = args.CycType == enumProcessType.AVG ? "AOSD" : args.CycType == enumProcessType.MAX ? "MASD" : args.CycType == enumProcessType.MIN ? "MNSD" : "POSD";
                string outFilename = ri.ToWksFullFileName(ri.Format);
                result.OutFilename.Add(fileInfo, outFilename);
                return outFilename;
            }
            return string.Empty;
        }

        private static string AddDays365FilesInfo(ref CYCAFileListInfo result, RasterIdentify ri, RegionArg args, SubProductDef subPro, string days090file, ref string fileInfo)
        {
            string days90FLag = "_" + GetSeason(ri.OrbitDateTime.AddHours(8).Month) + "_";
            fileInfo = fileInfo.Replace(days90FLag, "_");
            if (result.Days365Files.ContainsKey(fileInfo))
                result.Days365Files[fileInfo].Add(days090file);
            else
            {
                List<string> files = new List<string>();
                files.Add(days090file);
                result.Days365Files.Add(fileInfo, files);
                ri.CYCFlag = args.CycType == enumProcessType.AVG ? "AOAY" : args.CycType == enumProcessType.MAX ? "MAAY" : args.CycType == enumProcessType.MIN ? "MNAY" : "POAY";
                string outFilename = ri.ToWksFullFileName(ri.Format);
                result.OutFilename.Add(fileInfo, outFilename);
                return outFilename;
            }
            return string.Empty;
        }

        private static string AddDays090FilesInfo(ref CYCAFileListInfo result, RasterIdentify ri, RegionArg args, SubProductDef subPro, string days030file, ref string fileInfo)
        {
            string days30FLag = "_" + ri.OrbitDateTime.AddHours(8).Month + "_";
            fileInfo = fileInfo.Replace(days30FLag, "_");
            if (result.Days090Files.ContainsKey(fileInfo))
                result.Days090Files[fileInfo].Add(days030file);
            else
            {
                List<string> files = new List<string>();
                files.Add(days030file);
                result.Days090Files.Add(fileInfo, files);
                ri.CYCFlag = args.CycType == enumProcessType.AVG ? "AOAQ" : args.CycType == enumProcessType.MAX ? "MAAQ" : args.CycType == enumProcessType.MIN ? "MNAQ" : "POAQ";
                string outFilename = ri.ToWksFullFileName(ri.Format);
                result.OutFilename.Add(fileInfo, outFilename);
                return outFilename;
            }
            return string.Empty;
        }

        private static string AddDays030FilesInfo(ref CYCAFileListInfo result, RasterIdentify ri, RegionArg args, SubProductDef subPro, string days010file, ref string fileInfo)
        {
            string days10FLag = "_" + GetDays010(ri.OrbitDateTime.AddHours(8).Day) + "_";
            fileInfo = fileInfo.Replace(days10FLag, "_");
            if (result.Days030Files.ContainsKey(fileInfo))
                result.Days030Files[fileInfo].Add(days010file);
            else
            {
                List<string> files = new List<string>();
                files.Add(days010file);
                result.Days030Files.Add(fileInfo, files);
                ri.CYCFlag = args.CycType == enumProcessType.AVG ? "AOAM" : args.CycType == enumProcessType.MAX ? "MAAM" : args.CycType == enumProcessType.MIN ? "MNAM" : "POAM";
                string outFilename = ri.ToWksFullFileName(ri.Format);
                result.OutFilename.Add(fileInfo, outFilename);
                return outFilename;
            }
            return string.Empty;
        }

        private static string AddDays010FilesInfo(ref CYCAFileListInfo result, RasterIdentify ri, RegionArg args, SubProductDef subPro, string days001file, ref string fileInfo)
        {
            string days10FLag = GetDays010(ri.OrbitDateTime.AddHours(8).Day) + "_";
            string dayFlag = ri.OrbitDateTime.AddHours(8).Day + "_";
            fileInfo = fileInfo.Replace("_" + days10FLag + dayFlag, "_" + days10FLag);
            if (result.Days010Files.ContainsKey(fileInfo))
                result.Days010Files[fileInfo].Add(days001file);
            else
            {
                List<string> files = new List<string>();
                files.Add(days001file);
                result.Days010Files.Add(fileInfo, files);
                ri.CYCFlag = args.CycType == enumProcessType.AVG ? "AOTD" : args.CycType == enumProcessType.MAX ? "MATD" : args.CycType == enumProcessType.MIN ? "MNTD" : "POTD";
                string outFilename = ri.ToWksFullFileName(ri.Format);
                result.OutFilename.Add(fileInfo, outFilename);
                return outFilename;
            }
            return string.Empty;
        }

        private static string AddDays001FilesInfo(ref CYCAFileListInfo result, DataIdentify di, RegionArg args, SubProductDef subPro, string file, out  RasterIdentify ri, out string fileInfo, string outSubProIdnetify)
        {
            fileInfo = null;
            ri = null;
            if (args.CKSatellite)
                fileInfo = string.Format("{0}_{1}_{2}_{3}_{4}_{5}_{6}{7}", di.Satellite, di.Sensor, di.OrbitDateTime.AddHours(8).Year, GetSeason(di.OrbitDateTime.AddHours(8).Month),
                    di.OrbitDateTime.AddHours(8).Month, GetDays010(di.OrbitDateTime.AddHours(8).Day), di.OrbitDateTime.AddHours(8).Day, GetTimeRegion(args, di.OrbitDateTime.AddHours(8).ToString("HHdd")));
            else
                fileInfo = string.Format("{0}_{1}_{2}_{3}_{4}{5}", di.OrbitDateTime.AddHours(8).Year, GetSeason(di.OrbitDateTime.AddHours(8).Month),
                    di.OrbitDateTime.AddHours(8).Month, GetDays010(di.OrbitDateTime.AddHours(8).Day), di.OrbitDateTime.AddHours(8).Day, GetTimeRegion(args, di.OrbitDateTime.AddHours(8).ToString("HHdd")));
            if (result.Days001Files.ContainsKey(fileInfo))
                result.Days001Files[fileInfo].Add(file);
            else
            {
                List<string> files = new List<string>();
                files.Add(file);
                result.Days001Files.Add(fileInfo, files);
                string exInfo = args.CycType == enumProcessType.AVG ? "AOAD" : args.CycType == enumProcessType.MAX ? "MAAD" : args.CycType == enumProcessType.MIN ? "MNAD" : "POAD";
                string outFilename = GetResultFilename(out ri, file, di, subPro, exInfo, outSubProIdnetify);
                result.OutFilename.Add(fileInfo, outFilename);
                return outFilename;
            }
            return string.Empty;
        }

        private static string GetResultFilename(out RasterIdentify ri, string filenanme, DataIdentify di, SubProductDef subPro, string cycFlag, string outSubProIdnetify)
        {
            ri = new RasterIdentify(filenanme);
            ri.CYCFlag = cycFlag;
            ri.Format = ".dat";
            ri.GenerateDateTime = DateTime.Now;
            ri.IsOutput2WorkspaceDir = true;
            //ri.OrbitDateTime = di.OrbitDateTime;
            ri.ProductIdentify = subPro.ProductDef.Identify;
            //ri.Satellite = di.Satellite;
            //ri.Sensor = di.Sensor;
            ri.SubProductIdentify = string.IsNullOrEmpty(outSubProIdnetify) ? subPro.Identify : outSubProIdnetify;
            ri.ThemeIdentify = "CMA";
            return ri.ToWksFullFileName(ri.Format);
        }

        private static string GetTimeRegion(RegionArg args, string time)
        {
            if (!args.CKTime || args.TimeRegion == null || args.TimeRegion.Count == 0)
                return "_0000_2400";

            foreach (string key in args.TimeRegion.Keys)
            {
                if (time.CompareTo(key) < 0 || time.CompareTo(args.TimeRegion[key]) >= 0)
                    continue;
                return "_" + key + "_" + args.TimeRegion[key];
            }
            return "_0000_2400";
        }

        /// <summary>
        /// 获取该年中是第几周
        /// </summary>
        /// <param name="day">日期</param>
        /// <returns></returns>
        private static int WeekOfYear(DateTime day)
        {
            int weeknum;
            DateTime fDt = DateTime.Parse(day.Year.ToString() + "-01-01");
            int k = Convert.ToInt32(fDt.DayOfWeek);//得到该年的第一天是周几 
            if (k == 0)
            {
                k = 7;
            }
            int l = Convert.ToInt32(day.DayOfYear);//得到当天是该年的第几天 
            l = l - (7 - k + 1);
            if (l <= 0)
            {
                weeknum = 1;
            }
            else
            {
                if (l % 7 == 0)
                {
                    weeknum = l / 7 + 1;
                }
                else
                {
                    weeknum = l / 7 + 2;//不能整除的时候要加上前面的一周和后面的一周 
                }
            }
            return weeknum;
        }

        private static string GetDays010(int day)
        {
            if (day > 20)
                return "last";
            else if (day > 10)
                return "midd";
            else
                return "frist";
        }

        private static string GetSeason(int month)
        {
            if (month >= 11 || month == 1)
                return "winter";
            else if (month >= 8)
                return "autumn";
            else if (month >= 5)
                return "summer";
            else
                return "spring";
        }

        private static void GetMaxCyctime(RegionArg args, ref CYCAFileListInfo result)
        {
            if (args.CK365Days)
            {
                result.Days365Files = new Dictionary<string, List<string>>();
                if (result.Days090Files == null)
                    result.Days090Files = new Dictionary<string, List<string>>();
                if (result.Days030Files == null)
                    result.Days030Files = new Dictionary<string, List<string>>();
                if (result.Days010Files == null)
                    result.Days010Files = new Dictionary<string, List<string>>();
                if (result.Days001Files == null)
                    result.Days001Files = new Dictionary<string, List<string>>();
            }
            if (args.CK090Days)
            {
                result.Days090Files = new Dictionary<string, List<string>>();
                if (result.Days030Files == null)
                    result.Days030Files = new Dictionary<string, List<string>>();
                if (result.Days010Files == null)
                    result.Days010Files = new Dictionary<string, List<string>>();
                if (result.Days001Files == null)
                    result.Days001Files = new Dictionary<string, List<string>>();
            }
            if (args.CK030Days)
            {
                result.Days030Files = new Dictionary<string, List<string>>();
                if (result.Days010Files == null)
                    result.Days010Files = new Dictionary<string, List<string>>();
                if (result.Days001Files == null)
                    result.Days001Files = new Dictionary<string, List<string>>();
            }
            if (args.CK010Days)
            {
                result.Days010Files = new Dictionary<string, List<string>>();
                if (result.Days001Files == null)
                    result.Days001Files = new Dictionary<string, List<string>>();
            }
            if (args.CK007Days)
            {
                result.Days007Files = new Dictionary<string, List<string>>();
                if (result.Days001Files == null)
                    result.Days001Files = new Dictionary<string, List<string>>();
            }
            if (args.CK001Days)
                result.Days001Files = new Dictionary<string, List<string>>();
        }
    }

    public class CYCAFileListInfo
    {
        public Dictionary<string, List<string>> Days365Files = null;
        public Dictionary<string, List<string>> Days090Files = null;
        public Dictionary<string, List<string>> Days030Files = null;
        public Dictionary<string, List<string>> Days010Files = null;
        public Dictionary<string, List<string>> Days007Files = null;
        public Dictionary<string, List<string>> Days001Files = null;

        public Dictionary<string, string> OutFilename = new Dictionary<string, string>();

        public CYCAFileListInfo()
        { }
    }
}
