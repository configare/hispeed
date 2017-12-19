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
using GeoDo.RSS.Core.UI;
using GeoDo.RSS.MIF.Prds.ICE;
using System.Runtime.InteropServices;
using GeoDo.RSS.UI.AddIn.Theme;

namespace GeoDo.RSS.MIF.Prds.ICE
{
    public class SubProductTFQIICE : CmaMonitoringSubProduct
    {
        private IContextMessage _contextMessage;
        private string[] _shpFiles = null;
        private string _gxdFile = null;     //专题图文件
        private List<XElement> layers = new List<XElement>(); //加载的shpfile层
        private string _iceControlPointShpFile = null;
        private ProductColorTable _colorTable;
        private Envelope _shpEnvelope = new Envelope(117.44d, 36.44d, 122.56d, 41.56);
        private Dictionary<string, Color> _legendItems = new Dictionary<string, Color>();//图例项
        private string _subColorname = "TFQI";

        public SubProductTFQIICE(SubProductDef subProductDef)
            : base(subProductDef)
        {

        }

        public override IExtractResult Make(Action<int, string> progressTracker)
        {
            return Make(progressTracker, null);
        }

        public override IExtractResult Make(Action<int, string> progressTracker, IContextMessage contextMessage)
        {
            _contextMessage = contextMessage;
            if (_argumentProvider == null)
                return null;
            return ICETFQI();
        }

        private IExtractResult ICETFQI()
        {
            string outFileIdentify = GetStringArgument("OutFileIdentify");
            SubProductInstanceDef instatnce = GetSubProductInstanceByOutIdentify(outFileIdentify);
            if (instatnce != null)
                outFileIdentify = instatnce.OutFileIdentify;
            string templatName = GetStringArgument("ThemeGraphTemplateName");
            //获取冰缘线数据
            string[] shpFiles = _argumentProvider.GetArg("ShpFile") as string[];
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
                //return null;
            }
            else
            {
                _shpFiles = shpFiles;
                //if (_shpFiles.Length == 1)
                //    _iceControlPointShpFile = GeoDo.RSS.MIF.Prds.ICE.IceEdgeFileNameHelper.GetIceEdgeControlInfoFilename(_shpFiles[0]);
                ApplyMcd(_shpFiles, @"SystemData\ProductArgs\长序列冰缘线专题图文档.mcd");
                CreateLegendItems(_shpFiles);
            }
            IExtractResult result = null;
            SubProductInstanceDef instance = new SubProductInstanceDef();
            instance.OutFileIdentify = outFileIdentify;
            instance.LayoutName = templatName;
            instance.isautogenerate = false;
            result = ThemeGraphyByInstance(instance);
            _gxdFile = (result as FileExtractResult).FileName;
            if (CheckShpFile(ref shpFiles))
                AddShpLayerToGxd();
            (result as FileExtractResult).Add2Workspace = true;
            return result as IExtractResult;
        }

        #region 1、生成图例项列表

        private void CreateLegendItems(string[] shpFiles)
        {
            _colorTable = ProductColorTableFactory.GetColorTable(_subProductDef.ProductDef.Identify, _subColorname);//ICE,EDGE
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

        #endregion

        #region 1、应用图例、修改专题图中的图例项

        protected override void ApplyAttributesOfElement(string name, GeoDo.RSS.Layout.IElement ele)
        {
            if (ele is GeoDo.RSS.Layout.Elements.LinearLegendElement)
            {
                if (ele.Name == "冰缘线图例")
                {
                    GeoDo.RSS.Layout.Elements.LinearLegendElement lineLegendElement = ele as GeoDo.RSS.Layout.Elements.LinearLegendElement;
                    lineLegendElement.Color = CreateLegendItems();
                }
            }
            base.ApplyAttributesOfElement(name, ele);
        }

        private Color CreateLegendItems()
        {
            if (_legendItems == null)
                return Color.Empty;
            foreach (string legend in _legendItems.Keys)
                return _legendItems[legend];
            return Color.Empty;
        }
        #endregion


        private void ApplyMcd(string[] shpFiles, string mcdFile)
        {
            try
            {
                layers.Clear();
                //1.文件复制
                string mcdTempFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, mcdFile);
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
            layerElement.Attribute("name").Value = shpName;
            XElement dataSourceEle = layerElement.Element("FeatureClass").Element("DataSource");
            if (dataSourceEle != null)
            {
                dataSourceEle.Attribute("name").Value = shpName;
                dataSourceEle.Attribute("fileurl").Value = shpFile;
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
            _colorTable = ProductColorTableFactory.GetColorTable(_subProductDef.ProductDef.Identify, _subColorname);
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
                    dataSourceEle.Attribute("name").Value = shpName;
                    dataSourceEle.Attribute("fileurl").Value = shpFiles[i];
                }
                tempElement = new XElement(layerElement);
                resultElement.Add(tempElement);
            }
            return resultElement.Count == 0 ? null : resultElement.ToArray();
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
