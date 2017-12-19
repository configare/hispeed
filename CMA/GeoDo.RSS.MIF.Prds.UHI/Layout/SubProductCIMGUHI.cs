using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Prds.Comm;
using GeoDo.RSS.MIF.Core;
using System.IO;
using CodeCell.AgileMap.Core;
using GeoDo.RSS.Core.RasterDrawing;
using GeoDo.RSS.Core.UI;
using GeoDo.RSS.Core.DrawEngine;
using System.Xml.Linq;
using GeoDo.RSS.Layout;
using GeoDo.RSS.Layout.Elements;
using System.Drawing;

namespace GeoDo.RSS.MIF.Prds.UHI
{
    public class SubProductCIMGUHI : CmaMonitoringSubProduct
    {
        private string _gxdFile = null;
        private string _shpFile = null;
        private StringBuilder _pointInfos = new StringBuilder(); 
        private IContextMessage _contextMessage;

        public SubProductCIMGUHI(SubProductDef subProductDef)
            :base(subProductDef)
        {
        }

        public override IExtractResult Make(Action<int, string> progressTracker, IContextMessage contextMessage)
        {
            if (_argumentProvider == null)
                return null;
            _contextMessage = contextMessage;
            object algName = _argumentProvider.GetArg("AlgorithmName");
            if (algName == null)
                return null;
            if (algName.ToString() == "CMAAlgorithm")
            {
                return CMAAlgorithm();
            }
            return null;
        }

        private IExtractResult CMAAlgorithm()
        {
           
            string colorFileName = _argumentProvider.GetArg("DensitySetting").ToString();
            if(string.IsNullOrEmpty(colorFileName))
                return null;
            CreateLegendItems(colorFileName);
            //专题图
            string outIdentify = _argumentProvider.GetArg("OutFileIdentify") as string;
            if (string.IsNullOrEmpty(outIdentify))
                return null;
            SubProductInstanceDef instance = FindSubProductInstanceDefs(outIdentify);
            IExtractResult er = ThemeGraphyMCSI(instance);
            _gxdFile = (er as FileExtractResult).FileName;
            //矢量
            if (_argumentProvider.GetArg("ShpFile") != null)
            {
                string fileName = _argumentProvider.GetArg("ShpFile").ToString();
                if (!string.IsNullOrEmpty(fileName))
                {
                    _shpFile = GenerateShpFormTxt(fileName);
                    AddShpToGxd();
                }
            }      
            return er;
        }

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

        private string GenerateShpFormTxt(string fileName)
        {
            if (string.IsNullOrEmpty(fileName)||!File.Exists(fileName))
                return null;
            string[] pointInfos = File.ReadAllLines(fileName,Encoding.Default);
            if (pointInfos == null || pointInfos.Length < 2)
                return null;
            List<Feature> features = new List<Feature>();
            _pointInfos.Clear();
            ShapePoint pt;
            string[] fieldNames = pointInfos[0].Replace(" ","").Split(new char[]{'\t'},StringSplitOptions.RemoveEmptyEntries);
            string[] fieldValues; 
            for (int i = 1; i < pointInfos.Length; i++)
            {
                fieldValues = pointInfos[i].Replace(" ", "").Split(new char[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);
                if (fieldValues.Length != fieldNames.Length)
                    continue;
                pt = new ShapePoint(float.Parse(fieldValues[2]), float.Parse(fieldValues[3]));
                Feature f = new Feature(i-1, pt, fieldNames, fieldValues, null);
                features.Add(f);
                _pointInfos.AppendLine(string.Format("{0}\t{1}\t", i, fieldValues[1]));
            }
            if (features.Count < 1)
                return null;
            string shpFileName = Path.Combine(Path.GetDirectoryName(_gxdFile), Path.GetFileNameWithoutExtension(_gxdFile) + ".shp");
            EsriShapeFilesWriterII w = new EsriShapeFilesWriterII(shpFileName, enumShapeType.Point);
            w.BeginWrite();
            w.Write(features.ToArray());
            w.EndWriter();
            //mcd文件
            CreateMcd(shpFileName);
            return shpFileName;   
        }

        private string GetOutputGxdFileName(RasterIdentify rstIdentifyCopy, string productIdentify, string subProductIdentify)
        {
            RasterIdentify rstIdentify = rstIdentifyCopy;
            if (rstIdentifyCopy == null)
                rstIdentify = new RasterIdentify();
            else
                rstIdentify = rstIdentifyCopy;
            rstIdentify.ProductIdentify = productIdentify;
            rstIdentify.SubProductIdentify = subProductIdentify;
            return rstIdentify.ToWksFullFileName(".gxd");
        }

        private void CreateMcd(string shpFile)
        {
            try
            {
                //1.文件复制
                string sourceFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"SystemData\ProductArgs\城市热岛专题图模版.mcd");
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

        private void PrintInfo(string info)
        {
            _contextMessage.PrintMessage(info);
        }


        #region 1、生成图例项列表
        private Dictionary<string, ProductColor> _legendItems = new Dictionary<string, ProductColor>();//图例项
        private Random _random = new Random(1);
        private ProductColorTable _colorTable;

        private void CreateLegendItems(string xmlFile)
        {
            _legendItems.Clear();
            XDocument doc = XDocument.Load(xmlFile);
            if (doc == null)
                return;
            XElement ele = doc.Element("ImageProcessQueue").Element("DensitySlice").Element("RANGE");
            if (ele == null)
                return;
            IEnumerable<XElement> ranges = ele.Elements();
            int i = 0;
            string[] lableText = new string[] { "无热岛", "弱热岛", "中弱岛", "强弱岛" };
            foreach (XElement item in ranges)
            {
                ProductColor color = new ProductColor();
                color.MinValue = float.Parse(item.Element("L").Value);
                color.MaxValue = float.Parse(item.Element("R").Value);
                color.Color = Color.FromArgb(int.Parse(item.Element("RGB_R").Value), int.Parse(item.Element("RGB_G").Value), int.Parse(item.Element("RGB_B").Value));
                color.LableText = i<4?lableText[i]:"";
                _legendItems.Add(item.Name.ToString(), color);
                i++;
            }
            if (_legendItems.Count != 4)
            {
                foreach (ProductColor item in _legendItems.Values)
                {
                    item.LableText = item.MinValue.ToString();
                }
            }
        }
        #endregion

        #region 1、应用图例、修改专题图中的图例项
        protected override void ApplyAttributesOfElement(string name, IElement ele)
        {
            if (ele is ILegendElement)
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

        protected override void ApplyAttributesOfLayoutTemplate(ILayoutTemplate template)
        {
            Dictionary<string, string> vars = new Dictionary<string, string>();
            if (_pointInfos != null && _pointInfos.Length > 0)
            {
                vars.Add("{ControlPoints}", _pointInfos.ToString());
                template.ApplyVars(vars);
            }
        }
        #endregion
    }
}
