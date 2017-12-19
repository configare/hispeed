#region Version Info
/*========================================================================
* 功能概述：
* 
* 创建者：Administrator     时间：2014-3-27 11:27:22
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
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.MIF.Prds.Comm;
using GeoDo.RSS.Layout;
using GeoDo.RSS.UI.AddIn.DataPro;
using System.Drawing;
using GeoDo.RSS.Core.UI;
using System.ComponentModel.Composition;
using GeoDo.RSS.Core.RasterDrawing;
using System.Drawing.Imaging;
using GeoDo.RSS.UI.AddIn.Layout;
using System.IO;
using System.Windows.Forms;
using GeoDo.RSS.UI.AddIn.Theme;
using GeoDo.RSS.Core.DrawEngine;
using CodeCell.AgileMap.Core;
using System.Text.RegularExpressions;
using GeoDo.RSS.MicroWaveFYDataRetrieval;
using System.Xml.Linq;

namespace GeoDo.RSS.MIF.Prds.MWS
{
    /// <summary>
    /// 类名：SubProductHisDataIMG
    /// 属性描述：多年周期数据出专题图
    /// 创建者：lxj  创建日期：2014-3-27 11:27:22
    /// 修改者：             修改日期：
    /// 修改描述：
    /// 备注：
    /// </summary>

    public class SubProductHisDataIMG :CmaMonitoringSubProduct
    {
        GeoDo.RSS.Core.VectorDrawing.AOIContainerLayer aoiContainer = null;
        private IContextMessage _contextMessage;
        private Action<int, string> _progressTracker = null;
        private string imgname = null; //专题图上的名称
        private List<string> imgfiles = new List<string>();
        private string gxdsave = null;
        private string period = null;
        private string Iswinter = null;
        private List<XElement> layers = new List<XElement>(); //加载的shpfile层
        private string _gxdFile = null;
        private string _shpFile = null;
        private string regioNames;
        private Regex DataReg2 = new Regex(@"(?<year>\d{4})_(?<month>\d{2})_(?<year>\d{4})_(?<month>\d{1})", RegexOptions.Compiled);
        public SubProductHisDataIMG(SubProductDef subProductDef)
            : base(subProductDef)
        { 
        }
        public override IExtractResult Make(Action<int, string> progressTracker)
        {
            return Make(progressTracker, null);
        }

        public override IExtractResult Make(Action<int, string> progressTracker, IContextMessage contextMessage)
        {
            _progressTracker = progressTracker;
            _contextMessage = contextMessage;
            if (_argumentProvider == null)
                return null;
            if (_argumentProvider.GetArg("AlgorithmName") == null)
                return null;
            if (_argumentProvider.GetArg("AlgorithmName").ToString() == "HistoryDataLayoutAlgorithm")
            {
                return HistoryDataLayoutAlgorithm(progressTracker);
            }
            return ThemeGraphyResult(null);
        }
        private IExtractResult HistoryDataLayoutAlgorithm(Action<int, string> progressTracker)
        {
            //1.获取到数据
            List<string> list = ExportManager.GetInstance().List;  //这个针对查询出来的数据出专题图
            string[] inputfiles = list.ToArray();
            //再加一上基于 同期统计数据的专题图，如果是冬季的，还要再计算一次
            StatisticResultManager manager = StatisticResultManager.GetInstance();
            List<string> list2 = manager.GetFilePathFromList();    //得到的是基于数据查询的数据又做的统计数据 
            if (list2.Count!= 0)
            {
                inputfiles = list2.ToArray();
                period = "yes";
                Match m = DataReg2.Match(inputfiles[0]);
                if (m.Success)
                    Iswinter = "yes";
            }
            //处理后的数据存储路径
            string savePath = _argumentProvider.GetArg("HistoryDataSave") as string;
            regioNames = _argumentProvider.GetArg("regionNames") as string;
            string orbitType = _argumentProvider.GetArg("OrbitType") as string;
            string Str = null;
            if (orbitType == "Ascend")
                Str = "_A_";
            if (orbitType == "Descend")
                Str = "_D_";
            //同期统计计算传出来的文件没有分升降轨，这里进行区分
            List<string> fnamelist = new List<string>();
            foreach (string file in inputfiles)
            {
                if (Path.GetFileName(file).Contains(Str))
                {
                    fnamelist.Add(file);
                }
            }
            inputfiles = fnamelist.ToArray();
            //处理后的数据，用它来出专题图
            List<string> afterProcessfiles = new List<string>();
            //(插一步，通过这个原始选择的文件名时间信息把专题图名称确定)
            //2.确定选择区域，没有选或者中国区域不要用裁，如果是其地区首先裁切，并且放到指定文件夹下
            aoiContainer = new GeoDo.RSS.Core.VectorDrawing.AOIContainerLayer();
            MulRegionsClip muticlip = new MulRegionsClip();  //裁切
            MWSSmoothHelp smooth = new MWSSmoothHelp();  //平滑
            string regionsname = "";
            #region 获得目标区域
            aoiContainer = new GeoDo.RSS.Core.VectorDrawing.AOIContainerLayer();
            string fieldName;
            string shapeFilename;
            int fieldIndex = -1;
            List<string> fieldValues = new List<string>();
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
                            if (!string.IsNullOrEmpty(regioNames))
                                regionsname = regioNames.Trim();
                        }
                    }
                }
                else //没有点击确定，返回空
                {
                    return null;
                }
            }
            #endregion
            if (aoiContainer == null)
            {
                //不用裁切,只做中值滤波的平滑处理。
                regionsname = "全国";
                foreach (string infile in inputfiles)
                {
                    //创建专题图路径
                    gxdsave = savePath + "\\" + regionsname + "\\" + "专题图";
                    if (!System.IO.Directory.Exists(gxdsave))//如果不存在这个路径
                        System.IO.Directory.CreateDirectory(gxdsave);
                    //创建平滑路径
                    string smoothsave = savePath + "\\" + regionsname + "\\" + "平滑";
                    if (!System.IO.Directory.Exists(smoothsave))
                        System.IO.Directory.CreateDirectory(smoothsave);
                    //创建平滑\\中值
                    string filtersave = smoothsave + "\\" + "中值";
                    if (!System.IO.Directory.Exists(filtersave))
                        System.IO.Directory.CreateDirectory(filtersave);
                    string filterfile = smooth.ComputerMid(infile, 5, filtersave);
                    string hdrfile = Path.GetDirectoryName(filterfile) + "\\" + Path.GetFileNameWithoutExtension(filterfile) + ".hdr";
                    afterProcessfiles.Add(filterfile); 
                }
            }
            else
            {
                //创建裁切路径
                string clipsave = savePath + "\\" + regionsname + "\\" + "裁切";
                if (!System.IO.Directory.Exists(clipsave))//如果不存在这个路径
                    System.IO.Directory.CreateDirectory(clipsave);
                //创建专题图路径
                gxdsave = savePath + "\\" + regionsname + "\\" + "专题图";
                if (!System.IO.Directory.Exists(gxdsave))//如果不存在这个路径
                    System.IO.Directory.CreateDirectory(gxdsave);
                //创建平滑路径
                string smoothsave = savePath + "\\" + regionsname + "\\" + "平滑";
                if (!System.IO.Directory.Exists(smoothsave))
                    System.IO.Directory.CreateDirectory(smoothsave);
                //创建平滑\\中值
                string filtersave = smoothsave + "\\" + "中值";
                if (!System.IO.Directory.Exists(filtersave))
                    System.IO.Directory.CreateDirectory(filtersave);
                //创建平滑\\插值
                string bilisave = smoothsave + "\\" + "插值";
                if (!System.IO.Directory.Exists(bilisave))
                    System.IO.Directory.CreateDirectory(bilisave);
                foreach (string infile in inputfiles)
                {
                    //加一个条件判断要处理的数据是否已经存在，如果存就不用再做
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
                    //裁切后要对数据中值、插值
                    string filterfile = smooth.ComputerMid(newclipfile, 5, filtersave);
                    string bilifile = smooth.Bilinear(filterfile, 10, bilisave);
                    afterProcessfiles.Add(bilifile);
                }
            }
            //把处理后的afterProcessfiles.ToArray()数组文件名标识变为“HFSD”或“HFWE”。
            foreach (string file in afterProcessfiles.ToArray())
            {
                string hdrfile = Path.GetDirectoryName(file) + "\\" + Path.GetFileNameWithoutExtension(file) + ".hdr";
                string newfile = "";
                string newhdr = "";
                if (file.Contains("MWSD"))
                {
                   newfile = file.Replace("MWSD", "HFSD");
                   newhdr = hdrfile.Replace("MWSD", "HFSD");
                }
                if (file.Contains("MSWE"))
                {
                   newfile = file.Replace("MSWE", "HFWE");
                   newhdr = hdrfile.Replace("MSWE", "HFWE");
                }
                FileInfo fi = new FileInfo(file);
                if(!File.Exists(newfile))
                     fi.MoveTo(newfile);
                FileInfo fihdr = new FileInfo(hdrfile);
                if(!File.Exists(newhdr))
                     fihdr.MoveTo(newhdr);
                imgfiles.Add(newfile);

            }
            IExtractResultArray results = new ExtractResultArray("");
            foreach (string arguments in imgfiles.ToArray())
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
                //解析文件名确定专题图名称
                string filename = Path.GetFileNameWithoutExtension(arguments);
                if (period == "yes")        //同期统计的
                {
                    if (Iswinter == "yes")
                    {
                        Match m = DataReg2.Match(filename);
                            string year = "";
                        if(m.Success)
                            year = m.Value;
                        imgname = year.Substring(0,4) + "年" + "冬季" + regionsname + product + "分布图";
                    }
                    else
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
                        imgname = year + "年" + mx + regionsname + product + "分布图";
                    }
                }
                else
                {
                    Regex DataReg = new Regex(@"(?<year>\d{4})", RegexOptions.Compiled);
                    Match m = DataReg.Match(filename);
                    string year = "";
                    if (m.Success)
                        year = m.Value;
                    //提取年至avg之间的字符
                    string filetime = filename.Substring(filename.IndexOf(year), filename.Length - 3 - filename.IndexOf(year));
                    string[] mxchars = filetime.Split(new char[] { '_' });
                    string mx = "";//月+旬
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
                    if (filetime.Contains("Season"))
                        mx = mxchars[1] + "季度";
                    imgname = year + "年" + mx + regionsname + product + "分布图";
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
                    StreamReader rauto = new StreamReader(fauto,Encoding.GetEncoding("gb2312"));
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
                FileInfo fi = new FileInfo(result.FileName);
                string newresultfile = Path.Combine(gxdsave,Path.GetFileNameWithoutExtension(arguments) + ".gxd");
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
            return results;
        }
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
            if (spatialRef == null)
                return Project.ProjectionTransformFactory.GetDefault();
            else
                return Project.ProjectionTransformFactory.GetProjectionTransform(GeoDo.Project.SpatialReference.GetDefault(), GeoDo.Project.SpatialReferenceFactory.GetSpatialReferenceByWKT(spatialRef, Project.enumWKTSource.EsriPrjFile));
        }
    }
}
