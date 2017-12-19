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
using CodeCell.AgileMap.Core;

namespace GeoDo.RSS.MIF.Prds.ICE
{
    public class SubProductIMGICE : CmaMonitoringSubProduct
    {
        private IContextMessage _contextMessage = null;

        public SubProductIMGICE(SubProductDef subProductDef)
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
            if (_argumentProvider.GetArg("AlgorithmName") == null)
                return null;
            if (_argumentProvider.GetArg("AlgorithmName").ToString() == "0IMGAlgorithm")
            {
                string instanceIdentify = _argumentProvider.GetArg("OutFileIdentify") as string;
                if (instanceIdentify == "ISOI")//
                    return ISOIIMGAlgorithm();
                else
                    return IMGAlgorithm();
            }
            return null;
        }

        private IExtractResult IMGAlgorithm()
        {
            return ThemeGraphyResult(null);
        }

        #region 等值线专题图
        private IExtractResult ISOIIMGAlgorithm()
        {
            try
            {
                //获取等值线数据
                string shpFile = _argumentProvider.GetArg("SelectedPrimaryFiles") as string;
                if (!File.Exists(shpFile))
                {
                    PrintInfo("获取等值线shp文件失败。");
                    return null;
                }
                CreateLegendItems(shpFile);

                return ThemeGraphyResult(null);
            }
            finally
            {
                _argumentProvider.SetArg("SelectedPrimaryFiles", null);
            }
        }

        #region 1、生成图例项列表
        private Dictionary<string, Color> _legendItems = new Dictionary<string, Color>();//图例项
        private Random _random = new Random(1);
        ProductColorTable _colorTable;//ISOT

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
            _colorTable = ProductColorTableFactory.GetColorTable(_subProductDef.ProductDef.Identify, "ISOT");//ISOT
            _legendItems.Clear();
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
                        _legendItems.Add(v, Color.FromArgb(_random.Next(255), _random.Next(255), _random.Next(255)));
                }
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
                LegendItem item = new LegendItem(legend, _legendItems[legend]);
                items.Add(item);
            }
            return items.ToArray();
        }
        #endregion
        #endregion

        public void PrintInfo(string info)
        {
            if (_contextMessage != null)
                _contextMessage.PrintMessage(info);
            else
                Console.WriteLine(info);
        }
    }
}
