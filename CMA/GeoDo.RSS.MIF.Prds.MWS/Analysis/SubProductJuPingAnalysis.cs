#region Version Info
/*========================================================================
* 功能概述：
* 
* 创建者：Administrator     时间：2014-3-18 11:09:34
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
using System.Text.RegularExpressions;
using GeoDo.RSS.MIF.Prds.Comm;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.Core.DF;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using GeoDo.RSS.Core.UI;
using GeoDo.RSS.UI.AddIn.Theme;
using GeoDo.RSS.Core.DrawEngine;
using CodeCell.AgileMap.Core;
using GeoDo.RSS.BlockOper;
using GeoDo.RSS.DF.MEM;
using GeoDo.RSS.MicroWaveFYDataRetrieval;
using GeoDo.MicroWaveSnow.SNWParaStat;
using GeoDo.RSS.Core.RasterDrawing;
using System.Drawing.Imaging;
using GeoDo.RSS.UI.AddIn.Layout;
using GeoDo.RSS.Layout;
using GeoDo.RSS.UI.AddIn.DataPro;
using System.Xml.Linq;
namespace GeoDo.RSS.MIF.Prds.MWS
{
    /// <summary>
    /// 类名：SubProductJuPingAnalysis
    /// 属性描述：历史数据距平分析 主要用于出专题图吧
    /// 创建者：lxj   创建日期：2014-3-18 11:09:34
    /// 修改者：             修改日期：
    /// 修改描述：
    /// 备注：
    /// </summary>
    public class SubProductJuPingAnalysis:CmaMonitoringSubProduct
    {
        private IContextMessage _contextMessage;
        private IArgumentProvider _curArguments = null;
        GeoDo.RSS.Core.VectorDrawing.AOIContainerLayer aoiContainer = null;
        private string imgname = null; //专题图上的名称
        private List<string> imgfiles = new List<string>();
        private string gxdsave = null;
        private List<string> toBarfiles = new List<string>();//用于出柱状图的files  是全国数据或裁切的某地区数据
        private List<XElement> layers = new List<XElement>(); //加载的shpfile层
        private string _gxdFile = null;
        private string _shpFile = null;
        private string regionNames = null;
        public SubProductJuPingAnalysis(SubProductDef subProductDef)
            : base(subProductDef)
        { 
        }
        public override IExtractResult Make(Action<int, string> progressTracker)
        {
            _curArguments = _argumentProvider;
            if (_curArguments == null)
                return null;
            if (_curArguments.GetArg("AlgorithmName") == null)
                return null;

            if (_curArguments.GetArg("AlgorithmName").ToString() == "JuPingAnalysisAlgorithm")
            {
                return JuPingAnalysisAlgorithm(progressTracker);
            }
            return null;
        }
        private IExtractResult JuPingAnalysisAlgorithm(Action<int, string> progressTracker)
        {
            string[] inputCurrentFiles = null ;//= ExportManager.GetInstance().List.ToArray();//GetStringArray("RasterCurrentFile"); 由查询那里获得
            string[] inputHistoryFiles = null;
            List<string> list1 = ExportManager.GetInstance().List;//得到的是从数据库里查询出来的周期数据
            StatisticResultManager manager = StatisticResultManager.GetInstance();
            List<StatisticResult> list = manager.List;
            List<string> list2 = manager.GetFilePathFromList();    //得到的是基于数据查询的数据又做的统计数据 
            regionNames = _argumentProvider.GetArg("regionNames") as string;
            //处理后的数据，用它来出专题图
            List<string> afterProcessfiles = new List<string>();
            #region 获得目标区域
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
            List<string> jpfiles = new List<string>();//存放相减的距平分析文件
            string date = "";
            string jupingfile = "";
            IExtractResultArray array = new ExtractResultArray("距平分析");
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
                }
                inputCurrentFiles = files.ToArray();
                string outHistoryFile = jpsavepath +"\\"+ Path.GetFileNameWithoutExtension(inputCurrentFiles[0]).Substring(0, 14) +"_" +paras[2] + ".dat";
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
                foreach (string inputCurrentFile in inputCurrentFiles)
                {
                    Regex DataReg2 = new Regex(@"(?<year>\d{4})_(?<month>\d{2})_(?<year>\d{4})_(?<month>\d{1})", RegexOptions.Compiled);
                    Match m = DataReg2.Match(inputCurrentFile); //提取每年冬季的时间
                    string year = "";
                    if (m.Success)
                        year = m.Value;
                    jupingfile = JuPinComputer(inputCurrentFile, inputHistoryFiles[0], jpsavepath);
                     string hdrfile = Path.GetDirectoryName(jupingfile) + "\\" + Path.GetFileNameWithoutExtension(jupingfile) + ".hdr";
                    FileInfo fi = new FileInfo(jupingfile);
                    string newjpfile = Path.Combine(jpsavepath, Path.GetFileNameWithoutExtension(jupingfile) + "_"+ year.Substring(0, 4) + ".dat");
                    string newhdrfile = Path.Combine(jpsavepath, Path.GetFileNameWithoutExtension(jupingfile) + "_"+ year.Substring(0, 4) + ".hdr");
                    fi.MoveTo(newjpfile);
                    FileInfo fihdr = new FileInfo(hdrfile);
                    fihdr.MoveTo(newhdrfile);
                    jpfiles.Add(newjpfile);
                }
            }
            else
            {                                        //例 2013年11月上旬-2013年11月下旬分别与1987-2013年11月上中下旬 同期比较   一对一计算距平
                inputCurrentFiles = list1.ToArray();
                inputHistoryFiles = list2.ToArray();
                if (inputCurrentFiles == null || inputCurrentFiles.Length < 0 || inputHistoryFiles == null || inputHistoryFiles.Length < 0)
                {
                    PrintInfo("缺少分析文件");
                    return null;
                }
                if ((inputCurrentFiles.Length)*2 != inputHistoryFiles.Length)
                {
                    MessageBox.Show("当年数据与同期数据不对应");
                    return null; 
                }
                foreach (string inputCurrentFile in inputCurrentFiles)
                {
                    //先假设选择的是旬数据 MWS_MWSD_China_Xun_0SD_A_2011_1_avg.dat
                    //       则同期旬数据 MWS_MWSD_China_Xun_0SD_A_1989_2011_1_avg.dat
                    string cfile = Path.GetFileNameWithoutExtension(inputCurrentFile);
                    Match m = DataReg.Match(cfile);
                    if (m.Success)
                    {
                        date = m.Value;    //根据年4个数字来拆分字符串
                    }
                    string bhalf = cfile.Substring(0, cfile.IndexOf(date));
                    string ahalf = cfile.Substring(cfile.IndexOf(date)+4, cfile.Length - 4 - cfile.IndexOf(date));
                    foreach (string inputHistoryFile in inputHistoryFiles)
                    {
                        string aa = Path.GetFileNameWithoutExtension(inputHistoryFile);
                        if (aa.Contains(bhalf) && aa.Contains(ahalf)&&aa.Contains(date) && !aa.Contains(Nostr))
                        {
                            jupingfile = JuPinComputer(inputCurrentFile, inputHistoryFile, jpsavepath);
                            jpfiles.Add(jupingfile);
                            break;
                        }
                    }
                }
            }
            #region  距平的文件在jpfiles 中，下面是裁切和平滑处理
            MulRegionsClip muticlip = new MulRegionsClip();  //裁切
            MWSSmoothHelp smooth = new MWSSmoothHelp();  //平滑
            //创建专题图路径
            gxdsave = jpsavepath + "\\" + regionsname + "\\" + "专题图";
            if (!System.IO.Directory.Exists(gxdsave))//如果不存在这个路径
                System.IO.Directory.CreateDirectory(gxdsave);
            //创建平滑路径
            string smoothsave = jpsavepath + "\\" + regionsname + "\\" + "平滑";
            if (!System.IO.Directory.Exists(smoothsave))
                System.IO.Directory.CreateDirectory(smoothsave);
            //创建平滑\\中值
            string filtersave = smoothsave + "\\" + "中值";
            if (!System.IO.Directory.Exists(filtersave))
                System.IO.Directory.CreateDirectory(filtersave);
            if (aoiContainer == null)
            {
                //只做中值处理
                foreach (string infile in jpfiles)
                {
                    // 先判断这个文件是否存在，存在了就不做
                    string filterfile = filtersave + "\\" + Path.GetFileName(infile);
                    if (!File.Exists(filterfile))
                    {
                        filterfile = smooth.ComputerMid(infile, 5, filtersave);
                    }
                    afterProcessfiles.Add(filterfile);
                    toBarfiles.Add(infile);
                }
            }
            else
            { 
                // 裁切 // 中值 // 插值
                string clipsave = jpsavepath + "\\" + regionsname + "\\" + "裁切";
                if (!System.IO.Directory.Exists(clipsave))//如果不存在这个路径
                    System.IO.Directory.CreateDirectory(clipsave);
                string bilisave = smoothsave + "\\" + "插值";
                if (!System.IO.Directory.Exists(bilisave))
                    System.IO.Directory.CreateDirectory(bilisave);
                foreach (string infile in jpfiles)
                {
                    string newclipfile = clipsave + "\\" + Path.GetFileName(infile).Replace("China", regionsname);
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
                    toBarfiles.Add(newclipfile); //加上裁切后的文件
                    //中值、插值
                    string filterfile = smooth.ComputerMid(newclipfile, 5, filtersave);
                    string bilifile = smooth.Bilinear(filterfile, 10, bilisave);
                    afterProcessfiles.Add(bilifile);
                }
            }
            #endregion
            //出专题图
            #region
            IExtractResultArray results = new ExtractResultArray("");
            foreach (string arguments in afterProcessfiles.ToArray())
            {
                string product = "";
                if (arguments.Contains("MWSD"))
                { 
                    product = "雪深";
                    _argumentProvider.SetArg("OutFileIdentify", "JPDI");
                 }
                if (arguments.Contains("MSWE"))
                { 
                    product = "雪水当量";
                    _argumentProvider.SetArg("OutFileIdentify", "JPEI");
                }
                //解析文件名确定专题图名称
                string filename = Path.GetFileNameWithoutExtension(arguments);
                if (paras[1] == "usual")
                {
                    Regex DataReg1 = new Regex(@"(?<year>\d{4})_(?<year>\d{4})", RegexOptions.Compiled);
                    Match m = DataReg1.Match(filename);
                    string year = "";
                    if (m.Success)
                        year = m.Value;
                    //提取年至avg之间的字符
                    string filetime = filename.Substring(filename.IndexOf(year), filename.Length - 3 - filename.IndexOf(year));
                    string[] mxchars = filetime.Split(new char[] { '_' });
                    string mx = "";//月+旬
                    if (filename.Contains("Xun"))
                    {
                        if (mxchars[3] == "1")
                            mx = mxchars[2] + "月" + "上旬";
                        if (mxchars[3] == "2")
                            mx = mxchars[2] + "月" + "中旬";
                        if (mxchars[3] == "3")
                            mx = mxchars[2] + "月" + "下旬";
                    }
                    if (filename.Contains("Month"))
                    {
                        mx = mxchars[2] + "月";
                    }
                    imgname = year.Substring(5, 4) + "年" + mx + regionsname + product + "距平分布图(" + paras[2] + "年)";
                }
                if (paras[1] == "winter")
                {
                    imgname = Path.GetFileNameWithoutExtension(arguments).Substring(Path.GetFileNameWithoutExtension(arguments).Length-4,4) + "年" + "冬季" + regionsname + product + "距平分布图(" + paras[2] + "年)";
                }
                _argumentProvider.SetArg("SelectedPrimaryFiles", arguments);
                _argumentProvider.SetArg("fileOpenArgs", arguments);
                FileExtractResult result = ThemeGraphyResult(null) as FileExtractResult;
                //增加矢量
                string autopath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"SystemData\ProductArgs\MWS\shppath.txt");
                string shpFile = "";
                if (File.Exists(autopath))
                {
                    FileStream fauto = new FileStream(autopath, FileMode.Open, FileAccess.Read);
                    StreamReader rauto = new StreamReader(fauto, Encoding.GetEncoding("gb2312"));
                    shpFile = rauto.ReadLine();
                    rauto.Close();
                    fauto.Close();
                }
                else
                    shpFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"数据引用\基础矢量\行政区划\线\中国边界_线.shp");
                CreateMcd(shpFile);
                _shpFile = shpFile;
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
                }
               
                IFileExtractResult res = new FileExtractResult(_subProductDef.Identify, newresultfile, false);
                results.Add(res);
            }
            #endregion
            return results;
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
            vars.Add("{NAME}", imgname);
            template.ApplyVars(vars);

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
        private string JuPinComputer(string CurrentFile, string HistoryFile,string savePath)
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
               string outfilename = savePath + "\\" + Path.GetFileName(HistoryFile).Replace(reChars, addChars);
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
        private void PrintInfo(string info)
        {
            if (_contextMessage != null)
                _contextMessage.PrintMessage(info);
            else
                Console.WriteLine(info);
        }
    }
}
