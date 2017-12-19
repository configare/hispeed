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
using System.Xml.Linq;

namespace GeoDo.RSS.MIF.Prds.ICE
{
    /// <summary>
    /// 海冰等值线产品生成，专题图制作
    /// </summary>
    public class SubProductISOTICE : CmaMonitoringSubProduct
    {
        private IContextMessage _contextMessage;
        private Action<int, string> _progressTracker = null;

        public SubProductISOTICE(SubProductDef subProductDef)
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
            if (_argumentProvider.GetArg("AlgorithmName").ToString() == "ISOTAlgorithm")
            {
                return ISOTAlgorithm();
            }
            return null;
        }

        private IExtractResult ISOTAlgorithm()
        {
            int FarInfraredCH = (int)_argumentProvider.GetArg("FarInfrared");
            double FarInfraredZoom = (double)_argumentProvider.GetArg("FarInfrared_Zoom");
            int smaping = (int)_argumentProvider.GetArg("Smaping");
            int tempratureMin = (int)(((int)_argumentProvider.GetArg("TempratureMin") + 273) * FarInfraredZoom);
            int tempratureMax = (int)(((int)_argumentProvider.GetArg("TempratureMax") + 273) * FarInfraredZoom);
            int interval = (int)((int)_argumentProvider.GetArg("IntervalMax") * FarInfraredZoom);
            bool isOutputUncompleted = (bool)_argumentProvider.GetArg("IsOutputUncompleted");

            string filename = _argumentProvider.GetArg("CurrentRasterFile") as string;
            using (IRasterDataProvider rasterProvider = RasterDataDriver.Open(filename) as IRasterDataProvider)
            {
                if (rasterProvider != null)
                {
                    IBandNameRaster bandNameRaster = rasterProvider as IBandNameRaster;
                    FarInfraredCH = TryGetBandNo(bandNameRaster, "FarInfrared");
                }
            }

            if (FarInfraredCH == -1)
            {
                PrintInfo("获取波段序号失败,可能是波段映射表配置错误或判识算法波段参数配置错误。");
                return null;
            }
            if (_argumentProvider.DataProvider == null)
            {
                PrintInfo("获取文件错误,可能是没有打开影像文件。");
                return null;
            }
            try
            {
                //生成等值线shp文件
                string shpFile = GenISOTFiename(filename);
                //bool isOutputUncompleted = true;
                GenerateContourLines gcl = new GenerateContourLines(_progressTracker, _contextMessage);
                gcl.DoGenerateContourLines(_argumentProvider.DataProvider, FarInfraredCH, _argumentProvider.AOI, interval, tempratureMin, tempratureMax, smaping, shpFile, isOutputUncompleted);
                if (!File.Exists(shpFile))
                {
                    PrintInfo("生成等值线数据失败。");
                    return null;
                }
                //根据等值线的配色表，生成等值线shp付色方案mcd文件,_legendItems
                CreateLegendItems(shpFile);
                CreateMcd(shpFile);
                return new FileExtractResult(_subProductDef.Identify, shpFile, true);
            }
            finally
            {
            }
        }

        #region 1、生成图例项列表
        private Dictionary<string, ProductColor> _legendItems = new Dictionary<string, ProductColor>();//图例项
        private Random _random = new Random(1);
        private ProductColorTable _colorTable;//ISOT

        private void CreateLegendItems(string shpFile)
        {
            using (IVectorFeatureDataReader dr = VectorDataReaderFactory.GetUniversalDataReader(shpFile) as IVectorFeatureDataReader)
            {
                if (dr == null)
                    return;
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
                }
                //else if(!_legendItems.ContainsKey(v))
                //    _legendItems.Add(v, Color.FromArgb(_random.Next(255), _random.Next(255), _random.Next(255)));
            }
            //温度按照从大到小排序.
            KeyValuePair<string,ProductColor>[] orderItems =  _legendItems.OrderByDescending((item) => { return item.Key; }).ToArray();
            _legendItems.Clear();
            foreach (KeyValuePair<string, ProductColor> v in orderItems)
            {
                _legendItems.Add(v.Key,v.Value);
            }
        }

        private ProductColor GetColorByValue(string v)
        {
            float val = 0f;
            float.TryParse(v, out val);
            return _colorTable.GetColor(val);
        }
        #endregion

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
                //根据前面取得的颜色表，设置等值线颜色
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
                    foreach (ProductColor item in _legendItems.Values)
                    {
                        XElement addElement = new XElement(element);
                        addElement.Attribute("color").Value = string.Format("{0},{1},{2},{3}", item.Color.A, item.Color.R, item.Color.G, item.Color.B);
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

        private string GenISOTFiename(string rasterFile)
        {
            RasterIdentify rd = new RasterIdentify(rasterFile);
            rd.SubProductIdentify = _subProductDef.Identify;
            rd.ProductIdentify = _subProductDef.ProductDef.Identify;
            rd.Format = ".shp";
            return rd.ToWksFullFileName(".shp");
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
