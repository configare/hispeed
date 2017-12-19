using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GeoDo.RSS.MIF.Core
{
    public class GlobalWorkspace
    {
        static GlobalWorkspace()
        {
            //InitLog();
        }

        #region 日志
        //private static string LogFileName = string.Empty;

        private static void InitLog()
        {
            //string wkddir = MifEnvironment.GetWorkspaceDir();
            //string logFileName = string.Format("FLOG_{0}{1}", DateTime.Now.ToString("yyyyMMddHHmmssfffffff"), ".log");
            //string dir = Path.Combine(wkddir, "Log");
            //if (!Directory.Exists(dir))
            //    Directory.CreateDirectory(dir);
            //LogFileName = Path.Combine(dir, logFileName);
        }
        private static StringBuilder stringBuilder = new StringBuilder(1024);
        private static object lockobj = new object();

        public class LogItem
        {
            public LogItem()
            {
                DateTime = DateTime.Now;
            }
            public string Message { get; set; }
            public DateTime DateTime { get; set; }
        }
        public class LogItemCollection : List<LogItem>
        {
            public LogItemCollection():base(5000)
            {
                
            }

            public StringBuilder ToStringBuilder()
            {
                StringBuilder stringBuilder = new StringBuilder(this.Count);
                //foreach (LogItem item in this)
                //for (int i=1; i<Count-1; i++)
                //{
                //    LogItem item = this[i];

                //    TimeSpan timeSpan;
                //    if (i == 0) timeSpan = DateTime.Now - item.DateTime;
                //    else timeSpan = this[i+1].DateTime - this[i].DateTime;
                //    string info = string.Format("间隔：,{2},时间：,{0},{1}",
                //                                           item.DateTime.ToString("yyyy-MM-dd HH:mm:ss fffffff"),
                //                                           item.Message, timeSpan.TotalSeconds.ToString("##.0000000"));
                //    stringBuilder.AppendLine(info);
                //}
                for (int i = 0; i < Count; i++)
                {
                    LogItem item = this[i];
                    string info = string.Format("{2},时间：,{0},{1}",
                        item.DateTime.ToString("yyyy-MM-dd HH:mm:ss fffffff"),
                        item.Message, i);
                    stringBuilder.AppendLine(info);
                }
                return stringBuilder;
            }
        }

        static LogItemCollection logItems = new LogItemCollection();

        //public static void SaveLog(string sMessageInfo)
        //{
        //    if (isStop) return;
        //    logItems.Add(new LogItem{Message = sMessageInfo});
        //    //string info = string.Format("时间： {0}\t{1}\r\n",
        //    //                                       DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fffffff"),
        //    //                                       sMessageInfo);
        //    //stringBuilder.AppendLine(info);
        //    if (logItems.Count > 10000)
        //    {
        //        isStop = true;
        //        EndSaveLog();
        //    }
        //}

        private static bool isStop = false;
        //public static void EndSaveLog()
        //{
        //    string wkddir = MifEnvironment.GetWorkspaceDir();
        //    string logFileName = string.Format("FLOG_{0}{1}", DateTime.Now.ToString("yyyyMMddHHmmssfffffff"), ".log");
        //    string dir = Path.Combine(wkddir, "Log");
        //    if (!Directory.Exists(dir))
        //        Directory.CreateDirectory(dir);
        //    var logFileName2 = Path.Combine(dir, logFileName);

        //    logItems.Insert(0, new LogItem());

        //    SaveLog(logItems.ToStringBuilder(), logFileName2);
        //}

        //public static void SaveLog(StringBuilder sBuilder, string sFileName)
        //{
        //    SaveLog(sBuilder.ToString(), sFileName);
        //}


        /// <summary>
        /// 日志保存
        /// </summary>
        /// <param name="sMessageInfo"></param>
        /// <param name="sFileName"></param>
        //public static void SaveLog(string sMessageInfo, string sFileName)
        //{
        //    lock (lockobj)
        //    {
        //        bool append = true;
        //        if (File.Exists(sFileName))
        //        {
        //            long fileLengthByte = new FileInfo(sFileName).Length;
        //            double fileLengthK = fileLengthByte / 1024.0;
        //            double fileLengthM = fileLengthByte / 1048576.0; // 1024 * 1024
        //            double fileLengthG = fileLengthByte / 1073741824.0; // (1024 * 1024 * 1024);

        //            if (fileLengthM >= 3.0d)
        //                append = false;
        //        }

        //        StreamWriter myWriter = null;
        //        try
        //        {
        //            myWriter = new StreamWriter(sFileName, append, Encoding.Default);
        //            append = true;
        //            //string info = string.Format("时间： {0}\r\n{1}\r\n\r\n",
        //            //                            DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fffffff"),
        //            //                            sMessageInfo);
        //            myWriter.Write(sMessageInfo);
        //        }
        //        catch
        //        {
        //        }
        //        finally
        //        {
        //            if (myWriter != null)
        //            {
        //                myWriter.Close();
        //            }
        //        }
        //    }
        //}

        #endregion 日志

        private static Dictionary<string,WorkspaceInfo> _dicWorkspaceInfos = new Dictionary<string, WorkspaceInfo>();

        //public static CatalogItem GetWorkspaceInfoBySourceFileName(string sourceFileName)
        //{
        //    string wkddir = MifEnvironment.GetWorkspaceDir();
        //    var filetypetmp = sourceFileName.Replace(wkddir, "");
        //    var filetypes = filetypetmp.Split(new char[]{'\\'}, StringSplitOptions.RemoveEmptyEntries);
        //    var filetype = filetypes[0];
        //    WorkspaceInfo workspaceinfo;
        //    if (_dicWorkspaceInfos.ContainsKey(filetype))
        //        workspaceinfo = _dicWorkspaceInfos[filetype];
        //    else
        //    {
        //        workspaceinfo = GetWorkspaceinfo(wkddir, filetype);
        //        _dicWorkspaceInfos.Add(filetype, workspaceinfo);
        //    }
        //    var catalogItem = workspaceinfo.GetBySourceFileName(sourceFileName);
        //    return catalogItem;
        //}

        private static WorkspaceInfo GetWorkspaceinfo(string dir, string type)
        {
            string wkddir = dir; ;
            string filename = string.Format("WorkspaceInfo_{0}.xml", type);
            var file = Path.Combine(wkddir, type, filename);
            WorkspaceInfo workspaceInfo;
            if (!File.Exists(file))
                workspaceInfo = CreateWorkspaceInfo(file);
            else
                workspaceInfo = GetWorkspaceInfo(file);

            if (workspaceInfo.ValidateFileExists())
                SaveWorkspace(file, workspaceInfo);

            return workspaceInfo;
        }

        private static WorkspaceInfo GetWorkspaceInfo(string file)
        {
            var strWorkspaceInfo = UIO.ReadFile(file);
            WorkspaceInfo workspaceInfo = UXmlConvert.GetObject<WorkspaceInfo>(strWorkspaceInfo);

            DateTime cdt = workspaceInfo.SaveTime;

            var dir = Path.GetDirectoryName(file);
            string[] files = Directory.GetFiles(dir, "*", SearchOption.AllDirectories);
            List<string> newFiles = new List<string>();
            foreach (string f in files)
            {
                if (f == file) continue;
                DateTime dt = File.GetCreationTime(f);
                if (dt > cdt)
                {
                    newFiles.Add(f);
                }
            }
            if(newFiles.Count!=0)
                CreateWorkspaceInfo(file, newFiles, workspaceInfo);
            return workspaceInfo;
        }

        private static WorkspaceInfo CreateWorkspaceInfo(string filename, List<string> newFiles = null, WorkspaceInfo workspaceInfoRef = null)
        {
            var wks = workspaceInfoRef ?? new WorkspaceInfo();

            wks.SaveTime = DateTime.Now;
            var datinfos = new SDatinfos();
            wks.Datinfos = datinfos;

            var dir = Path.GetDirectoryName(filename);

            if (!Directory.Exists(dir))
                return wks;

            var workspaceDefs = WorkspaceDefinitionFactory.WorkspaceDefs;
            foreach (var workspaceDef in workspaceDefs)
            {
                foreach (var catalogDef in workspaceDef.CatalogDefs)
                {
                    var def = catalogDef as SubProductCatalogDef;
                    if (def == null) 
                        continue;
                    string[] fnames = GetFiles(dir, def.Identify, def.Pattern);
                    if (fnames == null || fnames.Length == 0)
                        continue;

                    foreach (var fname in fnames)
                    {
                        if (newFiles != null && !newFiles.Contains(fname))
                            continue;

                        var datinfo = CreateDatinfo(fname, def);
                        if(datinfo != null)
                            datinfos.AddDatinfo(datinfo);
                    }
                }
            }

            SaveWorkspace(filename, wks);
            return wks;
        }

        private static void SaveWorkspace(string filename, WorkspaceInfo workspaceInfo)
        {
            var str = UXmlConvert.GetString(workspaceInfo);
            UIO.SaveFile(str, filename);
        }

        private static SDatinfo CreateDatinfo(string fname, SubProductCatalogDef catalogDef)
        {
            string infoFileName = GetInfoFileName(fname);
            CatalogItemInfo ctl;
            if (!File.Exists(infoFileName))
                ctl = TryCreateInfoFromMainFile(fname, infoFileName, catalogDef);
            else
            {
                ctl = new CatalogItemInfo(infoFileName);
                if (!ctl.Properties.ContainsKey("CatalogDef") ||
                    string.IsNullOrEmpty(ctl.Properties["CatalogDef"].ToString()) ||
                    !ctl.Properties.ContainsKey("CycFlagCN"))
                    ctl = TryCreateInfoFromMainFile(fname, infoFileName, catalogDef);
            }
            if (ctl == null)
                ctl = new CatalogItemInfo(infoFileName);
            var datinfo = new SDatinfo
            {
                CatalogDef = ctl.GetPropertyValue("CatalogDef"),
                CatalogItemCN = ctl.GetPropertyValue("CatalogItemCN"),
                CycFlagCN = ctl.GetPropertyValue("CycFlagCN"),
                ExtInfos = ctl.GetPropertyValue("ExtInfos"),
                FileDir = ctl.GetPropertyValue("FileDir"),
                FileName = ctl.GetPropertyValue("FileName"),
                OrbitDateTime = ctl.GetPropertyValue("OrbitDateTime"),
                OrbitTimeGroup = ctl.GetPropertyValue("OrbitTimeGroup"),
                OrbitTimes = ctl.GetPropertyValue("OrbitTimes"),
                ProductIdentify = ctl.GetPropertyValue("ProductIdentify"),
                Region = ctl.GetPropertyValue("Region"),
                Satellite = ctl.GetPropertyValue("Satellite"),
                Sensor = ctl.GetPropertyValue("Sensor"),
                SubProductIdentify = ctl.GetPropertyValue("SubProductIdentify"),
                SourceFileName = fname
            };
            return datinfo;
        }

        private static string GetInfoFileName(string fname)
        {
            var newName = Path.ChangeExtension(fname, "INFO");
            return newName;
        }

        private static string[] GetFiles(string dir, string identify, string pattern)
        {
            var files = new List<string>();
            var ids = identify.Split(',');
            foreach (var id in ids)
            {
                var searchPattern = string.Format(pattern, id);
                var fs = Directory.GetFiles(dir, searchPattern, SearchOption.AllDirectories);
                if (fs.Length > 0)
                    files.AddRange(fs);
            }
            return files.Count > 0 ? files.ToArray() : null;
        }


        private static CatalogItemInfo TryCreateInfoFromMainFile(string fname, string infoFileName, SubProductCatalogDef catalogDef)
        {
            string fileName = Path.GetFileNameWithoutExtension(fname);
            string[] parts = fileName.Split('_');
            if (parts.Length < 2)
                return null;
            RasterIdentify rst = new RasterIdentify(fname);
            CatalogItemInfo info = new CatalogItemInfo();
            info.Properties.Add("ProductIdentify", parts[0]);
            info.Properties.Add("SubProductIdentify", parts[1]);
            info.Properties.Add("FileName", Path.GetFileName(fname));
            info.Properties.Add("FileDir", Path.GetDirectoryName(fname));
            info.Properties.Add("CatalogDef", catalogDef == null ? "" : catalogDef.ClassString);
            info.Properties.Add("Satellite", rst.Satellite);
            info.Properties.Add("Sensor", rst.Sensor);
            info.Properties.Add("OrbitDateTime", GetDateFormart(rst.OrbitDateTime, catalogDef, "OrbitDateTime"));
            info.Properties.Add("OrbitTimeGroup", GetDateFormart(rst.OrbitDateTime, catalogDef, "OrbitTimeGroup"));
            info.Properties.Add("CatalogItemCN", GetCatalogCN(info, MifEnvironment.CatalogItemCNDic));
            RasterIdentify ri = new RasterIdentify(fname);
            info.Properties.Add("Region", GetRegionInfo(ri, fname));
            info.Properties.Add("ExtInfos", GetExtInfo(ri, fname));
            info.Properties.Add("CycFlagCN", string.IsNullOrEmpty(ri.CYCFlag) ? "\\" : GetCycFlagCN(info, ri.CYCFlag, MifEnvironment.CatalogItemCNDic));
            info.Properties.Add("OrbitTimes", GetOrbitTimes(ri));
            //.....
            info.SaveTo(infoFileName);
            return info;
        }

        private static object GetExtInfo(RasterIdentify ri, string filename)
        {
            string result = "\\";
            string extInfo = string.IsNullOrEmpty(ri.ExtInfos) ? "" : ri.ExtInfos;
            if (string.IsNullOrEmpty(extInfo))
                return result;
            else if (extInfo.EndsWith("_"))
                extInfo = extInfo.Substring(0, extInfo.Length - 1);
            return result.Replace("\\", "") + extInfo;
        }

        private static object GetRegionInfo(RasterIdentify ri, string filename)
        {
            string result = "\\";
            string regionInfo = string.IsNullOrEmpty(ri.RegionIdentify) ? "" : ri.RegionIdentify;
            if (string.IsNullOrEmpty(regionInfo))
                return result;
            else if (regionInfo.EndsWith("_"))
                regionInfo = regionInfo.Substring(0, regionInfo.Length - 1);
            return result.Replace("\\", "") + regionInfo;
        }

        private static string GetCatalogCN(CatalogItemInfo info, Dictionary<string, Dictionary<string, string>> catalogCNDic)
        {
            if (!info.Properties.ContainsKey("ProductIdentify") || info.Properties["ProductIdentify"] == null)
                return string.Empty;
            string result = string.Empty;
            if (catalogCNDic == null || catalogCNDic.Count == 0)
                result = info.Properties["SubProductIdentify"].ToString();
            else if (catalogCNDic.ContainsKey(info.Properties["ProductIdentify"].ToString().ToUpper()))
            {
                if (info.Properties["SubProductIdentify"] == null || !info.Properties.ContainsKey("SubProductIdentify"))
                    return string.Empty;
                Dictionary<string, string> temp = catalogCNDic[info.Properties["ProductIdentify"].ToString().ToUpper()];
                if (temp.ContainsKey(info.Properties["SubProductIdentify"].ToString().ToUpper()))
                    result = temp[info.Properties["SubProductIdentify"].ToString().ToUpper()];
                else
                    result = info.Properties["SubProductIdentify"].ToString();
            }
            return result;
        }

        private static object GetOrbitTimes(RasterIdentify rid)
        {
            string defFormat = "yyyy-MM-dd HH:mm:ss";
            if (rid.ObritTiems == null || rid.ObritTiems.Length <= 1)
                return "\\";
            else
            {
                string result = rid.ObritTiems[0].ToString(defFormat) + ";" + rid.ObritTiems[1].ToString(defFormat);
                return result;
            }
        }

        private static string GetCycFlagCN(CatalogItemInfo info, string cycFlag, Dictionary<string, Dictionary<string, string>> catalogCNDic)
        {
            if (!info.Properties.ContainsKey("ProductIdentify") || info.Properties["ProductIdentify"] == null)
                return cycFlag;
            string result = cycFlag;
            if (catalogCNDic.ContainsKey(info.Properties["ProductIdentify"].ToString().ToUpper()))
            {
                Dictionary<string, string> temp = catalogCNDic[info.Properties["ProductIdentify"].ToString().ToUpper()];
                if (temp.ContainsKey(cycFlag.ToUpper()))
                    result = temp[cycFlag.ToUpper()];
            }
            return result;
        }

        private static string GetDateFormart(DateTime dt, SubProductCatalogDef catalogDef, string identify)
        {
            string defFormat = "yyyy-MM-dd HH:mm:ss";
            if (catalogDef == null)
                return dt.ToString(defFormat);
            CatalogAttriteDef cad = catalogDef.GetCatalogAttrDefByIdentify(identify);
            if (cad == null || string.IsNullOrEmpty(cad.Format))
                return dt.ToString(defFormat);
            return dt.ToString(cad.Format);
        }

        //public static WorkspaceInfo WorkspaceInfo { get; set; }
    }

}