#region Version Info
/*========================================================================
* 功能概述：
* 
* 创建者：Administrator     时间：2014-4-1 16:09:51
* ------------------------------------------------------------------------
* 变更记录：
* 时间：                 修改者：                
* 修改说明：
* 
* ------------------------------------------------------------------------
* ========================================================================
*/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Core;
using System.Drawing;
using System.IO;
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.MIF.Prds.Comm;
using CodeCell.AgileMap.Core;
using System.Text.RegularExpressions;
using GeoDo.RSS.MicroWaveFYDataRetrieval;
using System.Windows.Forms;
namespace GeoDo.RSS.MIF.Prds.MWS
{
    /// <summary>
    /// 类名：SubProductHisDataSat
    /// 属性描述：历史周期统计数据出柱状图
    /// 创建者：LiXJ   创建日期：2014-4-1 16:09:51
    /// 修改者：             修改日期：
    /// 修改描述：
    /// 备注：
    /// </summary>
    public class SubProductHisDataSat:CmaMonitoringSubProduct
    {
        GeoDo.RSS.Core.VectorDrawing.AOIContainerLayer aoiContainer = null;
        private List<string[]> resultList = new List<string[]>();
        private Regex DataReg2 = new Regex(@"(?<year>\d{4})_(?<month>\d{2})_(?<year>\d{4})_(?<month>\d{1})", RegexOptions.Compiled);
        private string period = null;
        private string Iswinter = null;
        private string regionNames = null;
        public SubProductHisDataSat(SubProductDef subProductDef)
            : base(subProductDef)
        {

        }

        public override IExtractResult Make(Action<int, string> progressTracker,IContextMessage contexMessage)
        {
             if (_argumentProvider == null)
                return null;
            if (_argumentProvider.GetArg("AlgorithmName") == null)
                return null;
            if (_argumentProvider.GetArg("AlgorithmName").ToString() == "HistoryDataStatAlgorithm")
            {
                return HistoryDataStatAlgorithm(progressTracker);
            }
            return null;
        }
        private IExtractResult HistoryDataStatAlgorithm(Action<int, string> progressTracker)
        {
            //1、获取数据
            List<string> list = ExportManager.GetInstance().List;
            string[] fname = list.ToArray();
            //再加一上基于 同期统计数据
            StatisticResultManager manager = StatisticResultManager.GetInstance();
            List<string> list2 = manager.GetFilePathFromList();    //得到的是基于数据查询的数据又做的统计数据 
            if (list2.Count != 0)
            {
                fname = list2.ToArray();
                period = "yes";
                Match m = DataReg2.Match(fname[0]);
                if (m.Success)
                    Iswinter = "yes";
            }
            //这里要对文件进行一下裁切//处理后的数据存储路径
            string savePath = _argumentProvider.GetArg("HistoryDataSave") as string;
            regionNames = _argumentProvider.GetArg("regionNames") as string;
            string orbitType = _argumentProvider.GetArg("OrbitType") as string;
            string Str = null;
            if (orbitType == "Ascend")
                Str = "_A_";
            if (orbitType == "Descend")
                Str = "_D_";
            //同期统计计算传出来的文件没有分升降轨，这里进行区分
            List<string> fnamelist = new List<string>();
            foreach (string file in fname)
            {
                if (Path.GetFileName(file).Contains(Str))
                {
                    fnamelist.Add(file);
                }  
            }
            fname = fnamelist.ToArray();
            //2.确定选择区域，没有选或者中国区域不要用裁，如果是其地区首先裁切，并且放到指定文件夹下
            aoiContainer = new GeoDo.RSS.Core.VectorDrawing.AOIContainerLayer();
            MulRegionsClip muticlip = new MulRegionsClip();  //裁切
            string regionsname = "";
            IStatResult fresult = null;
            using (frmStatSubRegionTemplates frm = new frmStatSubRegionTemplates())
            {
                frm.listView1.MultiSelect = true;
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    Feature[] fets = frm.GetSelectedFeatures();
                    if (fets == null)
                    {
                        #region 没选中任何矢量
                        regionsname = "全国";
                        resultList.Clear();
                        //体积文件SVOL存放路径
                        string svolPath = savePath + "\\"+ regionsname + "\\" + "体积文件";
                        if (!System.IO.Directory.Exists(svolPath))
                            System.IO.Directory.CreateDirectory(svolPath);
                        //统计出的各个文件夹的数据放在txt文本里
                        string statxt = savePath + "\\" + regionsname + "\\" + regionsname + ".txt";
                        FileStream fauto = new FileStream(statxt, FileMode.Create, FileAccess.Write);
                        StreamWriter rauto = new StreamWriter(fauto);
                        //直接对当前中国区域数据做统计
                       //提取年至avg之间的字符
                        string filetime = "";
                        string startDate = "";
                        string endDate = "";
                        foreach (string infile in fname)
                        { 
                            //解析文件名确定时间
                            string filename = Path.GetFileNameWithoutExtension(infile);
                            Regex DataReg = new Regex(@"(?<year>\d{4})", RegexOptions.Compiled);
                            Match m = DataReg.Match(filename);
                            string year = "";
                            if (m.Success)
                                year = m.Value;
                            //提取年至avg之间的字符
                            filetime = filename.Substring(filename.IndexOf(year), filename.Length - 3 - filename.IndexOf(year));
                            string[] mxchars = filetime.Split(new char[] { '_' });
                            string mx = "";//月+旬
                            if (mxchars.Length == 5 && !filename.Contains("Xun")) //手动统计叫 MWS_MWSD_China_Month_0SD_D_1987_11_1988_2_avg.dat 冬季
                            {                        
                                mx = year;
                            }
                            else if (mxchars.Length == 4)//          MWS_MWSD_China_Month_0SD_D_1987_1988_2_avg.dat
                            {
                                //mx = year + "_" + mx[0] + "年" + mx[2] + "月";
                                mx = mx[2] + "月";
                            }
                            else if (mxchars.Length == 5 && filename.Contains("Xun"))//          MWS_MWSD_China_Month_0SD_D_1987_1988_2_1_avg.dat
                            {
                                if (mxchars[3] == "1")
                                    mx = mxchars[2] + "月" + "上旬";
                                if (mxchars[3] == "2")
                                    mx = mxchars[2] + "月" + "中旬";
                                if (mxchars[3] == "3")
                                    mx = mxchars[2] + "月" + "下旬";
                            }
                            else
                            {
                                if (filename.Contains("Xun"))
                                {
                                    if (mxchars[2] == "1")
                                        mx = mxchars[1] + "月" + "上旬";
                                    if (mxchars[2] == "2")
                                        mx = mxchars[1] + "月" + "中旬";
                                    if (mxchars[2] == "3")
                                        mx = mxchars[1] + "月" + "下旬";
                                }
                                if (filename.Contains("Month"))
                                {
                                    mx = mxchars[1] + "月";
                                }
                            }
                            #region  获得excel文件名上时间标识
                            if (infile == fname[0])
                            {
                                startDate = year + mx;
                            }
                            if (infile == fname[fname.Length - 1])
                            {
                                endDate = year + mx;
                            }
                            #endregion
                            string sweVolFile = svolPath + "\\" + Path.GetFileName(infile).Replace("MSWE", "SVOL");
                            if (!File.Exists(sweVolFile))
                            {
                                IFileExtractResult sweVolResult = ComputeSnowSWEVOL(infile, sweVolFile);
                                sweVolFile = sweVolResult.FileName;
                            }
                            //只返回一个体积和
                            double pixelSum;
                            using (IRasterDataProvider inRaster = RasterDataDriver.Open(sweVolFile) as IRasterDataProvider)
                            {
                                pixelSum = CompuCurPixel(sweVolFile);
                            }
                            resultList.Add(new string[] { mx, (pixelSum/100000000).ToString() });
                            rauto.WriteLine(mx + " " + (pixelSum / 100000000).ToString());
                        }
                        rauto.Close();
                        fauto.Close();
                        string coluString = regionsname;
                        string sentitle = "统计日期：" + DateTime.Now.ToShortDateString();
                        string[] columns = new string[] { coluString, "累计雪水当量体积(亿立方米)" };
                        fresult = new StatResult(sentitle, columns, resultList.ToArray());
                        string outputIdentify = regionsname;// _argumentProvider.GetArg("OutFileIdentify").ToString();
                        string title = coluString + "时间序列雪水当量体积统计"+ startDate + "_" + endDate ;
                        string fileexcel = StatResultToFile(new string[] { fname[0] }, fresult, "MWS", outputIdentify, title, null, 1, true, 1);
                        string newexcelfile = Path.Combine(svolPath, Path.GetFileNameWithoutExtension(fname[0]).Replace(filetime, startDate + "_" + endDate) + ".XLSX");
                        FileInfo fi = new FileInfo(fileexcel);
                        fi.MoveTo(newexcelfile);
                        IFileExtractResult res = new FileExtractResult(_subProductDef.Identify, newexcelfile, false);
                        return new FileExtractResult(outputIdentify, newexcelfile);
                    }
                    #endregion
                    else
                    {
                        string fieldName;
                        string shapeFilename;
                        int fieldIndex = -1;
                        List<string> fieldValues = new List<string>();
                        fets = frm.GetStatFeatures(out fieldName, out shapeFilename, out fieldIndex);
                        string chinafieldValue = fets[0].GetFieldValue(fieldIndex);
                        //提取年至avg之间的字符
                        string filetime = "";
                        string startDate = "";
                        string endDate = "";
                        if (chinafieldValue == "中国") 
                        {
                            #region 选中中国矢量
                            //直接对当前中国区域数据做统计
                            regionsname = "全国"; 
                            resultList.Clear();
                            //体积文件SVOL存放路径
                            string svolPath = savePath + "\\" + regionsname + "\\" + "体积文件";
                            if (!System.IO.Directory.Exists(svolPath))
                                System.IO.Directory.CreateDirectory(svolPath);
                            //统计出的各个文件夹的数据放在txt文本里
                            string statxt = savePath + "\\" + regionsname + "\\" + regionsname + ".txt";
                            FileStream fauto = new FileStream(statxt, FileMode.Create, FileAccess.Write);
                            StreamWriter rauto = new StreamWriter(fauto);
                            //直接对当前中国区域数据做统计
                            foreach (string infile in fname)
                            {
                                //解析文件名确定时间
                                string filename = Path.GetFileNameWithoutExtension(infile);
                                Regex DataReg = new Regex(@"(?<year>\d{4})", RegexOptions.Compiled);
                                Match m = DataReg.Match(filename);
                                string year = "";
                                if (m.Success)
                                    year = m.Value;
                                //提取年至avg之间的字符
                                filetime = filename.Substring(filename.IndexOf(year), filename.Length - 3 - filename.IndexOf(year));
                                string[] mxchars = filetime.Split(new char[] { '_' });
                                string mx = "";//月+旬
                                if (mxchars.Length == 5 && !filename.Contains("Xun")) //手动统计叫 MWS_MWSD_China_Month_0SD_D_1987_11_1988_2_avg.dat 冬季
                                {
                                    mx = year;
                                }
                                else if (mxchars.Length == 4)//          MWS_MWSD_China_Month_0SD_D_1987_1988_2_avg.dat
                                {
                                    //mx = year + "_" + mx[0] + "年" + mx[2] + "月";
                                    mx = mx[2] + "月";
                                }
                                else if (mxchars.Length == 5 && filename.Contains("Xun"))//          MWS_MWSD_China_Month_0SD_D_1987_1988_2_1_avg.dat
                                {
                                    if (mxchars[3] == "1")
                                        mx = mxchars[2] + "月" + "上旬";
                                    if (mxchars[3] == "2")
                                        mx = mxchars[2] + "月" + "中旬";
                                    if (mxchars[3] == "3")
                                        mx = mxchars[2] + "月" + "下旬";
                                }

                                else
                                {
                                    if (filename.Contains("Xun"))
                                    {
                                        if (mxchars[2] == "1")
                                            mx = mxchars[1] + "月" + "上旬";
                                        if (mxchars[2] == "2")
                                            mx = mxchars[1] + "月" + "中旬";
                                        if (mxchars[2] == "3")
                                            mx = mxchars[1] + "月" + "下旬";
                                    }
                                    if (filename.Contains("Month"))
                                    {
                                        mx = mxchars[1] + "月";
                                    }
                                }
                                //获得excel文件名上的时间标识
                                #region
                                if (infile == fname[0])
                                {
                                    startDate = year + mx;
                                }
                                if (infile == fname[fname.Length - 1])
                                {
                                    endDate = year + mx;
                                }
                                #endregion
                                string sweVolFile = svolPath + "\\" + Path.GetFileName(infile).Replace("MSWE", "SVOL");
                                if (!File.Exists(sweVolFile))
                                {
                                    IFileExtractResult sweVolResult = ComputeSnowSWEVOL(infile, sweVolFile);
                                    sweVolFile = sweVolResult.FileName;
                                }
                                //只返回一个体积和
                                double pixelSum;
                                using (IRasterDataProvider inRaster = RasterDataDriver.Open(sweVolFile) as IRasterDataProvider)
                                {
                                    pixelSum = CompuCurPixel(sweVolFile);
                                }
                                resultList.Add(new string[] { mx, (pixelSum / 100000000).ToString() });

                                rauto.WriteLine(mx + " " + (pixelSum / 100000000).ToString());
                            }
                            rauto.Close();
                            fauto.Close();
                            string coluString = regionsname;
                            string sentitle = "统计日期：" + DateTime.Now.ToShortDateString();
                            string[] columns = new string[] { coluString, "累计雪水当量体积(亿立方米)" };
                            fresult = new StatResult(sentitle, columns, resultList.ToArray());
                            string outputIdentify = regionsname;// _argumentProvider.GetArg("OutFileIdentify").ToString();
                            string title = coluString + "时间序列雪水当量体积统计";
                            string fileexcel = StatResultToFile(new string[] { fname[0] }, fresult, "MWS", outputIdentify, title, null, 1, true, 1);
                            string newexcelfile = Path.Combine(svolPath, Path.GetFileNameWithoutExtension(fname[0]).Replace(filetime, startDate + "_" + endDate) + ".XLSX");
                            FileInfo fi = new FileInfo(fileexcel);
                            fi.MoveTo(newexcelfile);
                            IFileExtractResult res = new FileExtractResult(_subProductDef.Identify, newexcelfile, false);
                            return new FileExtractResult(outputIdentify, newexcelfile);
                        }
                        #endregion
                        else
                        {
                            #region 选中非中国矢量
                            resultList.Clear();
                            foreach (Feature fet in fets)
                            {
                                fieldValues.Add(fet.GetFieldValue(fieldIndex)); //获得选择区域名称
                                aoiContainer.AddAOI(fet);
                            }
                            foreach (string region in fieldValues)
                            {
                                regionsname += region;
                            }
                            if (regionsname.Contains("西藏") && regionsname.Contains("青海"))
                                regionsname = "青藏地区";
                            if (!string.IsNullOrEmpty(regionNames))
                                regionsname = regionNames.Trim();
                            //创建裁切路径
                            string clipsave = savePath + "\\" + regionsname + "\\" + "裁切";
                            if (!System.IO.Directory.Exists(clipsave))//如果不存在这个路径
                                System.IO.Directory.CreateDirectory(clipsave);
                            //体积文件SVOL存放路径
                            string svolPath = savePath + "\\" + regionsname + "\\" + "体积文件";
                            if (!System.IO.Directory.Exists(svolPath))
                                System.IO.Directory.CreateDirectory(svolPath);
                            //统计出的各个文件夹的数据放在txt文本里
                            string statxt = savePath + "\\" + regionsname + "\\" + regionsname + ".txt";
                            FileStream fauto = new FileStream(statxt, FileMode.Create, FileAccess.Write);
                            StreamWriter rauto = new StreamWriter(fauto);
                            string excelYear = "";
                            string file1Time = "";
                            foreach (string infile in fname)
                            {
                                //解析文件名确定时间
                                string filename = Path.GetFileNameWithoutExtension(infile);
                                Regex DataReg = new Regex(@"(?<year>\d{4})", RegexOptions.Compiled);
                                Match m = DataReg.Match(filename);
                                string year = "";
                                if (m.Success)
                                    year = m.Value;
                                //提取年至avg之间的字符  
                                filetime = filename.Substring(filename.IndexOf(year), filename.Length - 3 - filename.IndexOf(year));
                                string[] mxchars = filetime.Split(new char[] { '_' });
                                string mx = "";//月+旬
                                if (mxchars.Length == 5 && !filename.Contains("Xun")) // MWS_MWSD_China_Month_0SD_D_1987_11_1988_2_avg.dat 冬季
                                {
                                    mx = year;
                                    excelYear = year + "年-" + mxchars[2] + "年冬季";
                                }
                                else if (mxchars.Length == 4 && !filename.Contains("Xun"))//          MWS_MWSD_China_Month_0SD_D_1987_1988_2_avg.dat
                                {
                                       
                                    mx = mxchars[2] + "月";
                                    excelYear = year + "年-" + mxchars[1] + "年";
                                }
                                else if (mxchars.Length == 5 && filename.Contains("Xun"))//          MWS_MWSD_China_Xun_0SD_D_1987_1988_2_1_avg.dat
                                {
                                    if (mxchars[3] == "1")
                                        mx = mxchars[2] + "月" + "上旬";
                                    if (mxchars[3] == "2")
                                        mx = mxchars[2] + "月" + "中旬";
                                    if (mxchars[3] == "3")
                                        mx = mxchars[2] + "月" + "下旬";
                                    excelYear = year + "年-" + mxchars[1] + "年";
                                }

                                else
                                {
                                    if (filename.Contains("Xun")) //旬MWS_MSWE_China_Xun_SWE_A_2011_1_1_avg.dat
                                    {
                                        if (mxchars[2] == "1")
                                            mx = mxchars[1] + "月" + "上旬";
                                        if (mxchars[2] == "2")
                                            mx = mxchars[1] + "月" + "中旬";
                                        if (mxchars[2] == "3")
                                            mx = mxchars[1] + "月" + "下旬";
                                        excelYear = year + "年";
                                    }
                                    if (filename.Contains("Month"))//月MWS_MSWE_China_Month_SWE_A_2011_1_avg.dat
                                    {
                                        mx = mxchars[1] + "月";
                                        excelYear = year + "年";
                                    }
                                }
                                #region 获得excel文件名上的时间标识
                                if (infile == fname[0])
                                {
                                    startDate = year;
                                    file1Time = filetime;
                                }
                                if (infile == fname[fname.Length - 1])
                                {
                                    endDate = year;
                                }
                                #endregion
                                string newclipfile = Path.Combine(clipsave, Path.GetFileName(infile).Replace("China", regionsname));
                                if (!File.Exists(newclipfile))
                                {
                                    string clipfile = muticlip.MutiRegionsClip(infile, aoiContainer, clipsave);
                                    string hdrfile = Path.GetDirectoryName(clipfile) + "\\" + Path.GetFileNameWithoutExtension(clipfile) + ".hdr";
                                    //重命名
                                    //string newclipfile = Path.Combine(Path.GetDirectoryName(clipfile), Path.GetFileName(infile).Replace("China", regionsname));
                                    string newhdrfile = Path.Combine(Path.GetDirectoryName(clipfile), Path.GetFileNameWithoutExtension(infile).Replace("China", regionsname) + ".hdr");
                                    FileInfo fi = new FileInfo(clipfile);
                                    fi.MoveTo(newclipfile);
                                    FileInfo fihdr = new FileInfo(hdrfile);
                                    fihdr.MoveTo(newhdrfile);
                                }
                                string sweVolFile = svolPath + "\\" + Path.GetFileName(newclipfile).Replace("MSWE", "SVOL");
                                if (!File.Exists(sweVolFile))
                                {
                                    IFileExtractResult sweVolResult = ComputeSnowSWEVOL(newclipfile, sweVolFile);
                                    sweVolFile = sweVolResult.FileName;
                                }
                                //只返回一个体积和
                                double pixelSum;
                                using (IRasterDataProvider inRaster = RasterDataDriver.Open(sweVolFile) as IRasterDataProvider)
                                {
                                    pixelSum = CompuCurPixel(sweVolFile);
                                }
                                resultList.Add(new string[] { mx, (pixelSum / 100000000).ToString() });

                                rauto.WriteLine(mx + " " + (pixelSum / 100000000).ToString());
                            }
                            rauto.Close();
                            fauto.Close();
                            string coluString = regionsname;
                            string sentitle = "统计日期：" + DateTime.Now.ToShortDateString();
                            string[] columns = new string[] { coluString, "累计雪水当量体积(亿立方米)" };
                            fresult = new StatResult(sentitle, columns, resultList.ToArray());
                            string outputIdentify = regionsname;// _argumentProvider.GetArg("OutFileIdentify").ToString();
                            //string title = coluString + excelYear+ "雪水当量体积统计";
                            string title = coluString + startDate + "-" + endDate + "雪水当量体积统计";
                            string fileexcel = StatResultToFile(new string[] { fname[0] }, fresult, "MWS", outputIdentify, title, null, 1, true, 1);
                            string newexcelfile = Path.Combine(svolPath, Path.GetFileNameWithoutExtension(fname[0]).Replace(file1Time, startDate + "_" + endDate) + ".XLSX");
                            FileInfo fi1 = new FileInfo(fileexcel);
                            fi1.MoveTo(newexcelfile);
                            IFileExtractResult res = new FileExtractResult(_subProductDef.Identify, newexcelfile, false);
                            return new FileExtractResult(outputIdentify, newexcelfile);
                            #endregion
                        }
                    }
                }
                //没有点确定
                else                              
                {
                    return null;
                }
            }
        }
        private double CompuCurPixel(string rasterFileName)
        {
            IRasterDataProvider inRaster = RasterDataDriver.Open(rasterFileName) as IRasterDataProvider;
            ArgumentProvider ap = new ArgumentProvider(inRaster, null);
            RasterPixelsVisitor<float> visitor = new RasterPixelsVisitor<float>(ap);
            double result = 0;
            visitor.VisitPixel(new int[] { 1 }, (index, values) =>
            {
                if (values[0] > 0)
                    result += values[0];
            });
            return result;
        }
        /// <summary>
        /// 计算雪水当量体积
        /// </summary>
        /// <param name="swefilename">输入文件名</param>
        /// <param name="sweVolFile">输出文件名</param>
        /// <returns></returns>
        private IFileExtractResult ComputeSnowSWEVOL(string swefilename, string sweVolFile)
        {
            List<RasterMaper> rms = null;
            IRasterDataProvider outRaster = null;
            RasterProcessModel<float, float> rfr = null;
            try
            {
                rms = new List<RasterMaper>();
                IRasterDataProvider inRaster1 = GeoDataDriver.Open(swefilename) as IRasterDataProvider;
                //计算文件中每个像元的面积
                int width = inRaster1.Width;
                float maxLat = (float)inRaster1.CoordEnvelope.MaxY;
                float res = inRaster1.ResolutionX;
                int row = inRaster1.Height;
                RasterMaper fileIn1 = new RasterMaper(inRaster1, new int[] { 1 });
                rms.Add(fileIn1);
                outRaster = CreateOutRaster(sweVolFile, enumDataType.Float, rms.ToArray(), inRaster1.ResolutionX);
                RasterMaper fileOut = new RasterMaper(outRaster, new int[] { 1 });
                RasterMaper[] fileIns = rms.ToArray();
                RasterMaper[] fileOuts = new RasterMaper[] { fileOut };
                rfr = new RasterProcessModel<float, float>();
                rfr.SetRaster(fileIns, fileOuts);
                rfr.RegisterCalcModel(new RasterCalcHandler<float, float>((rvInVistor, rvOutVistor, aoi) =>
                {
                    if (rvInVistor[0].RasterBandsData[0] != null)
                    {
                        int dataLength = rvInVistor[0].SizeY * rvInVistor[0].SizeX;
                        float[] swetmpVol = new float[dataLength];
                        for (int i = 0; i < dataLength; i++)
                        {
                            row = (int)(i / rvInVistor[0].SizeX) + 1;
                            float area = ComputePixelArea(row, maxLat, res);
                            //面积由平方公里转换为平方米，雪水当量mm转换为m
                            //swetmpVol[i] = rvInVistor[0].RasterBandsData[0][i] * area * 1000000.0f * 0.001f;
                            if (rvInVistor[0].RasterBandsData[0][i] != -999.0f)
                            {
                                if (rvInVistor[0].RasterBandsData[0][i] < 5.0f && rvInVistor[0].RasterBandsData[0][i] > 0.00001f) 
                                    rvInVistor[0].RasterBandsData[0][i] = 5.0f;
                                swetmpVol[i] = rvInVistor[0].RasterBandsData[0][i] * area * 1000.0f;
                            }
                            rvOutVistor[0].RasterBandsData[0][i] = swetmpVol[i];
                        }

                    }
                }));
                rfr.Excute();
                IFileExtractResult res1 = new FileExtractResult(_subProductDef.Identify, sweVolFile, true);
                res1.SetDispaly(false);
                return res1;
            }
            finally
            {
                if (outRaster != null)
                    outRaster.Dispose();
                if (rms != null && rms.Count > 0)
                {
                    foreach (RasterMaper rm in rms)
                    {
                        if (rm.Raster != null)
                            rm.Raster.Dispose();
                    }
                }
            }
        }
        public float ComputePixelArea(int row, float maxLat, float resolution)
        {
            float lat = maxLat - row * resolution;
            float a = 6378.137f;
            float c = 6356.7523142f;
            float latComputeFactor = 111.13f;
            float factor = (float)Math.Pow(Math.Tan(lat * Math.PI / 180d), 2);
            float lon = (float)(resolution * 2 * Math.PI * a * c * Math.Sqrt(1 / (c * c + a * a * factor)) / 360d);
            return lon * latComputeFactor * resolution;
        }

        //创建输出删格文件
        protected IRasterDataProvider CreateOutRaster(string outFileName, enumDataType dataType, RasterMaper[] inrasterMaper, float resolution)
        {
            IRasterDataDriver raster = RasterDataDriver.GetDriverByName("MEM") as IRasterDataDriver;
            CoordEnvelope outEnv = null;
            foreach (RasterMaper inRaster in inrasterMaper)
            {
                if (outEnv == null)
                    outEnv = inRaster.Raster.CoordEnvelope;
                else
                    outEnv = outEnv.Intersect(inRaster.Raster.CoordEnvelope);
            }
            float resX, resY;
            if (resolution != 0f)
            {
                resX = resolution;
                resY = resolution;
            }
            else
            {
                resX = inrasterMaper[0].Raster.ResolutionX;
                resY = inrasterMaper[0].Raster.ResolutionY;
            }
            int width = (int)(Math.Round(outEnv.Width / resX));
            int height = (int)(Math.Round(outEnv.Height / resY));
            string mapInfo = outEnv.ToMapInfoString(new Size(width, height));
            RasterDataProvider outRaster = raster.Create(outFileName, width, height, 1, dataType, mapInfo) as RasterDataProvider;
            return outRaster;
        }
    }
}
