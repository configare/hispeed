using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Core;
using System.Drawing;
using System.IO;
using GeoDo.RSS.MIF.Prds.Comm;
using CodeCell.AgileMap.Core;
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.Layout.Elements;
using GeoDo.RSS.Layout;
using GeoDo.RSS.UI.AddIn.Theme;
using GeoDo.RSS.Core.UI;
using System.Xml.Linq;

namespace GeoDo.RSS.MIF.Prds.ICE
{
    /// <summary>
    /// 手绘冰缘线专题图:
    /// 根据冰缘线shapefile和冰缘线控制点shapefile生成冰缘线专题图
    /// </summary>
    public class SubProductEDGIICE : CmaMonitoringSubProduct
    {
        private IContextMessage _contextMessage;
        private Action<int, string> _progressTracker = null;
        private string[] _shpFiles = null;
        private string _gxdFile = null;     //专题图文件
        private List<XElement> layers = new List<XElement>(); //加载的shpfile层
        private Envelope _shpEnvelope = new Envelope(117.44d, 36.44d, 122.56d, 41.56);
        private StringBuilder _iceCptInfos = new StringBuilder();
        private Dictionary<string, Color> _legendItems = new Dictionary<string, Color>();//图例项
        private string _iceControlPointShpFile = null;
        private ProductColorTable _colorTable;

        public SubProductEDGIICE(SubProductDef subProductDef)
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
            return ISOIAlgorithm();
        }

        private IExtractResult ISOIAlgorithm()
        {
            try
            {
                //获取冰缘线数据
                string[] shpFiles = _argumentProvider.GetArg("SelectedPrimaryFiles") as string[];
                if (shpFiles == null || shpFiles.Length == 0)
                {
                    ISmartSession session = _argumentProvider.GetArg("SmartSession") as ISmartSession;
                    IMonitoringSession ms = session.MonitoringSession as IMonitoringSession;
                    IWorkspace wks = ms.GetWorkspace();
                    if (wks.ActiveCatalog != null)
                    {
                        string[] fs = wks.ActiveCatalog.GetSelectedFiles("EDGE");
                        if (fs != null && fs.Length != 0)
                        {
                            shpFiles = fs;
                            _argumentProvider.SetArg("SelectedPrimaryFiles", shpFiles);
                        }
                    }
                }
                if (!CheckShpFile(ref shpFiles))
                {
                    PrintInfo("冰缘线获取失败!");
                    return null;
                }
                _shpFiles = shpFiles;
                if (_shpFiles.Length == 1)
                    _iceControlPointShpFile = GeoDo.RSS.MIF.Prds.ICE.IceEdgeFileNameHelper.GetIceEdgeControlInfoFilename(_shpFiles[0]);
                ApplyMcd(_shpFiles);
                //ReadShpFileAttribute(shpFile);

                if (!string.IsNullOrEmpty(_iceControlPointShpFile) && File.Exists(_iceControlPointShpFile))
                {
                    ReadIceControlPointInfo(_iceControlPointShpFile);
                }
                CreateLegendItems(_shpFiles);

                IExtractResult er = VectoryThemeGraphy(null);//生成空白的专题图
                _gxdFile = (er as FileExtractResult).FileName;
                AddShpLayerToGxd();
                return er;
            }
            finally
            {
                _argumentProvider.SetArg("SelectedPrimaryFiles", null);
            }
        }

        private bool CheckShpFile(ref string[] shpFiles)
        {
            if (shpFiles == null || shpFiles.Length == 0)
                return false;
            List<string> doShpFiles = new List<string>();
            foreach (string file in shpFiles)
            {
                if (File.Exists(file))
                    doShpFiles.Add(file);
            }
            if (doShpFiles.Count == 0)
            {
                doShpFiles = null;
                return false;
            }
            shpFiles = doShpFiles.ToArray();
            return true;
        }

        #region 1、生成图例项列表

        private void CreateLegendItems(string[] shpFiles)
        {
            _colorTable = ProductColorTableFactory.GetColorTable(_subProductDef.ProductDef.Identify, "EDGE");//ICE,EDGE
            _legendItems.Clear();
            int index = 0;
            foreach (string file in shpFiles)
            {
                index++;
                RasterIdentify identify = new RasterIdentify(file);
                DateTime dt = identify.OrbitDateTime;
                ProductColor pcolor = _colorTable.GetColor(index);
                if (pcolor == null)
                    _legendItems.Add(dt.AddHours(8).ToString(), Color.FromArgb(255, 166, 208, 255));
                else
                    _legendItems.Add(dt.AddHours(8).ToString(), pcolor.Color);
            }
        }

        private ProductColor GetColorByValue(string v)
        {
            float val = 0f;
            float.TryParse(v, out val);
            return _colorTable.GetColor(val);
        }

        protected override void ApplyAttributesOfLayoutTemplate(ILayoutTemplate template)
        {
            Dictionary<string, string> vars = new Dictionary<string, string>();
            vars.Add("{ControlPoints}", _iceCptInfos.ToString());
            template.ApplyVars(vars);
        }
        #endregion

        /// <summary>
        /// 读取shpfile，获取范围信息
        /// </summary>
        /// <param name="shpFile"></param>
        private void ReadShpFileAttribute(string shpFile)
        {
            using (IVectorFeatureDataReader dr = VectorDataReaderFactory.GetUniversalDataReader(shpFile) as IVectorFeatureDataReader)
            {
                if (dr == null)
                    return;
                _shpEnvelope = dr.Envelope;
            }
        }

        /// <summary>
        /// 读取海冰控制点，获取海冰控制单信息
        /// 最多6列，每列最多显示如下行数22,22,14,9,5,5=77，总共最多显示77个点，这样组织显示
        /// </summary>
        /// <param name="shpFile"></param>
        private void ReadIceControlPointInfo(string shpFile)
        {
            _iceCptInfos.Clear();
            List<string> infos = new List<string>();
            using (IVectorFeatureDataReader dr = VectorDataReaderFactory.GetUniversalDataReader(shpFile) as IVectorFeatureDataReader)
            {
                if (dr == null)
                    return;
                if (!dr.Fields.Contains("NO") || !dr.Fields.Contains("TEMP"))
                    return;
                Feature[] features = dr.FetchFeatures();
                if (features == null || features.Length == 0)
                    return;
                _iceCptInfos.AppendLine(string.Format("{0}\t{1}\t{2}\t{3}\t", "编号", "经度", "纬度", "温度"));
                foreach (Feature fet in features)
                {
                    string iceNo = fet.GetFieldValue("NO");
                    string iceTemp = fet.GetFieldValue("TEMP");
                    ShapePoint sp = fet.Geometry as ShapePoint;
                    infos.Add(string.Format("{0}\t{1}\t{2}\t{3}\t", iceNo, sp.X.ToString("f2"), sp.Y.ToString("f2"), iceTemp));
                }
            }
            int maxCol = 22;
            int[] eachColCount = new int[] { 22, 21, 13, 7, 4, 4 };
            int colCount = eachColCount.Length;
            int infoCount = infos.Count;
            for (int i = 0; i < maxCol && i < infoCount; i++)
            {
                int colBeginIndex = i;
                StringBuilder curLine = new StringBuilder(infos[i]);
                for (int j = 1; j < colCount; j++)
                {
                    colBeginIndex += eachColCount[j - 1];
                    if (colBeginIndex >= infoCount)
                        break;
                    if (i < eachColCount[j])
                        curLine.AppendFormat("  {0}", infos[colBeginIndex]);
                    else
                        curLine.AppendFormat("  {0}", " ".PadRight(infos[colBeginIndex].Length));
                }
                _iceCptInfos.AppendLine(curLine.ToString());
            }
        }

        #region 1、应用图例、修改专题图中的图例项

        protected override void ApplyAttributesOfElement(string name, GeoDo.RSS.Layout.IElement ele)
        {
            if (ele is GeoDo.RSS.Layout.Elements.ILegendElement)
            {
                LegendItem[] legendItems = CreateLegendItems();
                ILegendElement legendElement = ele as ILegendElement;
                legendElement.LegendItems = legendItems;
            }
            base.ApplyAttributesOfElement(name, ele);
        }

        private LegendItem[] CreateLegendItems()
        {
            List<LegendItem> items = new List<LegendItem>();
            foreach (string legend in _legendItems.Keys)
            {
                LegendItem item = new LegendItem(legend, _legendItems[legend]);
                items.Add(item);
            }
            return items.ToArray();
        }
        #endregion

        private void ApplyMcd(string[] shpFiles)
        {
            try
            {
                layers.Clear();
                //1.文件复制
                string mcdTempFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"SystemData\ProductArgs\冰缘线专题图文档.mcd");
                //2.修改属性
                XDocument doc = XDocument.Load(mcdTempFileName);
                //2.0 Extend
                XElement mcdExtent = doc.Element("Map").Element("MapArguments").Element("Extent");
                if (mcdExtent != null)
                {
                    double minx, miny, maxx, maxy;
                    double.TryParse(mcdExtent.Attribute("MinX").Value, out minx);
                    double.TryParse(mcdExtent.Attribute("MinY").Value, out miny);
                    double.TryParse(mcdExtent.Attribute("MaxX").Value, out maxx);
                    double.TryParse(mcdExtent.Attribute("MaxY").Value, out maxy);
                    _shpEnvelope = new Envelope(minx, miny, maxx, maxy);
                }
                //2.1 iceline //
                IEnumerable<XElement> eles = doc.Element("Map").Element("Layers").Elements("Layer");
                XElement layerElement = eles.Single((ele) => { return ele.Attribute("name").Value == "EDGE"; });
                XElement[] layerElements = null;
                if (layerElement != null)
                {
                    layerElements = ApplyIcelineLayer(shpFiles, layerElement);
                    layers.AddRange(layerElements);
                }
                //2.2 iceControlpoint
                if (shpFiles.Length == 1)
                {
                    string iceCptShpFile = _iceControlPointShpFile;
                    if (File.Exists(iceCptShpFile))
                    {
                        XElement iceCptElement = doc.Element("Map").Element("Layers").Elements("Layer").Single((ele) => { return ele.Attribute("name").Value == "EDGE.CONTROLPOINT"; });
                        if (iceCptElement != null)
                        {
                            ApplyIcecptLayer(iceCptShpFile, iceCptElement);
                            layers.Add(iceCptElement);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                PrintInfo("应用mcd显示模版失败。" + ex.Message);
            }
        }

        private void ApplyIcecptLayer(string shpFile, XElement layerElement)
        {
            string shpName = Path.GetFileNameWithoutExtension(shpFile);
            layerElement.Attribute("name").Value = shpName;//
            XElement dataSourceEle = layerElement.Element("FeatureClass").Element("DataSource");
            if (dataSourceEle != null)
            {
                dataSourceEle.Attribute("name").Value = shpName;//
                dataSourceEle.Attribute("fileurl").Value = shpFile;//".\\" + Path.GetFileName(shpFile);
            }
        }

        private XElement[] ApplyIcelineLayer(string[] shpFiles, XElement layerElement)
        {
            List<XElement> resultElement = new List<XElement>();
            XElement tempElement = null;
            int length = shpFiles.Length;
            string shpName;
            XElement uniqueSymbolsEle = layerElement.Element("Renderer").Element("Symbol");
            XElement dataSourceEle = layerElement.Element("FeatureClass").Element("DataSource");
            _colorTable = ProductColorTableFactory.GetColorTable(_subProductDef.ProductDef.Identify, "EDGE");
            for (int i = 0; i < length; i++)
            {
                shpName = Path.GetFileNameWithoutExtension(shpFiles[i]);
                layerElement.Attribute("name").Value = shpName;
                if (uniqueSymbolsEle != null)
                {
                    Color itemColor = _colorTable.ProductColors[i].Color;
                    uniqueSymbolsEle.Attribute("color").Value = string.Format("{0},{1},{2},{3}", itemColor.A, itemColor.R, itemColor.G, itemColor.B);
                }
                if (dataSourceEle != null)
                {
                    dataSourceEle.Attribute("name").Value = shpName;//
                    dataSourceEle.Attribute("fileurl").Value = shpFiles[i]; //".\\" + Path.GetFileName(shpFile);
                }
                tempElement = new XElement(layerElement);
                resultElement.Add(tempElement);
            }
            return resultElement.Count == 0 ? null : resultElement.ToArray();
        }

        //设置数据层范围
        protected override void ApplyAttributesOfDataFrame(IGxdDataFrame gxdDataFrame, IDataFrame dataFrame, ILayout layout)
        {
            try
            {
                Layout.GxdEnvelope evp = ToPrjEnvelope(_shpEnvelope, gxdDataFrame, dataFrame);
                if (evp != null)
                {
                    FitSizeToTemplateWidth(layout, (float)(evp.MaxX - evp.MinX), (float)(evp.MaxY - evp.MinY));
                    gxdDataFrame.Envelope = evp;
                }

            }
            catch (Exception ex)
            {
                PrintInfo(ex.Message);
            }
            base.ApplyAttributesOfDataFrame(gxdDataFrame, dataFrame, layout);
        }

        private Layout.GxdEnvelope ToPrjEnvelope(Envelope env, Layout.IGxdDataFrame gxdDataFrame, Layout.IDataFrame dataFrame)
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

        private Project.IProjectionTransform GetProjectionTransform(string spatialRef)
        {
            if (spatialRef == null || !spatialRef.Contains("PROJCS"))
                return Project.ProjectionTransformFactory.GetDefault();
            else
                return Project.ProjectionTransformFactory.GetProjectionTransform(GeoDo.Project.SpatialReference.GetDefault(), GeoDo.Project.SpatialReferenceFactory.GetSpatialReferenceByWKT(spatialRef, Project.enumWKTSource.EsriPrjFile));
        }

        protected override void ApplyAttributesOfLayoutBorder(IBorder border)
        {
            base.ApplyAttributesOfLayoutBorder(border);
        }

        //添加Shpfile层
        private void AddShpLayerToGxd()
        {
            try
            {
                PrintInfo("加载Layer个数：" + layers.Count);
                //设置layer
                XElement xGxdDoc = XElement.Load(_gxdFile);
                XElement xLayers = xGxdDoc.Element("GxdDataFrames").Element("GxdDataFrame").Element("GxdVectorHost").Element("Map").Element("Layers");
                for (int i = 0; i < layers.Count; i++)
                {
                    PrintInfo("Layer" + i + ":" + layers[i].Attribute("name").Value);
                    xLayers.Add(layers[i]);
                }
                xGxdDoc.Save(_gxdFile);
            }
            catch (Exception ex)
            {
                PrintInfo(ex.Message);
            }
        }

        public void PrintInfo(string info)
        {
            if (_contextMessage != null)
                _contextMessage.PrintMessage(info);
            else
                Console.WriteLine(info);
        }
    }
}
