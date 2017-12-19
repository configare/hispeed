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
using System.Xml.Linq;
using GeoDo.RSS.UI.AddIn.Layout;
using GeoDo.RSS.Layout;
using GeoDo.MicroWaveSnow.SNWParaStat;
namespace GeoDo.RSS.MIF.Prds.MWS
{
    public class SubProductTimeSeriesAnalysis:CmaMonitoringSubProduct
    {
        private Dictionary<string, object> _arguments = null;
        GeoDo.RSS.Core.VectorDrawing.AOIContainerLayer aoiContainer = null;
        private string MergeAoisName = "";
        private string[] MergerAoiNames = new string[] { };
        private bool isMergeAois;
        private string[] startEndTime = new string[] { };
        private string periodTypes = "";
        private string[] startEndyears;
        private bool avgLayout = false; private bool avgSat = false; private bool jupingLayout = false; private bool jupingSat = false;
        private List<XElement> layers = new List<XElement>(); //加载的shpfile层
        private string outPut;
        private string statTypes;
        private string GxdTitleName = null; //专题图上的名称
        private string choseAois = "";      //统计图上的名
        private string Product = "";        //根据prdType确定产品
        private string prdTypes = "";
        private string queryTypes = "";
        //二次裁切时用，string 是regionName , 后面是对应的AOI
        private Dictionary<string, GeoDo.RSS.Core.VectorDrawing.AOIContainerLayer> repetClipAOI = new Dictionary<string,GeoDo.RSS.Core.VectorDrawing.AOIContainerLayer>();
        private string IsAllContry = "NO";
        private MWSSmoothHelp smooth = new MWSSmoothHelp();
        private string _gxdFile = null;
        private string _shpFile = null;
        private Dictionary<string, List<string>> _retrievAvgFiles = null;

        public SubProductTimeSeriesAnalysis(SubProductDef subProductDef)
            : base(subProductDef)
        {

        }
        public override IExtractResult Make(Action<int, string> progressTracker, IContextMessage contexMessage)
        {
            if (_argumentProvider == null)
                return null;
            if (_argumentProvider.GetArg("AlgorithmName") == null)
                return null;
            if (_argumentProvider.GetArg("AlgorithmName").ToString() == "TimeSeriesAlgorithm")
            {
                return TimeSeriesAlgorithm(progressTracker);
            }
            return null;
        }
        private IExtractResult TimeSeriesAlgorithm(Action<int, string> progressTracker)
        {
            if (_argumentProvider.GetArg("TimeSeriesParas") == null)
                return null;
            if (repetClipAOI != null)
                repetClipAOI.Clear();
            _arguments = _argumentProvider.GetArg("TimeSeriesParas") as Dictionary<string, object>;
            prdTypes = _arguments["prdTypes"] as string;
            if (prdTypes != "SD" && prdTypes != "SWE")
                return null;
            if (prdTypes == "SD")
                Product = "雪深";
            else
                Product = "雪水当量";
            string[] orbTypes = _arguments["orbTypes"] as string[];
            startEndyears = _arguments["startEndyears"] as string[];
            periodTypes = _arguments["periodTypes"] as string;
            startEndTime = _arguments["startEndTime"] as string[];
            statTypes = _arguments["statTypes"] as string;
            if (statTypes.ToUpper() == "AVG")
                statTypes = "均值";
            if (statTypes.ToUpper() == "MIN")
                statTypes = "最小值";
            if (statTypes.ToUpper() == "MAX")
                statTypes = "最大值";
            string[] timeSeriesOpts = _arguments["timeSeriesOpts"] as string[];
            List<string> timeOpt = new List<string>(timeSeriesOpts);
            string timeOpts = null;
            foreach (string s in timeOpt)
                timeOpts += s;
            if (timeOpts.Contains("AvgLayout"))
                avgLayout = true;
            if (timeOpts.Contains("AvgStat"))
                avgSat = true;
            if (timeOpts.Contains("JupingLayout"))
                jupingLayout = true;
            if (timeOpts.Contains("JupingStat"))
                jupingSat = true;
            aoiContainer = _arguments["outAOIs"] as GeoDo.RSS.Core.VectorDrawing.AOIContainerLayer;
            isMergeAois = _arguments["isMergeAois"].ToString() == "1" ? true : false;
            if (isMergeAois == true || aoiContainer.AOIs.Count() ==1 )
                MergeAoisName = _arguments["MergeAoisName"] as string;
            else
                MergerAoiNames = _arguments["MergeAoisName"] as string[];
            choseAois = _arguments["ChoseAois"] as string;
            outPut = _arguments["outPath"] as string;
            outPut = outPut + "\\" + "时间序列分析" + "\\" + choseAois;
            IsAllContry = _arguments["IsAllContry"] as string;
            if (!System.IO.Directory.Exists(outPut))
                System.IO.Directory.CreateDirectory(outPut);
            TSANRetrieval ts = new TSANRetrieval(_arguments);
            progressTracker(10, "查询数据...");
            string IsSeniorQuery = _arguments["IsSeniorQuery"] as string;
            if (IsSeniorQuery.ToUpper() == "NO")
                _retrievAvgFiles = ts.DoRetrievalAndCompute();
            else
            {
                List<string> list = ExportManager.GetInstance().List;
                Dictionary<string, List<string>> _retrievFiles = new Dictionary<string, List<string>>();
                _retrievFiles.Add("continue", list);  //连续时间的处理
                _retrievAvgFiles = _retrievFiles; 
            }
            queryTypes = _arguments["queryTypes"] as string;
            if (_retrievAvgFiles != null && _retrievAvgFiles.Count >= 0)
            {
                if (periodTypes.ToUpper() == "WINTER")
                {
                    string[] files = _retrievAvgFiles["Winter"].ToArray();
                    string outWinterFile = outPut + "\\" + Path.GetFileNameWithoutExtension(files[0]).Substring(0, 27) + "_" + startEndyears[0] + "_" + startEndyears[1] + ".dat";
                    SNWParaStat snwStat = new SNWParaStat();
                    snwStat.SNWParaAvgStat(files, 0.1f, outWinterFile);
                    _retrievAvgFiles.Clear();
                    _retrievAvgFiles.Add(outWinterFile, files.ToList());
                }
                if (queryTypes == "continue")
                {
                    return ContinueTimeSeriesAnalysis(_retrievAvgFiles, progressTracker);
                }
                else
                {
                    if (queryTypes == "sametime")
                    {
                        return TimeSeriesAnalysisRegions(_retrievAvgFiles, progressTracker);
                    }
                    else
                        return null;
                }
            }
            else
                return null;
        }

        private IExtractResult TimeSeriesAnalysisRegions(Dictionary<string, List<string>> dicFiles, Action<int, string> progressTracker)
        {
            IExtractResultArray resultsArray = new ExtractResultArray("");
            MulRegionsClip muticlip = new MulRegionsClip();
            string smoothsave = ""; string filtersave = ""; string bilisave = ""; string outsave = "";
            #region 专题图：周期分布专题图，距平分布专题图
            if (avgLayout == true || jupingLayout == true)
            {
                //先按矩形提数据 再中值 再插值 再按矢量边裁切
                Dictionary<string, List<string>> clipFiles = new Dictionary<string, List<string>>();
                progressTracker(25, "数据处理...");
                if (IsAllContry.ToUpper() == "YES")
                    clipFiles = dicFiles;
                else
                   clipFiles = ClipRectFiles(dicFiles); 
                smoothsave = outPut + "\\" + "中间文件（可删除）";       //创建平滑路径
                if (!System.IO.Directory.Exists(smoothsave))
                    System.IO.Directory.CreateDirectory(smoothsave);
                filtersave = smoothsave;                                // 创建平滑\\中值
                bilisave = smoothsave +"\\" + "处理文件";               //创建平滑\\插值
                if (!System.IO.Directory.Exists(bilisave))
                    System.IO.Directory.CreateDirectory(bilisave);
                outsave = outPut + "\\" + "专题图数据源";
                if (!System.IO.Directory.Exists(outsave))
                    System.IO.Directory.CreateDirectory(outsave);
                #region 周期分布图
                if (avgLayout == true)
                {
                    progressTracker(35, "周期分布图...");
                    string layoutType = "分布图";
                    List<string> files = new List<string>();
                    if (isMergeAois == true || aoiContainer.AOIs.Count() == 1)
                    {
                        #region 区域合成
                        string gxdsave = outPut + "\\" + "周期分布专题图";
                        if (!System.IO.Directory.Exists(gxdsave))
                            System.IO.Directory.CreateDirectory(gxdsave);
                        List<string> files2layout = new List<string>();
                        foreach (string file in clipFiles.Keys)
                        {
                            string filterfile = smooth.ComputerMid(file, 5, filtersave);
                            string hfilterfile = Path.GetDirectoryName(filterfile) + "\\" + Path.GetFileNameWithoutExtension(filterfile) + ".hdr";
                            #region 中国区
                            if (IsAllContry == "YES")
                            {
                                string fileforlayout = Path.Combine(outsave, Path.GetFileName(filterfile)); 
                                string fileforlayoutHdr = Path.Combine(outsave, Path.GetFileNameWithoutExtension(filterfile)) + ".hdr"; 
                                //文件名标识变为“HFSD”或“HFWE”
                                #region
                                if (fileforlayout.Contains("MWSD"))
                                {
                                    fileforlayout = fileforlayout.Replace("MWSD", "HFSD");
                                    fileforlayoutHdr = fileforlayoutHdr.Replace("MWSD", "HFSD");
                                }
                                if (fileforlayout.Contains("MSWE"))
                                {
                                    fileforlayout = fileforlayout.Replace("MSWE", "HFWE");
                                    fileforlayoutHdr = fileforlayoutHdr.Replace("MSWE", "HFWE");
                                }
                                FileInfo fif = new FileInfo(filterfile);
                                if (!File.Exists(fileforlayout))
                                    fif.MoveTo(fileforlayout);
                                FileInfo fih = new FileInfo(hfilterfile);
                                if (!File.Exists(fileforlayoutHdr))
                                    fih.MoveTo(fileforlayoutHdr);
                                #endregion
                                files2layout.Add(fileforlayout);
                            }
                            #endregion
                            else
                            {
                                string bilifile = smooth.Bilinear(filterfile, 10, bilisave);
                                string fileforlayout = Path.Combine(outsave, Path.GetFileName(bilifile)); //切后文件
                                string fileforlayoutHdr = Path.Combine(outsave, Path.GetFileNameWithoutExtension(bilifile)) + ".hdr"; //切后的文件hdr
                                if (!File.Exists(fileforlayout))
                                {
                                    string clipfile = muticlip.MutiRegionsClip(bilifile, aoiContainer, outsave);
                                    string hfile = Path.GetDirectoryName(clipfile) + "\\" + Path.GetFileNameWithoutExtension(clipfile) + ".hdr";
                                    //文件名标识变为“HFSD”或“HFWE”
                                    #region
                                    if (fileforlayout.Contains("MWSD"))
                                    {
                                        fileforlayout = fileforlayout.Replace("MWSD", "HFSD");
                                        fileforlayoutHdr = fileforlayoutHdr.Replace("MWSD", "HFSD");
                                    }
                                    if (fileforlayout.Contains("MSWE"))
                                    {
                                        fileforlayout = fileforlayout.Replace("MSWE", "HFWE");
                                        fileforlayoutHdr = fileforlayoutHdr.Replace("MSWE", "HFWE");
                                    }
                                    FileInfo fif = new FileInfo(clipfile);
                                    if (!File.Exists(fileforlayout))
                                        fif.MoveTo(fileforlayout);
                                    FileInfo fih = new FileInfo(hfile);
                                    if (!File.Exists(fileforlayoutHdr))
                                        fih.MoveTo(fileforlayoutHdr);
                                    #endregion
                                }
                                files2layout.Add(fileforlayout);
                            }
                            List<string> newfiles = clipFiles[file];
                            foreach (string infile in newfiles)
                            {
                                string fileF = smooth.ComputerMid(infile, 5, filtersave);
                                string hfilterF = Path.GetDirectoryName(fileF) + "\\" + Path.GetFileNameWithoutExtension(fileF) + ".hdr";
                                #region 中国区
                                if (IsAllContry.ToUpper() == "YES")
                                {
                                    string fileforlayout = Path.Combine(outsave, Path.GetFileName(fileF));
                                    string fileforlayoutHdr = Path.Combine(outsave, Path.GetFileNameWithoutExtension(fileF)) + ".hdr";
                                    //文件名标识变为“HFSD”或“HFWE”
                                    #region
                                    if (fileforlayout.Contains("MWSD"))
                                    {
                                        fileforlayout = fileforlayout.Replace("MWSD", "HFSD");
                                        fileforlayoutHdr = fileforlayoutHdr.Replace("MWSD", "HFSD");
                                    }
                                    if (fileforlayout.Contains("MSWE"))
                                    {
                                        fileforlayout = fileforlayout.Replace("MSWE", "HFWE");
                                        fileforlayoutHdr = fileforlayoutHdr.Replace("MSWE", "HFWE");
                                    }
                                    FileInfo fif = new FileInfo(fileF);
                                    if (!File.Exists(fileforlayout))
                                        fif.MoveTo(fileforlayout);
                                    FileInfo fih = new FileInfo(hfilterF);
                                    if (!File.Exists(fileforlayoutHdr))
                                        fih.MoveTo(fileforlayoutHdr);
                                    #endregion
                                    files2layout.Add(fileforlayout);
                                }
                                #endregion 
                                else
                                {
                                    string fileB = smooth.Bilinear(fileF, 10, bilisave);
                                    string fileforlayout1 = Path.Combine(outsave, Path.GetFileName(fileB)); //切后文件 
                                    string fileforlayoutHdr1 = Path.Combine(outsave, Path.GetFileNameWithoutExtension(fileB)) + ".hdr"; //切后的文件hdr
                                    if (!File.Exists(fileforlayout1))
                                    {
                                        string fileC = muticlip.MutiRegionsClip(fileB, aoiContainer, outsave);
                                        string hdrfile = Path.GetDirectoryName(fileC) + "\\" + Path.GetFileNameWithoutExtension(fileC) + ".hdr";
                                        //文件名标识变为“HFSD”或“HFWE”
                                        #region
                                        if (fileforlayout1.Contains("MWSD"))
                                        {
                                            fileforlayout1 = fileforlayout1.Replace("MWSD", "HFSD");
                                            fileforlayoutHdr1 = fileforlayoutHdr1.Replace("MWSD", "HFSD");
                                        }
                                        if (fileforlayout1.Contains("MSWE"))
                                        {
                                            fileforlayout1 = fileforlayout1.Replace("MSWE", "HFWE");
                                            fileforlayoutHdr1 = fileforlayoutHdr1.Replace("MSWE", "HFWE");
                                        }
                                        FileInfo fi = new FileInfo(fileC);
                                        if (!File.Exists(fileforlayout1))
                                            fi.MoveTo(fileforlayout1);
                                        FileInfo fihdr = new FileInfo(hdrfile);
                                        if (!File.Exists(fileforlayoutHdr1))
                                            fihdr.MoveTo(fileforlayoutHdr1);
                                    }
                                        #endregion
                                    files2layout.Add(fileforlayout1);
                                }
                            }
                        }
                        List<string> resultList = MakeLayout(files2layout, MergeAoisName, gxdsave, layoutType);   //出专题图函数
                        foreach (string fileGXD in resultList)
                        {
                            IFileExtractResult res = new FileExtractResult(_subProductDef.Identify, fileGXD, false);
                            resultsArray.Add(res);
                        }
                        #endregion
                    }
                    else
                    {
                        #region 非区域合成
                        foreach (string regionName in clipFiles.Keys)
                        {
                            string gxdsave = outPut + "\\" + regionName + "\\" + "周期分布专题图";
                            if (!System.IO.Directory.Exists(gxdsave))
                                System.IO.Directory.CreateDirectory(gxdsave);
                            List<string> files2layout = new List<string>();
                            List<string> newfiles = clipFiles[regionName];
                            foreach (string infile in newfiles)
                            {
                                string fileF = smooth.ComputerMid(infile, 5, filtersave);
                                string fileB = smooth.Bilinear(fileF, 10, bilisave);
                                string fileforlayout1 = Path.Combine(outsave, Path.GetFileName(fileB)); //切后文件 
                                string fileforlayoutHdr1 = Path.Combine(outsave, Path.GetFileNameWithoutExtension(fileB)) + ".hdr"; //切后的文件hdr
                                if (!File.Exists(fileforlayout1))
                                {
                                    string fileC = muticlip.MutiRegionsClip(fileB, repetClipAOI[regionName], outsave);
                                    string hdrfile = Path.GetDirectoryName(fileC) + "\\" + Path.GetFileNameWithoutExtension(fileC) + ".hdr";
                                    //文件名标识变为“HFSD”或“HFWE”
                                    #region
                                    if (fileforlayout1.Contains("MWSD"))
                                    {
                                        fileforlayout1 = fileforlayout1.Replace("MWSD", "HFSD");
                                        fileforlayoutHdr1 = fileforlayoutHdr1.Replace("MWSD", "HFSD");
                                    }
                                    if (fileforlayout1.Contains("MSWE"))
                                    {
                                        fileforlayout1 = fileforlayout1.Replace("MSWE", "HFWE");
                                        fileforlayoutHdr1 = fileforlayoutHdr1.Replace("MSWE", "HFWE");
                                    }
                                    FileInfo fi = new FileInfo(fileC);
                                    if (!File.Exists(fileforlayout1))
                                        fi.MoveTo(fileforlayout1);
                                    FileInfo fihdr = new FileInfo(hdrfile);
                                    if (!File.Exists(fileforlayoutHdr1))
                                        fihdr.MoveTo(fileforlayoutHdr1);
                                }
                                    #endregion
                                files2layout.Add(fileforlayout1);
                            }
                            List<string> resultList = MakeLayout(files2layout, regionName, gxdsave, layoutType);   //出专题图函数
                            foreach (string fileGXD in resultList)
                            {
                                IFileExtractResult res = new FileExtractResult(_subProductDef.Identify, fileGXD, false);
                                resultsArray.Add(res);
                            }
                        }
                        #endregion
                    }
                }
                #endregion
                #region 距平分布图
                if (jupingLayout == true) //距平分布图
                {
                    progressTracker(55, "距平分布图...");
                    string layoutType = "距平分布图";
                    string jpsavepath = outPut + "\\" + "距平分析";
                    if (!System.IO.Directory.Exists(jpsavepath))//如果不存在这个路径
                        System.IO.Directory.CreateDirectory(jpsavepath);
                    if (isMergeAois == true || aoiContainer.AOIs.Count() == 1)
                    {
                        #region 区域合成
                        string jupingFile = ""; List<string> jpFiles = new List<string>();
                        foreach (string avgAllFiles in clipFiles.Keys)
                        {
                            List<string> newfiles = clipFiles[avgAllFiles];
                            foreach (string file in newfiles)
                            {
                                jupingFile = JuPinComputer(file, avgAllFiles, jpsavepath);
                                string filterfile = smooth.ComputerMid(jupingFile, 5, filtersave);
                                string hfilterF = Path.GetDirectoryName(filterfile) + "\\" + Path.GetFileNameWithoutExtension(filterfile) + ".hdr";
                                if (IsAllContry.ToUpper() == "YES")
                                {
                                    string fileforlayout1 = Path.Combine(outsave, Path.GetFileName(filterfile)); 
                                    string fileforlayoutHdr1 = Path.Combine(outsave, Path.GetFileNameWithoutExtension(filterfile)) + ".hdr";
                                    FileInfo fi = new FileInfo(filterfile);
                                    if (!File.Exists(fileforlayout1))
                                        fi.MoveTo(fileforlayout1);
                                    FileInfo fihdr = new FileInfo(hfilterF);
                                    if (!File.Exists(fileforlayoutHdr1))
                                        fihdr.MoveTo(fileforlayoutHdr1);
                                    jpFiles.Add(fileforlayout1);
                                }
                                else
                                {
                                    string fileB = smooth.Bilinear(filterfile, 10, bilisave);
                                    string fileforlayout1 = Path.Combine(outsave, Path.GetFileName(fileB)); //切后文件 
                                    string fileforlayoutHdr1 = Path.Combine(outsave, Path.GetFileNameWithoutExtension(fileB)) + ".hdr"; //切后的文件hdr
                                    if (!File.Exists(fileforlayout1))
                                    {
                                        string fileC = muticlip.MutiRegionsClip(fileB, aoiContainer, outsave);
                                        string hdrfile = Path.GetDirectoryName(fileC) + "\\" + Path.GetFileNameWithoutExtension(fileC) + ".hdr";
                                        FileInfo fi = new FileInfo(fileC);
                                        if (!File.Exists(fileforlayout1))
                                            fi.MoveTo(fileforlayout1);
                                        FileInfo fihdr = new FileInfo(hdrfile);
                                        if (!File.Exists(fileforlayoutHdr1))
                                            fihdr.MoveTo(fileforlayoutHdr1);
                                    }
                                    jpFiles.Add(fileforlayout1);
                                }
                            }
                        }
                        string gxdsave = outPut + "\\" + "距平专题图";
                        if (!System.IO.Directory.Exists(gxdsave))//如果不存在这个路径
                            System.IO.Directory.CreateDirectory(gxdsave);
                        List<string> resultList = MakeLayout(jpFiles, MergeAoisName, gxdsave, layoutType);   //出专题图函数
                        foreach (string fileGXD in resultList)
                        {
                            IFileExtractResult res = new FileExtractResult(_subProductDef.Identify, fileGXD, false);
                            resultsArray.Add(res);
                        }
                        #endregion
                    }
                    else
                    {
                        #region 非区域合成
                        foreach (string regionName in clipFiles.Keys)  //dic<区域名，同期+周期>
                        {
                            string gxdsave = outPut + "\\" + regionName + "\\" + "距平专题图";
                            if (!System.IO.Directory.Exists(gxdsave))
                                System.IO.Directory.CreateDirectory(gxdsave);
                            string jupingFile = ""; List<string> jpFiles = new List<string>();
                            List<string> files2layout = new List<string>();
                            List<string> newfiles = clipFiles[regionName];
                            string confile = newfiles.First(); newfiles.Remove(confile);
                            foreach (string file in newfiles)
                            {
                                jupingFile = JuPinComputer(file, confile, jpsavepath);
                                string filterfile = smooth.ComputerMid(jupingFile, 5, filtersave);
                                string fileB = smooth.Bilinear(filterfile, 10, bilisave);
                                string fileforlayout1 = Path.Combine(outsave, Path.GetFileName(fileB)); //切后文件 
                                string fileforlayoutHdr1 = Path.Combine(outsave, Path.GetFileNameWithoutExtension(fileB)) + ".hdr"; //切后的文件hdr
                                if (!File.Exists(fileforlayout1))
                                {
                                    string fileC = muticlip.MutiRegionsClip(fileB, repetClipAOI[regionName], outsave);
                                    string hdrfile = Path.GetDirectoryName(fileC) + "\\" + Path.GetFileNameWithoutExtension(fileC) + ".hdr";
                                    #region
                                    FileInfo fi = new FileInfo(fileC);
                                    if (!File.Exists(fileforlayout1))
                                        fi.MoveTo(fileforlayout1);
                                    FileInfo fihdr = new FileInfo(hdrfile);
                                    if (!File.Exists(fileforlayoutHdr1))
                                        fihdr.MoveTo(fileforlayoutHdr1);
                                }
                                    #endregion
                                jpFiles.Add(fileforlayout1);
                            }
                            List<string> resultList = MakeLayout(jpFiles, regionName, gxdsave, layoutType);   //出专题图函数
                            foreach (string fileGXD in resultList)
                            {
                                IFileExtractResult res = new FileExtractResult(_subProductDef.Identify, fileGXD, false);
                                resultsArray.Add(res);
                            }
                        }
                        #endregion
                    }
                    Directory.Delete(jpsavepath,true); //删除中间文件
                }
                #endregion
                //Directory.Delete(filtersave, true);       //删除中间文件： 平滑、中值
                //Directory.Delete(smoothsave, true);
            }
            #endregion
            #region 统计图：周期柱状图，距平柱状图
            if (avgSat == true || jupingSat == true) //柱状图和距平柱状图，都要先计算柱，距平的再相减和除
            {
                Dictionary<string, List<string>> clipFiles = ClipFiles(dicFiles); //裁切
                //Dictionary<double,string> SWESDRecord = new Dictionary<double,string>(); // 同期数 + 同期的时间
                Dictionary<List<double>,List<string>> dateRecord = new Dictionary<List<double>,List<string>>(); // 周期数 + 周期的时间
                List<string> AllYears = new List<string>();//记录各年
                //Dictionary<string, double> yearData = new Dictionary<string,double>();//1987年：1987年上旬，1987年中旬，1987年下旬； 1988年：1988年上旬，1988年中旬，1988年下旬
                List<string> regionNs = new List<string>();//记录区域名称
                Dictionary<List<double>, List<string>> dateRecordAois = new Dictionary<List<double>, List<string>>(); // 数据 + 地区
                Dictionary<List<double>, List<string>> JupingdateRecord = new Dictionary<List<double>, List<string>>(); // 距平数 + 周期的时间
                //Dictionary<string, List<double>> doubleAois = new Dictionary<string, List<double>>();   // 区域名，这个区域历年周期数值
                //Dictionary<string, List<string>> doubleAoisYear = new Dictionary<string, List<string>>();//区域名称，对应历年年份
                if (isMergeAois == true || aoiContainer.AOIs.Count() == 1)
                {
                    #region 区域合成
                    #region SWE 取累加和
                    if (prdTypes == "SWE")
                    {
                        string svolPath = outPut + "\\" + "体积文件";
                        if (!System.IO.Directory.Exists(svolPath))
                            System.IO.Directory.CreateDirectory(svolPath);
                        foreach (string avgAllFiles in clipFiles.Keys)   // 上旬或中旬或下旬
                        {
                            AllYears.Clear();
                            double pixelSumCon = 0;
                            //string avgAllFilesDate = ParaseFileTime(avgAllFiles);
                            //SWESDRecord.Add(n, avgAllFiles);  //同期数 + 时间
                            List<double> pixelSumDouble = new List<double>();    //周期数值
                            List<string> fDate = new List<string>();             //年份
                            List<double> JuPingSumDouble = new List<double>();   //距平数值
                            List<string> newfiles = clipFiles[avgAllFiles];
                            foreach (string infile in newfiles)
                            {
                                string sweVolFile = svolPath + "\\" + Path.GetFileName(infile).Replace("MSWE", "SVOL");
                                if (!File.Exists(sweVolFile))
                                {
                                    IFileExtractResult sweVolResult = ComputeSnowSWEVOL(infile, sweVolFile);
                                    sweVolFile = sweVolResult.FileName;
                                }
                                double pixelSum = CompuCurPixel(sweVolFile);
                                pixelSumDouble.Add(pixelSum);
                                Regex DataReg = new Regex(@"(?<year>\d{4})", RegexOptions.Compiled);
                                string year = "";
                                Match m = DataReg.Match(Path.GetFileNameWithoutExtension(infile));
                                if (m.Success)
                                    year = m.Value;
                                fDate.Add(year); AllYears.Add(year);
                                pixelSumCon += pixelSum;
                            }
                            pixelSumCon = pixelSumCon / pixelSumDouble.Count;
                            foreach (double pixelSum in pixelSumDouble)
                            {
                                double pixelSumJP = (pixelSum - pixelSumCon) / pixelSumCon * 100.0d;
                                JuPingSumDouble.Add(pixelSumJP);
                            }
                            dateRecord.Add(pixelSumDouble,fDate); //周期数 + 周期时间
                            JupingdateRecord.Add(JuPingSumDouble, fDate);// 距平数值 + 周期时间
                        }
                        Directory.Delete(svolPath, true);
                    }
                    #endregion
                    #region SD  取均值
                    else
                    {
                        foreach (string avgAllFiles in clipFiles.Keys)
                        {
                            AllYears.Clear();
                            List<string> newfiles = clipFiles[avgAllFiles];
                            double pixelSumCon = 0;
                            using (IRasterDataProvider inRaster = RasterDataDriver.Open(avgAllFiles) as IRasterDataProvider)
                            {
                                pixelSumCon = CompuCurPixel(avgAllFiles);
                                pixelSumCon = pixelSumCon / (inRaster.Width * inRaster.Height);
                            }
                            string avgAllFilesDate = ParaseFileTime(avgAllFiles);
                            //SWESDRecord.Add(pixelSumCon,avgAllFilesDate);  //同期数 + 时间
                            List<double> pixelSumDouble = new List<double>();    //周期数值
                            List<string> fDate = new List<string>();             //年份
                            List<double> JuPingSumDouble = new List<double>();   //距平数值
                            foreach (string infile in newfiles)
                            {
                                double pixelSum;
                                using (IRasterDataProvider inRaster = RasterDataDriver.Open(infile) as IRasterDataProvider)
                                {
                                    pixelSum = CompuCurPixel(infile);
                                    pixelSum = pixelSum / (inRaster.Width * inRaster.Height);
                                    pixelSumDouble.Add(pixelSum);
                                }
                                double pixelSumJP = (pixelSum - pixelSumCon) / pixelSumCon * 100.0d;
                                JuPingSumDouble.Add(pixelSumJP);
                                Regex DataReg = new Regex(@"(?<year>\d{4})", RegexOptions.Compiled);
                                string year = "";
                                Match m = DataReg.Match(Path.GetFileNameWithoutExtension(infile));
                                if (m.Success)
                                    year = m.Value;
                                fDate.Add(year); AllYears.Add(year);
                            }
                            dateRecord.Add(pixelSumDouble,fDate); //同期数 + 同期时间
                            JupingdateRecord.Add(JuPingSumDouble, fDate);// 距平数值 + 周期时间
                            //Record.Add(SWESDRecord,dateRecord);
                        }
                    }
                    #endregion
                    #endregion
                }
                else
                {
                    #region 非区域合成
                    #region SWE 取累加和
                    if (prdTypes == "SWE")
                    {
                        foreach (string regionName in clipFiles.Keys)      // 计算后的数据组织还是dic<地名，同期+周期>
                        {
                            AllYears.Clear();
                            string svolPath = outPut + "\\" + regionName + "\\" + "体积文件";
                            if (!System.IO.Directory.Exists(svolPath))
                                System.IO.Directory.CreateDirectory(svolPath);
                            List<string> newfiles = clipFiles[regionName];
                            double pixelSumCon = 0;
                            string confile = newfiles.First(); newfiles.Remove(confile);  //第一个元素是同期的，提出来单做
                            List<double> pixelSumDouble = new List<double>();    //周期数值
                            List<string> fDate = new List<string>();             //年份
                            List<double> JuPingSumDouble = new List<double>();   //距平数值
                            foreach (string infile in newfiles)
                            {
                                string sweVolFile = svolPath + "\\" + Path.GetFileName(infile).Replace("MSWE", "SVOL");
                                if (!File.Exists(sweVolFile))
                                {
                                    IFileExtractResult sweVolResult = ComputeSnowSWEVOL(infile, sweVolFile);
                                    sweVolFile = sweVolResult.FileName;
                                }
                                double pixelSum = CompuCurPixel(sweVolFile);
                                pixelSumDouble.Add(pixelSum);
                                Regex DataReg = new Regex(@"(?<year>\d{4})", RegexOptions.Compiled);
                                string year = "";
                                Match m = DataReg.Match(Path.GetFileNameWithoutExtension(infile));
                                if (m.Success)
                                    year = m.Value;
                                fDate.Add(year); AllYears.Add(year);
                                pixelSumCon += pixelSum;
                            }
                            pixelSumCon = pixelSumCon / pixelSumDouble.Count ;
                            foreach (double pixelSum in pixelSumDouble)
                            {
                                double pixelSumJP = (pixelSum - pixelSumCon) / pixelSumCon * 100.0d;
                                JuPingSumDouble.Add(pixelSumJP);
                            }
                            dateRecord.Add(pixelSumDouble, fDate);         //周期数值 + 周期时间年 
                            JupingdateRecord.Add(JuPingSumDouble, fDate);  // 距平数值 + 周期时间
                            //SWESDAois.Add(regionName, pixelSumDouble);   //区域 + 数据
                            regionNs.Add(regionName);
                            Directory.Delete(svolPath, true);                    //删除中间文件： 雪水当量体积文件 
                        }
                    }
                    #endregion
                    #region SD  取均值
                    else
                    {
                        foreach (string regionName in clipFiles.Keys)            // 计算后的数据组织还是dic<地名，同期+周期>
                        {
                            AllYears.Clear();
                            List<string> newfiles = clipFiles[regionName];
                            List<double> pixelSumDouble = new List<double>();    //周期数值
                            List<string> fDate = new List<string>();             //年份
                            List<double> JuPingSumDouble = new List<double>();   //距平数值
                            string confile = newfiles.First(); newfiles.Remove(confile);  //第一个元素是同期的，提出来单做
                            double pixelSumCon;
                            using (IRasterDataProvider inRaster = RasterDataDriver.Open(confile) as IRasterDataProvider)
                            {
                                pixelSumCon = CompuCurPixel(confile);                    //同期的统计值
                                pixelSumCon = pixelSumCon / (inRaster.Width * inRaster.Height);
                            }          
                            foreach (string infile in newfiles)
                            {
                                double pixelSum;
                                using (IRasterDataProvider inRaster = RasterDataDriver.Open(infile) as IRasterDataProvider)
                                {
                                    pixelSum = CompuCurPixel(infile);
                                    pixelSum = pixelSum / (inRaster.Width * inRaster.Height);
                                    pixelSumDouble.Add(pixelSum); 
                                }
                                double pixelSumJP = (pixelSum - pixelSumCon) / pixelSumCon * 100.0d;
                                JuPingSumDouble.Add(pixelSumJP);
                                Regex DataReg = new Regex(@"(?<year>\d{4})", RegexOptions.Compiled);
                                string year = "";
                                Match m = DataReg.Match(Path.GetFileNameWithoutExtension(infile));
                                if (m.Success)
                                    year = m.Value;
                                fDate.Add(year); AllYears.Add(year);
                            }
                            dateRecord.Add(pixelSumDouble, fDate); //周期数值 + 周期时间
                            JupingdateRecord.Add(JuPingSumDouble, fDate);// 距平数值 + 周期时间
                            //SWESDAois.Add(regionName, pixelSumDouble);
                            regionNs.Add(regionName);
                        }
                    }
                    #endregion
                    #endregion
                }
                #region 统计柱状图
                if (avgSat == true)
                {
                    List<string[]> rows = new List<string[]>();
                    string[] columns; 
                    if (isMergeAois == true || aoiContainer.AOIs.Count() == 1)
                    {
                        progressTracker(75, "周期统计图...");
                        #region 区域合成
                        #region 废代码
                        //if (Record.Count == 1)   //只有一个周期，只用dateRecord：<List<double>,List<string>>(); 周期数 + 周期的时间即可
                        //{
                        //    foreach (List<double> pixel in dateRecord.Keys)
                        //    {
                        //        double[] pixelSum = pixel.ToArray(); string[] filesDate = dateRecord[pixel].ToArray();
                        //        for (int i = 0; i < pixelSum.Length; i++)
                        //        {
                        //            if (prdTypes == "SWE")
                        //                rows.Add(new string[] { filesDate[i], (pixelSum[i] / 100000000).ToString() });
                        //            if (prdTypes == "SD")
                        //                rows.Add(new string[] { filesDate[i], (pixelSum[i]).ToString() });
                        //        }
                        //    }
                        //    if (prdTypes == "SWE")
                        //        columns = new string[] { MergeAoisName,"累计SWE体积（亿立方米）" };
                        //    else
                        //        columns = new string[] { MergeAoisName,"雪深均值（厘米）" };
                        //}
                        //else                     //多个周期
                        //{
                        #endregion
                        string[] rowKeys = AllYears.ToArray();  // 行
                        List<string> cols = new List<string>(); // 列
                        cols.Add("周期类型");
                        string[] filterKeys = new string[] { }; List<string> filterString = new List<string>();
                        #region 设置filterKeys
                        string exinfos = "";
                        if (prdTypes == "SWE")
                            exinfos = "(亿立方米)";
                        else
                            exinfos = "(厘米)";
                        switch (periodTypes.ToUpper())
                        {
                            case "MONTH":
                                {
                                    int startMonth = Convert.ToInt32(startEndTime[0].Split('_')[0]);
                                    int endMonth = Convert.ToInt32(startEndTime[1].Split('_')[0]);
                                    filterString.Add(startEndTime[0].Split('_')[0] + "月" + exinfos);
                                    if (startMonth < endMonth)
                                    {
                                        for (int i = 1; i <= endMonth - startMonth; i++)
                                        {
                                            if (startMonth + i <= endMonth)
                                            {
                                                filterString.Add(Convert.ToString(startMonth + i) + "月" + exinfos);
                                            }
                                        }
                                    }
                                    filterKeys = filterString.ToArray();
                                    break;
                                }
                            case "TEN":
                                {
                                    string startTen = startEndTime[0].Split('_')[1];
                                    string endTen = startEndTime[1].Split('_')[1];
                                    if (startTen == endTen)
                                    {
                                        if (startTen == "1")
                                            filterKeys = new string[] { "上旬" + exinfos };
                                        if(startTen == "2")
                                            filterKeys = new string[] { "中旬" + exinfos };
                                        if (startTen == "3")
                                            filterKeys = new string[] { "下旬" + exinfos };
                                    }
                                    else
                                    {
                                        if (startTen == "1" && endTen == "3")
                                            filterKeys = new string[] { "上旬" + exinfos, "中旬" + exinfos, "下旬" + exinfos };
                                        else
                                        {
                                            if (startTen == "1")
                                                filterKeys = new string[] { "上旬" + exinfos, "中旬" + exinfos };
                                            else
                                                filterKeys = new string[] { "中旬" + exinfos, "下旬" + exinfos };
                                        }
                                    }
                                    break;
                                }
                            case "PENTAD":
                                {
                                    int start = Convert.ToInt32(startEndTime[0].Split('_')[2]);
                                    int end = Convert.ToInt32(startEndTime[1].Split('_')[2]);
                                    if (start == end)
                                    {
                                        filterString.Add("第" + startEndTime[0].Split('_')[0] + "侯" + exinfos);
                                    }
                                    else
                                    {
                                        if (start < end)
                                        {
                                            for (int i = 0; i <= end - start; i++)
                                            {
                                                if (start + i <= end)
                                                    filterString.Add("第" + Convert.ToString(start + i) + "侯" + exinfos);
                                            }
                                        }
                                    }
                                    filterKeys = filterString.ToArray();
                                    break;
                                }
                            case "WINTER":
                                {
                                    filterString.Add("雪季" + exinfos);
                                    filterKeys = filterString.ToArray();
                                    break;
                                }
                        }
                        #endregion
                        cols.AddRange(filterKeys);
                        columns = cols.ToArray();
                        Dictionary<string, double[]> dRow = new Dictionary<string, double[]>();
                        for (int i = 0; i < rowKeys.Length; i++)
                        {
                            string type = rowKeys[i]; //年 如1988，1989，1990 .........
                            string[] row = new string[1 + filterKeys.Length];
                            row[0] = type + "年";
                            for (int j = 0; j < filterKeys.Length; j++) //上 中  下旬
                            {
                                int id = 0;
                                foreach (List<double> pixel in dateRecord.Keys)   // 周期数值 一旬的 1987、19888、1989、1990 上旬的数值
                                {
                                    double[] pixelSum = pixel.ToArray(); string[] filesDate = dateRecord[pixel].ToArray();
                                    if (id == j)
                                    {
                                        for (int n = 0; n < pixelSum.Length; n++)    //  周期的时间  1987、19888、1989、1990
                                        {
                                            if (filesDate[n] == type)
                                            {
                                                if (prdTypes == "SD")
                                                    row[j + 1] = pixelSum[n].ToString();
                                                else
                                                    row[j + 1] = (pixelSum[n] / 100000000).ToString();
                                                continue;
                                            }
                                        }
                                    }
                                    id++;
                                }
                            }
                            rows.Add(row);
                        }
                        //}
                        #endregion
                    }
                    else
                    {
                        progressTracker(75, "周期统计图...");
                        #region 非区域合成
                        string[] rowKeys = AllYears.ToArray();  // 行
                        List<string> cols = new List<string>(); // 列
                        cols.Add("区域名称");
                        List<string> filterKeysString = new List<string>();
                        string exinfos = "";
                        if(prdTypes == "SWE")
                            exinfos = "(亿立方米)";
                        else
                            exinfos = "(厘米)";
                        foreach(string region in regionNs)
                        {
                            filterKeysString.Add(region + exinfos); 
                        }
                        string[] filterKeys = filterKeysString.ToArray(); //各地方名称
                        cols.AddRange(filterKeys);
                        columns = cols.ToArray();
                        for (int i = 0; i < rowKeys.Length; i++)
                        {
                            string type = rowKeys[i]; //年 
                            string[] row = new string[1 + filterKeys.Length];
                            row[0] = type + "年";
                            for (int j = 0; j < filterKeys.Length; j++) //黑 吉  
                            {
                                int id = 0;
                                foreach (List<double> pixel in dateRecord.Keys)   // 周期数值地区的 2012\2013 1月的数值
                                {
                                    double[] pixelSum = pixel.ToArray(); string[] filesDate = dateRecord[pixel].ToArray();
                                    if (id == j)
                                    {
                                        for (int n = 0; n < pixelSum.Length; n++)    //  周期的时间  2012、2013
                                        {
                                            if (filesDate[n] == type)
                                            {
                                                if (prdTypes == "SWE")
                                                    row[j + 1] = (pixelSum[n] / 100000000).ToString();
                                                else
                                                    row[j + 1] = pixelSum[n].ToString();
                                                continue;
                                            }
                                        }
                                    }
                                    id++;
                                }
                            }
                            rows.Add(row);
                        }
                        #endregion
                    }
                    string subTitle = "统计日期：" + DateTime.Now.ToShortDateString();
                    IStatResult results = null;
                    if (rows == null || rows.Count == 0)
                        return null;
                    else
                        results = new StatResult(subTitle, columns, rows.ToArray());
                    if (results == null)
                        return null;
                    string exinfo = statTypes;
                    string title = GetExcelTitle(choseAois, exinfo);
                    string fname = "";//excelfile; 
                    string outputIdentify = "STAT";
                    string filename = StatResultToFile(new string[] { fname }, results, "MWS", outputIdentify, title, null, 1, true, 1);
                    string newexcelfile = Path.Combine(outPut, "MWS_STAT_" + title + ".XLSX");
                    if (File.Exists(newexcelfile))
                        Console.WriteLine("文件已存在");
                    else
                    {
                        FileInfo fi1 = new FileInfo(filename);
                        fi1.MoveTo(newexcelfile);
                    }
                    IFileExtractResult res = new FileExtractResult(_subProductDef.Identify, newexcelfile, false);
                    resultsArray.Add(res);
                }
                #endregion
                #region 距平柱状图
                if (jupingSat == true)
                {
                    List<string[]> rows = new List<string[]>();
                    string[] columns;
                    if (isMergeAois == true || aoiContainer.AOIs.Count()==1)
                    {
                        #region 区域合成
                        progressTracker(95, "距平统计图...");
                        string[] rowKeys = AllYears.ToArray();  // 行
                        List<string> cols = new List<string>(); // 列
                        cols.Add("周期类型");
                        string[] filterKeys = new string[] { }; List<string> filterString = new List<string>();
                        #region 设置filterKeys
                    string exinfos = "";
                    switch (periodTypes.ToUpper())
                    {
                        case "MONTH":
                            {
                                int startMonth = Convert.ToInt32(startEndTime[0].Split('_')[0]);
                                int endMonth = Convert.ToInt32(startEndTime[1].Split('_')[0]);
                                filterString.Add(startEndTime[0].Split('_')[0] + "月" + exinfos);
                                if (startMonth < endMonth)
                                {
                                    for (int i = 1; i <= endMonth - startMonth; i++)
                                    {
                                        if (startMonth + i <= endMonth)
                                        {
                                            filterString.Add(Convert.ToString(startMonth + i) + "月" + exinfos);
                                        }

                                    }
                                }
                                filterKeys = filterString.ToArray();
                                break;
                            }
                        case "TEN":
                            {
                                string startTen = startEndTime[0].Split('_')[1];
                                string endTen = startEndTime[1].Split('_')[1];
                                if (startTen == endTen)
                                {
                                    if (startTen == "1")
                                        filterKeys = new string[] { "上旬" + exinfos };
                                    if(startTen == "2")
                                        filterKeys = new string[] { "中旬" + exinfos };
                                    if (startTen == "3")
                                        filterKeys = new string[] { "下旬" + exinfos };
                                }
                                else
                                {
                                    if (startTen == "1" && endTen == "3")
                                        filterKeys = new string[] { "上旬" + exinfos, "中旬" + exinfos, "下旬" + exinfos };
                                    else
                                    {
                                        if (startTen == "1")
                                            filterKeys = new string[] { "上旬" + exinfos, "中旬" + exinfos };
                                        else
                                            filterKeys = new string[] { "中旬" + exinfos, "下旬" + exinfos };
                                    }
                                }
                                break;
                            }
                        case "PENTAD":
                            {
                                int start = Convert.ToInt32(startEndTime[0].Split('_')[2]);
                                int end = Convert.ToInt32(startEndTime[1].Split('_')[2]);
                                //filterString.Add(startEndTime[0].Split('_')[0] + "侯" + exinfos);
                                if (start == end)
                                {
                                    filterString.Add("第" + startEndTime[0].Split('_')[0] + "侯" + exinfos);
                                }
                                else
                                {
                                    if (start < end)
                                    {
                                        for (int i = 0; i <= end - start; i++)
                                        {
                                            if (start + i <= end)
                                                filterString.Add("第" + Convert.ToString(start + i) + "侯" + exinfos);
                                        }
                                    }
                                }
                                filterKeys = filterString.ToArray();
                                break;
                            }
                        case "WINTER":
                            {
                                filterString.Add("雪季" + exinfos);
                                filterKeys = filterString.ToArray();
                                break;
                            }
                    }
                    #endregion
                        cols.AddRange(filterKeys);
                        columns = cols.ToArray();
                        Dictionary<string, double[]> dRow = new Dictionary<string, double[]>();
                        for (int i = 0; i < rowKeys.Length; i++)
                        {
                            string type = rowKeys[i]; //年 如1988，1989，1990 .........
                            string[] row = new string[1 + filterKeys.Length];
                            row[0] = type + "年";
                            for (int j = 0; j < filterKeys.Length; j++) //上 中  下旬
                            {
                                int id = 0;
                                foreach (List<double> pixel in JupingdateRecord.Keys)   // 周期数值 一旬的 1987、19888、1989、1990 上旬的数值
                                {
                                    double[] pixelSum = pixel.ToArray(); string[] filesDate = JupingdateRecord[pixel].ToArray();
                                    if (id == j)
                                    {
                                        for (int n = 0; n < pixelSum.Length; n++)    //  周期的时间  1987、19888、1989、1990
                                        {
                                            if (filesDate[n] == type)
                                            {
                                                row[j + 1] = pixelSum[n].ToString() + "%";
                                                continue;
                                            }
                                        }
                                    }
                                    id++;
                                }
                            }
                            rows.Add(row);
                        }
                        #endregion
                    }
                    else
                    {
                        #region 非区域合成
                        progressTracker(95, "距平统计图...");
                        string[] rowKeys = AllYears.ToArray();  // 行
                        List<string> cols = new List<string>(); // 列
                        cols.Add("区域名称");
                        List<string> filterKeysString = new List<string>();
                        string exinfos = "";
                        foreach(string region in regionNs)
                        {
                            filterKeysString.Add(region + exinfos); 
                        }
                        string[] filterKeys = regionNs.ToArray(); //各地方名称
                        cols.AddRange(filterKeys);
                        columns = cols.ToArray();
                        for (int i = 0; i < rowKeys.Length; i++)
                        {
                            string type = rowKeys[i]; //年 
                            string[] row = new string[1 + filterKeys.Length];
                            row[0] = type + "年";
                            for (int j = 0; j < filterKeys.Length; j++) //黑 吉  
                            {
                                int id = 0;
                                foreach (List<double> pixel in JupingdateRecord.Keys)   // 周期数值地区的 2012\2013 1月的数值
                                {
                                    double[] pixelSum = pixel.ToArray(); string[] filesDate = JupingdateRecord[pixel].ToArray();
                                    if (id == j)
                                    {
                                        for (int n = 0; n < pixelSum.Length; n++)    //  周期的时间  2012、2013
                                        {
                                            if (filesDate[n] == type)
                                            {
                                                row[j + 1] = pixelSum[n].ToString() + "%";
                                                continue;
                                            }
                                        }
                                    }
                                    id++;
                                }
                            }
                            rows.Add(row);
                        }
                        #endregion
                    }
                    string subTitle = "统计日期：" + DateTime.Now.ToShortDateString();
                    IStatResult results = null;
                    if (rows == null || rows.Count == 0)
                        return null;
                    else
                        results = new StatResult(subTitle, columns, rows.ToArray());
                    if (results == null)
                        return null;
                    string exinfo = "距平";
                    string title = GetExcelTitle(choseAois, exinfo);
                    string fname = "";// excelfile; 
                    string outputIdentify = "JUPI";
                    string filename = StatResultToFile(new string[] { fname }, results, "MWS", outputIdentify, title, null, 1, true, 1);
                    string newexcelfile = Path.Combine(outPut, "MWS_JUPI_" + title + ".XLSX");
                    if (File.Exists(newexcelfile))
                        Console.WriteLine("文件已存在");
                    else
                    {
                        FileInfo fi1 = new FileInfo(filename);
                        fi1.MoveTo(newexcelfile);
                    }
                    IFileExtractResult res = new FileExtractResult(_subProductDef.Identify, newexcelfile, false);
                    resultsArray.Add(res);
                }
                #endregion
            }
            #endregion
            //Directory.Delete(clipsave,true);    //删除中间文件：裁切
            progressTracker(99, "时间序列分析完成.");
            return resultsArray;
        }
        //连续时间数据处理
        private IExtractResult ContinueTimeSeriesAnalysis(Dictionary<string, List<string>> dicFiles, Action<int, string> progressTracker)
        {
            IExtractResultArray resultsArray = new ExtractResultArray("");
            List<string> newfiles = dicFiles["continue"]; 
            MulRegionsClip muticlip = new MulRegionsClip();
            #region 范围
            Dictionary<List<double>, List<string>> dateRecord = new Dictionary<List<double>, List<string>>(); // 周期数 + 周期的时间
            List<string> regionNs = new List<string>();//记录区域名称
            #region 周期专题图
            if (avgLayout == true)
            {
                //先按矩形提数据 再中值 再插值 再按矢量边裁切
                Dictionary<string, List<string>> clipFiles = new Dictionary<string, List<string>>();
                progressTracker(25, "数据处理...");
                if (IsAllContry.ToUpper() == "YES")
                {
                    List<string> templist = dicFiles["continue"];
                    clipFiles.Add(choseAois,templist);
                }//new Dictionary<choseAois,dicFiles["continue"]>();
                else
                    clipFiles = ClipRectFiles1(dicFiles);  //矩形提数据
                progressTracker(60, "周期专题图...");
                string smoothsave = ""; string filtersave = ""; string bilisave = ""; string outsave = "";
                smoothsave = outPut + "\\" + "中间文件（可删除）";       //创建平滑路径
                if (!System.IO.Directory.Exists(smoothsave))
                    System.IO.Directory.CreateDirectory(smoothsave);
                filtersave = smoothsave;                       //创建平滑\\中值
                bilisave = smoothsave + "\\" + "处理文件";    //创建平滑\\插值
                if (!System.IO.Directory.Exists(bilisave))
                    System.IO.Directory.CreateDirectory(bilisave);
                outsave = outPut + "\\" + "专题图数据源";
                if (!System.IO.Directory.Exists(outsave))
                    System.IO.Directory.CreateDirectory(outsave);
                string layoutType = "分布图";
                foreach (string regionName in clipFiles.Keys)
                {
                    string gxdsave = outPut + "\\" + regionName + "\\" + "周期分布专题图";
                    if (!System.IO.Directory.Exists(gxdsave))
                        System.IO.Directory.CreateDirectory(gxdsave);
                    List<string> files2layout = new List<string>();
                    List<string> files = clipFiles[regionName];
                    foreach (string infile in files)
                    {
                        string fileF = smooth.ComputerMid(infile, 5, filtersave); //中值
                        string hfilterF = Path.GetDirectoryName(fileF) + "\\" + Path.GetFileNameWithoutExtension(fileF) + ".hdr";
                        if (IsAllContry.ToUpper() == "YES")
                        {
                            string fileforlayout = Path.Combine(outsave, Path.GetFileName(fileF));
                            string fileforlayoutHdr = Path.Combine(outsave, Path.GetFileNameWithoutExtension(fileF)) + ".hdr";
                            //文件名标识变为“HFSD”或“HFWE”
                            #region
                            if (fileforlayout.Contains("MWSD"))
                            {
                                fileforlayout = fileforlayout.Replace("MWSD", "HFSD");
                                fileforlayoutHdr = fileforlayoutHdr.Replace("MWSD", "HFSD");
                            }
                            if (fileforlayout.Contains("MSWE"))
                            {
                                fileforlayout = fileforlayout.Replace("MSWE", "HFWE");
                                fileforlayoutHdr = fileforlayoutHdr.Replace("MSWE", "HFWE");
                            }
                            FileInfo fif = new FileInfo(fileF);
                            if (!File.Exists(fileforlayout))
                                fif.MoveTo(fileforlayout);
                            FileInfo fih = new FileInfo(hfilterF);
                            if (!File.Exists(fileforlayoutHdr))
                                fih.MoveTo(fileforlayoutHdr);
                            #endregion
                            files2layout.Add(fileforlayout);
                            
                        }
                        else
                        {
                            string fileB = smooth.Bilinear(fileF, 10, bilisave);      //插值
                            string fileforlayout1 = Path.Combine(outsave, Path.GetFileName(fileB)); //切后文件 
                            string fileforlayoutHdr1 = Path.Combine(outsave, Path.GetFileNameWithoutExtension(fileB)) + ".hdr"; //切后的文件hdr
                            if (!File.Exists(fileforlayout1))
                            {
                                string fileC = muticlip.MutiRegionsClip(fileB, repetClipAOI[regionName], outsave);
                                string hdrfile = Path.GetDirectoryName(fileC) + "\\" + Path.GetFileNameWithoutExtension(fileC) + ".hdr";
                                #region 文件名标识变为“HFSD”或“HFWE”
                                if (fileforlayout1.Contains("MWSD"))
                                {
                                    fileforlayout1 = fileforlayout1.Replace("MWSD", "HFSD");
                                    fileforlayoutHdr1 = fileforlayoutHdr1.Replace("MWSD", "HFSD");
                                }
                                if (fileforlayout1.Contains("MSWE"))
                                {
                                    fileforlayout1 = fileforlayout1.Replace("MSWE", "HFWE");
                                    fileforlayoutHdr1 = fileforlayoutHdr1.Replace("MSWE", "HFWE");
                                }
                                FileInfo fi = new FileInfo(fileC);
                                if (!File.Exists(fileforlayout1))
                                    fi.MoveTo(fileforlayout1);
                                FileInfo fihdr = new FileInfo(hdrfile);
                                if (!File.Exists(fileforlayoutHdr1))
                                    fihdr.MoveTo(fileforlayoutHdr1);
                                #endregion
                            } 
                            files2layout.Add(fileforlayout1);
                        }
                    }
                    List<string> resultList = MakeLayout(files2layout, regionName, gxdsave, layoutType);   //出专题图函数
                    foreach (string fileGXD in resultList)
                    {
                        IFileExtractResult res = new FileExtractResult(_subProductDef.Identify, fileGXD, false);
                        resultsArray.Add(res);
                    }
                }
            }
            #endregion
            #region 周期统计图
            if (avgSat == true)
            {
                progressTracker(80, "周期统计图...");
                Dictionary<string, List<string>> clipFiles = new Dictionary<string, List<string>>();
                if (IsAllContry.ToUpper() == "YES")
                {
                    List<string> templist = dicFiles["continue"];
                    clipFiles.Add(choseAois, templist);
                }
                else
                    clipFiles = ClipRectFiles1(dicFiles);  //矩形提数据
                #region SWE 取累加和
                List<string> AllYears = new List<string>();   //记录周期时间
                if (prdTypes == "SWE")
                {
                    foreach (string regionName in clipFiles.Keys)      // 计算后的数据组织还是dic<地名，同期+周期>
                    {
                        AllYears.Clear();
                        string svolPath = outPut + "\\" + regionName + "\\" + "体积文件";
                        if (!System.IO.Directory.Exists(svolPath))
                            System.IO.Directory.CreateDirectory(svolPath);
                        List<string> files = clipFiles[regionName];
                        List<double> pixelSumDouble = new List<double>();    //周期数值
                        List<string> fDate = new List<string>();             //时间
                        List<double> JuPingSumDouble = new List<double>();   //距平数值
                        foreach (string infile in files)
                        {
                            string sweVolFile = svolPath + "\\" + Path.GetFileName(infile).Replace("MSWE", "SVOL");
                            if (!File.Exists(sweVolFile))
                            {
                                IFileExtractResult sweVolResult = ComputeSnowSWEVOL(infile, sweVolFile);
                                sweVolFile = sweVolResult.FileName;
                            }
                            double pixelSum = CompuCurPixel(sweVolFile);
                            pixelSumDouble.Add(pixelSum);
                            string filetime = ParaseFileTime(infile);
                            fDate.Add(filetime);
                            AllYears.Add(filetime);
                        }
                        dateRecord.Add(pixelSumDouble, fDate);               //周期数值 + 时间年 
                        regionNs.Add(regionName);
                        Directory.Delete(svolPath, true);                    //删除中间文件： 雪水当量体积文件 
                    }
                }
                #endregion
                #region SD  取均值
                else
                {
                    foreach (string regionName in clipFiles.Keys)            // 计算后的数据组织还是dic<地名，同期+周期>
                    {
                        AllYears.Clear();
                        List<double> pixelSumDouble = new List<double>();    //周期数值
                        List<string> fDate = new List<string>();             //年份
                        List<double> JuPingSumDouble = new List<double>();   //距平数值
                        string confile = newfiles.First(); newfiles.Remove(confile);  //第一个元素是同期的，提出来单做
                        double pixelSumCon;
                        using (IRasterDataProvider inRaster = RasterDataDriver.Open(confile) as IRasterDataProvider)
                        {
                            pixelSumCon = CompuCurPixel(confile);                    //同期的统计值
                            pixelSumCon = pixelSumCon / (inRaster.Width * inRaster.Height);
                        }
                        foreach (string infile in newfiles)
                        {
                            double pixelSum;
                            using (IRasterDataProvider inRaster = RasterDataDriver.Open(infile) as IRasterDataProvider)
                            {
                                pixelSum = CompuCurPixel(infile);
                                pixelSum = pixelSum / (inRaster.Width * inRaster.Height);
                                pixelSumDouble.Add(pixelSum);
                            }
                            double pixelSumJP = (pixelSum - pixelSumCon) / pixelSumCon * 100.0d;
                            string filetime = ParaseFileTime(infile);
                            fDate.Add(filetime);
                            AllYears.Add(filetime);
                        }
                        dateRecord.Add(pixelSumDouble, fDate); //周期数值 + 周期时间
                        regionNs.Add(regionName);
                    }
                }
                #endregion
                progressTracker(75, "周期统计图...");
                List<string[]> rows = new List<string[]>();
                string[] columns;
                string[] rowKeys = AllYears.ToArray();  // 行 时间 年+具体时间
                List<string> cols = new List<string>(); // 列 区域名称
                cols.Add("区域名称");
                List<string> filterKeysString = new List<string>();
                string exinfos = "";
                if (prdTypes == "SWE")
                    exinfos = "(亿立方米)";
                else
                    exinfos = "(厘米)";
                foreach (string region in regionNs)
                {
                    filterKeysString.Add(region + exinfos);
                }
                string[] filterKeys = filterKeysString.ToArray(); //各地方名称
                cols.AddRange(filterKeys);
                columns = cols.ToArray();
                for (int i = 0; i < rowKeys.Length; i++)
                {
                    string type = rowKeys[i]; //年 
                    string[] row = new string[1 + filterKeys.Length];
                    row[0] = type;
                    for (int j = 0; j < filterKeys.Length; j++) //黑  吉  辽
                    {
                        int id = 0;
                        foreach (List<double> pixel in dateRecord.Keys)   // 周期数值地区的 2012\2013 1月的数值
                        {
                            double[] pixelSum = pixel.ToArray(); string[] filesDate = dateRecord[pixel].ToArray();
                            if (id == j)
                            {
                                for (int n = 0; n < pixelSum.Length; n++)    //  周期的时间  2012、2013
                                {
                                    if (filesDate[n] == type)
                                    {
                                        if (prdTypes == "SWE")
                                            row[j + 1] = (pixelSum[n] / 100000000).ToString();
                                        else
                                            row[j + 1] = pixelSum[n].ToString();
                                        continue;
                                    }
                                }
                            }
                            id++;
                        }
                    }
                    rows.Add(row);
                }
                string subTitle = "";
                if (prdTypes == "SWE")
                    subTitle = "统计日期：" + DateTime.Now.ToShortDateString();
                if (prdTypes == "SD")
                    subTitle = "统计日期：" + DateTime.Now.ToShortDateString();
                IStatResult results = null;
                if (rows == null || rows.Count == 0)
                    return null;
                else
                    results = new StatResult(subTitle, columns, rows.ToArray());
                if (results == null)
                    return null;
                string exinfo = statTypes;
                string title = GetExcelTitle(choseAois, exinfo);
                string fname = ""; string outputIdentify = "STAT";
                string filename = StatResultToFile(new string[] { fname }, results, "MWS", outputIdentify, title, null, 1, true, 1);
                string newexcelfile = Path.Combine(outPut, "MWS_STAT_" + title + ".XLSX");
                if (File.Exists(newexcelfile))
                {
                    Console.WriteLine("文件已存在");
                    File.Delete(newexcelfile);
                }
                else
                {
                    FileInfo fi1 = new FileInfo(filename);
                    fi1.MoveTo(newexcelfile);
                }
                IFileExtractResult res = new FileExtractResult(_subProductDef.Identify, newexcelfile, false);
                resultsArray.Add(res);
            }
            #endregion
            #endregion
            progressTracker(95, "时间序列分析完成.");
            return resultsArray;
        }
        private List<string> MakeLayout(List<string> files, string regionName, string gxdsave, string layoutType)
        {
            IExtractResultArray results = new ExtractResultArray("");
            List<string> resultGxds = new List<string>();
            foreach (string arguments in files.ToArray())
            {
                string product = "";
                if (arguments.Contains("HFSD"))
                {
                    _argumentProvider.SetArg("OutFileIdentify", "HSDI");
                    product = "雪深";
                }
                if (arguments.Contains("HFWE"))
                {
                    _argumentProvider.SetArg("OutFileIdentify", "HSWI");
                    product = "雪水当量";
                }
                if (arguments.Contains("JPEA"))
                {
                    product = "雪水当量";
                    _argumentProvider.SetArg("OutFileIdentify", "JPEI");
                }
                if (arguments.Contains("JPDA"))
                {
                    product = "雪深";
                    _argumentProvider.SetArg("OutFileIdentify", "JPDI");
                }
                GxdTitleName = GetGxdTitle(arguments, layoutType, product, regionName, statTypes);
                _argumentProvider.SetArg("SelectedPrimaryFiles", arguments);
                _argumentProvider.SetArg("fileOpenArgs", arguments);
                FileExtractResult result = ThemeGraphyResult(null) as FileExtractResult;
                //增加矢量
                string shpPathDir = AppDomain.CurrentDomain.BaseDirectory;
                string shpPath = Path.Combine(shpPathDir, @"数据引用\基础矢量\行政区划\面\中国行政区.shp");
                switch (regionName)
                {
                    case "全国":
                        shpPath = Path.Combine(shpPathDir, @"数据引用\基础矢量\行政区划\面\中国行政区.shp");
                        break;
                    case "青藏地区":
                        shpPath = Path.Combine(shpPathDir, @"数据引用\基础矢量\行政区划\线\青藏.shp");
                        break;
                    case "黑龙江省":
                        shpPath = Path.Combine(shpPathDir, @"数据引用\基础矢量\行政区划\线\黑龙江省.shp");
                        break;
                    case "吉林省":
                        shpPath = Path.Combine(shpPathDir, @"数据引用\基础矢量\行政区划\线\吉林省.shp");
                        break;
                    case "青海省":
                        shpPath = Path.Combine(shpPathDir, @"数据引用\基础矢量\行政区划\线\青海省.shp");
                        break;
                    case "西藏自治区":
                        shpPath = Path.Combine(shpPathDir, @"数据引用\基础矢量\行政区划\线\西藏自治区.shp");
                        break;
                    case "内蒙古自治区":
                        shpPath = Path.Combine(shpPathDir, @"数据引用\基础矢量\行政区划\线\内蒙古自治区.shp");
                        break;
                    case "新疆古自治区":
                        shpPath = Path.Combine(shpPathDir, @"数据引用\基础矢量\行政区划\线\新疆古自治区.shp");
                        break;    
                }
                CreateMcd(shpPath);
                _shpFile = shpPath;
                _gxdFile = (result as FileExtractResult).FileName;
                AddShpToGxd();
                string newresultfile = Path.Combine(gxdsave, Path.GetFileNameWithoutExtension(arguments) + ".gxd");
                FileInfo fi = new FileInfo(result.FileName);
                if (!File.Exists(newresultfile))
                    fi.MoveTo(newresultfile);
                else
                {
                    FileInfo fi1 = new FileInfo(newresultfile);
                    fi1.Delete();
                    fi.MoveTo(newresultfile);
                }
                resultGxds.Add(newresultfile);
                IFileExtractResult res = new FileExtractResult(_subProductDef.Identify, newresultfile, false);
                results.Add(res);
            }
            return resultGxds;
        }
        #region 专题图设置相关
        private void CreateMcd(string shpFile)
        {
            try
            {
                //1.文件复制
                string sourceFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"SystemData\ProductArgs\MWS\历史周期积雪监测专题图.mcd");
                if (!File.Exists(sourceFileName))
                    return;
                string shpName = Path.GetFileNameWithoutExtension(shpFile);
                string newFileName = Path.Combine(Path.GetDirectoryName(shpFile), shpName + ".mcd");
                File.Copy(sourceFileName, newFileName, true);
                //2.修改属性
                XDocument doc = XDocument.Load(newFileName);
                XElement layerElement = doc.Element("Map").Element("Layers").Element("Layer");
                if (layerElement == null)
                    return;
                layerElement.Attribute("name").Value = shpName;
                XElement dataSourceEle = layerElement.Element("FeatureClass").Element("DataSource");
                if (dataSourceEle == null)
                    return;
                dataSourceEle.Attribute("name").Value = shpName;
                dataSourceEle.Attribute("fileurl").Value = ".\\" + Path.GetFileName(shpFile);
                doc.Save(newFileName);
            }
            catch
            {
                //PrintInfo("创建mcd失败。");
            }
        }
        GxdEnvelope _envelope = null;
        protected override void ApplyAttributesOfLayoutTemplate(ILayoutTemplate template)
        {
            string instanceIdentify = _argumentProvider.GetArg("OutFileIdentify") as string;
            string[] files = GetStringArray("SelectedPrimaryFiles");
            ILayout layout = template.Layout;
            double minx;
            double miny;
            double maxx;
            double maxy;
            using (IRasterDataProvider dataPrd = GeoDataDriver.Open(files[0], "") as IRasterDataProvider)
            {
                minx = dataPrd.CoordEnvelope.MinX - 1;   //扩大数据框范围
                maxx = dataPrd.CoordEnvelope.MaxX + 1;
                miny = dataPrd.CoordEnvelope.MinY - 1;
                maxy = dataPrd.CoordEnvelope.MaxY + 1;
            }
            _envelope = new Layout.GxdEnvelope(minx, maxx, miny, maxy);
            Dictionary<string, string> vars = new Dictionary<string, string>();
            vars.Add("{NAME}", GxdTitleName);
            template.ApplyVars(vars);

        }
        //添加Shp
        private void AddShpToGxd()
        {
            //1.文件复制
            //string mcdTempFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"SystemData\ProductArgs\历史周期雪水当量监测专题图.mcd");
            try
            {
                string shpMcd = Path.ChangeExtension(_shpFile, ".mcd");
                XElement xShpMcd = XElement.Load(shpMcd);
                XElement xShpLayer = xShpMcd.Element("Layers").Element("Layer");
                xShpLayer.Element("FeatureClass").Element("DataSource").SetAttributeValue("fileurl", _shpFile);

                XElement xGxd = XElement.Load(_gxdFile);
                XElement xLayers = xGxd.Element("GxdDataFrames").Element("GxdDataFrame").Element("GxdVectorHost").Element("Map").Element("Layers");
                xLayers.Add(xShpLayer);
                xGxd.Save(_gxdFile);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        protected override void ApplyAttributesOfDataFrame(IGxdDataFrame gxdDataFrame, IDataFrame dataFrame, ILayout layout)
        {
            string instanceIdentify = _argumentProvider.GetArg("OutFileIdentify") as string;

            Layout.GxdEnvelope evp = ToPrjEnvelope(_envelope, gxdDataFrame, dataFrame);
            if (evp != null)
            {
                FitSizeToTemplateWidth(layout, (float)(evp.MaxX - evp.MinX), (float)(evp.MaxY - evp.MinY));
                gxdDataFrame.Envelope = evp;
                _envelope = null;
            }
        }
        private GxdEnvelope ToPrjEnvelope(GxdEnvelope env, IGxdDataFrame gxdDataFrame, IDataFrame dataFrame)
        {
            if (env == null)
                return null;
            GeoDo.Project.IProjectionTransform tran = GetProjectionTransform(gxdDataFrame.SpatialRef);
            if (tran == null)
                return null;
            double[] xs = new double[] { env.MinX, env.MaxX };
            double[] ys = new double[] { env.MaxY, env.MinY };
            try
            {
                tran.Transform(xs, ys);
                return new Layout.GxdEnvelope(xs[0], xs[1], ys[1], ys[0]);
            }
            catch
            {
                return null;
            }
        }
        private void ChangeTemplateSize(ILayout layout, int width, int height)
        {
            IElement[] dfs = layout.QueryElements((e) => { return e is IDataFrame; });
            if (dfs == null || dfs.Length == 0)
                return;
            IDataFrame df = (dfs[0] as IDataFrame);
            if (df.Size.Width == width && df.Size.Height == height)
                return;
            float yOffset = height - df.Size.Height;
            float xOffset = width - df.Size.Width;
            df.IsLocked = false;
            df.ApplySize(xOffset, yOffset);
            df.IsLocked = true;
            layout.Size = new System.Drawing.SizeF(width, height);
            List<IElement> eles = layout.Elements;
            for (int i = 0; i < eles.Count; i++)
            {
                if (eles[i].Name == "标题" ||
                    eles[i].Name.Contains("Time") ||
                    eles[i].Name.Contains("Date"))
                    continue;
                if (eles[i] is IBorder ||
                    eles[i] is IDataFrame)
                    continue;
                if (eles[i] is ISizableElement)
                {
                    (eles[i] as ISizableElement).ApplyLocation(xOffset, yOffset);
                }
            }
        }
        private Project.IProjectionTransform GetProjectionTransform(string spatialRef)
        {
            if (spatialRef == null || !spatialRef.Contains("PROJCS"))
                return Project.ProjectionTransformFactory.GetDefault();
            else
                return Project.ProjectionTransformFactory.GetProjectionTransform(GeoDo.Project.SpatialReference.GetDefault(), GeoDo.Project.SpatialReferenceFactory.GetSpatialReferenceByWKT(spatialRef, Project.enumWKTSource.EsriPrjFile));
        }
        #endregion
        #region 体积+和 相关
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
            if (inRaster != null)
                inRaster.Dispose();
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
        #endregion
        private string JuPinComputer(string CurrentFile, string HistoryFile, string savePath)
        {
            float resolution = 0.1f;
            List<RasterMaper> rms = new List<RasterMaper>();
            IRasterDataProvider outRaster = null;
            RasterProcessModel<float, float> rfr = null;
            try
            {
                IRasterDataProvider cRaster = GeoDataDriver.Open(CurrentFile) as IRasterDataProvider;
                RasterMaper rm1 = new RasterMaper(cRaster, new int[] { 1 });
                rms.Add(rm1);
                IRasterDataProvider hRaster = GeoDataDriver.Open(HistoryFile) as IRasterDataProvider;
                RasterMaper rm2 = new RasterMaper(hRaster, new int[] { 1 });
                rms.Add(rm2);
                string reChars = "";
                string addChars = "";
                if (Path.GetFileName(CurrentFile).Contains("MSWE"))
                {
                    reChars = "MSWE";
                    addChars = "JPEA_MSWE";
                }
                if (Path.GetFileName(CurrentFile).Contains("MWSD"))
                {
                    reChars = "MWSD";
                    addChars = "JPDA_MWSD";
                }
                string outfilename = savePath + "\\" + Path.GetFileName(CurrentFile).Replace(reChars, addChars);
                outRaster = CreateOutRaster(outfilename, enumDataType.Float, rms.ToArray(), resolution);
                //栅格数据映射  
                RasterMaper[] fileIns = rms.ToArray();
                RasterMaper[] fileOuts = new RasterMaper[] { new RasterMaper(outRaster, new int[] { 1 }) };
                //创建处理模型
                rfr = new RasterProcessModel<float, float>();
                rfr.SetRaster(fileIns, fileOuts);
                rfr.RegisterCalcModel(new RasterCalcHandler<float, float>((rvInVistor, rvOutVistor, aoi) =>
                {
                    int dataLength = rvOutVistor[0].SizeY * rvOutVistor[0].SizeX;
                    for (int index = 0; index < dataLength; index++)
                    {
                        if (rvInVistor[0].RasterBandsData[0][index] == -999.0f)
                            rvOutVistor[0].RasterBandsData[0][index] = -999.0f; //考虑无效值
                        else
                            rvOutVistor[0].RasterBandsData[0][index] = rvInVistor[0].RasterBandsData[0][index] - rvInVistor[1].RasterBandsData[0][index];
                    }
                }));
                rfr.Excute();
                return outfilename;
            }
            finally
            {
                foreach (RasterMaper rm in rms)
                {
                    rm.Raster.Dispose();
                }
                if (outRaster != null)
                    outRaster.Dispose();
            }
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
        //连续时间
        private string ParaseFileTime(string file)
        {
            Regex DataReg = new Regex(@"(?<year>\d{4})", RegexOptions.Compiled);
            string year = "";
            string filename = Path.GetFileNameWithoutExtension(file);
            Match m = DataReg.Match(filename);
            if (m.Success)
                year = m.Value;
            string filetime = filename.Substring(filename.IndexOf(year), filename.Length - 4 - filename.IndexOf(year));
            string[] timeChars = filetime.Split('_');
            string periodExpress = "";
            if (filename.Contains("Month"))
            { 
                 //周期月 MWS_MSWE_China_Month_SWE_D_2013_12_avg.dat
                periodExpress = timeChars[0] + "年" + timeChars[1] + "月";
            }
            if (filename.Contains("Xun"))
            {
                if (timeChars.Length == 3)  //周期旬 MWS_MSWE_China_Hou _SWE_D_2013_12_1_avg.dat
                {
                    switch (timeChars[2])
                    {
                        case "1":
                            periodExpress = timeChars[0] + "年" + timeChars[1] + "月" + "上旬";
                            break;
                        case "2":
                            periodExpress = timeChars[0] + "年" + timeChars[1] + "月" + "中旬";
                            break;
                        case "3":
                            periodExpress = timeChars[0] + "年" + timeChars[1] + "月" + "下旬";
                            break;
                    }
                }
            }
            if (filename.Contains("Hou"))
            {
                if(timeChars.Length == 3)  //周期侯 MWS_MSWE_China_Hou _SWE_D_2013_12_1_avg.dat
                    periodExpress = timeChars[0] + "年" + timeChars[1] + "月第" + timeChars[2] + "侯";
            }
            if (filename.Contains("Winter"))
            {
                periodExpress = year + "年";
            }
            return periodExpress;
        }
        private string GetGxdTitle(string file, string layout, string prdType, string regionName,string statTypes)
        {
            Regex DataReg = new Regex(@"(?<year>\d{4})", RegexOptions.Compiled);
            string year = "";
            string filename = Path.GetFileNameWithoutExtension(file);
            Match m = DataReg.Match(filename);
            if (m.Success)
                year = m.Value;
            string filetime = filename.Substring(filename.IndexOf(year), filename.Length - 4 - filename.IndexOf(year));
            string[] timeChars = filetime.Split('_');
            string periodExpress = "";
            if (filename.Contains("Month"))
            {
                if (timeChars.Length == 3)  //同期月 MWS_MSWE_China_Month_SWE_D_1987_2013_12_avg.dat 
                    periodExpress = timeChars[0] + "-" + timeChars[1] + "年" + timeChars[2] + "月" + regionName + prdType + statTypes + layout;
                if (timeChars.Length == 2)  //周期月 MWS_MSWE_China_Month_SWE_D_2013_12_avg.dat
                {
                    if(layout == "距平分布图")
                        periodExpress = timeChars[0] + "年" + timeChars[1] + "月" + regionName + prdType + layout + "("+ startEndyears[0] + "-" + startEndyears[1] + ")";
                    else
                        periodExpress = timeChars[0] + "年" + timeChars[1] + "月" + regionName + prdType + statTypes + layout;
                }
            }
            if (filename.Contains("Xun"))
            {
                if (timeChars.Length == 4)  //同期旬 MWS_MSWE_China_Xun _SWE_D_1987_2013_12_1_avg.dat
                {
                    switch (timeChars[3])
                    {
                        case "1":
                            periodExpress = timeChars[0] + "-" + timeChars[1] + "年" + timeChars[2] + "月" + "上旬" + regionName + prdType + statTypes + layout;
                            break;
                        case "2":
                            periodExpress = timeChars[0] + "-" + timeChars[1] + "年" + timeChars[2] + "月" + "中旬" + regionName + prdType + statTypes + layout;
                            break;
                        case "3":
                            periodExpress = timeChars[0] + "-" + timeChars[1] + "年" + timeChars[2] + "月" + "下旬" + regionName + prdType + statTypes + layout;
                            break;
                    }
                }
                if (timeChars.Length == 3)  //周期旬 MWS_MSWE_China_Hou _SWE_D_2013_12_1_avg.dat
                {
                    switch (timeChars[2])
                    {
                        case "1":
                            if (layout == "距平分布图")
                                periodExpress = timeChars[0] + "年" + timeChars[1] + "月" + "上旬" + regionName + prdType + layout + "(" + startEndyears[0] + "-" + startEndyears[1] + ")";
                            else
                                periodExpress = timeChars[0] + "年" + timeChars[1] + "月" + "上旬" + regionName + prdType + statTypes + layout;
                            break;
                        case "2":
                            if (layout == "距平分布图")
                                periodExpress = timeChars[0] + "年" + timeChars[1] + "月" + "中旬" + regionName + prdType + layout + "(" + startEndyears[0] + "-" + startEndyears[1] + ")";
                            else
                                periodExpress = timeChars[0] + "年" + timeChars[1] + "月" + "中旬" + regionName + prdType + statTypes + layout;
                            break;
                        case "3":
                            if (layout == "距平分布图")
                                periodExpress = timeChars[0] + "年" + timeChars[1] + "月" + "下旬" + regionName + prdType + layout + "(" + startEndyears[0] + "-" + startEndyears[1] + ")";
                            else
                                periodExpress = timeChars[0] + "年" + timeChars[1] + "月" + "下旬" + regionName + prdType + statTypes + layout;
                            break;
                    }
                }
            }
            if (filename.Contains("Hou"))
            {
                if (timeChars.Length == 4)  //同期侯 MWS_MSWE_China_Hou_SWE_D_1987_2013_12_1_avg.dat 
                    periodExpress = timeChars[0] + "-" + timeChars[1] + "年" + timeChars[2] + "月第" + timeChars[3] + "侯" + regionName + prdType + statTypes + layout;
                if (timeChars.Length == 3)  //周期侯 MWS_MSWE_China_Hou _SWE_D_2013_12_1_avg.dat
                {
                    if (layout == "距平分布图")
                        periodExpress = timeChars[0] + "年" + timeChars[1] + "月第" + timeChars[2] + "侯" + regionName + prdType + layout + "(" + startEndyears[0] + "-" + startEndyears[1] + ")";
                    else
                        periodExpress = timeChars[0] + "年" + timeChars[1] + "月第" + timeChars[2] + "侯" + regionName + prdType + statTypes + layout;
                }
            }
            if (filename.Contains("Winter")) 
            {
                if (timeChars.Length == 4)  //当年 冬季 MWS_MSWE_China_Winter_SWE_D_1987_11_1988_2_avg.dat;
                    periodExpress = year + "年雪季" + regionName + prdType + statTypes + layout;
                if (timeChars.Length == 2) //同期 冬季 MWS_MSWE_China_Winter_SWE_D_1987_1988_avg.dat;
                {
                    if (layout == "距平分布图")
                        periodExpress = timeChars[0] + "-" + timeChars[1] + "年雪季" + regionName + prdType + layout + "(" + startEndyears[0] + "-" + startEndyears[1] + ")";
                    else
                        periodExpress = timeChars[0] + "-" + startEndyears[1] + "年雪季" + regionName + prdType + statTypes + layout;
                }
            }
            return periodExpress;
        }
        private string GetExcelTitle( string regionName, string exinfo)
        {
            // 2011-2013年11月地区雪水当量周期统计图 ：2011-2013年11月各旬雪水当量周期统计图：2011-2013年11月上旬雪水当量周期统计图
            // 2011-2013年11月地区雪水当量距平统计图 ：2011-2013年11月-12月雪水当量周期统计图
            string title = startEndyears[0] + "-" + startEndyears[1] + "年" + regionName + Product + "统计";
            switch (periodTypes.ToUpper())
            { 
                case ("MONTH"):
                {
                    if (queryTypes == "continue")
                    {
                        title = startEndyears[0] + "-" + startEndyears[1] + "年" + startEndTime[0].Split('_')[0] + "-"+startEndTime[1].Split('_')[0] + "月" + regionName + Product + exinfo + "统计";
                    }
                    else
                    {
                        if (startEndTime[0] == startEndTime[1])
                            title = startEndyears[0] + "-" + startEndyears[1] + "年" + startEndTime[0].Split('_')[0] + "月" + regionName + Product + exinfo + "统计";
                        else
                            title = startEndyears[0] + "-" + startEndyears[1] + "年" + startEndTime[0].Split('_')[0] + "-" + startEndTime[1].Split('_')[0] + "月" + regionName + Product + exinfo + "统计";
                    }
                    break;
                }
                case ("TEN"):
                {
                    if (queryTypes == "continue")
                    {
                        title = startEndyears[0] + "-" + startEndyears[1] + "年" + startEndTime[0].Split('_')[0] + "-" + startEndTime[1].Split('_')[0] + "月各旬" + regionName + Product + exinfo + "统计";
                    }
                    else
                    {
                        string startTen = startEndTime[0].Split('_')[1];
                        string endTen = startEndTime[1].Split('_')[1];
                        if (startTen == "1" && endTen == "3")
                            title = startEndyears[0] + "-" + startEndyears[1] + "年" + startEndTime[0].Split('_')[0] + "月各旬" + regionName + Product + exinfo + "统计";
                        else
                        {
                            if (startTen == endTen)
                            {
                                if (startTen == "1")
                                    title = startEndyears[0] + "-" + startEndyears[1] + "年" + startEndTime[0].Split('_')[0] + "月上旬" + regionName + Product + exinfo + "统计";
                                if (startTen == "2")
                                    title = startEndyears[0] + "-" + startEndyears[1] + "年" + startEndTime[0].Split('_')[0] + "月中旬" + regionName + Product + exinfo + "统计";
                                if (startTen == "3")
                                    title = startEndyears[0] + "-" + startEndyears[1] + "年" + startEndTime[0].Split('_')[0] + "月下旬" + regionName + Product + exinfo + "统计";
                            }
                            if (startTen == "1" && endTen == "2")
                                title = startEndyears[0] + "-" + startEndyears[1] + "年" + startEndTime[0].Split('_')[0] + "月上中旬" + regionName + Product + exinfo + "统计";
                            if (startTen == "2" && endTen == "3")
                                title = startEndyears[0] + "-" + startEndyears[1] + "年" + startEndTime[0].Split('_')[0] + "月中下旬" + regionName + Product + exinfo + "统计";
                            if (startTen == "1" && endTen == "3")
                                title = startEndyears[0] + "-" + startEndyears[1] + "年" + startEndTime[0].Split('_')[0] + "月上下旬" + regionName + Product + exinfo + "统计";
                        }
                    }
                    break;
                }
                case "PENTAD":
                {
                    if (queryTypes == "continue")
                    {
                        title = startEndyears[0] + "-" + startEndyears[1] + "年" + startEndTime[0].Split('_')[0] + "-" + startEndTime[1].Split('_')[0] + "月各侯" + regionName + Product + exinfo + "统计";
                    }
                    else
                    {
                        int start = Convert.ToInt32(startEndTime[0].Split('_')[2]);
                        int end = Convert.ToInt32(startEndTime[1].Split('_')[2]);
                        if (start == end)
                            title = startEndyears[0] + "-" + startEndyears[1] + "年" + startEndTime[0].Split('_')[0] + "月第" + startEndTime[0].Split('_')[2] + "侯" + regionName + Product + exinfo + "统计";
                        else
                            title = startEndyears[0] + "-" + startEndyears[1] + "年" + startEndTime[0].Split('_')[0] + "月第" + startEndTime[0].Split('_')[2] + "-" + startEndTime[1].Split('_')[0] + "侯" + regionName + Product + exinfo + "统计";
                    }
                    break;
                }
                case "WINTER":
                {
                    title = startEndyears[0] + "-" + startEndyears[1] + "年雪季" + regionName + Product + exinfo + "统计";
                    break;
                }
            }
            return title;
        }
        //按矢量裁切
        private Dictionary<string, List<string>> ClipFiles(Dictionary<string, List<string>> dicFiles)
        {
            #region 按选定区域裁切数据
            MulRegionsClip muticlip = new MulRegionsClip();
            string clipsave = outPut + "中间文件（可删除）" + "\\" + "矢量边裁切";
            if (!System.IO.Directory.Exists(clipsave))
                System.IO.Directory.CreateDirectory(clipsave);
            Dictionary<string, List<string>> clipFiles = new Dictionary<string, List<string>>(); //多时段：string为同期，List<string>为周期；多区域:string为每个区域名，List<string>为同期加周期
            string excelfile = ""; //后面调用excel表格时用的
            if (isMergeAois == true || aoiContainer.AOIs.Count() == 1)
            {
                //progressTracker(25, "数据处理...");
                #region 区域合裁切时，周期段可以为1，也可为多个
                foreach (string avgAllFiles in dicFiles.Keys)
                {
                    List<string> clipfiles = new List<string>();
                    string avgAllFilesClip = Path.Combine(clipsave, Path.GetFileName(avgAllFiles.Replace("China", MergeAoisName)));
                    if (!File.Exists(avgAllFilesClip))
                    {
                        string clipFile = muticlip.MutiRegionsClip(avgAllFiles, aoiContainer, clipsave);
                        string hdrfile = Path.GetDirectoryName(clipFile) + "\\" + Path.GetFileNameWithoutExtension(clipFile) + ".hdr";
                        //重命名
                        string newhdrfile = Path.Combine(Path.GetDirectoryName(clipFile), Path.GetFileNameWithoutExtension(avgAllFiles).Replace("China", MergeAoisName) + ".hdr");
                        FileInfo fi = new FileInfo(clipFile);
                        fi.MoveTo(avgAllFilesClip);
                        FileInfo fihdr = new FileInfo(hdrfile);
                        fihdr.MoveTo(newhdrfile);
                    }
                    List<string> newfiles = dicFiles[avgAllFiles];
                    foreach (string infile in newfiles)
                    {
                        string newclipfile = Path.Combine(clipsave, Path.GetFileName(infile).Replace("China", MergeAoisName));
                        if (!File.Exists(newclipfile))
                        {
                            string clipfile = muticlip.MutiRegionsClip(infile, aoiContainer, clipsave);
                            string hdrfile = Path.GetDirectoryName(clipfile) + "\\" + Path.GetFileNameWithoutExtension(clipfile) + ".hdr";
                            //重命名
                            string newhdrfile = Path.Combine(Path.GetDirectoryName(clipfile), Path.GetFileNameWithoutExtension(infile).Replace("China", MergeAoisName) + ".hdr");
                            FileInfo fi = new FileInfo(clipfile); fi.MoveTo(newclipfile);
                            FileInfo fihdr = new FileInfo(hdrfile); fihdr.MoveTo(newhdrfile);
                        }
                        clipfiles.Add(newclipfile);
                        excelfile = newclipfile;
                    }
                    clipFiles.Add(avgAllFilesClip, clipfiles);  //同期，周期 .count可能为1也可能大于1
                }
                #endregion
            }
            else
            {
                #region 多个区域独立裁切时，周期段为1
                int i = 0; //获取每个独产区域名称
                foreach (Feature fet in aoiContainer.AOIs)
                {
                    GeoDo.RSS.Core.VectorDrawing.AOIContainerLayer aoi = new GeoDo.RSS.Core.VectorDrawing.AOIContainerLayer();
                    aoi.AddAOI(fet);
                    string regionName = MergerAoiNames[i++];
                    List<string> clipfiles = new List<string>();
                    foreach (string avgAllFiles in dicFiles.Keys)
                    {
                        string avgAllFilesClip = Path.Combine(clipsave, Path.GetFileName(avgAllFiles.Replace("China", regionName)));
                        if (!File.Exists(avgAllFilesClip))
                        {
                            string clipFile = muticlip.MutiRegionsClip(avgAllFiles, aoi, clipsave);
                            string hdrfile = Path.GetDirectoryName(clipFile) + "\\" + Path.GetFileNameWithoutExtension(clipFile) + ".hdr";
                            //重命名
                            string newhdrfile = Path.Combine(Path.GetDirectoryName(clipFile), Path.GetFileNameWithoutExtension(avgAllFiles).Replace("China", regionName) + ".hdr");
                            FileInfo fi = new FileInfo(clipFile); fi.MoveTo(avgAllFilesClip);
                            FileInfo fihdr = new FileInfo(hdrfile); fihdr.MoveTo(newhdrfile);
                        }
                        clipfiles.Add(avgAllFilesClip);
                        List<string> newfiles = dicFiles[avgAllFiles];
                        foreach (string infile in newfiles)
                        {
                            string newclipfile = Path.Combine(clipsave, Path.GetFileName(infile).Replace("China", regionName));
                            if (!File.Exists(newclipfile))
                            {
                                string clipfile = muticlip.MutiRegionsClip(infile, aoi, clipsave);
                                string hdrfile = Path.GetDirectoryName(clipfile) + "\\" + Path.GetFileNameWithoutExtension(clipfile) + ".hdr";
                                //重命名
                                string newhdrfile = Path.Combine(Path.GetDirectoryName(clipfile), Path.GetFileNameWithoutExtension(infile).Replace("China", regionName) + ".hdr");
                                FileInfo fi = new FileInfo(clipfile); fi.MoveTo(newclipfile);
                                FileInfo fihdr = new FileInfo(hdrfile); fihdr.MoveTo(newhdrfile);
                            }
                            clipfiles.Add(newclipfile);
                            excelfile = newclipfile;
                        }
                    }
                    clipFiles.Add(regionName, clipfiles); //区域名，同期+周期的区域裁切文件  count可能1，也可能大于1
                }
                #endregion
            }
            #endregion
            return clipFiles;
        }
        //按矩形提取数据
        private Dictionary<string, List<string>> ClipRectFiles(Dictionary<string, List<string>> dicFiles)
        {
            #region 按选定区域裁切数据
            MulRegionsClip muticlip = new MulRegionsClip();
            ExtractRectRegion rectClip = new ExtractRectRegion();
            string clipsave = outPut + "\\" + "中间文件（可删除）" +"\\" + "数据提取";
            if (!System.IO.Directory.Exists(clipsave))
                System.IO.Directory.CreateDirectory(clipsave);
            Dictionary<string, List<string>> clipFiles = new Dictionary<string, List<string>>(); //多时段：string为同期，List<string>为周期；多区域:string为每个区域名，List<string>为同期加周期
            if (isMergeAois == true || aoiContainer.AOIs.Count() == 1)
            {
                #region 区域合裁切时，周期段可以为1，也可为多个
                foreach (string avgAllFiles in dicFiles.Keys)
                {
                    List<string> clipfiles = new List<string>();
                    string avgAllFilesClip = rectClip.ExractRect(avgAllFiles, aoiContainer, clipsave, MergeAoisName);
                    List<string> newfiles = dicFiles[avgAllFiles];
                    foreach (string infile in newfiles)
                    {
                        string newclipfile = rectClip.ExractRect(infile, aoiContainer, clipsave, MergeAoisName);
                        clipfiles.Add(newclipfile);
                    }
                    clipFiles.Add(avgAllFilesClip, clipfiles);  //同期，周期 .count可能为1也可能大于1
                }
                repetClipAOI.Add(choseAois, aoiContainer);
                #endregion
            }
            else
            {
                #region 多个区域独立裁切时，周期段为1
                int i = 0; //获取每个独产区域名称
                foreach (Feature fet in aoiContainer.AOIs)
                {
                    GeoDo.RSS.Core.VectorDrawing.AOIContainerLayer aoi = new GeoDo.RSS.Core.VectorDrawing.AOIContainerLayer();
                    aoi.AddAOI(fet);
                    string regionName = MergerAoiNames[i++];
                    List<string> clipfiles = new List<string>();
                    foreach (string avgAllFiles in dicFiles.Keys)
                    {
                       string avgAllFilesClip = rectClip.ExractRect(avgAllFiles, aoi, clipsave, regionName);
                       avgAllFilesClip = rectClip.ExractRect(avgAllFiles, aoi, clipsave, regionName);
                       clipfiles.Add(avgAllFilesClip);
                       List<string> newfiles = dicFiles[avgAllFiles];
                       foreach (string infile in newfiles)
                       {
                            string newclipfile = rectClip.ExractRect(infile, aoi, clipsave, regionName);
                            clipfiles.Add(newclipfile);
                       }
                    }
                    clipFiles.Add(regionName, clipfiles); //区域名，同期+周期的区域裁切文件  count可能1，也可能大于1
                    repetClipAOI.Add(regionName,aoi);
                }
                #endregion
            }
            #endregion
            return clipFiles;
        }
        private Dictionary<string, List<string>> ClipRectFiles1(Dictionary<string, List<string>> dicFiles)
        {
            #region 按选定区域裁切数据
            MulRegionsClip muticlip = new MulRegionsClip();
            ExtractRectRegion rectClip = new ExtractRectRegion();
            string clipsave = outPut + "\\" + "中间文件（可删除）" + "\\" + "数据提取";
            if (!System.IO.Directory.Exists(clipsave))
                System.IO.Directory.CreateDirectory(clipsave);
            Dictionary<string, List<string>> clipFiles = new Dictionary<string, List<string>>(); //多时段：string为同期，List<string>为周期；多区域:string为每个区域名，List<string>为同期加周期
            if (isMergeAois == true || aoiContainer.AOIs.Count() == 1)
            {
                #region 区域合裁切时，周期段可以为1，也可为多个
                foreach (string avgAllFiles in dicFiles.Keys)
                {
                    List<string> clipfiles = new List<string>();
                    List<string> newfiles = dicFiles[avgAllFiles];
                    foreach (string infile in newfiles)
                    {
                        string newclipfile = rectClip.ExractRect(infile, aoiContainer, clipsave, MergeAoisName);
                        clipfiles.Add(newclipfile);
                    }
                    clipFiles.Add(choseAois, clipfiles);  //同期，周期 .count可能为1也可能大于1
                }
                repetClipAOI.Add(choseAois, aoiContainer);
                #endregion
            }
            else
            {
                #region 多个区域独立裁切时，周期段为1
                int i = 0; //获取每个独产区域名称
                foreach (Feature fet in aoiContainer.AOIs)
                {
                    GeoDo.RSS.Core.VectorDrawing.AOIContainerLayer aoi = new GeoDo.RSS.Core.VectorDrawing.AOIContainerLayer();
                    aoi.AddAOI(fet);
                    string regionName = MergerAoiNames[i++];
                    List<string> clipfiles = new List<string>();
                    foreach (string avgAllFiles in dicFiles.Keys)
                    {
                        string avgAllFilesClip = rectClip.ExractRect(avgAllFiles, aoi, clipsave, regionName);
                        avgAllFilesClip = rectClip.ExractRect(avgAllFiles, aoi, clipsave, regionName);
                        clipfiles.Add(avgAllFilesClip);
                        List<string> newfiles = dicFiles[avgAllFiles];
                        foreach (string infile in newfiles)
                        {
                            string newclipfile = rectClip.ExractRect(infile, aoi, clipsave, regionName);
                            clipfiles.Add(newclipfile);
                        }
                    }
                    clipFiles.Add(regionName, clipfiles); //区域名，同期+周期的区域裁切文件  count可能1，也可能大于1
                    repetClipAOI.Add(regionName, aoi);
                }
            }
            #endregion
            #endregion
            return clipFiles;
        }
    }
}
