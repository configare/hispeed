using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Core;
using System.Drawing;
using System.IO;
using GeoDo.RSS.MIF.Prds.Comm;
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.DF.MEM;
using GeoDo.RSS.UI.AddIn.Tools;

namespace GeoDo.RSS.MIF.Prds.FLD
{
    public class SubProductLayoutFldImg : CmaMonitoringSubProduct
    {
        private List<ProductColor> _legendItems = new List<ProductColor>();//图例项
        private ProductColorTable _colorTable;

        public SubProductLayoutFldImg(SubProductDef subProductDef)
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
                return IMGAlgorithm();
            }
            return null;
        }

        private IExtractResult IMGAlgorithm()
        {
            string instanceIdentify = _argumentProvider.GetArg("OutFileIdentify") as string;
            if (string.IsNullOrWhiteSpace(instanceIdentify))
                return null;
            SubProductInstanceDef instance = FindSubProductInstanceDefs(instanceIdentify);
            if (instance == null)
                return ThemeGraphyResult(null);
            if (instanceIdentify == "0MSI")
            {
                return ThemeGraphyMCSIDBLV(instance);
            }
            else if (instanceIdentify == "FLSI")
            {
                string[] files = GetStringArray("SelectedPrimaryFiles");
                if (files == null || files.Length == 0)
                {
                    TrySetSelectedPrimaryFiles(ref files);
                }
                if (files == null || files.Length == 0)
                    return null;
                string colorTableFile = ProductColorTableParser.LoadProColorTable("FLD");
                ProductColorTable[] colorTabels = ProductColorTableParser.Parse(colorTableFile);
                ProductColorTable[] colorTabelsBack = ProductColorTableParser.Parse(colorTableFile);
                foreach (ProductColorTable pct in colorTabels)
                {
                    if (pct.ColorTableName != "FLD" + instanceIdentify)
                        continue;
                    List<int> colorNums = new List<int>();
                    using (IRasterDataProvider rdp = GeoDataDriver.Open(files[0]) as IRasterDataProvider)
                    {
                        LastDaysSetValue outLastDays = (rdp as MemoryRasterDataProvider).GetExtHeader<LastDaysSetValue>();
                        for (int i = 0; i < outLastDays.LastDaysColor.Length; i++)
                        {
                            if (outLastDays.LastDaysColor[i] == 0)
                                break;
                            colorNums.Add(outLastDays.LastDaysColor[i]);
                        }
                    }
                    if (colorNums.Count == 0)
                        break;
                    List<ProductColor> pcList = new List<ProductColor>();
                    pcList.Add(pct.ProductColors[0]);
                    pct.ProductColors[1].MaxValue = colorNums[0] + 1;
                    pct.ProductColors[1].LableText = "  " + (pct.ProductColors[1].MaxValue - 1).ToString();
                    pcList.Add(pct.ProductColors[1]);
                    for (int i = 2; i < colorNums.Count + 1; i++)
                    {
                        pct.ProductColors[i].MinValue = colorNums[i - 2] + 1;
                        pct.ProductColors[i].MaxValue = colorNums[i - 1] + 1;
                        pct.ProductColors[i].LableText = "  " + (pct.ProductColors[i].MaxValue - 1).ToString();
                        pcList.Add(pct.ProductColors[i]);
                    }
                    //ProductColor newpc = new ProductColor();
                    //newpc.Color = pct.ProductColors[pct.ProductColors.Length - 3].Color;
                    //newpc.DisplayLengend = pct.ProductColors[pct.ProductColors.Length - 3].DisplayLengend;
                    //newpc.LableText = ">" + colorNums[colorNums.Count - 1] + "天";
                    //newpc.MaxValue = pct.ProductColors[pct.ProductColors.Length - 3].MaxValue;
                    //newpc.MinValue = colorNums[colorNums.Count - 1] + 1;
                    //pcList.Add(newpc);
                    pcList.Add(pct.ProductColors[pct.ProductColors.Length - 2]);
                    pcList.Add(pct.ProductColors[pct.ProductColors.Length - 1]);
                    pct.ProductColors = pcList.ToArray();
                    ProductColorTableParser.WriteToXml(colorTabels, colorTableFile);
                    ProductColorTableFactory.ReLoadAllColorTables();
                    LayoutTemplateHelper.UpdateLegend(colorTabels, null);
                    ProductColorTableParser.WriteToXml(colorTabelsBack, colorTableFile);
                }
                return ThemeGraphyResult(null);
            }
            else
            {
                return ThemeGraphyResult(null);
            }
        }
    }
}
