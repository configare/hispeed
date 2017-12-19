using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Core;
using System.Drawing;
using System.IO;
using GeoDo.RSS.MIF.Prds.Comm;
using GeoDo.RSS.Layout;
using GeoDo.RSS.Layout.Elements;
using System.Xml.Linq;

namespace GeoDo.RSS.MIF.Prds.UHE
{
    public class SubProductIMGUHE : CmaMonitoringSubProduct
    {
        private List<ProductColor> _legendItems = new List<ProductColor>();//图例项
        private ProductColorTable _colorTable;

        public SubProductIMGUHE(SubProductDef subProductDef)
            : base(subProductDef)
        {

        }

        public override IExtractResult Make(Action<int, string> progressTracker)
        {
            if (_argumentProvider == null)
                return null;
            if (_argumentProvider.GetArg("AlgorithmName") == null)
                return null;
            if (_argumentProvider.GetArg("AlgorithmName").ToString() == "0IMGAlgorithm")
            {
                try
                {
                    string instanceIdentify = _argumentProvider.GetArg("OutFileIdentify") as string;
                    if (string.IsNullOrWhiteSpace(instanceIdentify))
                        return null;
                    SubProductInstanceDef instance = FindSubProductInstanceDefs(instanceIdentify);
                    if (instance == null || instanceIdentify == "MCSI"||instanceIdentify=="OMCS")
                        return ThemeGraphyResult(null);
                    if (instance.FileProvider.Contains("DBLV"))
                    {
                        string[] selectedFileNames = _argumentProvider.GetArg("SelectedPrimaryFiles") as string[];
                        if (!string.IsNullOrEmpty(selectedFileNames[0]))
                            CreateLegendItems(selectedFileNames[0], instanceIdentify);
                        if (_colorTable != null)
                        {
                            instance.ColorTableName = _colorTable.ColorTableName;
                        }
                        else
                            throw new Exception("无法获取合适的颜色表");
                    }
                    return ThemeGraphyByInstance(instance);
                }
                finally
                {
                    _colorTable = null;
                    _legendItems.Clear();
                }
            }
            return null;
        }

        private void CreateLegendItems(string selectedFileName, string instanceIdentify)
        {
            string colorTableName = null;
            if (instanceIdentify == "ONMI")
                colorTableName = "LSTANMI";
            else
            {
                RasterIdentify rstIdentify = new RasterIdentify(selectedFileName);
                string season = GetSeason(rstIdentify);
                //未能读取到日期时默认设置为春天
                if (string.IsNullOrEmpty(season))
                    season = "Spring";
                switch (season)
                {
                    case "Spring":
                        colorTableName = "LSTSP";
                        break;
                    case "Summer":
                        colorTableName = "LSTSU";
                        break;
                    case "Autumn":
                        colorTableName = "LSTFA";
                        break;
                    case "Winter":
                        colorTableName = "LSTWI";
                        break;
                }
            }
            if (string.IsNullOrEmpty(colorTableName))
                return;
            _colorTable = ProductColorTableFactory.GetColorTable(colorTableName);
            if (_colorTable == null || _colorTable.ProductColors.Length < 1)
                return;
            _legendItems.Clear();
            foreach (ProductColor color in _colorTable.ProductColors)
            {
                _legendItems.Add(color);
            }
        }

        private string GetSeason(RasterIdentify rstIdentify)
        {
            string season = "";
            if (rstIdentify.OrbitDateTime != null && rstIdentify.OrbitDateTime != DateTime.MinValue)
            {
                DateTime dateTime = rstIdentify.OrbitDateTime;
                //根据配置文件确定日期所在季节
                string settingXml = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "LSTSeasonSetting.xml");
                XDocument doc = XDocument.Load(settingXml);
                if (doc == null)
                    return season;
                XElement ele = doc.Element("Seasons");
                int month = dateTime.Month;
                IEnumerable<XElement> seasons = ele.Elements();
                int tempMonth = 0;
                foreach (XElement item in seasons)
                {
                    IEnumerable<XElement> months = item.Elements();
                    foreach (XElement mon in months)
                        if (int.TryParse(mon.Value, out tempMonth) && month == tempMonth)
                        {
                            season = item.Name.ToString();
                            return season;
                        }
                }
            }
            return season;
        }

        private IExtractResult IMGAlgorithm()
        {
            return ThemeGraphyResult(null);
        }

        #region 应用图例、修改专题图中的图例项

        protected override void ApplyAttributesOfElement(string name, IElement ele)
        {
            if (ele is ILegendElement)
            {
                if (_legendItems == null || _legendItems.Count < 1)
                    return;
                LegendItem[] legendItems = CreateLegendItems();
                ILegendElement legendElement = ele as ILegendElement;
                if (legendItems != null && legendItems.Length > 0)
                    legendElement.LegendItems = legendItems;
            }
            base.ApplyAttributesOfElement(name, ele);
        }

        private LegendItem[] CreateLegendItems()
        {
            List<LegendItem> items = new List<LegendItem>();
            foreach (ProductColor item in _legendItems)
            {
                if (!item.DisplayLengend)
                    continue;
                LegendItem it = new LegendItem(item.LableText,item.Color);
                items.Add(it);
            }
            return items.ToArray();
        }

        #endregion
    }
}
