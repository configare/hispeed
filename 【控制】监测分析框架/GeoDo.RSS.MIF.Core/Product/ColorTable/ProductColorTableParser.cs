using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Drawing;
using System.IO;

/*
 * 20140304修改ColorTable结构，产品颜色表不再定义进ColorTable.xml;
 * 拆分为独立的颜色表文件，放入SystemData\ColorTable\目录下
 */
namespace GeoDo.RSS.MIF.Core
{
    /// <summary>
    /// 
    /// </summary>
    public class ProductColorTableParser
    {
        private static string RootDir = System.AppDomain.CurrentDomain.BaseDirectory + @"SystemData\ColorTable\";

        public ProductColorTableParser()
        {
        }

        /// <summary>
        /// 加载系统目录下的所有产品颜色表
        /// </summary>
        public ProductColorTable[] Load()
        {
            string[] files = LoadColorTables();
            if (files.Length == 0)
                return null;
            List<ProductColorTable> tables = new List<ProductColorTable>();
            for (int i = 0; i < files.Length; i++)
            {
                ProductColorTable[] colorTables = Parse(files[i]);
                if (colorTables != null && colorTables.Length != 0)
                    tables.AddRange(colorTables);
            }
            return tables.ToArray();
        }

        public string[] LoadColorTables()
        {
            if (!Directory.Exists(RootDir))
                return null;
            string[] files = Directory.GetFiles(RootDir, "*.xml", SearchOption.AllDirectories);
            if (files.Length == 0)
                return null;
            return files;
        }

        public static string LoadProColorTable(string productIdentify)
        {
            if (!Directory.Exists(RootDir))
                return null;
            string[] files = Directory.GetFiles(RootDir, "*" + productIdentify + "*.xml", SearchOption.AllDirectories);
            if (files.Length == 0)
                return null;
            return files[0];
        }

        public static string Import(string filename, bool overwrite)
        {
            string dstFilename = Path.Combine(RootDir, Path.GetFileNameWithoutExtension(filename) + ".xml");
            File.Copy(filename, dstFilename, overwrite);
            return dstFilename;
        }

        public static ProductColorTable[] Parse(string colorTableFile)
        {
            try
            {
                XDocument doc = XDocument.Load(colorTableFile);
                if (doc == null)
                    return null;
                XElement root = doc.Root;
                XElement element = root.Element("ProductColorTables");
                if (element == null)
                    return null;
                IEnumerable<XElement> eles = element.Elements("ProductColorTable");
                if (eles == null || eles.Count() == 0)
                    return null;
                ProductColorTable[] tables = XmlToColorTables(eles);
                return tables;
            }
            catch (Exception ex)
            {
                CodeCell.Bricks.Runtime.Log.WriterError("解析颜色表错误：" + ex.Message + "。文件名：" + (string.IsNullOrWhiteSpace(colorTableFile) ? "空" : colorTableFile));
                return null;
            }
        }

        private static ProductColorTable[] XmlToColorTables(IEnumerable<XElement> eles)
        {
            List<ProductColorTable> tables = new List<ProductColorTable>();
            ProductColorTable table = null;
            foreach (XElement ele in eles)
            {
                table = XmlToColorTable(ele);
                if (table != null)
                    tables.Add(table);
            }
            return tables == null ? null : tables.ToArray();
        }

        private static ProductColorTable XmlToColorTable(XElement ele)
        {
            if (ele == null)
                return null;
            IEnumerable<XAttribute> atts = ele.Attributes();
            if (atts == null || atts.Count() == 0)
                return null;
            string identify = null;
            string subIdentify = null;
            string description = null;
            string colortablename = null;
            string labeltext = null;
            ProductColorTable table = null;
            foreach (XAttribute att in atts)
            {
                switch (att.Name.ToString().ToLower())
                {
                    case "identify":
                        if (!String.IsNullOrEmpty(att.Value))
                            identify = att.Value;
                        break;
                    case "subidentify":
                        if (!String.IsNullOrEmpty(att.Value))
                            subIdentify = att.Value;
                        break;
                    case "description":
                        if (!String.IsNullOrEmpty(att.Value))
                            description = att.Value;
                        break;
                    case "colortablename":
                        if (!String.IsNullOrEmpty(att.Value))
                            colortablename = att.Value;
                        break;
                    case "labeltext":
                        labeltext = att.Value;
                        break;
                }
            }
            if (identify == null || subIdentify == null)
                return null;
            table = new ProductColorTable(identify, subIdentify, colortablename, labeltext);
            table.Description = description;
            table.ProductColors = XmlToProductColors(ele);
            return table;
        }

        private static ProductColor[] XmlToProductColors(XElement ele)
        {
            if (ele.Element("Colors") == null)
                return null;
            IEnumerable<XElement> eles = ele.Element("Colors").Elements("Color");
            if (eles == null || eles.Count() == 0)
                return null;
            List<ProductColor> colors = new List<ProductColor>();
            ProductColor color = null;
            foreach (XElement element in eles)
            {
                color = XmlToProductColor(element);
                if (color != null)
                    colors.Add(color);
            }
            return colors == null ? null : colors.ToArray();
        }

        private static ProductColor XmlToProductColor(XElement ele)
        {
            if (ele == null)
                return null;
            ProductColor color = new ProductColor();
            IEnumerable<XAttribute> atts = ele.Attributes();
            foreach (XAttribute att in atts)
            {
                switch (att.Name.ToString().ToLower())
                {
                    case "color":
                        if (!String.IsNullOrEmpty(att.Value))
                            color.Color = StringToColor(att.Value);
                        break;
                    case "maxvalue":
                        if (!String.IsNullOrEmpty(att.Value))
                            color.MaxValue = float.Parse(att.Value);
                        else
                            color.MaxValue = float.MaxValue;
                        break;
                    case "minvalue":
                        if (!String.IsNullOrEmpty(att.Value))
                            color.MinValue = float.Parse(att.Value);
                        else
                            color.MinValue = float.MinValue;
                        break;
                    case "labeltext":
                        color.LableText = string.IsNullOrWhiteSpace(att.Value) ? "" : att.Value;
                        break;
                    case "displaylengend":
                        color.DisplayLengend = att.Value != null && att.Value.ToUpper() == "TRUE";
                        break;
                }
            }
            return color;
        }

        private static Color StringToColor(string p)
        {
            string[] colorStrings = p.Split(',');
            if (colorStrings == null)
                return Color.Empty;
            int a, r, g, b;
            if (colorStrings.Length == 4)
            {
                a = int.Parse(colorStrings[0]);
                r = int.Parse(colorStrings[1]);
                g = int.Parse(colorStrings[2]);
                b = int.Parse(colorStrings[3]);
                return Color.FromArgb(a, r, g, b);
            }
            else if (colorStrings.Length == 3)
            {
                r = int.Parse(colorStrings[0]);
                g = int.Parse(colorStrings[1]);
                b = int.Parse(colorStrings[2]);
                return Color.FromArgb(r, g, b);
            }
            else
                return Color.Empty;
        }

        public static void WriteToXml(ProductColorTable[] colorTables, string filename)
        {
            XElement[] subEle = new XElement[colorTables.Length];
            for (int i = 0; i < colorTables.Length; i++)
            {
                subEle[i] = ToXElement(colorTables[i]);
            }
            XElement ele = new XElement("ColorTable",
                new XElement("ProductColorTables",
                    subEle
                    ));
            ele.Save(filename);
        }

        private static XElement ToXElement(ProductColorTable colorTable)
        {
            if (colorTable == null)
                return null;
            XElement[] eleColors = ColorsToXElements(colorTable.ProductColors);
            XElement ele = new XElement("ProductColorTable",
                new XAttribute("identify", colorTable.Identify),
                new XAttribute("subIdentify", colorTable.SubIdentify),
                new XAttribute("colortablename", colorTable.ColorTableName),
                new XAttribute("description", colorTable.Description),
                new XAttribute("labeltext", colorTable.LabelText),
                new XElement("Colors", eleColors)
                );
            return ele;
        }

        private static XElement[] ColorsToXElements(ProductColor[] productColors)
        {
            if (productColors == null)
                return null;
            XElement[] eleColors = new XElement[productColors.Length];
            for (int j = 0; j < productColors.Length; j++)
            {
                eleColors[j] = new XElement("Color",
                new XAttribute("color", string.Format("{0},{1},{2}", productColors[j].Color.R, productColors[j].Color.G, productColors[j].Color.B)),
                new XAttribute("minValue", productColors[j].MinValue),
                new XAttribute("maxValue", productColors[j].MaxValue),
                new XAttribute("labeltext", productColors[j].LableText),
                new XAttribute("displaylengend", productColors[j].DisplayLengend)
                    );
            }
            return eleColors;
        }
    }
}
