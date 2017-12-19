using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using GeoDo.RSS.Layout;
using GeoDo.RSS.Layout.Elements;
using GeoDo.RSS.Layout.GDIPlus;
using GeoDo.RSS.MIF.Core;

namespace GeoDo.RSS.UI.AddIn.Tools
{
    public class LayoutTemplateHelper
    {
        public static void UpdateLegend(ProductColorTable[] colors, Action<int, string> percentProgress)
        {
            string path = System.AppDomain.CurrentDomain.BaseDirectory + "LayoutTemplate";
            if (!Directory.Exists(path))
                return;
            string[] files = Directory.GetFiles(path, "*.gxt", SearchOption.AllDirectories);
            if (files == null || files.Length == 0)
                return;
            for (int i = 0; i < files.Length; i++)
            {
                string fname = files[i];
                ILayoutTemplate template = LayoutTemplate.LoadTemplateFrom(fname);

                //LayoutTemplate.LoadTemplateFrom(fname);
                if (template == null)
                    return;
                if (percentProgress != null)
                    percentProgress((int)((i + 1) * 100f / files.Length), "");
                ILayout layout = template.Layout;
                bool haUpdate = false;
                for (int e = 0; e < layout.Elements.Count; e++)
                {
                    if (layout.Elements[e] is ILegendElement)
                    {
                        ILegendElement legendEle = layout.Elements[e] as ILegendElement;
                        string colotName = legendEle.ColorTableName;
                        if (!string.IsNullOrWhiteSpace(colotName))
                        {
                            ProductColorTable colorTable = ProductColorTableFactory.GetColorTable(colotName);
                            if (colorTable != null)
                            {
                                List<LegendItem> items = new List<LegendItem>();
                                foreach (ProductColor pc in colorTable.ProductColors)
                                {
                                    if (!pc.DisplayLengend)
                                        continue;
                                    LegendItem item = new LegendItem(pc.LableText, pc.Color);
                                    items.Add(item);
                                }
                                legendEle.LegendItems = items.ToArray();
                                haUpdate = true;
                            }
                        }
                    }
                }
                if (haUpdate)
                {
                    template.SaveTo(fname);
                }
            }
        }
    }
}
