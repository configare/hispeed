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
using System.Xml.Linq;
using GeoDo.RSS.Layout;

namespace GeoDo.RSS.MIF.Prds.ICE
{
    /// <summary>
    /// 自动提取海冰冰缘线
    /// </summary>
    public class SubProductAEDGICE : CmaMonitoringSubProduct
    {
        private IContextMessage _contextMessage;
        private Action<int, string> _progressTracker = null;
        private Envelope _shpEnvelope = null;
        private string _gxdFile = null;
        private string _shpFile = null;

        public SubProductAEDGICE(SubProductDef subProductDef)
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
            if (_argumentProvider.GetArg("AlgorithmName").ToString() == "AEDGAlgorithm")
            {
                return AEDGAlgorithm();
            }
            return null;
        }

        private IExtractResult AEDGAlgorithm()
        {
            int smaping,contourValue;
            if(!Int32.TryParse(_argumentProvider.GetArg("Smaping").ToString(), out smaping))
                return null;
            if (!Int32.TryParse(_argumentProvider.GetArg("contourValue").ToString(), out contourValue))
                return null;
            double[] contourValues = new double[] {contourValue};
            string[] files = GetStringArray("SelectedPrimaryFiles");
            if (files == null || files.Length ==0)
            {
                PrintInfo("获取文件错误,可能是没有选择判识结果文件。");
                return null;
            }

            IRasterDataProvider provider = null;
            try
            {
                provider = GeoDataDriver.Open(files[0], enumDataProviderAccess.ReadOnly, null) as IRasterDataProvider;
                if (provider == null)
                {
                    PrintInfo("获取文件错误,可能是系统不支持当前文件。");
                    return null;
                }

                string shpFile = GenFiename(files[0]);
                GenerateContourLines gcl = new GenerateContourLines(_progressTracker, _contextMessage);
                gcl.DoGenerateContourLines(provider, 1, _argumentProvider.AOI, contourValues, smaping, shpFile);
                if (!File.Exists(shpFile))
                {
                    PrintInfo("生成等值线数据失败。");
                    return null;
                }
                CreateLegendItems(shpFile);
                CreateMcd(shpFile);
                _argumentProvider.SetArg("SelectedPrimaryFiles", shpFile);
                _shpFile = shpFile;
                IExtractResult er = ThemeGraphyResult(null);
                _gxdFile = (er as FileExtractResult).FileName;
                AddShpToGxd();
                return er;
            }
            finally
            {
                provider.Dispose();
            }
        }

        private void PrintInfo(string info)
        {
            if (_contextMessage != null)
                _contextMessage.PrintMessage(info);
            else
                Console.WriteLine(info);
        }

        private string GenFiename(string rasterFile)
        {
            RasterIdentify rd = new RasterIdentify(rasterFile);
            rd.SubProductIdentify = _subProductDef.Identify;
            rd.ProductIdentify = _subProductDef.ProductDef.Identify;
            rd.Format = ".shp";
            return rd.ToWksFullFileName(".shp");
        }

        private void CreateMcd(string shpFile)
        {
            try
            {
                //1.文件复制
                string sourceFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"SystemData\ProductArgs\海冰表面温度模版.mcd");
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
                layerElement.Attribute("name").Value = shpName;//
                XElement uniqueSymbolsEle = layerElement.Element("Renderer").Element("UniqueSymbols");
                if (_legendItems != null && _legendItems.Count > 0)
                {
                    string uniqueValues = string.Empty;
                    foreach (string value in _legendItems.Keys)
                        uniqueValues += value + ",";
                    uniqueValues = uniqueValues.Remove(uniqueValues.Length - 1);
                    uniqueSymbolsEle.Attribute("UniqueValues").Value = uniqueValues;//keys
                    XElement element = new XElement(uniqueSymbolsEle.Element("Symbol"));
                    uniqueSymbolsEle.Elements("Symbol").Remove();
                    foreach (Color item in _legendItems.Values)
                    {
                        XElement addElement = new XElement(element);
                        addElement.Attribute("color").Value = string.Format("{0},{1},{2},{3}", item.A, item.R, item.G, item.B);
                        uniqueSymbolsEle.Add(addElement);
                    }
                }
                if (uniqueSymbolsEle == null)
                    return;
                XElement dataSourceEle = layerElement.Element("FeatureClass").Element("DataSource");
                if (dataSourceEle == null)
                    return;
                dataSourceEle.Attribute("name").Value = shpName;//
                dataSourceEle.Attribute("fileurl").Value = ".\\" + Path.GetFileName(shpFile);//

                doc.Save(newFileName);
            }
            catch
            {
                PrintInfo("创建mcd失败。");
            }
        }

        private Dictionary<string, Color> _legendItems = new Dictionary<string, Color>();//图例项
        ProductColorTable _colorTable;

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
            _colorTable = ProductColorTableFactory.GetColorTable(_subProductDef.ProductDef.Identify, "ISOT");//ISOT
            _legendItems.Clear();
            Random random = new Random(1);
            foreach (Feature fet in features)
            {
                string v = fet.GetFieldValue(field);
                if (v == null)
                    continue;
                v = v.Trim();
                if (!_legendItems.ContainsKey(v))
                {
                    ProductColor pc = GetColorByValue(v);
                    if (pc != null)
                        _legendItems.Add(pc.LableText, pc.Color);
                    else
                        _legendItems.Add(v, Color.FromArgb(random.Next(255), random.Next(255), random.Next(255)));
                }
            }
        }

        private ProductColor GetColorByValue(string v)
        {
            float val = 0f;
            float.TryParse(v, out val);
            return _colorTable.GetColor(val);
        }

        //添加Shp
        private void AddShpToGxd()
        {
            try
            {
                string shpMcd = Path.ChangeExtension(_shpFile, ".mcd");
                XElement xShpMcd = XElement.Load(shpMcd);
                XElement xShpLayer = xShpMcd.Element("Layers").Element("Layer");
                xShpLayer.Element("FeatureClass").Element("DataSource").SetAttributeValue("fileurl", _shpFile);

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
    }
}
