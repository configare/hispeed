#region Version Info
/*========================================================================
* 功能概述：
* 
* 创建者：Administrator     时间：2014-3-19 17:46:41
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
using System.Text.RegularExpressions;
using System.Drawing;
using System.IO;
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.MIF.Prds.Comm;
using GeoDo.RSS.Core.RasterDrawing;
using GeoDo.RSS.MicroWaveFYDataRetrieval;
using GeoDo.RSS.Core.DrawEngine;
using CodeCell.AgileMap.Core;
using System.Windows.Forms;
using GeoDo.MicroWaveSnow.SNWParaStat;

namespace GeoDo.RSS.MIF.Prds.MWS
{
    /// <summary>
    /// 类名：SubProductJPStat
    /// 属性描述： 距平分析柱状图
    /// 创建者：lxj  创建日期：2014-3-19 17:46:41
    /// 修改者：             修改日期：
    /// 修改描述：
    /// 备注：
    /// </summary>
    public class SubProductJPStat : CmaMonitoringSubProduct
    {
        private IContextMessage _contextMessage = null;
        GeoDo.RSS.Core.VectorDrawing.AOIContainerLayer aoiContainer = null;
        private List<string> toBarfiles = new List<string>();//用于出柱状图的files  是全国数据或裁切的某地区数据
        private List<string[]> resultList = new List<string[]>();
        IStatResult fresult = null;
        string title = "";
        private string regionNames = null;
        private string prdType = null;
        public SubProductJPStat(SubProductDef subProductDef)
            : base(subProductDef)
        {
            _identify = subProductDef.Identify;
            _name = subProductDef.Name;
            if (subProductDef.Algorithms != null)
            {
                _algorithmDefs = subProductDef.Algorithms.ToList();
            }
            AOITemplateStat<float> aoiTempStat = new AOITemplateStat<float>();
        }

        public override IExtractResult Make(Action<int, string> progressTracker)
        {
             if (_argumentProvider == null)
                return null;
            if (_argumentProvider.GetArg("AlgorithmName") == null)
                return null;
            if (_argumentProvider.GetArg("AlgorithmName").ToString() == "JPAnaStatAlgorithm")
            {
                return JPAnaStatAlgorithm(progressTracker);
            }
            return null; 
        }

        private IExtractResult JPAnaStatAlgorithm(Action<int, string> progressTracker)
        {
            string[] inputCurrentFiles = null;//= ExportManager.GetInstance().List.ToArray();//GetStringArray("RasterCurrentFile"); 由查询那里获得
            string[] inputHistoryFiles = null;
            List<string> list1 = ExportManager.GetInstance().List;//得到的是从数据库里查询出来的周期数据
            StatisticResultManager manager = StatisticResultManager.GetInstance();
            List<StatisticResult> list = manager.List;
            List<string> list2 = manager.GetFilePathFromList();    //得到的是基于数据查询的数据又做的统计数;
            #region 获得目标区域
            regionNames = _argumentProvider.GetArg("regionNames") as string;
            aoiContainer = new GeoDo.RSS.Core.VectorDrawing.AOIContainerLayer();
            string fieldName;
            string shapeFilename;
            int fieldIndex = -1;
            List<string> fieldValues = new List<string>();
            string regionsname = "";
            using (frmStatSubRegionTemplates frm = new frmStatSubRegionTemplates())
            {
                frm.listView1.MultiSelect = true;
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    Feature[] fets = frm.GetSelectedFeatures();
                    fets = frm.GetStatFeatures(out fieldName, out shapeFilename, out fieldIndex);
                    if (fets == null)
                    {
                        aoiContainer = null;
                        regionsname = "全国";
                    }
                    else
                    {
                        string chinafieldValue = fets[0].GetFieldValue(fieldIndex);
                        if (chinafieldValue == "中国")
                        {
                            aoiContainer = null;
                            regionsname = "全国";
                        }
                        else
                        {
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

                        }
                    }
                }
                else
                {
                    return null;
                }
            }
            #endregion
            Regex DataReg = new Regex(@"(?<year>\d{4})", RegexOptions.Compiled);
            //处理后的数据存储路径
            string savePath = _argumentProvider.GetArg("HistoryDataSave") as string;
            string jpsavepath = savePath + "\\" + "距平分析";
            if (!System.IO.Directory.Exists(jpsavepath))//如果不存在这个路径
                System.IO.Directory.CreateDirectory(jpsavepath);
            //参数设置
            List<string> paraset = _argumentProvider.GetArg("paraSet") as List<string>;
            string[] paras = paraset.ToArray();
            //先裁切分别算每个文件的雪水当量体积后再相减
             string Nostr = ""; //排除非选择的轨道数据
            if (paras[0] == "Ascend")
                Nostr = "_D_";
            if (paras[0] == "Descend")
                Nostr = "_A_";
            if (paras[1] == "winter")   // inputCurrentFiles = manager.GetFilePathFromList() 冬季的数所是统计出来的。
            {                           // inputHistoryFiles  需要再重新计算一次  *** 这是多对一计算距平
                //比如说要算2000-2013年冬季的距平，需要先分别算出每年冬季的值，用这些值算总的均值。再分别用每年冬季减去总冬季值。
                List<string> files = new List<string>();
                foreach (string file in list2)
                {
                    if (!Path.GetFileName(file).Contains(Nostr))
                        files.Add(file);
                    if (file.Contains("MWSE"))
                        prdType = "SWE";
                    else
                        prdType = "SD";
                }
                inputCurrentFiles = files.ToArray();
                string outHistoryFile = jpsavepath + "\\" + Path.GetFileNameWithoutExtension(inputCurrentFiles[0]).Substring(0, 14) + "_" + paras[2] + ".dat";
                SNWParaStat snwStat = new SNWParaStat();
                snwStat.SNWParaAvgStat(inputCurrentFiles, 0.1f, outHistoryFile);
                List<string> temphistory = new List<string>();
                temphistory.Add(outHistoryFile);
                inputHistoryFiles = temphistory.ToArray();
                if (inputCurrentFiles == null || inputCurrentFiles.Length < 0 || inputHistoryFiles == null || inputHistoryFiles.Length < 0)
                {
                    PrintInfo("缺少分析文件");
                    return null;
                }        
            }
            else
            {
                inputCurrentFiles = list1.ToArray();
                inputHistoryFiles = list2.ToArray();
                if (inputCurrentFiles[0].Contains("MWSE"))
                    prdType = "SWE";
                else
                    prdType = "SD";
                if (inputCurrentFiles == null || inputCurrentFiles.Length < 0 || inputHistoryFiles == null || inputHistoryFiles.Length < 0)
                {
                    PrintInfo("缺少分析文件");
                    return null;
                }
                if ((inputCurrentFiles.Length) * 2 != inputHistoryFiles.Length)
                {
                    MessageBox.Show("当年数据与同期数据不对应");
                    return null;
                }
            }
            #region 裁切文件
            string clipsave = jpsavepath + "\\" + regionsname + "\\" + "裁切";
            if (!System.IO.Directory.Exists(clipsave))//如果不存在这个路径
                System.IO.Directory.CreateDirectory(clipsave);
            //体积文件SVOL存放路径
            string svolPath = jpsavepath + "\\" + regionsname + "\\" + "体积文件";
            if (!System.IO.Directory.Exists(svolPath))
                System.IO.Directory.CreateDirectory(svolPath);
            //统计出的各个文件夹的数据放在txt文本里
            string statxt = jpsavepath + "\\" + regionsname + "\\" + regionsname + ".txt";
            FileStream fauto = new FileStream(statxt, FileMode.Create, FileAccess.Write);
            StreamWriter rauto = new StreamWriter(fauto);
            if (aoiContainer == null)
            {
                //不用裁切 直接计算
                resultList.Clear();
                if (paras[1] == "winter")
                {
                    #region 冬季
                    //只返回一个体积和
                    double pixelSumWinter; // 总冬季雪水当量的体积和，是被减数
                    if (prdType == "SWE")
                    {
                        title = regionsname + "冬季雪水当量距平" + "(" + paras[2] + "年)";
                        string sumsweVolFile = svolPath + "\\" + Path.GetFileName(inputHistoryFiles[0]).Replace("MSWE", "SVOL");
                        if (!File.Exists(sumsweVolFile))
                        {
                            IFileExtractResult sweVolResult = ComputeSnowSWEVOL(inputHistoryFiles[0], sumsweVolFile);
                            sumsweVolFile = sweVolResult.FileName;
                        }
                        using (IRasterDataProvider inRaster = RasterDataDriver.Open(sumsweVolFile) as IRasterDataProvider)
                        {
                            pixelSumWinter = CompuCurPixel(sumsweVolFile);
                        }
                        foreach (string infile in inputCurrentFiles)
                        {
                            //MWS_JPEA_MSWE_China_Month_SWE_D_2011_2013_1_avg.dat
                            Regex DataReg2 = new Regex(@"(?<year>\d{4})_(?<month>\d{2})_(?<year>\d{4})_(?<month>\d{1})", RegexOptions.Compiled);
                            Match m = DataReg2.Match(infile); //提取每年冬季的时间
                            string year = "";
                            if (m.Success)
                                year = m.Value;
                            string mx = ""; //横轴时间
                            mx = year.Substring(0, 4) + "年";
                            string sweVolFile = svolPath + "\\" + Path.GetFileName(infile).Replace("MSWE", "SVOL");
                            if (!File.Exists(sweVolFile))
                            {
                                IFileExtractResult sweVolResult = ComputeSnowSWEVOL(infile, sweVolFile);
                                sweVolFile = sweVolResult.FileName;
                            }
                            double pixelSum;//只返回一个体积和
                            using (IRasterDataProvider inRaster = RasterDataDriver.Open(sweVolFile) as IRasterDataProvider)
                            {
                                pixelSum = CompuCurPixel(sweVolFile);
                            }
                            resultList.Add(new string[] { mx, ((pixelSum - pixelSumWinter) / pixelSumWinter * 100).ToString() + "%" });
                            rauto.WriteLine(mx + " " + ((pixelSum - pixelSumWinter) / pixelSumWinter * 100).ToString() + "%");
                        }
                    }
                    if (prdType == "SD")
                    {
                        title = regionsname + "冬季雪水当量距平" + "(" + paras[2] + "年)";
                        using (IRasterDataProvider inRaster = RasterDataDriver.Open(inputHistoryFiles[0]) as IRasterDataProvider)
                        {
                            pixelSumWinter = CompuCurPixel(inputHistoryFiles[0]);
                        }
                        foreach (string infile in inputCurrentFiles)
                        {
                            //MWS_JPEA_MSWE_China_Month_SD_D_2011_2013_1_avg.dat
                            Regex DataReg2 = new Regex(@"(?<year>\d{4})_(?<month>\d{2})_(?<year>\d{4})_(?<month>\d{1})", RegexOptions.Compiled);
                            Match m = DataReg2.Match(infile); //提取每年冬季的时间
                            string year = "";
                            if (m.Success)
                                year = m.Value;
                            string mx = ""; //横轴时间
                            mx = year.Substring(0, 4) + "年";
                            string sweVolFile = svolPath + "\\" + Path.GetFileName(infile).Replace("MSWE", "SVOL");
                            //只返回一个体积和
                            double pixelSum;
                            using (IRasterDataProvider inRaster = RasterDataDriver.Open(infile) as IRasterDataProvider)
                            {
                                pixelSum = CompuCurPixel(infile);
                            }
                            resultList.Add(new string[] { mx, ((pixelSum - pixelSumWinter) / pixelSumWinter * 100).ToString() + "%" });
                            rauto.WriteLine(mx + " " + ((pixelSum - pixelSumWinter) / pixelSumWinter * 100).ToString() + "%");
                        }
                    }
                }
                    #endregion
                else           
                {
                    #region dang nian旬月
                    resultList.Clear();
                    foreach (string infile in inputCurrentFiles)
                    {
                        //先假设选择的是旬数据 MWS_MWSD_China_Xun_0SD_A_2011_1_avg.dat
                        //       则同期旬数据 MWS_MWSD_China_Xun_0SD_A_1989_2011_1_avg.dat
                        string cfile = Path.GetFileNameWithoutExtension(infile);
                        Match m = DataReg.Match(cfile);
                        string date = "";
                        if (m.Success)
                        {
                            date = m.Value;    //根据年4个数字来拆分字符串
                        }
                        string bhalf = cfile.Substring(0, cfile.IndexOf(date));
                        string ahalf = cfile.Substring(cfile.IndexOf(date) + 4, cfile.Length - 4 - cfile.IndexOf(date));
                        foreach (string inputHistoryFile in inputHistoryFiles)
                        {
                            string aa = Path.GetFileNameWithoutExtension(inputHistoryFile);
                            double infilepixelSum = 0;//只返回一个体积和
                            double hisfilepixelSum;  //只返回一个体积和
                            if (aa.Contains(bhalf) && aa.Contains(ahalf) && aa.Contains(date) && !aa.Contains(Nostr))
                            {
                                #region 当前文件体积计算，并相减
                                if (prdType == "SWE")
                                {
                                    string sweVolFile = svolPath + "\\" + Path.GetFileName(infile).Replace("MSWE", "SVOL");
                                    if (!File.Exists(sweVolFile))
                                    {
                                        IFileExtractResult sweVolResult = ComputeSnowSWEVOL(infile, sweVolFile);
                                        sweVolFile = sweVolResult.FileName;
                                    }
                                    using (IRasterDataProvider inRaster = RasterDataDriver.Open(sweVolFile) as IRasterDataProvider)
                                    {
                                        infilepixelSum = CompuCurPixel(sweVolFile);
                                    }
                                    #region 历史文件做裁切，体积计算，并相减
                                    string hissweVolFile = svolPath + "\\" + Path.GetFileName(inputHistoryFile).Replace("MSWE", "SVOL");
                                    if (!File.Exists(hissweVolFile))
                                    {
                                        IFileExtractResult sweVolResult = ComputeSnowSWEVOL(inputHistoryFile, hissweVolFile);
                                        hissweVolFile = sweVolResult.FileName;
                                    }
                                    using (IRasterDataProvider inRaster = RasterDataDriver.Open(hissweVolFile) as IRasterDataProvider)
                                    {
                                        hisfilepixelSum = CompuCurPixel(hissweVolFile);
                                    }
                                    #endregion
                                }
                                else 
                                {
                                    using (IRasterDataProvider inRaster = RasterDataDriver.Open(infile) as IRasterDataProvider)
                                    {
                                        infilepixelSum = CompuCurPixel(infile);
                                    }
                                    using (IRasterDataProvider inRaster = RasterDataDriver.Open(inputHistoryFile) as IRasterDataProvider)
                                    {
                                        hisfilepixelSum = CompuCurPixel(inputHistoryFile);
                                    }
                                }
                                #endregion
                                //提取年至avg之间的字符
                                string filetime = cfile.Substring(cfile.IndexOf(date), cfile.Length - 3 - cfile.IndexOf(date));
                                string[] mxchars = filetime.Split(new char[] { '_' });
                                string mx = "";//月+旬
                                if (cfile.Contains("Xun"))
                                {
                                    title = regionsname + date + "年冬季雪水当量旬距平" + "(" + paras[2] + "年)";
                                    if (mxchars[2] == "1")
                                        mx = mxchars[1] + "月" + "上旬";
                                    if (mxchars[2] == "2")
                                        mx = mxchars[1] + "月" + "中旬";
                                    if (mxchars[2] == "3")
                                        mx = mxchars[1] + "月" + "下旬";
                                }
                                else if (cfile.Contains("Month"))
                                {
                                    title = regionsname + date + "年冬季雪水当量月距平" + "(" + paras[2] + "年)";
                                    mx = mxchars[1] + "月";
                                }
                                else
                                {
                                    mx = filetime;
                                }
                                resultList.Add(new string[] { mx, ((infilepixelSum - hisfilepixelSum) / hisfilepixelSum * 100).ToString() + "%" });
                                rauto.WriteLine(mx + " " + ((infilepixelSum - hisfilepixelSum) / hisfilepixelSum * 100).ToString() + "%");
                            }
                        }
                    }
                    #endregion
                }
            }
            else                                                //选择了aois
            {
                MulRegionsClip muticlip = new MulRegionsClip();  //裁切
                if (paras[1] == "winter")
                {
                    resultList.Clear();
                    double pixelSumWinter = 0d; // 总冬季雪水当量的体积和，是被减数
                    #region
                    title = regionsname + "冬季雪水当量距平" + "(" + paras[2] + "年)";
                    //先把总的冬季给裁了
                    string newsumWinter = Path.Combine(clipsave, Path.GetFileName(inputHistoryFiles[0]).Replace("China", regionsname));
                    if (!File.Exists(newsumWinter))
                    {
                        string sumWinter = muticlip.MutiRegionsClip(inputHistoryFiles[0], aoiContainer, clipsave);
                        string hdrfile = Path.GetDirectoryName(sumWinter) + "\\" + Path.GetFileNameWithoutExtension(sumWinter) + ".hdr";
                        //重命名
                        string newhdrfile = Path.Combine(Path.GetDirectoryName(sumWinter), Path.GetFileNameWithoutExtension(inputHistoryFiles[0]).Replace("China", regionsname) + ".hdr");
                        FileInfo fi = new FileInfo(sumWinter);
                        fi.MoveTo(newsumWinter);
                        FileInfo fihdr = new FileInfo(hdrfile);
                        fihdr.MoveTo(newhdrfile);
                    }
                    if (prdType == "SWE")
                    {
                        string sumsweVolFile = svolPath + "\\" + Path.GetFileName(newsumWinter).Replace("MSWE", "SVOL");
                        if (!File.Exists(sumsweVolFile))
                        {
                            IFileExtractResult sweVolResult = ComputeSnowSWEVOL(newsumWinter, sumsweVolFile);
                            sumsweVolFile = sweVolResult.FileName;
                        }
                        using (IRasterDataProvider inRaster = RasterDataDriver.Open(sumsweVolFile) as IRasterDataProvider)
                        {
                            pixelSumWinter = CompuCurPixel(sumsweVolFile);
                        }
                    }
                    if (prdType == "SD")
                    {
                        using (IRasterDataProvider inRaster = RasterDataDriver.Open(inputHistoryFiles[0]) as IRasterDataProvider)
                        {
                            pixelSumWinter = CompuCurPixel(inputHistoryFiles[0]);
                        }
                    }
                    foreach (string infile in inputCurrentFiles)
                    {
                        //MWS_JPEA_MSWE_China_Month_SWE_D_2011_2013_1_avg.dat
                        Regex DataReg2 = new Regex(@"(?<year>\d{4})_(?<month>\d{2})_(?<year>\d{4})_(?<month>\d{1})", RegexOptions.Compiled);
                        Match m = DataReg2.Match(infile); //提取每年冬季的时间
                        string year = "";
                        if (m.Success)
                            year = m.Value;
                        string mx = ""; //横轴时间
                        mx = year.Substring(0, 4) + "年";
                        string newclipfile = Path.Combine(clipsave, Path.GetFileName(infile).Replace("China", regionsname));
                        if (!File.Exists(newclipfile))
                        {
                            string clipfile = muticlip.MutiRegionsClip(infile, aoiContainer, clipsave);
                            string hdrfile = Path.GetDirectoryName(clipfile) + "\\" + Path.GetFileNameWithoutExtension(clipfile) + ".hdr";
                            //重命名
                            string newhdrfile = Path.Combine(Path.GetDirectoryName(clipfile), Path.GetFileNameWithoutExtension(infile).Replace("China", regionsname) + ".hdr");
                            FileInfo fi = new FileInfo(clipfile);
                            fi.MoveTo(newclipfile);
                            FileInfo fihdr = new FileInfo(hdrfile);
                            fihdr.MoveTo(newhdrfile);
                        }
                        //只返回一个体积和
                        double pixelSum = 0d;
                        if (prdType == "SWE")
                        {
                            string sweVolFile = svolPath + "\\" + Path.GetFileName(newclipfile).Replace("MSWE", "SVOL");
                            if (!File.Exists(sweVolFile))
                            {
                                IFileExtractResult sweVolResult = ComputeSnowSWEVOL(newclipfile, sweVolFile);
                                sweVolFile = sweVolResult.FileName;
                            }
                            using (IRasterDataProvider inRaster = RasterDataDriver.Open(sweVolFile) as IRasterDataProvider)
                            {
                                pixelSum = CompuCurPixel(sweVolFile);
                            }
                        }
                        if (prdType == "SD")
                        {
                            using (IRasterDataProvider inRaster = RasterDataDriver.Open(infile) as IRasterDataProvider)
                            {
                                pixelSum = CompuCurPixel(infile);
                            }
                        }
                        resultList.Add(new string[] { mx, ((pixelSum - pixelSumWinter) / pixelSumWinter *100).ToString()+"%" });
                        rauto.WriteLine(mx + " " + ((pixelSum - pixelSumWinter) / pixelSumWinter).ToString());
                    }

                }
                    #endregion
                else                //选择的是旬、月
                {
                    #region
                    resultList.Clear();
                    foreach (string infile in inputCurrentFiles)
                    {
                        //先假设选择的是旬数据 MWS_MWSD_China_Xun_0SD_A_2011_1_avg.dat
                        //       则同期旬数据 MWS_MWSD_China_Xun_0SD_A_1989_2011_1_avg.dat
                        string cfile = Path.GetFileNameWithoutExtension(infile);
                        Match m = DataReg.Match(cfile);
                        string date = "";
                        if (m.Success)
                        {
                            date = m.Value;    //根据年4个数字来拆分字符串
                        }
                        string bhalf = cfile.Substring(0, cfile.IndexOf(date));
                        string ahalf = cfile.Substring(cfile.IndexOf(date) + 4, cfile.Length - 4 - cfile.IndexOf(date));
                        foreach (string inputHistoryFile in inputHistoryFiles)
                        {   
                            string aa = Path.GetFileNameWithoutExtension(inputHistoryFile);
                            if (aa.Contains(bhalf) && aa.Contains(ahalf) && aa.Contains(date) && !aa.Contains(Nostr))
                            {
                                //只返回一个体积和
                                double infilepixelSum;
                                //当前文件做裁切，体积计算，并相减
                                #region
                                string newclipfile = Path.Combine(clipsave, Path.GetFileName(infile).Replace("China", regionsname));
                                if (!File.Exists(newclipfile))
                                {
                                    string clipfile = muticlip.MutiRegionsClip(infile, aoiContainer, clipsave);
                                    string hdrfile = Path.GetDirectoryName(clipfile) + "\\" + Path.GetFileNameWithoutExtension(clipfile) + ".hdr";
                                    //重命名
                                    string newhdrfile = Path.Combine(Path.GetDirectoryName(clipfile), Path.GetFileNameWithoutExtension(infile).Replace("China", regionsname) + ".hdr");
                                    FileInfo fi = new FileInfo(clipfile);
                                    fi.MoveTo(newclipfile);
                                    FileInfo fihdr = new FileInfo(hdrfile);
                                    fihdr.MoveTo(newhdrfile);
                                }
                                if (prdType == "SWE")
                                {
                                    string sweVolFile = svolPath + "\\" + Path.GetFileName(newclipfile).Replace("MSWE", "SVOL");
                                    if (!File.Exists(sweVolFile))
                                    {
                                        IFileExtractResult sweVolResult = ComputeSnowSWEVOL(newclipfile, sweVolFile);
                                        sweVolFile = sweVolResult.FileName;
                                    }
                                    using (IRasterDataProvider inRaster = RasterDataDriver.Open(sweVolFile) as IRasterDataProvider)
                                    {
                                        infilepixelSum = CompuCurPixel(sweVolFile);
                                    }
                                }
                                else 
                                {
                                    using (IRasterDataProvider inRaster = RasterDataDriver.Open(newclipfile) as IRasterDataProvider)
                                    {
                                        infilepixelSum = CompuCurPixel(newclipfile);
                                    }
                                }
                                #endregion
                                //历史文件做裁切，体积计算，并相减
                                #region
                                //只返回一个体积和
                                double hisfilepixelSum;
                                string newhisclipfile = Path.Combine(clipsave, Path.GetFileName(inputHistoryFile).Replace("China", regionsname));
                                if (!File.Exists(newhisclipfile))
                                {
                                    string clipfile = muticlip.MutiRegionsClip(inputHistoryFile, aoiContainer, clipsave);
                                    string hdrfile = Path.GetDirectoryName(clipfile) + "\\" + Path.GetFileNameWithoutExtension(clipfile) + ".hdr";
                                    //重命名
                                    string newhdrfile = Path.Combine(Path.GetDirectoryName(clipfile), Path.GetFileNameWithoutExtension(inputHistoryFile).Replace("China", regionsname) + ".hdr");
                                    FileInfo fi = new FileInfo(clipfile);
                                    fi.MoveTo(newhisclipfile);
                                    FileInfo fihdr = new FileInfo(hdrfile);
                                    fihdr.MoveTo(newhdrfile);
                                }
                                if (prdType == "SWE")
                                {
                                    string hissweVolFile = svolPath + "\\" + Path.GetFileName(newhisclipfile).Replace("MSWE", "SVOL");
                                    if (!File.Exists(hissweVolFile))
                                    {
                                        IFileExtractResult sweVolResult = ComputeSnowSWEVOL(newhisclipfile, hissweVolFile);
                                        hissweVolFile = sweVolResult.FileName;
                                    }
                                    using (IRasterDataProvider inRaster = RasterDataDriver.Open(hissweVolFile) as IRasterDataProvider)
                                    {
                                        hisfilepixelSum = CompuCurPixel(hissweVolFile);
                                    }
                                }
                                else
                                {
                                    using (IRasterDataProvider inRaster = RasterDataDriver.Open(newhisclipfile) as IRasterDataProvider)
                                    {
                                        hisfilepixelSum = CompuCurPixel(newhisclipfile);
                                    }
                                }
                                #endregion
                                //提取年至avg之间的字符
                                string filetime = cfile.Substring(cfile.IndexOf(date), cfile.Length - 3 - cfile.IndexOf(date));
                                string[] mxchars = filetime.Split(new char[] { '_' });
                                string mx = "";//月+旬
                                if (cfile.Contains("Xun"))
                                {
                                    title = regionsname + date + "年冬季雪水当量旬距平" + "(" + paras[2] + "年)";
                                    if (mxchars[2] == "1")
                                        mx = mxchars[1] + "月" + "上旬";
                                    if (mxchars[2] == "2")
                                        mx = mxchars[1] + "月" + "中旬";
                                    if (mxchars[2] == "3")
                                        mx = mxchars[1] + "月" + "下旬";
                                }
                                else if (cfile.Contains("Month"))
                                {
                                    title = regionsname + date + "年冬季雪水当量月距平" + "(" + paras[2] + "年)";
                                    mx = mxchars[1] + "月";
                                }
                                else
                                {
                                    mx = filetime;
                                }
                                resultList.Add(new string[] { mx, ((infilepixelSum - hisfilepixelSum) / hisfilepixelSum * 100).ToString() + "%" });
                                rauto.WriteLine(mx + " " + ((infilepixelSum - hisfilepixelSum) / hisfilepixelSum).ToString());
                            }
                        }
                    }
                    #endregion
                }
            }
            rauto.Close();
            fauto.Close();
            string coluString = regionsname;
            string sentitle = "统计日期：" + DateTime.Now.ToShortDateString();
            string[] columns = new string[] { coluString, "距平" };
            fresult = new StatResult(sentitle, columns, resultList.ToArray());
            string outputIdentify = regionsname;
            string fileexcel = StatResultToFile(new string[] { inputCurrentFiles[0] }, fresult, "MWS", outputIdentify, title, null, 1, true, 1);
            string newexcelfile = Path.Combine(svolPath, title + ".XLSX");//inputCurrentFiles[0].Substring(0,9) + "_"+ regionsname + paras[2]+ ".XLSX");
            FileInfo fi1 = new FileInfo(fileexcel);
            fi1.MoveTo(newexcelfile);
            IFileExtractResult res = new FileExtractResult(_subProductDef.Identify, newexcelfile, false);
            return new FileExtractResult(outputIdentify, newexcelfile);
            #endregion
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
    
        private float CompuCurPixel(string rasterFileName)
        {
            IRasterDataProvider inRaster = RasterDataDriver.Open(rasterFileName) as IRasterDataProvider;
            ArgumentProvider ap = new ArgumentProvider(inRaster, null);
            RasterPixelsVisitor<float> visitor = new RasterPixelsVisitor<float>(ap);
            float result = 0;
            visitor.VisitPixel(new int[] { 1 }, (index, values) =>
            {
                if (values[0] > 0)
                result += values[0];

            });
            return result;
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
        protected IRasterDataProvider CreateOutRaster(string outFileName, enumDataType dataType, RasterMaper[] inrasterMaper, float resolution)
        {
            IRasterDataDriver raster = RasterDataDriver.GetDriverByName("MEM") as IRasterDataDriver;
            GeoDo.RSS.Core.DF.CoordEnvelope outEnv = null;
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
      
        private void PrintInfo(string info)
        {
            if (_contextMessage != null)
                _contextMessage.PrintMessage(info);
            else
                Console.WriteLine(info);
        }
    }
}
