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
    /// 等值线专题图
    /// </summary>
    public class SubProductISOIICE : CmaMonitoringSubProduct
    {
        private IContextMessage _contextMessage;
        private Action<int, string> _progressTracker = null;
        private string _shpFile = null;
        private string _gxdFile = null;
        private Envelope _shpEnvelope = null;
        private Dictionary<string, Color> _symbolColor = new Dictionary<string, Color>();

        public SubProductISOIICE(SubProductDef subProductDef)
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
            if (_argumentProvider.GetArg("AlgorithmName").ToString() == "ISOIAlgorithm")
            {
                return ISOIAlgorithm();
            }
            return null;
        }

        private IExtractResult ISOIAlgorithm()
        {
            try
            {
                //获取等值线数据
                string shpFile = _argumentProvider.GetArg("SelectedPrimaryFiles") as string;
                if (string.IsNullOrWhiteSpace(shpFile))
                {
                    ISmartSession session = _argumentProvider.GetArg("SmartSession") as ISmartSession;
                    IMonitoringSession ms = session.MonitoringSession as IMonitoringSession;
                    IWorkspace wks = ms.GetWorkspace();
                    if (wks.ActiveCatalog != null)
                    {
                        string[] fs = wks.ActiveCatalog.GetSelectedFiles("ISOT");
                        if (fs != null && fs.Length != 0)
                        {
                            shpFile = fs[0];
                            _argumentProvider.SetArg("SelectedPrimaryFiles", shpFile);
                        }
                    }
                }
                if (!File.Exists(shpFile))
                {
                    PrintInfo("获取等值线shp文件失败。");
                    return null;
                }
                _symbolColor.Clear();
                CreateLegendItems(shpFile);
                _shpFile = shpFile;
                IExtractResult er = ThemeGraphyResult(null);
                _gxdFile = (er as FileExtractResult).FileName;
                AddShpToGxd();
                return er;
            }
            finally
            {
                _argumentProvider.SetArg("SelectedPrimaryFiles", null);
            }
        }

        #region 1、生成图例项列表
        private Dictionary<string, ProductColor> _legendItems = new Dictionary<string, ProductColor>();//图例项
        private ProductColorTable _colorTable;   //ISOT

        private void CreateLegendItems(string shpFile)
        {
            using (IVectorFeatureDataReader dr = VectorDataReaderFactory.GetUniversalDataReader(shpFile) as IVectorFeatureDataReader)
            {
                if (dr == null)
                    return;
                _shpEnvelope = dr.Envelope;
                Feature[] features = dr.FetchFeatures();
                if (features == null || features.Length == 0)
                    return;
                string field = dr.Fields[0];//Contour
                GetFeatureValueType(features, field);
            }
        }

        private void GetFeatureValueType(Feature[] features, string field)
        {
            _colorTable = ProductColorTableFactory.GetColorTable(_subProductDef.ProductDef.Identify, "ISOT");//ICE,ISOT
            _legendItems.Clear();
            foreach (Feature fet in features)
            {
                string v = fet.GetFieldValue(field);
                if (string.IsNullOrWhiteSpace(v))
                    continue;
                v = v.Trim();
                ProductColor pc = GetColorByValue(v);
                if (pc != null && !_legendItems.ContainsKey(v))
                {
                    _legendItems.Add(v, pc);
                    _symbolColor.Add(v, pc.Color);
                }
                //else if(!_legendItems.ContainsKey(v))
                //    _legendItems.Add(v, Color.FromArgb(_random.Next(255), _random.Next(255), _random.Next(255)));
            }
            //温度按照从大到小排序.
            KeyValuePair<string, ProductColor>[] orderItems = _legendItems.OrderByDescending((item) => { return item.Key; }).ToArray();
            _legendItems.Clear();
            foreach (KeyValuePair<string, ProductColor> v in orderItems)
            {
                _legendItems.Add(v.Key, v.Value);
            }
        }

        private ProductColor GetColorByValue(string v)
        {
            float val = 0f;
            float.TryParse(v, out val);
            return _colorTable.GetColor(val);
        }
        #endregion

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
                LegendItem item = new LegendItem(_legendItems[legend].LableText, _legendItems[legend].Color);
                items.Add(item);
            }
            return items.ToArray();
        }
        #endregion

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
                Console.WriteLine(ex.Message);
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

        //添加Shp
        private void AddShpToGxd()
        {
            try
            {
                string shpMcd = Path.ChangeExtension(_shpFile, ".mcd");
                XElement xShpMcd = XElement.Load(shpMcd);
                XElement xShpLayer = xShpMcd.Element("Layers").Element("Layer");
                ApplyColorTable(_shpFile, xShpLayer);
                //xShpLayer.Element("FeatureClass").Element("DataSource").SetAttributeValue("fileurl", _shpFile);

                XElement xGxd = XElement.Load(_gxdFile);// gxdDataFrame.GxdVectorHost.McdFileContent.ToString());     //
                XElement xLayers = xGxd.Element("GxdDataFrames").Element("GxdDataFrame").Element("GxdVectorHost").Element("Map").Element("Layers");
                xLayers.Add(xShpLayer);
                xGxd.Save(_gxdFile);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void ApplyColorTable(string shpFile, XElement layerElement)
        {
            string shpName;
            XElement uniqueSymbolsEle = layerElement.Element("Renderer").Element("UniqueSymbols");
            XElement symbolEle = null;
            if (uniqueSymbolsEle == null)
                return;
            symbolEle = uniqueSymbolsEle.Element("Symbol");
            if (symbolEle == null)
                return;
            XElement dataSourceEle = layerElement.Element("FeatureClass").Element("DataSource");
            shpName = Path.GetFileNameWithoutExtension(shpFile);
            layerElement.Attribute("name").Value = shpName;
            List<XElement> symbols = new List<XElement>();
            XElement tmepSymbol = null;
            string values = string.Empty;
            Color cl;
            uniqueSymbolsEle.RemoveNodes();
            foreach (string value in _symbolColor.Keys)
            {
                values += value + ",";
                tmepSymbol = new XElement(symbolEle);
                cl = _symbolColor[value];
                tmepSymbol.Attribute("color").Value = string.Format("{0},{1},{2},{3}", cl.A, cl.R, cl.G, cl.B);
                uniqueSymbolsEle.Add(tmepSymbol);
            }
            uniqueSymbolsEle.Attribute("UniqueValues").Value = values.Substring(0, values.Length - 1);
            if (dataSourceEle != null)
            {
                dataSourceEle.Attribute("name").Value = shpName;//
                dataSourceEle.Attribute("fileurl").Value = shpFile; //".\\" + Path.GetFileName(shpFile);
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
