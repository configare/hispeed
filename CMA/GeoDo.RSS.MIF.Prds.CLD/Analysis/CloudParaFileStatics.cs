using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using GeoDo.RSS.Core.DF;
using System.Drawing;
using GeoDo.RSS.RasterTools;
using GeoDo.FileProject;
using GeoDo.RasterProject;
using CodeCell.AgileMap.Core;
using System.Text.RegularExpressions;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.Core.VectorDrawing;
using GeoDo.Tools;


namespace GeoDo.RSS.MIF.Prds.CLD
{
    public class CloudParaFileStatics
    {
        protected const int MAXSVDLength = 1000;
        public static int[] _vectorAOIIndex = null;

        #region 长时间序列均值统计(优化)，2014.11.24，zyb
        private CoordEnvelope _subenv = null;
        public string[] FilesSeriesMeanStatNew(string[] filesL, int bandNumL, string[] fillvalueL, string outDir,bool isOutMeanRaster, Action<int, string> progressCallback)
        {
            if (filesL == null || filesL.Length == 0)
                throw new Exception("待统计文件不能为空!");
            if (!Directory.Exists(outDir))
                Directory.CreateDirectory(outDir);
            string regionName = "当前",prdName = "";
            Dictionary<DateTime, string[]> filedate = null;
            List<string[]> resultList = null;
            PrjEnvelope RegionEnv = null;
            GeoDo.RSS.Core.VectorDrawing.AOIContainerLayer _aoiContainer = null;
            string[] parts = Path.GetFileNameWithoutExtension(filesL[0]).Split('_');
            if (parts.Length > 1)
                prdName = parts[0];
            if (progressCallback != null)
                progressCallback(1, "开始长时间序列均值统计...");
            object rasteraoi = null;
            if (StatRegionSet.UseVectorAOIRegion)
            {
                RegionEnv = StatRegionSet.AOIPrjEnvelope;
                _aoiContainer  = StatRegionSet.AoiContainer;
                regionName = StatRegionSet.AOIName;
                if (RegionEnv == null || RegionEnv.Height <= 0 || RegionEnv.Width <= 0)
                    throw new ArgumentException("矢量AOI区域设置无效或范围不合法！");
                resultList = SeriesStaticRegion(filesL, bandNumL, fillvalueL,null, _aoiContainer, out filedate, progressCallback);
                rasteraoi = _aoiContainer;
            }
            else 
            {
                if (StatRegionSet.UseRegion || StatRegionSet.UseRecgRegion)//使用自定义区域
                {
                    RegionEnv = StatRegionSet.SelectedRegionEnvelope.PrjEnvelope;
                    if (RegionEnv == null || RegionEnv.Height <= 0 || RegionEnv.Width <= 0)
                        throw new ArgumentException("矢量AOI区域设置无效或范围不合法！");
                    regionName = StatRegionSet.RegionName;
                }
                resultList = SeriesStaticRegion(filesL, bandNumL, fillvalueL, RegionEnv,null, out filedate, progressCallback);
                rasteraoi = _subenv;
            }
            if (progressCallback != null)
                progressCallback(85, "统计完成，输出统计结果...");
            string[] excelNames = OutputExcelsNew(resultList, filedate, regionName, outDir, prdName);
            if (isOutMeanRaster)
            {
                if (progressCallback != null)
                    progressCallback(90, "正在计算均值栅格文件...");
                OutMeanRaster(filesL, fillvalueL, rasteraoi, Path.ChangeExtension(excelNames[0], ".ldf"), progressCallback);
            }
            return excelNames;
        }
        public bool GetFileTime(string file, out string date, out DateTime t)
        {
            #region 获取文件名中的时间信息
            Regex DataReg;
            date = "";
            t = DateTime.MinValue;
            int year, month, day,hour,minute,second;
            string cfile = Path.GetFileNameWithoutExtension(file);
            if (file.ToUpper().Contains("YEAR"))
            {
                DataReg = new Regex(@"(?<year>\d{4})", RegexOptions.Compiled);
                Match m = DataReg.Match(cfile);
                if (m.Success)
                {
                    year = int.Parse(m.Groups["year"].Value);
                    date = year.ToString() + "年";    //根据年4个数字来拆分字符串
                    t = new DateTime(year, 1, 1);
                    return true;
                }
            }
            else if (file.ToUpper().Contains("MONTH"))
            {
                DataReg = new Regex(@"(?<year>\d{4})-(?<month>\d{2})", RegexOptions.Compiled);
                Match m = DataReg.Match(cfile);
                if (m.Success)
                {
                    year = int.Parse(m.Groups["year"].Value);
                    date = year.ToString() + "年";    //根据年4个数字来拆分字符串
                    month = int.Parse(m.Groups["month"].Value);
                    date += month.ToString() + "月";    //根据年4个数字来拆分字符串
                    t = new DateTime(year, month, 1);
                    return true;
                }
            }
            else if (file.ToUpper().Contains("TEN"))
            {
                DataReg = new Regex(@"(?<year>\d{4})-(?<month>\d{2})-(?<ten>\d{1})", RegexOptions.Compiled);
                Match m = DataReg.Match(cfile);
                if (m.Success)
                {
                    year = int.Parse(m.Groups["year"].Value);
                    date = year.ToString() + "年";    //根据年4个数字来拆分字符串
                    month = int.Parse(m.Groups["month"].Value);
                    date += month.ToString() + "月";    //根据年4个数字来拆分字符串
                    day = int.Parse(m.Groups["ten"].Value);
                    date += "第" + day.ToString() + "旬";    //根据年4个数字来拆分字符串
                    t = new DateTime(year, month, day);
                    return true;
                }
            }
            else if (file.ToUpper().Contains("DAY"))
            {
                DataReg = new Regex(@"(?<year>\d{4})(?<month>\d{2})(?<day>\d{2})", RegexOptions.Compiled);
                Match m = DataReg.Match(cfile);
                if (m.Success)
                {
                    year = int.Parse(m.Groups["year"].Value);
                    date = year.ToString() + "年";    //根据年4个数字来拆分字符串
                    month = int.Parse(m.Groups["month"].Value);
                    date += month.ToString() + "月";    //根据年4个数字来拆分字符串
                    day = int.Parse(m.Groups["day"].Value);
                    date += day.ToString() + "日";    //根据年4个数字来拆分字符串
                    t = new DateTime(year, month, day);
                    return true;
                }
            }
            else
            {
                DataReg = new Regex(@"(?<year>\d{4})(?<month>\d{2})(?<day>\d{2})", RegexOptions.Compiled);
                Match m = DataReg.Match(cfile);
                if (m.Success)
                {
                    year = int.Parse(m.Groups["year"].Value);
                    date = year.ToString() + "年";    //根据年4个数字来拆分字符串
                    month = int.Parse(m.Groups["month"].Value);
                    date += month.ToString() + "月";    //根据年4个数字来拆分字符串
                    day = int.Parse(m.Groups["day"].Value);
                    date += day.ToString() + "日";    //根据年4个数字来拆分字符串
                    t = new DateTime(year, month, day);
                    return true;
                }
                DataReg = new Regex(@"(?<year>\d{4})(?<month>\d{2})(?<day>\d{2})(?<hour>\d{2})(?<minnute>\d{2})(?<second>\d{2})", RegexOptions.Compiled);
                m = DataReg.Match(cfile);
                if (m.Success)
                {
                    year = int.Parse(m.Groups["year"].Value);
                    date = year.ToString() + "年";    //根据年4个数字来拆分字符串
                    month = int.Parse(m.Groups["month"].Value);
                    date += month.ToString() + "月";    //根据年4个数字来拆分字符串
                    day = int.Parse(m.Groups["day"].Value);
                    date += day.ToString() + "日";    //根据年4个数字来拆分字符串
                    hour = int.Parse(m.Groups["hour"].Value);
                    date = hour.ToString() + "时";    //根据年4个数字来拆分字符串
                    minute = int.Parse(m.Groups["minute"].Value);
                    date += minute.ToString() + "分";    //根据年4个数字来拆分字符串
                    second = int.Parse(m.Groups["second"].Value);
                    date += second.ToString() + "秒";    //根据年4个数字来拆分字符串
                    t = new DateTime(year, month, day, hour,minute,second);
                    return true;
                }
                DataReg = new Regex(@"(?<year>\d{4})(?<month>\d{2})(?<day>\d{2})_(?<hour>\d{2})(?<minnute>\d{2})", RegexOptions.Compiled);
                m = DataReg.Match(cfile);
                if (m.Success)
                {
                    year = int.Parse(m.Groups["year"].Value);
                    date = year.ToString() + "年";    //根据年4个数字来拆分字符串
                    month = int.Parse(m.Groups["month"].Value);
                    date += month.ToString() + "月";    //根据年4个数字来拆分字符串
                    day = int.Parse(m.Groups["day"].Value);
                    date += day.ToString() + "日";    //根据年4个数字来拆分字符串
                    hour = int.Parse(m.Groups["hour"].Value);
                    date = hour.ToString() + "时";    //根据年4个数字来拆分字符串
                    minute = int.Parse(m.Groups["minute"].Value);
                    date += minute.ToString() + "分";    //根据年4个数字来拆分字符串
                    t = new DateTime(year, month, day, hour, minute, 0);
                    return true;
                }
            }
            if (date == "")
                date = Path.GetFileNameWithoutExtension(file);
            return false;
            #endregion
        }

        public static bool IsSameTime(DateTime[] date)
        {
            bool isSameTime = false;
            if (date.Length > 1)
            {
                isSameTime = true;
                bool is1st = true;
                int month = 0, day = 0;
                foreach (DateTime dt in date)
                {
                    if (is1st)
                    {
                        month = dt.Month;
                        day = dt.Day;
                        is1st = false;
                    }
                    else if (month != dt.Month || day != dt.Day)
                    {
                        isSameTime = false;
                        break;
                    }
                }
            }
            return isSameTime;
        }

        public List<string[]> SeriesStaticRegion(string[] filesL, int bandNumL, string[] fillvalueL, PrjEnvelope RegionEnv, AOIContainerLayer aoicontainer, out Dictionary<DateTime, string[]> filedate, Action<int, string> progressCallback)
        {
            List<string[]> resultList = new List<string[]>();
            filedate = new Dictionary<DateTime, string[]>();
            DateTime t = DateTime.MinValue;
            IRasterDataProvider dataPrd = null;
            try
            {
                PrjEnvelope dstleftPrj;
                int[] aoi = null;
                double [] values=null;
                string date;
                int interval = (int)((85 - 1) / (filesL.Length * 1.0f));
                int countNN = 0;
                double[][] fillValuesd = new double[1][];
                fillValuesd[0] = GetFillValues<double>(fillvalueL, enumDataType.Double);
                enumDataType datatype = enumDataType.Unknow;
                if (progressCallback != null)
                    progressCallback(1, "开始逐文件进行均值统计...");
                foreach (string file in filesL)
                {
                    using (dataPrd = GeoDataDriver.Open(file) as IRasterDataProvider)
                    {
                        datatype = dataPrd.DataType;
                        if (RegionEnv != null)
                        {
                            if (!CheckAOIIntersect(dataPrd, RegionEnv, out dstleftPrj, out aoi))
                                throw new ArgumentException("设置的区域没有可分析的左场数据!");
                            if (_subenv == null)
                                _subenv = new CoordEnvelope(dstleftPrj.MinX, dstleftPrj.MaxX, dstleftPrj.MinY, dstleftPrj.MaxY);
                        }
                        else if (aoicontainer!=null)
                        {
                            Size fileSize = new Size(dataPrd.Width, dataPrd.Height);
                            aoi = CloudParaFileStaticsAOI.GetAOI(dataPrd.CoordEnvelope, aoicontainer, fileSize);
                            if (aoi == null || aoi.Length <= 0)
                                throw new ArgumentException(Path.GetFileName(file) + "与矢量AOI区域无相交区域！");
                        }
                        if (progressCallback != null)
                            progressCallback(1 + interval * countNN, "统计波段数据均值...");
                        if (CloudParaFileStatics.ComputeMinMaxAvgNew(dataPrd, datatype, new int[] { bandNumL }, fillValuesd, aoi, out values, progressCallback))
                        {
                            GetFileTime(file, out date, out t);
                            if (!filedate.ContainsKey(t))
                                filedate.Add(t, new string[] { date,values[0].ToString("f2"),values[1].ToString("f2"), values[2].ToString("f2") });
                            resultList.Add(new string[] { date, values[2].ToString("f2") });
                            if (progressCallback != null)
                                progressCallback(-1, Path.GetFileName(file) + "统计完成！");
                        }
                    }
                }
                return resultList;
            }
            finally
            {
                if (dataPrd != null)
                    dataPrd.Dispose();
            }
        }
        
        public static bool ComputeMinMaxAvgNew(IRasterDataProvider dataProvider, enumDataType datatype, int[] bandNos, double[][] fillValuesd, int[] aoi, out double[] values, Action<int, string> progressCallback)
        {
            double[] minValues = new double[bandNos.Length];
            double[] maxValues = new double[bandNos.Length];
            double[] meanValues = new double[bandNos.Length];
            values = null;
            IMaxMinValueComputer computer = MaxMinValueComputerFactory.GetMaxMinValueComputer(datatype);
            IRasterBand[] srcRasters = new IRasterBand[bandNos.Length];
            for (int i = 0; i < bandNos.Length; i++)
            {
                srcRasters[i] = dataProvider.GetRasterBand(bandNos[i]);
            }
            computer.Compute(srcRasters, aoi, fillValuesd, out minValues, out maxValues, out meanValues, progressCallback);
            values = new double[3] { minValues[0], meanValues[0], maxValues[0] };
            return true;
        }
        
        public static string[] OutputExcelsNew(List<string[]> resultList, Dictionary<DateTime, string[]> filedate, string regionName, string outDir, string prd = "")
        {
            string title, filename;
            string coluString = "数据时次", sentitle = "统计日期：" + DateTime.Now.ToShortDateString();
            string[] columns = new string[] { coluString, "均值" };
            IStatResult fresult = null;
            string[][] excelData = new string[filedate.Count][];
            bool isSameTime = IsSameTime(filedate.Keys.ToArray());
            DateTime dtn;
            int count = 0;
            StringBuilder titleb = new StringBuilder(regionName);
            titleb.Append("区域");
            if (isSameTime)
            {
                title = titleb.Append(filedate.Keys.Min().Year.ToString()).Append("年-").Append(filedate.Keys.Max().Year.ToString()).Append("年同期").Append(filedate.Keys.Max().Month.ToString()).Append("月" + prd + "均值统计").ToString();
                filename = Path.Combine(outDir, title + ".XLSX");
                while (filedate.Count > 0)
                {
                    dtn = filedate.Keys.Min();
                    excelData[count++] = new string[] { filedate[dtn][0].Substring(0, 5), filedate[dtn][2],filedate[dtn][1],  filedate[dtn][3] };
                    filedate.Remove(dtn);
                }
            }
            else
            {
                title = titleb.Append(filedate[filedate.Keys.Min()][0].ToString()).Append("-").Append(filedate[filedate.Keys.Max()][0].ToString()).Append(prd + "均值统计").ToString();
                filename = Path.Combine(outDir, title + ".XLSX");
                while (filedate.Count > 0)
                {
                    dtn = filedate.Keys.Min();
                    excelData[count++] = new string[] { filedate[dtn][0], filedate[dtn][2],filedate[dtn][1],  filedate[dtn][3] };
                    filedate.Remove(dtn);
                }
            }
            fresult = new StatResult(sentitle, columns, excelData);
            try
            {
                if (File.Exists(filename))
                    File.Delete(filename);
                int notStatCol = 1;
                using (StatResultToExcelFile excelControl = new StatResultToExcelFile())
                {
                    excelControl.Init();
                    excelControl.Zoom = 1;
                    excelControl.Add(true, title, fresult, fresult.Columns.Length - notStatCol, false, 1, 1);
                    if (!filename.ToUpper().EndsWith(".XLSX"))
                        filename += ".XLSX";
                    excelControl.SaveFile(filename);
                }
            }
            catch (Exception ex)
            {
                using (StatResultToTxtFile txtControl = new StatResultToTxtFile())
                {
                    if (!filename.ToUpper().EndsWith(".TXT"))
                        filename += ".TXT";
                    txtControl.WriteResultToTxt(title + "\n");
                    txtControl.WriteResultToTxt("统计日期：" + DateTime.Today.Date.ToShortDateString() + "\n");
                    txtControl.WriteResultToTxt(fresult);
                    bool isSave = txtControl.SaveFile(filename);
                }
            }
            return new string[] { filename };
        }
        
        public void OutMeanRaster(string[] filesL, string[] fillvalueL, object aoicontainer, string outname,Action<int, string> progressCallback)
        {
            try
            {
                RasterStatics sta = new RasterStatics();
                if (fillvalueL!=null&&fillvalueL.Length>0)
                {
                    double[] invalidValues = new double[fillvalueL.Length];
                    double value;
                    for (int i = 0; i < fillvalueL.Length;i++ )
                    {
                        if(double.TryParse(fillvalueL[i],out value))
                        invalidValues[i]=value;
                    }
                    sta.SetDstInvalidValues(invalidValues);
                }
                if (sta.PeriodicAvgStat(filesL, outname, aoicontainer, progressCallback))
                {
                    if (progressCallback != null)
                        progressCallback(-1, "\t\t\t\t合成处理完成！");
                }
                else
                {
                    if (progressCallback != null)
                        progressCallback(0, "\t\t\t\t合成处理失败！");
                    if (File.Exists(outname))
                    {
                        File.Delete(outname);
                    }
                }
                if (File.Exists(outname))
                {
                    //生成快视图
                    string filename = OverViewHelper.OverView(outname, 800);
                    if (progressCallback != null && File.Exists(filename))
                        progressCallback(-1, "\t\t\t\t生成快视图成功！");
                }
            }
            finally
            {

            }
        }        
        #endregion

        #region SVD分解
        public string[] FilesSVDStat(string[] filesL, int bandNumL,string []fillvalueL, string[] filesR, int bandNumR,string []fillvalueR, double leftRatio, double rightRatio, string outDir, Action<int, string> progressCallback,bool LisMicaps=false,bool RisMicaps=false)
        {
            //check
            if (filesL.Length == 0 || filesR.Length == 0 || filesR.Length != filesL.Length)
                return null;
            if (LisMicaps||RisMicaps)
                return FilesSVDStatWithMicaps(filesL, bandNumL,fillvalueL, filesR, bandNumR,fillvalueR, leftRatio, rightRatio, outDir, progressCallback,LisMicaps,RisMicaps);
            if (!CheckFiles(filesL, bandNumL) || !CheckFiles(filesR, bandNumR))
                return null;
            if (!Directory.Exists(outDir))
                Directory.CreateDirectory(outDir);
            if (StatRegionSet.UseVectorAOIRegion)
            {
                if (progressCallback != null)
                    progressCallback(-1, "利用AOI区域，开始SVD分解...");
                CloudParaFileStaticsAOI ai = new CloudParaFileStaticsAOI();
                return ai.FilesSVDStatAOI(filesL, bandNumL, fillvalueL, filesR, bandNumR, fillvalueR, leftRatio, rightRatio, outDir, progressCallback);
            }
            Size leftSize;  //左场数据大小
            Size rightSize; //右场数据大小
            if (StatRegionSet.UseRegion || StatRegionSet.UseRecgRegion)//使用自定义区域
            {
                PrjEnvelope RegionEnv=null;
                string regionName="all";
                if (StatRegionSet.UseRegion||StatRegionSet.UseRecgRegion)
                {
                    RegionEnv = StatRegionSet.SelectedRegionEnvelope.PrjEnvelope;
                    regionName = StatRegionSet.RegionName;
                }
                //else if (StatRegionSet.UseVectorAOIRegion)
                //{
                //    RegionEnv = StatRegionSet.AOIPrjEnvelope;
                //    regionName = StatRegionSet.AOIName;
                //}
                PrjEnvelope dstleftPrj, dstrightPrj;
                float reslxL, reslyL, reslxR, reslyR;
                int[] dataPosL,dataPosR;
                if (!CheckRegionIntersect(filesL[0], RegionEnv, out dstleftPrj, out reslxL, out reslyL, out dataPosL))
                        throw new ArgumentException("设置的区域没有可分析的左场数据!");
                if (!CheckRegionIntersect(filesR[0], RegionEnv, out dstrightPrj, out  reslxR, out reslyR, out dataPosR))
                        throw new ArgumentException("设置的区域没有可分析的右场数据!");
                {
                    if (leftRatio < 1)
                        leftRatio = 1;
                    //double lratio = Math.Sqrt((dstleftPrj.Width / reslxL * dstleftPrj.Height / reslyL) * 1.0 / MAXSVDLength);
                    //leftRatio=(leftRatio>lratio?lratio:leftRatio);
                    if (rightRatio < 1)
                        rightRatio = 1;
                    //double rratio = Math.Sqrt((dstrightPrj.Width / reslxR * dstrightPrj.Height / reslyR) * 1.0 / MAXSVDLength);
                    //rightRatio = (rightRatio > rratio ? rratio : rightRatio);
                    //根据采样系数计算采样后数据大小
                    if (progressCallback != null)
                        progressCallback(0, "开始SVD分解...");
                    int LWidth = (int)(dstleftPrj.Width / leftRatio / reslxL + 0.5f);
                    int LHeight = (int)(dstleftPrj.Height / leftRatio / reslyL + 0.5f);
                    leftSize = new Size(LWidth, LHeight);
                    if (progressCallback != null)
                        progressCallback(5, "开始抽样左场数据并标准化...");
                    double[,] leftData = StandardReadData(filesL, bandNumL, leftSize, dataPosL);          // ReadRastersDataToMarix(filesL, bandNumL, out outSize);
                    int RWidth = (int)(dstrightPrj.Width / rightRatio / reslxR + 0.5f);
                    int RHeight = (int)(dstrightPrj.Height / rightRatio / reslyR + 0.5f);
                    rightSize = new Size(RWidth, RHeight);
                    if (progressCallback != null)
                        progressCallback(10, "开始抽样右场数据并标准化...");
                    double[,] rightData = StandardReadData(filesR, bandNumR, rightSize, dataPosR);        // ReadRastersDataToMarix(filesR, bandNumR, out outSize);
                    if (progressCallback != null)
                        progressCallback(15, "完成数据采样及标准化,开始矩阵计算...");
                    CloudParaStat stat = new CloudParaStat();
                    return stat.AlglibSVD(leftData, rightData, outDir, filesL[0], leftSize, rightSize, progressCallback);
                }
            }
            else//不使用自定义区域
            {
                //Read Data
                if (leftRatio < 1)
                    leftRatio = CalcSample(filesL[0]);
                if (rightRatio < 1)
                    rightRatio = CalcSample(filesR[0]);
                RatioSize(filesL[0], bandNumL, leftRatio, out leftSize);
                RatioSize(filesR[0], bandNumR, rightRatio, out rightSize);
                if (progressCallback != null)
                    progressCallback(0, "开始SVD分解...");
                if (progressCallback != null)
                    progressCallback(5, "开始抽样左场数据并标准化...");
                double[,] leftData = StandardReadData(filesL, bandNumL, leftSize);          // ReadRastersDataToMarix(filesL, bandNumL, out outSize);
                if (progressCallback != null)
                    progressCallback(10, "开始抽样右场数据并标准化...");
                double[,] rightData = StandardReadData(filesR, bandNumR, rightSize);        // ReadRastersDataToMarix(filesR, bandNumR, out outSize);
                if (progressCallback != null)
                    progressCallback(15, "完成数据采样及标准化,开始矩阵计算...");
                CloudParaStat stat = new CloudParaStat();
                return stat.AlglibSVD(leftData, rightData, outDir, filesL[0], leftSize, rightSize, progressCallback);
            }
        }

        public string[] FilesSVDStatWithMicaps(string[] filesL, int bandNumL,string []fillvalueL, string[] filesR, int bandNumR,string []fillvalueR, double leftRatio, double rightRatio, string outDir, Action<int, string> progressCallback, bool LisMicaps = false, bool RisMicaps = false)
        {
            //check
            if (!LisMicaps&&!CheckFiles(filesL, bandNumL))
                return null;
            if (!RisMicaps&&!CheckFiles(filesR, bandNumR))
                return null;                
            if (!Directory.Exists(outDir))
                Directory.CreateDirectory(outDir);
            string regionName = "all";
            PrjEnvelope envelope = null;
            Envelope regionenv = null;
            if (StatRegionSet.UseRegion)
            {
                envelope = StatRegionSet.SelectedRegionEnvelope.PrjEnvelope;
                regionenv = StatRegionSet.Envelope;
                regionName = StatRegionSet.RegionName;
            }
            else if (StatRegionSet.UseVectorAOIRegion)
            {
                envelope = StatRegionSet.AOIPrjEnvelope;
                regionenv = StatRegionSet.AOIEnvelope;
                regionName = StatRegionSet.AOIName;
            }
            double[][] marixl = null, marixr = null;
            List<ShapePoint> matchedpos;        
            if (LisMicaps && !RisMicaps)//左场为micaps
                MicapsDataProcess.GetMatchedMatrixs(filesR, bandNumR, fillvalueR, filesL, bandNumL, fillvalueL, envelope, out marixr, out marixl, out matchedpos);
            else if (RisMicaps && !LisMicaps)
                MicapsDataProcess.GetMatchedMatrixs(filesL, bandNumL, fillvalueL, filesR, bandNumR, fillvalueR, envelope, out marixl, out marixr, out matchedpos);
            else//当两边都为micaps数据时，
                MicapsDataProcess.GetMicapsDataMatrixs(filesL, bandNumL, fillvalueL, filesR, bandNumR, fillvalueR, regionenv, out marixl, out marixr, out matchedpos);
            if (matchedpos == null || matchedpos.Count < 1)
                throw new ArgumentException("左右场没有匹配的数据!");
            int width = matchedpos.Count;
            double[,] leftData = StandardMicapsData(marixl);
            double[,] rightData = StandardMicapsData(marixr);
            if (progressCallback != null)
                progressCallback(15, "完成数据采样及标准化,开始矩阵计算...");
            CloudParaStat stat = new CloudParaStat();
            //return stat.AlglibSVDWithMicaps(leftData, rightData, outDir, filesL[0], progressCallback);
            return stat.AlglibSVD(leftData, rightData, outDir, filesL[0], new Size(width, 1), new Size(width, 1),progressCallback,LisMicaps,RisMicaps,  matchedpos);
        }
        #endregion

        #region 自定义区域处理

        protected bool CheckRegionIntersect(string file, PrjEnvelope env, out PrjEnvelope dstmainPrj, out float reslx, out float resly, out int[] dataPos)
        {
            dstmainPrj = null;
            dataPos = null;
            using (IRasterDataProvider dataPrd = GeoDataDriver.Open(file) as IRasterDataProvider)
            {
                CoordEnvelope cordenv = dataPrd.CoordEnvelope;
                reslx = dataPrd.ResolutionX;
                resly = dataPrd.ResolutionY;
                if (cordenv != null)
                {
                    PrjEnvelope filePrj = new PrjEnvelope(cordenv.MinX, cordenv.MaxX, cordenv.MinY, cordenv.MaxY);
                    if (env != null)
                        dstmainPrj = PrjEnvelope.Intersect(env, filePrj);
                    else
                        dstmainPrj = filePrj;
                    if (dstmainPrj != null && dstmainPrj.Width > 0 && dstmainPrj.Height>0)
                    {
                        int xoffset = (int)((dstmainPrj.MinX - cordenv.MinX)/reslx);
                        int yoffset = (int)((cordenv.MaxY - dstmainPrj.MaxY) / resly);
                        if (xoffset < 0)
                            xoffset = 0;
                        if (yoffset < 0)
                            yoffset = 0;
                        int width = (int)(dstmainPrj.Width / reslx);
                        int height = (int)(dstmainPrj.Height / resly);
                        dataPos = new int[4] { xoffset, yoffset, width, height };
                        return true;
                    }
                    throw new Exception(Path.GetFileName(file)+ "自定义区域与文件不存在相交区域！");
                }
                throw new Exception(Path.GetFileName(file)+"文件坐标范围不可用！");
            }
        }

        public static bool CheckAOIIntersect(IRasterDataProvider dataPrd, PrjEnvelope env, out PrjEnvelope dstmainPrj,out int[] aoiindex)
        {
            dstmainPrj = null;
            aoiindex = null;
            CoordEnvelope cordenv = dataPrd.CoordEnvelope;
            if (cordenv != null && cordenv.Width > 0 && cordenv.Height>0)
            {
                PrjEnvelope filePrj = new PrjEnvelope(cordenv.MinX, cordenv.MaxX, cordenv.MinY, cordenv.MaxY);
                if (env != null)
                {
                    dstmainPrj = PrjEnvelope.Intersect(env, filePrj);
                    if (dstmainPrj != null && dstmainPrj.Width > 0 && dstmainPrj.Height > 0)
                    {
                        int xoffset = (int)((dstmainPrj.MinX - cordenv.MinX) / dataPrd.ResolutionX+0.5);
                        int yoffset = (int)((cordenv.MaxY - dstmainPrj.MaxY) / dataPrd.ResolutionY + 0.5);
                        if (xoffset < 0)
                            xoffset = 0;
                        if (yoffset < 0)
                            yoffset = 0;
                        if (xoffset > dataPrd.Width)
                            xoffset = dataPrd.Width;
                        if (yoffset > dataPrd.Height)
                            yoffset = dataPrd.Height;
                        int width = (int)(dstmainPrj.Width / dataPrd.ResolutionX+0.5);
                        int height = (int)(dstmainPrj.Height / dataPrd.ResolutionY + 0.5);
                        int i, j;
                        aoiindex = new int[width * height];
                        for (i = 0; i < height;i++ )
                        {
                            for (j = 0; j < width; j++)
                            {
                                aoiindex[i * width + j] = dataPrd.Width * (yoffset + i) + (xoffset + j);
                            }
                        }
                        return true;
                    }
                    throw new Exception(Path.GetFileName(dataPrd.fileName) + "自定义区域与文件不存在相交区域！");
                }
                else
                {
                    dstmainPrj = filePrj;
                    return true;
                }
            }
            throw new Exception(Path.GetFileName(dataPrd.fileName) + "文件坐标范围不可用！");
        }

        protected bool CheckRegionIntersect(string file, PrjEnvelope env, out int[] dataPos, out PrjEnvelope dstmainPrj)
        {
            dataPos = null;
            using (IRasterDataProvider dataPrd = GeoDataDriver.Open(file) as IRasterDataProvider)
            {
                CoordEnvelope cordenv = dataPrd.CoordEnvelope;
                float reslx = dataPrd.ResolutionX;
                float resly = dataPrd.ResolutionY;
                dstmainPrj = null;
                if (cordenv != null)
                {
                    PrjEnvelope filePrj = new PrjEnvelope(cordenv.MinX, cordenv.MaxX, cordenv.MinY, cordenv.MaxY);
                    dstmainPrj = PrjEnvelope.Intersect(env, filePrj);
                    if (dstmainPrj != null && dstmainPrj.Width > 0 && dstmainPrj.Height>0)
                    {
                        int xoffset = (int)((dstmainPrj.MinX - cordenv.MinX) / reslx);
                        int yoffset = (int)((cordenv.MaxY - dstmainPrj.MaxY) / resly);
                        if (xoffset < 0)
                            xoffset = 0;
                        if (yoffset < 0)
                            yoffset = 0;
                        int width = (int)(dstmainPrj.Width / reslx);
                        int height = (int)(dstmainPrj.Height / resly);
                        dataPos = new int[4] { xoffset, yoffset, width, height };
                        return true;
                    }
                    throw new Exception(Path.GetFileName(file)+ "自定义区域与文件不存在相交区域！");
                }
                throw new Exception(Path.GetFileName(file)+ "文件坐标范围不可用！");
            }
        }

        /// <summary>
        /// 按照指定的采样大小读取数据，并处理为标准化矩阵
        /// </summary>
        /// <param name="files"></param>
        /// <param name="bandNum"></param>
        /// <param name="readSize"></param>
        /// <returns></returns>
        protected double[,] StandardReadData(string[] files, int bandNum, Size readSize, int[] dataPos)
        {
            double[] outData;
            int columnCount = readSize.Width * readSize.Height;
            double[,] marix = new double[files.Length, columnCount];
            for (int i = 0; i < files.Length; i++)
            {
                outData = GetFileDataToStandardArray(files[i], bandNum, readSize, dataPos[2], dataPos[3], dataPos[0], dataPos[1]);
                for (int j = 0; j < columnCount; j++)
                {
                    marix[i, j] = outData[j];
                }
            }
            return marix;
        }
        #endregion

        public static Envelope GetIntersectEnvolop(string [] rasterfiles,PrjEnvelope regionEnv)
        {
            Envelope dstEnv = null;
            CoordEnvelope fileEnv = null;
            foreach (string file in rasterfiles)
            {
                using (IRasterDataProvider dataPrd = GeoDataDriver.Open(file) as IRasterDataProvider)
                {
                    if (dataPrd == null)
                        continue;
                    fileEnv = dataPrd.CoordEnvelope;
                    break;
                }
            }
            if (regionEnv == null && fileEnv != null)
                dstEnv = new Envelope(fileEnv.MinX, fileEnv.MinY, fileEnv.MaxX, fileEnv.MaxY);
            else if (regionEnv != null && fileEnv == null)
                dstEnv = new Envelope(regionEnv.MinX,regionEnv.MinY,regionEnv.MaxX,regionEnv.MaxY);
            else
            {
                PrjEnvelope filePrj = new PrjEnvelope(fileEnv.MinX, fileEnv.MaxX, fileEnv.MinY, fileEnv.MaxY);
                PrjEnvelope dstPrjEnv = PrjEnvelope.Intersect(regionEnv, filePrj);
                if (dstPrjEnv != null && dstPrjEnv.Width > 0 && dstPrjEnv.Height > 0)
                    dstEnv=new Envelope(dstPrjEnv.MinX, dstPrjEnv.MinY, dstPrjEnv.MaxX, dstPrjEnv.MaxY);
            }
            return dstEnv;
        }

        /// <summary>
        /// 计算采样系数
        /// </summary>
        /// <param name="filesL"></param>
        /// <returns></returns>
        public static double CalcSample(string filesL,bool useregion =false)
        {
            using (IRasterDataProvider dataPrd = GeoDataDriver.Open(filesL) as IRasterDataProvider)
            {
                if(dataPrd==null)
                    throw new FileLoadException(Path.GetFileName(filesL), "加载失败！");
                float reslx = dataPrd.ResolutionX;
                float resly = dataPrd.ResolutionY;
                if (useregion)
                {
                    CoordEnvelope cordenv = dataPrd.CoordEnvelope;
                    PrjEnvelope RegionEnv;
                    if (StatRegionSet.UseRegion)
                        RegionEnv = StatRegionSet.SelectedRegionEnvelope.PrjEnvelope;
                    else if (StatRegionSet.UseVectorAOIRegion)
                    {
                        RegionEnv = StatRegionSet.AOIPrjEnvelope;
                        _vectorAOIIndex = CloudParaFileStaticsAOI.GetAOI(cordenv, StatRegionSet.AoiContainer, new Size(dataPrd.Width, dataPrd.Height));
                        if(_vectorAOIIndex==null)
                            throw new ArgumentOutOfRangeException(Path.GetFileName(filesL), "获取文件的自定义区域索引失败！");
                        double ratio = Math.Sqrt(_vectorAOIIndex.Length * 1.0 / MAXSVDLength);
                        return (ratio > 1 ? ratio : 1);
                    }
                    else
                        RegionEnv = new PrjEnvelope(-180, 180, -90, 90);
                    if (cordenv != null)
                    {
                        PrjEnvelope filePrj = new PrjEnvelope(cordenv.MinX, cordenv.MaxX, cordenv.MinY, cordenv.MaxY);
                        PrjEnvelope dstmainPrj = PrjEnvelope.Intersect(RegionEnv, filePrj);
                        if (dstmainPrj != null)
                        {
                            double ratio = Math.Sqrt((dstmainPrj.Width / reslx * dstmainPrj.Height / resly) * 1.0 / MAXSVDLength);
                            return (ratio > 1 ? ratio : 1);
                        }
                        throw new ArgumentOutOfRangeException(Path.GetFileName(filesL), "自定义区域与文件不存在相交区域！");
                    }
                    throw new ArgumentException(Path.GetFileName(filesL), "文件坐标范围不可用！");
                }
                else
                {
                    double ratio = Math.Sqrt((dataPrd.Width * dataPrd.Height) * 1.0 / MAXSVDLength);
                    return ratio < 1 ? 1 : ratio;
                }
            }
        }

        /// <summary>
        /// 根据采样系数计算采样后数据大小
        /// </summary>
        /// <param name="filesL"></param>
        /// <param name="bandNumL"></param>
        /// <param name="ratio"></param>
        /// <param name="size"></param>
        public static void RatioSize(string filesL, int bandNumL, double ratio, out Size size)
        {
            using (IRasterDataProvider dataPrd = GeoDataDriver.Open(filesL) as IRasterDataProvider)
            {
                float reslx = dataPrd.ResolutionX;
                float resly = dataPrd.ResolutionY;
                if (StatRegionSet.UseRegion)
                {
                    PrjEnvelopeItem RegionEnv = StatRegionSet.SelectedRegionEnvelope;
                    CoordEnvelope cordenv = dataPrd.CoordEnvelope;
                    if (cordenv != null)
                    {
                        PrjEnvelope filePrj = new PrjEnvelope(cordenv.MinX, cordenv.MaxX, cordenv.MinY, cordenv.MaxY);
                        PrjEnvelope dstmainPrj = PrjEnvelope.Intersect(RegionEnv.PrjEnvelope, filePrj);
                        if (dstmainPrj != null)
                        {
                            int tWidth = (int)(dstmainPrj.Width / ratio / reslx + 0.5f);
                            int tHeight = (int)(dstmainPrj.Height / ratio / resly + 0.5f);
                            size = new Size(tWidth, tHeight);
                            return;
                        }
                        throw new ArgumentOutOfRangeException(Path.GetFileName(filesL), "自定义区域与文件不存在相交区域！");
                    }
                    throw new ArgumentException(Path.GetFileName(filesL), "文件坐标范围不可用！");
                }
                else
                {
                    int tWidth = (int)(dataPrd.Width / ratio + 0.5f);
                    int tHeight = (int)(dataPrd.Height / ratio + 0.5f);
                    size = new Size(tWidth, tHeight);
                }
            }
        }

        /// <summary>
        /// 按照指定的采样大小读取数据，并处理为标准化矩阵
        /// </summary>
        /// <param name="files"></param>
        /// <param name="bandNum"></param>
        /// <param name="readSize"></param>
        /// <returns></returns>
        protected double[,] StandardReadData(string[] files, int bandNum, Size readSize)
        {
            double[] outData;
            int columnCount = readSize.Width * readSize.Height;
            double[,] marix = new double[files.Length, columnCount];
            for (int i = 0; i < files.Length; i++)
            {
                outData = GetFileDataToStandardArray(files[i], bandNum, readSize);
                for (int j = 0; j < columnCount; j++)
                {
                    marix[i, j] = outData[j];
                }
            }
            return marix;
        }

        /// <summary>
        /// 将micaps数据处理为标准化矩阵
        /// </summary>
        /// <param name="files"></param>
        /// <param name="bandNum"></param>
        /// <param name="readSize"></param>
        /// <returns></returns>
        protected double[,] StandardMicapsData(double[][] micapsdata)
        {
            int filelength=micapsdata.Length;
            int columnCount = micapsdata[0].Length;
            double[] oriData,outData;
            double[,] marix = new double[filelength, columnCount];
            for (int i = 0; i < filelength; i++)
            {
                oriData = micapsdata[i];
                AnaliysisDataPreprocess.StandardDeviation(oriData, out outData);
                for (int j = 0; j < columnCount; j++)
                {
                    marix[i, j] = outData[j];
                }
            }
            return marix;
        }

        /// <summary>
        /// 检测文件是否合法
        /// </summary>
        protected bool CheckFiles(string[] files, int bandNum)
        {
            int width = 0, height = 0;
            enumDataType dataType = enumDataType.Unknow;
            if (files.Length == 1)
                return true;
            foreach (string item in files)
            {
                if (!File.Exists(item))
                    return false;
                using (IRasterDataProvider dataPrd = GeoDataDriver.Open(item) as IRasterDataProvider)
                {
                    if (dataPrd==null)
                        throw new FileLoadException("文件加载失败：打开" + item+"失败！请确认未损坏！");
                    if (bandNum > dataPrd.BandCount)
                        throw new ArgumentOutOfRangeException("波段数超出范围：读取" + item + "的波段"+bandNum+"失败！");
                    if (width == 0 && height == 0)
                    {
                        width = dataPrd.Width;
                        height = dataPrd.Height;
                        dataType = dataPrd.DataType;
                    }
                    else
                    {
                        if (width != dataPrd.Width || height != dataPrd.Height)
                            throw new ArgumentOutOfRangeException("文件大小不一致：运算失败！");
                        if(dataType != dataPrd.DataType)
                            throw new FileLoadException("文件数据类型不一致：运算失败！");
                    }
                }
            }
            return true;
        }

        protected double[] GetFileDataToStandardArray(string file, int bandNum, Size outSize, int width = 0, int height = 0, int xoffset = 0, int yoffset = 0)
        {
            using (IRasterDataProvider dataPrd = GeoDataDriver.Open(file) as IRasterDataProvider)
            {
                double[] standardData = new double[] { };
                int datawidth=(width > 0) ? width : dataPrd.Width;
                int dataheight =(height>0)?height:dataPrd.Height;
                int dataxoffset=xoffset>0?xoffset:0;
                int datayoffset = yoffset > 0 ? yoffset : 0;
                if (dataPrd.DataType == enumDataType.Float)
                {
                    float[] outData;
                    float[] oriData = GetDataValue<float>(dataPrd, bandNum, datawidth, dataheight, dataxoffset, datayoffset);
                    AnaliysisDataPreprocess.MedianRead(oriData, new Size(datawidth, dataheight), outSize, out outData);
                    AnaliysisDataPreprocess.StandardDeviation(outData, out standardData);
                }
                else if (dataPrd.DataType == enumDataType.Int16)
                {
                    Int16[] outData;
                    short[] oriData = GetDataValue<short>(dataPrd, bandNum, datawidth, dataheight, dataxoffset, datayoffset);
                    AnaliysisDataPreprocess.MedianRead(oriData, new Size(datawidth, dataheight), outSize, out outData);
                    AnaliysisDataPreprocess.StandardDeviation(outData, out standardData);
                }
                else if (dataPrd.DataType == enumDataType.Byte)
                {
                    Byte[] outData;
                    Byte[] oriData = GetDataValue<Byte>(dataPrd, bandNum, datawidth, dataheight, dataxoffset, datayoffset);
                    AnaliysisDataPreprocess.MedianRead(oriData, new Size(datawidth, dataheight), outSize, out outData);
                    AnaliysisDataPreprocess.StandardDeviation(outData, out standardData);
                }
                double a = 1 ;        //检验标准化后的数据是否均值为0，标准差为1
                a = AnaliysisDataPreprocess.StandardDeviationCalc(standardData);
                return standardData;
            }
        }

        public static unsafe T[] GetDataValue<T>(IRasterDataProvider dataPrd, int bandNum,int width,int height,int xoffset=0,int yoffset=0)
        {
            int length = width * height;
            enumDataType dataType = dataPrd.DataType;
            IRasterBand band;
            switch(dataType)
            {
                case enumDataType.Float:
                    {
                        float[] buffer = new float[length];
                        band = dataPrd.GetRasterBand(bandNum);
                        fixed (float* ptr = buffer)
                        {
                            IntPtr bufferPtr = new IntPtr(ptr);
                            band.Read(xoffset, yoffset, width, height, bufferPtr, enumDataType.Float, width, height);
                        }
                        return buffer as T[];
                    }
                case enumDataType.Int16:
                    {
                        short[] buffer = new short[length];
                        band = dataPrd.GetRasterBand(bandNum);
                        fixed (short* ptr = buffer)
                        {
                            IntPtr bufferPtr = new IntPtr(ptr);
                            band.Read(xoffset, yoffset, width, height, bufferPtr, enumDataType.Int16, width, height);
                        }
                        return buffer as T[];
                    }
                case enumDataType.Byte:
                    {
                        Byte[] buffer = new Byte[width * height];
                        band = dataPrd.GetRasterBand(bandNum);
                        fixed (Byte* ptr = buffer)
                        {
                            IntPtr bufferPtr = new IntPtr(ptr);
                            band.Read(xoffset, yoffset, width, height, bufferPtr, enumDataType.Byte, width, height);
                        }
                        return buffer as T[];
                    }

            }
            return null;
        }

        #region 直方图统计

        public Dictionary<int, RasterQuickStatResult> FilesHistoStatBetween(string[] files, int[] bands, string[] fillValues, string min, string max, Action<int, string> progressCallback)
        {
            int b = bands[0];
            if (files.Length == 0)
                return null;
            if (!CheckFiles(files, b))
                throw new ArgumentException("输入文件错误！bands" + b + "大小或类型不一致！");
            int width = 0, height = 0, length = files.Length;
            enumDataType datatype = enumDataType.Unknow;
            int xoffset = 0, yoffset = 0;
            if (StatRegionSet.UseRegion)
            {
                PrjEnvelope RegionEnv = StatRegionSet.SelectedRegionEnvelope.PrjEnvelope;
                int[] dataPosL;
                PrjEnvelope dstmainPrj = null;
                if (CheckRegionIntersect(files[0], RegionEnv, out dataPosL,out dstmainPrj))
                {
                    xoffset = dataPosL[0];
                    yoffset = dataPosL[1];
                    width = dataPosL[2];
                    height = dataPosL[3];
                }
            }
            else if (StatRegionSet.UseVectorAOIRegion)
            {
                CloudParaFileStaticsAOI aoihist = new CloudParaFileStaticsAOI();
                return aoihist.FilesHistoStatAOI(files, bands, fillValues, min, max, progressCallback);
            }
            using (IRasterDataProvider dataPrd = GeoDataDriver.Open(files[0]) as IRasterDataProvider)
            {
                datatype = dataPrd.DataType;
                if (width == 0 || height == 0)
                {
                    width = dataPrd.Width;
                    height = dataPrd.Height;
                }
            }
            IRasterDataProvider arrayPrd = null;
            int count;
            if (progressCallback != null)
                progressCallback(3,"开始读取波段数据...");
            switch (datatype)
            {
                case enumDataType.Float:
                    float[][] rasterbandsF = new float[bands.Max()][];
                    rasterbandsF[b - 1] = GetHistArrayBetween<float>(files, b, width,height, fillValues, datatype, min, max, out count,xoffset,yoffset);
                    arrayPrd = new ArrayRasterDataProvider<float>("Array", rasterbandsF, count, 1);
                    break;
                case enumDataType.Int16:
                    short[][] rasterbandsS = new short[bands.Max()][];
                    rasterbandsS[b - 1] = GetHistArrayBetween<short>(files, b, width, height, fillValues, datatype, min, max, out count, xoffset, yoffset);
                    arrayPrd = new ArrayRasterDataProvider<short>("Array", rasterbandsS, count, 1);
                    break;
                case enumDataType.Byte:
                    Byte[][] rasterbandsB = new Byte[bands.Max()][];
                    rasterbandsB[b - 1] = GetHistArrayBetween<Byte>(files, b, width,height, fillValues, datatype, min, max, out count, xoffset, yoffset);
                    arrayPrd = new ArrayRasterDataProvider<Byte>("Array", rasterbandsB, count, 1);
                    break;
                default:
                    throw new ArgumentException("暂不支持" + datatype.ToString() + "类型的统计！");
            }
            try
            {
                if (arrayPrd.Width == 0 || arrayPrd.Height == 0)
                    throw new ArgumentException("创建待统计数据失败！");
                if (progressCallback != null)
                    progressCallback(5, "开始统计波段数据...");
                return DoStat(arrayPrd, bands, null, progressCallback);
            }
            catch
            {
                return null;
            }
            finally
            {
                if (arrayPrd != null)
                    arrayPrd.Dispose();
            }
        }

        public Dictionary<int, RasterQuickStatResult>  FilesHistoStat(string [] files,int [] bands,string [] fillValues,Action<int, string> progressCallback)
        {
            if (files.Length == 0 )
                return null;
            int width = 0, height=0,length = files.Length;
            enumDataType datatype = enumDataType.Unknow;
            foreach (int b in bands)
            {
                if (!CheckFiles(files, b))
                    throw new ArgumentException("输入文件错误！band" + b + "大小或类型不一致！");
            }
            int xoffset = 0, yoffset = 0;
            if (StatRegionSet.UseRegion)
            {
                PrjEnvelope RegionEnv = StatRegionSet.SelectedRegionEnvelope.PrjEnvelope;
                int[] dataPosL;
                PrjEnvelope dstmainPrj = null;
                if (CheckRegionIntersect(files[0], RegionEnv, out dataPosL, out dstmainPrj))
                {
                    xoffset = dataPosL[0];
                    yoffset = dataPosL[1];
                    width = dataPosL[2];
                    height = dataPosL[3];
                }
            }
            else if (StatRegionSet.UseVectorAOIRegion)
            {
                CloudParaFileStaticsAOI aoihist = new CloudParaFileStaticsAOI();
                return aoihist.FilesHistoStatAOI(files, bands, fillValues, null, null, progressCallback);
            }

            using (IRasterDataProvider dataPrd = GeoDataDriver.Open(files[0]) as IRasterDataProvider)
            {
                datatype = dataPrd.DataType;
                if (width ==0||height==0)
                {
                    width = dataPrd.Width;
                    height = dataPrd.Height;
                }
            }
            IRasterDataProvider arrayPrd=null;
            if (progressCallback != null)
                progressCallback(3, "开始读取波段数据...");
            switch (datatype)
            {
                case enumDataType.Float:
                    float[][] rasterbandsF = new float[bands.Max()][];
                    foreach (int b in bands)
                    {
                        rasterbandsF[b - 1] = GetHistArray<float>(files, b, width, height, fillValues, datatype, xoffset, yoffset);
                    }
                    arrayPrd = new ArrayRasterDataProvider<float>("Array", rasterbandsF, width * height, length);
                    break;
                case enumDataType.Int16:
                    short[][] rasterbandsS = new short[bands.Max()][];
                    foreach (int b in bands)
                    {
                        rasterbandsS[b - 1] = GetHistArray<short>(files, b, width, height, fillValues, datatype, xoffset, yoffset);
                    }
                    arrayPrd = new ArrayRasterDataProvider<short>("Array", rasterbandsS, width * height, length);
                    break;
                case enumDataType.Byte:
                    Byte[][] rasterbandsB= new Byte[bands.Max()][];
                    foreach (int b in bands)
                    {
                        rasterbandsB[b - 1] = GetHistArray<Byte>(files, b, width, height, fillValues, datatype, xoffset, yoffset);
                    }
                    arrayPrd = new ArrayRasterDataProvider<Byte>("Array", rasterbandsB, width * height, length);
                    break;
                    default:
                    throw new ArgumentException("暂不支持" + datatype.ToString() + "类型的统计！");
            }
            try
            {
                if (arrayPrd.Width == 0 || arrayPrd.Height ==0)
                    throw new ArgumentException("创建待统计数据失败！");
                if (progressCallback != null)
                    progressCallback(5, "开始统计波段数据...");
                return DoStat(arrayPrd, bands, null, progressCallback); 
            }
            finally
            {
                if(arrayPrd !=null)
                    arrayPrd.Dispose();
            }
        }

        public Dictionary<int, RasterQuickStatResult> MicapsHistoStat(string[] files, int[] bands, string[] fillValues, Action<int, string> progressCallback)
        {
            if (files.Length == 0)
                return null;
            if (bands==null||bands.Length<1)
                return null;
            int width = 0, height = files.Length;
            bool isMicaps = Path.GetExtension(files[0]) == ".000";
            if (isMicaps)
            {
                IRasterDataProvider arrayPrd = null;
                try
                {
                    if (progressCallback != null)
                        progressCallback(10, "开始读取波段数据...");
                    double[][] rasterbandsF = new double[bands.Max()][];
                    Envelope envelope = null;
                    if (StatRegionSet.UseRegion)
                        envelope = StatRegionSet.Envelope;
                    else if(StatRegionSet.UseVectorAOIRegion)
                        envelope = StatRegionSet.AOIEnvelope;
                    foreach (int b in bands)
                    {
                        rasterbandsF[b - 1] = MicapsDataProcess.GetMicapsData(files, b, fillValues, envelope);
                    }
                    foreach (int b in bands)
                    {
                        int length =rasterbandsF[b - 1].Length;
                        if (width == 0)
                            width = length;
                        else
                            width =  length<width?length:width;
                    }
                    if (progressCallback != null)
                        progressCallback(50, "开始生成待统计波段数据...");
                    arrayPrd = new ArrayRasterDataProvider<double>("Array", rasterbandsF, width, 1);
                    if (arrayPrd.Width == 0 || arrayPrd.Height == 0)
                        throw new ArgumentException("创建待统计数据失败！");
                    if (progressCallback != null)
                        progressCallback(5, "开始统计波段数据...");
                    if (progressCallback != null)
                        progressCallback(70, "开始统计波段数据,请稍候...");
                    return DoStat(arrayPrd, bands, null, progressCallback);
                }
                finally
                {
                    if (arrayPrd != null)
                        arrayPrd.Dispose();
                }
            }
            return null;
        }

        public Dictionary<int, RasterQuickStatResult> MicapsHistoStatBetween(string[] files, int[] bands, string[] fillValues, string min, string max, Action<int, string> progressCallback)
        {
            if (files.Length == 0)
                return null;
            if (bands == null || bands.Length < 1)
                return null;
            int width = 0, height = files.Length;
            bool isMicaps = Path.GetExtension(files[0]) == ".000";
            if (isMicaps)
            {
                IRasterDataProvider arrayPrd = null;
                try
                {
                    if (progressCallback != null)
                        progressCallback(5, "开始读取波段数据...");
                    double[][] rasterbandsF = new double[bands.Max()][];
                    Envelope envelope = null;
                    if (StatRegionSet.UseRegion)
                        envelope = StatRegionSet.Envelope;
                    else if (StatRegionSet.UseVectorAOIRegion)
                        envelope = StatRegionSet.AOIEnvelope;
                    foreach (int b in bands)
                    {
                        rasterbandsF[b - 1] = MicapsDataProcess.GetMicapsData(files, b, fillValues,min,max, envelope);
                    }
                    foreach (int b in bands)
                    {
                        int length = rasterbandsF[b - 1].Length;
                        if (width == 0)
                            width = length;
                        else
                            width = length < width ? length : width;
                    }
                    arrayPrd = new ArrayRasterDataProvider<double>("Array", rasterbandsF, width, 1);
                    if (arrayPrd.Width == 0 || arrayPrd.Height == 0)
                        throw new ArgumentException("创建待统计数据失败！");
                    if (progressCallback != null)
                        progressCallback(50, "开始统计波段数据...");
                    return DoStat(arrayPrd, bands, null, progressCallback);
                }
                finally
                {
                    if (arrayPrd != null)
                        arrayPrd.Dispose();
                }
            }
            return null;
        }

        protected T[] GetHistArray<T>(string[] filesL, int bandNumL,int width,int height, string[] fillValuesStr,enumDataType datatype,int xoffset=0,int yoffset=0)
        {
            int count = 0;
            int totalwidth = width * height;
            switch (datatype)
            {
                case enumDataType.Float:
                    {
                        float[] marixl = new float[totalwidth * filesL.Length];
                        float[] fillValues = GetFillValues<float>(fillValuesStr, enumDataType.Float);
                        float[] oriData = null;
                        for (int i = 0; i < filesL.Length; i++)
                        {
                            using (IRasterDataProvider dataPrd = GeoDataDriver.Open(filesL[i]) as IRasterDataProvider)
                            {
                                oriData = GetDataValue<float>(dataPrd, bandNumL, width, height, xoffset, yoffset);
                                for (int j = 0; j < totalwidth; j++)
                                {
                                    if (fillValues != null && fillValues.LongLength > 0 && fillValues.Contains(oriData[j]))
                                        continue;
                                    marixl[count++] = oriData[j];
                                }
                            }
                        }
                        return marixl as T[];
                    }
                case enumDataType.Int16:
                    {
                        short[] marixl = new short[totalwidth * filesL.Length];
                        short[] fillValues = GetFillValues<short>(fillValuesStr, enumDataType.Int16);
                        short[] oriData = null;
                        for (int i = 0; i < filesL.Length; i++)
                        {
                            using (IRasterDataProvider dataPrd = GeoDataDriver.Open(filesL[i]) as IRasterDataProvider)
                            {
                                //fillValues = GetFillValues<short>(fillValuesStr, enumDataType.Int16);
                                oriData = GetDataValue<short>(dataPrd, bandNumL, width, height, xoffset, yoffset);
                                for (int j = 0; j < totalwidth; j++)
                                {
                                    if (fillValues != null && fillValues.LongLength > 0 && fillValues.Contains(oriData[j]))
                                        continue;
                                    marixl[count++] = oriData[j];
                                }
                            }
                        }
                        return marixl as T[];
                    }
                case enumDataType.Byte:
                    {
                        Byte[] marixl = new Byte[totalwidth * filesL.Length];
                        Byte[] fillValues = GetFillValues<Byte>(fillValuesStr, enumDataType.Byte);
                        Byte[] oriData = null;
                        for (int i = 0; i < filesL.Length; i++)
                        {
                            using (IRasterDataProvider dataPrd = GeoDataDriver.Open(filesL[i]) as IRasterDataProvider)
                            {
                                //fillValues = GetFillValues<Byte>(fillValuesStr, enumDataType.Byte);
                                oriData = GetDataValue<Byte>(dataPrd, bandNumL, width, height, xoffset, yoffset);
                                for (int j = 0; j < totalwidth; j++)
                                {
                                    if (fillValues != null && fillValues.LongLength > 0 && fillValues.Contains(oriData[j]))
                                        continue;
                                    marixl[count++] = oriData[j];
                                }
                            }
                        }
                        return marixl as T[];
                    }
                    default:
                    return null;
            }
        }

        protected T[] GetHistArrayBetween<T>(string[] filesL, int bandNumL, int width, int height, string[] fillValuesStr, enumDataType datatype, string min, string max, out int count,int xoffset=0,int yoffset=0)
        {
            count = 0;
            switch (datatype)
            {
                case enumDataType.Float:
                    {
                        float[] marixl = new float[width *height* filesL.Length];
                        float minval = float.Parse(min);
                        float maxval = float.Parse(max);
                        float[] fillValues = GetFillValues<float>(fillValuesStr, enumDataType.Float);
                        float[] oriData;
                        for (int i = 0; i < filesL.Length; i++)
                        {
                            using (IRasterDataProvider dataPrd = GeoDataDriver.Open(filesL[i]) as IRasterDataProvider)
                            {
                                oriData = GetDataValue<float>(dataPrd, bandNumL, width, height, xoffset, yoffset);
                                for (int j = 0; j < width * height; j++)
                                {
                                    if (fillValues != null && fillValues.LongLength > 0 && fillValues.Contains(oriData[j]))
                                        continue;
                                    if (oriData[j] >= minval && oriData[j] <= maxval)
                                        marixl[count++] = oriData[j];
                                }
                            }
                        }
                        float[] matrixBetween = new float[count];
                        for (int b = 0; b < count; b++)
                        {
                            matrixBetween[b] = marixl[b];
                        }
                        return matrixBetween as T[];
                    }
                case enumDataType.Int16:
                    {
                        short[] marixl = new short[width * height * filesL.Length];
                        short minval = short.Parse(min);
                        short maxval = short.Parse(max);
                        short[] fillValues = GetFillValues<short>(fillValuesStr, enumDataType.Int16);
                        short[] oriData;
                        for (int i = 0; i < filesL.Length; i++)
                        {
                            using (IRasterDataProvider dataPrd = GeoDataDriver.Open(filesL[i]) as IRasterDataProvider)
                            {
                                oriData = GetDataValue<short>(dataPrd, bandNumL, width, height, xoffset, yoffset);
                                for (int j = 0; j < width * height; j++)
                                {
                                    if (fillValues != null && fillValues.LongLength > 0 && fillValues.Contains(oriData[j]))
                                        continue;
                                    if (oriData[j] >= minval && oriData[j] <= maxval)
                                        marixl[count++] = oriData[j];
                                }
                            }
                        }
                        short[] matrixBetween = new short[count];
                        for (int b = 0; b < count; b++)
                        {
                            matrixBetween[b] = marixl[b];
                        }
                        return matrixBetween as T[];
                    }
                case enumDataType.Byte:
                    {
                        Byte[] marixl = new Byte[width * height * filesL.Length];
                        Byte minval = Byte.Parse(min);
                        Byte maxval = Byte.Parse(max);
                        Byte[] fillValues = GetFillValues<Byte>(fillValuesStr, enumDataType.Byte);
                        Byte[] oriData;
                        for (int i = 0; i < filesL.Length; i++)
                        {
                            using (IRasterDataProvider dataPrd = GeoDataDriver.Open(filesL[i]) as IRasterDataProvider)
                            {
                                oriData = GetDataValue<Byte>(dataPrd, bandNumL, width, height, xoffset, yoffset);
                                for (int j = 0; j < width * height; j++)
                                {
                                    if (fillValues != null && fillValues.LongLength > 0 && fillValues.Contains(oriData[j]))
                                        continue;
                                    if (oriData[j] >= minval && oriData[j] <= maxval)
                                        marixl[count++] = oriData[j];
                                }
                            }
                        }
                        Byte[] matrixBetween = new Byte[count];
                        for (int b = 0; b < count; b++)
                        {
                            matrixBetween[b] = marixl[b];
                        }
                        return matrixBetween as T[];
                    }
                default:
                    return null;
            }
        }

        protected Dictionary<int, RasterQuickStatResult> DoStat(IRasterDataProvider dataProvider, int[] bandNos, int[] aoi, Action<int, string> progressCallback)
        {
            Dictionary<int, RasterQuickStatResult> results =null;
            try
            {
                IRasterQuickStatTool stat = new RasterQuickStatTool();
                if (progressCallback==null)
                    results = stat.Compute(dataProvider, aoi, bandNos, null);
                else
                {
                    results = stat.Compute(dataProvider, aoi, bandNos,
                        (idx, tip) =>
                        {
                            progressCallback(idx, "正在执行直方图统计...");
                        });
                }
                return results;
            }
            finally
            {
                if (progressCallback != null)
                {
                    progressCallback(0, "直方图统计完成！");
                }
            }
        }
        #endregion

        #region 相关系数计算
        public double FilesCorrelateStat(string[] filesL, int bandNumL, string[] fillvalueL, bool LisMicaps, string[] filesR, int bandNumR, string[] fillvalueR, bool RisMicaps, Action<int, string> progressCallback, out long scL, out long scR)
        {
            if (LisMicaps || RisMicaps)
                return MicapsCorrelateStat(filesL, bandNumL, fillvalueL, LisMicaps, filesR, bandNumR, fillvalueR, RisMicaps, progressCallback, out scL, out scR);
            double corre=0;
            scR = 0; scL = 0;
            if (filesL.Length == 0 || filesR.Length == 0 || filesR.Length != filesL.Length)
                throw new ArgumentException("待统计的左右场数据维度不一致！");
            if (!CheckFiles(filesL, bandNumL) || !CheckFiles(filesR, bandNumR))
                return corre;
            if (progressCallback != null)
                progressCallback(0, "开始读取数据...");
            if (StatRegionSet.UseVectorAOIRegion)
            {
                CloudParaFileStaticsAOI ca = new CloudParaFileStaticsAOI();
                return ca.FilesCorrelateStatAOI(filesL, bandNumL, fillvalueL, filesR, bandNumR, fillvalueR, progressCallback, out scL, out scR);
            }
            int widthl = 0, heightl = 0, lengthl = filesL.Length;
            int widthr = 0, heightr = 0, lengthr = filesR.Length;
            enumDataType datatype = enumDataType.Unknow;
            int xoffsetl = 0, yoffsetl = 0,xoffsetr = 0, yoffsetr = 0;
            if (StatRegionSet.UseRegion)
            {
                PrjEnvelope RegionEnv = StatRegionSet.SelectedRegionEnvelope.PrjEnvelope;
                int[] dataPosL, dataPosR;
                PrjEnvelope dstmainPrj = null;
                if (CheckRegionIntersect(filesL[0], RegionEnv, out dataPosL, out dstmainPrj))
                {
                    xoffsetl = dataPosL[0];
                    yoffsetl = dataPosL[1];
                    widthl = dataPosL[2];
                    heightl = dataPosL[3];
                }
                    if (CheckRegionIntersect(filesR[0], RegionEnv, out dataPosR, out dstmainPrj))
                {
                    xoffsetr = dataPosR[0];
                    yoffsetr = dataPosR[1];
                    widthr = dataPosR[2];
                    heightr = dataPosR[3];
                }
            }
            using (IRasterDataProvider dataPrd = GeoDataDriver.Open(filesL[0]) as IRasterDataProvider)
            {
                if (dataPrd == null)
                    throw new FileLoadException(filesL[0] + "打开失败！");
                datatype = dataPrd.DataType;
                if (widthl == 0 || heightl == 0)
                {
                    widthl = dataPrd.Width;
                    heightl = dataPrd.Height;
                }
            }
            double[] marixl = GetCorrelateArray(filesL, bandNumL, fillvalueL, datatype, widthl,heightl, out scL,xoffsetl,yoffsetl);
            using (IRasterDataProvider dataPrd = GeoDataDriver.Open(filesR[0]) as IRasterDataProvider)
            {
                if (dataPrd == null)
                    throw new FileLoadException(filesR[0] + "打开失败！");
                datatype = dataPrd.DataType;
                if (widthr == 0 || heightr == 0)
                {
                    widthr = dataPrd.Width;
                    heightr = dataPrd.Height;
                }
            }
            double[] marixr = GetCorrelateArray(filesR, bandNumR, fillvalueR, datatype, widthr , heightr, out scR,xoffsetr,yoffsetr);
            //计算相关系数
            CloudParaStat stat = new CloudParaStat();
            if (progressCallback != null)
                progressCallback(50, "开始计算相关系数...");
            double cor;// = stat.CalculateCorrelationCoefficient(marixl, marixr);
            cor = stat.CalculateCorrelationCoefficient(marixl, marixr, scL, scR);//取样本数最小
            if (progressCallback != null)
                progressCallback(100, "相关系数计算完成！");
            return cor;
        }

        protected double[] GetCorrelateArray(string[] filesL, int bandNumL, string[] fillValuesStr, enumDataType dataType, int width,int height, out long validCount,int xoffset=0,int yoffset=0)
        {
            int length = filesL.Length;
            int totalcount = width * height;
            validCount = 0;
            double[] validValue;
            switch (dataType)
            {
                case enumDataType.Float:
                    validValue = new double[totalcount * length];
                    for (int i = 0; i < length; i++)
                    {
                        using (IRasterDataProvider dataPrdi = GeoDataDriver.Open(filesL[i]) as IRasterDataProvider)
                        {
                            float[] oriData = GetDataValue<float>(dataPrdi, bandNumL, width,height,xoffset,yoffset);
                            float[] fillValues = GetFillValues<float>(fillValuesStr, enumDataType.Float);
                            for (int j = 0; j < totalcount; j++)
                            {
                                if (fillValues != null && fillValues.LongLength > 0 && fillValues.Contains(oriData[j]))
                                    continue;
                                validValue[validCount++] = oriData[j];
                            }
                        }
                    }
                    return validValue;
                case enumDataType.Int16:
                    validValue = new double[totalcount*length];
                    for (int i = 0; i < length; i++)
                    {
                        using (IRasterDataProvider dataPrdi = GeoDataDriver.Open(filesL[i]) as IRasterDataProvider)
                        {
                            short[] oriData = GetDataValue<short>(dataPrdi, bandNumL, width, height, xoffset, yoffset);
                            short[] fillValues = GetFillValues<short>(fillValuesStr, enumDataType.Int16);
                            for (int j = 0; j < totalcount; j++)
                            {
                                if (fillValues != null && fillValues.LongLength > 0 && fillValues.Contains(oriData[j]))
                                    continue;
                                validValue[validCount++] = oriData[j];
                            }
                        }
                    }
                    return validValue;
                case enumDataType.Byte:
                    validValue = new double[totalcount * length];
                    for (int i = 0; i < length; i++)
                    {
                        using (IRasterDataProvider dataPrdi = GeoDataDriver.Open(filesL[i]) as IRasterDataProvider)
                        {
                            Byte[] oriData = GetDataValue<Byte>(dataPrdi, bandNumL, width, height, xoffset, yoffset);
                            Byte[] fillValues = GetFillValues<Byte>(fillValuesStr, enumDataType.Byte);
                            for (int j = 0; j < totalcount; j++)
                            {
                                if (fillValues != null && fillValues.LongLength > 0 && fillValues.Contains(oriData[j]))
                                    continue;
                                validValue[validCount++] = oriData[j];
                            }
                        }
                    }
                    return validValue;
            }
            return null;
        }

        #region Micaps处理
        public double MicapsCorrelateStat(string[] filesL, int bandNumL, string[] fillvalueL, bool LisMicaps, string[] filesR, int bandNumR, string[] fillvalueR, bool RisMicaps, Action<int, string> progressCallback, out long scL, out long scR)
        {
            scR = 0; scL = 0;
            //if (filesL.Length == 0 || filesR.Length == 0 || filesR.Length != filesL.Length)
            //    throw new ArgumentException("待统计的左右场数据维度不一致！");
            if (progressCallback != null)
                progressCallback(0, "开始读取数据...");
            PrjEnvelope envelope = null;
            Envelope regionenv =null;
            if (StatRegionSet.UseRegion)
            {
                envelope = StatRegionSet.SelectedRegionEnvelope.PrjEnvelope;
                regionenv = StatRegionSet.Envelope;
            }
            else if (StatRegionSet.UseVectorAOIRegion)
            {
                envelope = StatRegionSet.AOIPrjEnvelope;
                regionenv = StatRegionSet.AOIEnvelope;
            }
            double[] marixl = null, marixr = null ;
            if (LisMicaps && !RisMicaps)//左场为micaps
                MicapsDataProcess.GetMatchedValues(filesR, bandNumR, fillvalueR, filesL, bandNumL, fillvalueL, envelope, out marixr, out marixl, out scR, out scL);
            else if(RisMicaps && !LisMicaps)
                MicapsDataProcess.GetMatchedValues(filesL, bandNumL, fillvalueL, filesR, bandNumR, fillvalueR, envelope, out marixl, out marixr, out scL, out scR);
            else//当两边都为micaps数据时，
            {
                MicapsDataProcess.GetMicapsDataAvg(filesL, bandNumL, fillvalueL, filesR, bandNumR, fillvalueR, out marixl, out marixr, out scL, out scR, regionenv);
            }
            //计算相关系数
            CloudParaStat stat = new CloudParaStat();
            if (progressCallback != null)
                progressCallback(50, "开始计算相关系数...");
            double cor;// = stat.CalculateCorrelationCoefficient(marixl, marixr);
            cor = stat.CalculateCorrelationCoefficient(marixl, marixr);//取样本数最小
            if (progressCallback != null)
                progressCallback(100, "相关系数计算完成！");
            return cor;
        }
        #endregion

        #endregion

        public static  T[] GetFillValues<T>(string[] fillValues, enumDataType dataType)
        {
            if (fillValues==null)
                return null;
            switch (dataType)
            {
                case enumDataType.Double:
                    {
                        List<double> buffer = new List<double>();
                        double result;
                        foreach (string fillvalue in fillValues)
                        {
                            if (double.TryParse(fillvalue, out result))
                                buffer.Add(result);
                        }
                        return buffer.ToArray() as T[];
                    }
                case enumDataType.Float:
                    {
                        List<float> buffer = new List<float>();
                        float result;
                        foreach (string fillvalue in fillValues)
                        {
                            if (float.TryParse(fillvalue, out result))
                                buffer.Add(result);
                        }
                        return buffer.ToArray() as T[];
                    }
                case enumDataType.Int16:
                    {
                        List<short> buffer = new List<short>();
                        short result;
                        foreach (string fillvalue in fillValues)
                        {
                            if (short.TryParse(fillvalue, out result))
                                buffer.Add(result);
                        }
                        return buffer.ToArray() as T[];
                    }
                case enumDataType.Byte:
                    {
                        List<Byte> buffer = new List<Byte>();
                        Byte result;
                        foreach (string fillvalue in fillValues)
                        {
                            if (Byte.TryParse(fillvalue, out result))
                                buffer.Add(result);
                        }
                        return buffer.ToArray() as T[];
                    }
            }
            return null;
        }

        #region EOF分解
        public string[] FilesEOFStat(string[] files, int bandNum, string bandname, string[] fillvalue, bool isMicaps, string outDir, Action<int, string> progressCallback, string[] Micapsfiles = null, string[] Micapsfillvalue = null, double leftRatio = 1)
        {
            //check
            if (files.Length == 0)
                return null;
            if (Path.GetExtension(files[0])!=".000"&&!CheckFiles(files, bandNum))
                return null;
            if (isMicaps)
                return FilesEOFStatOnlyMicaps(files, bandNum, bandname,fillvalue, outDir, progressCallback);
            else
            {
                string regionName ="all";
                PrjEnvelope envelope = null;
                if (StatRegionSet.UseRegion)
                {
                    envelope = StatRegionSet.SelectedRegionEnvelope.PrjEnvelope;
                    regionName = StatRegionSet.RegionName;
                }
                else if (StatRegionSet.UseVectorAOIRegion)
                {
                    envelope = StatRegionSet.AOIPrjEnvelope;
                    regionName = StatRegionSet.AOIName;
                }
                double[][] marixl = null;
                if (Micapsfiles == null)//整个栅格
                {
                    if (StatRegionSet.UseVectorAOIRegion)
                    {
                        CloudParaFileStaticsAOI ai =new CloudParaFileStaticsAOI();
                        return ai.FilesEOFStatAOI(files, bandNum, bandname, fillvalue, outDir, progressCallback, leftRatio);
                    }
                    float reslxL, reslyL;
                    int[] dataPosL;
                    PrjEnvelope dstleftPrj = null;
                    if (CheckRegionIntersect(files[0], envelope, out dstleftPrj, out reslxL, out reslyL, out dataPosL))
                    {
                        if (leftRatio < 1)
                            leftRatio = 1;
                        //根据采样系数计算采样后数据大小
                        if (progressCallback != null)
                            progressCallback(0, "开始计算待分解矩阵...");
                        int LWidth = (int)(dstleftPrj.Width / leftRatio / reslxL + 0.5f);
                        int LHeight = (int)(dstleftPrj.Height / leftRatio / reslyL + 0.5f);
                        Size leftSize = new Size(LWidth, LHeight);
                        CoordEnvelope subFileEnv = new CoordEnvelope(dstleftPrj.MinX, dstleftPrj.MaxX, dstleftPrj.MinY, dstleftPrj.MaxY);
                        StatRegionSet.SubRegionEnv = subFileEnv;
                        StatRegionSet.SubRegionOutSize = leftSize;
                        double[,] leftData = StandardReadData(files, bandNum, leftSize, dataPosL);          // ReadRastersDataToMarix(filesL, bandNumL, out outSize);
                        if (progressCallback != null)
                            progressCallback(15, "完成数据采样及标准化,开始EOF分解计算...");
                        CloudParaStat stat = new CloudParaStat();
                        return stat.AlglibEOF(leftData, outDir, bandname, progressCallback, false,null);
                    }
                    return null;
                }
                else////根据micaps数据抽点
                {
                    Envelope dstEnv = GetIntersectEnvolop(files, envelope);
                    if (dstEnv == null || dstEnv.Width <= 0 || dstEnv.Height <= 0)
                        throw new ArgumentException("设置的区域没有可分析的数据!");
                    List<ShapePoint> matchedpos = MicapsDataProcess.GetUnionStationPos(Micapsfiles, dstEnv);
                    if (matchedpos == null || matchedpos.Count < 1)
                        throw new ArgumentException("设置的区域没有匹配的站点观测数据!");
                    MicapsDataProcess.GetMatchedRasterValues(files, bandNum, fillvalue, matchedpos, out marixl);
                    double[,] leftData = StandardMicapsData(marixl);
                    if (progressCallback != null)
                        progressCallback(15, "完成数据采样及标准化,开始矩阵计算...");
                    CloudParaStat stat = new CloudParaStat();
                    return stat.AlglibEOF(leftData, outDir, bandname, progressCallback, true, matchedpos);
                }
            }
        }

        public string[] FilesEOFStatOnlyMicaps(string[] filesL, int bandNumL, string bandname,string[] fillvalueL,string outDir, Action<int, string> progressCallback)
        {
            //check
            if (!Directory.Exists(outDir))
                Directory.CreateDirectory(outDir);
            string regionName = "all";
            PrjEnvelope envelope = null;
            Envelope regionenv = null;
            if (StatRegionSet.UseRegion)
            {
                envelope = StatRegionSet.SelectedRegionEnvelope.PrjEnvelope;
                regionenv = StatRegionSet.Envelope;
                regionName = StatRegionSet.RegionName;
            }
            double[][] marixl = null;
            List<ShapePoint> matchedpos;
            MicapsDataProcess.GetMatchedPosAndValues(filesL, bandNumL, fillvalueL, regionenv, out marixl, out matchedpos);
            if (matchedpos == null || matchedpos.Count < 1)
                throw new ArgumentException("设置的区域没有匹配的站点观测数据!");
            double[,] leftData = StandardMicapsData(marixl);
            if (progressCallback != null)
                progressCallback(15, "完成数据采样及标准化,开始矩阵计算...");
            CloudParaStat stat = new CloudParaStat();
            //return stat.AlglibSVDWithMicaps(leftData, rightData, outDir, filesL[0], progressCallback);
            return stat.AlglibEOF(leftData, outDir, bandname, progressCallback, true, matchedpos);
        }


        #endregion
    }
}
