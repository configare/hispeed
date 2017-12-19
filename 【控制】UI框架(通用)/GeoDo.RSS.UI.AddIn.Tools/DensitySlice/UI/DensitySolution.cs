using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Linq;
using GeoDo.RSS.Core.RasterDrawing;
using System.Drawing;

namespace GeoDo.RSS.UI.AddIn.Tools
{
    public class DensitySolution
    {
        public static DensityDef Read(string filename)
        {
            if (string.IsNullOrEmpty(filename) || !File.Exists(filename))
                return null;
            XElement node = XElement.Load(filename);
            if (node == null)
                return null;
            node = node.Element("DensitySlice");
            if (node == null)
                return null;
            DensityDef densityDef = new DensityDef();
            ReadBaseInfo(ref densityDef, node);
            if (densityDef == null)
                return null;
            ReadRanges(ref densityDef, node);
            return densityDef;
        }

        private static void ReadRanges(ref DensityDef densityDef, XElement node)
        {
            XElement rangeNode = node.Element("RANGE");
            if (rangeNode == null)
                return;
            IEnumerable<XElement> rangeSubNodes = rangeNode.Elements();
            if (rangeSubNodes == null || rangeSubNodes.Count() == 0)
                return;
            List<DensityRange> ranges = new List<DensityRange>();
            XElement tempNode = null;
            foreach (XElement item in rangeSubNodes)
            {
                DensityRange temprange = new DensityRange(0, 0, 0, 0, 0);
                tempNode = item.Element("L");
                if (tempNode == null)
                    continue;
                temprange.minValue = float.Parse(tempNode.Value);
                tempNode = item.Element("R");
                if (tempNode == null)
                    continue;
                temprange.maxValue = float.Parse(tempNode.Value);
                tempNode = item.Element("RGB_R");
                if (tempNode == null)
                    continue;
                temprange.RGB_r = byte.Parse(tempNode.Value);
                tempNode = item.Element("RGB_G");
                if (tempNode == null)
                    continue;
                temprange.RGB_g = byte.Parse(tempNode.Value);
                tempNode = item.Element("RGB_B");
                if (tempNode == null)
                    continue;
                temprange.RGB_b = byte.Parse(tempNode.Value);
                ranges.Add(temprange);
            }
            densityDef.Ranges = ranges.Count == 0 ? null : ranges.ToArray();
        }

        private static void ReadBaseInfo(ref DensityDef densityDef, XElement node)
        {
            float max = float.MaxValue;
            if (float.TryParse(node.Element("MAX").Value, out max))
                densityDef.MaxValue = max;
            float min = float.MinValue;
            if (float.TryParse(node.Element("MIN").Value, out min))
                densityDef.MinValue = min;
            int rangeCount = 0;
            if (int.TryParse(node.Element("NUM").Value, out rangeCount))
                densityDef.RangeCount = rangeCount;
            float interval = 0;
            if (float.TryParse(node.Element("JIANGE").Value, out interval))
                densityDef.Interval = interval;
            bool applayInterval = false;
            if (bool.TryParse(node.Element("DoJIANGE").Value, out applayInterval))
                densityDef.ApplayInterval = applayInterval;
        }

        public static bool Save(string filename, DensityDef densityDef)
        {
            if (string.IsNullOrEmpty(filename))
                return false;
            string dir = Path.GetDirectoryName(filename);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            if (densityDef == null || densityDef.Ranges == null)
                return false;
            XElement root = new XElement("ImageProcessQueue");
            root.SetElementValue("DensitySlice", "");
            if (!SetBaseInfo(root.Element("DensitySlice"), densityDef))
                return false;
            if (!SetRangeInfo(root.Element("DensitySlice"), densityDef))
                return false;
            root.Save(filename);
            return true;
        }

        private static bool SetRangeInfo(XElement pNode, DensityDef densityDef)
        {
            if (densityDef.Ranges == null || densityDef.Ranges.Length == 0)
                return false;
            pNode.SetElementValue("RANGE", "");
            XElement rangeRoot = pNode.Element("RANGE");
            string tempNodename;
            int length = densityDef.Ranges.Length;
            for (int i = 0; i < length; i++)
            {
                tempNodename = "R" + i.ToString() + "_RANGE";
                rangeRoot.SetElementValue(tempNodename, "");
                SetSubRangeInfo(rangeRoot.Element(tempNodename), densityDef.Ranges[i]);
            }
            return true;
        }

        private static void SetSubRangeInfo(XElement pNode, DensityRange densityRange)
        {
            pNode.SetElementValue("L", densityRange.minValue.ToString());
            pNode.SetElementValue("R", densityRange.maxValue.ToString());
            pNode.SetElementValue("RGB_R", densityRange.RGB_r.ToString());
            pNode.SetElementValue("RGB_G", densityRange.RGB_g.ToString());
            pNode.SetElementValue("RGB_B", densityRange.RGB_b.ToString());
        }

        private static bool SetBaseInfo(XElement pNode, DensityDef densityDef)
        {
            pNode.SetElementValue("MAX", densityDef.MaxValue.ToString());
            pNode.SetElementValue("MIN", densityDef.MinValue.ToString());
            pNode.SetElementValue("NUM", densityDef.RangeCount.ToString());
            pNode.SetElementValue("JIANGE", densityDef.Interval.ToString());
            pNode.SetElementValue("DoJIANGE", densityDef.ApplayInterval.ToString());
            return true;
        }

        public static ColorMapTable<double> GetColorMapTable(string filename)
        {
            if (string.IsNullOrEmpty(filename) || !File.Exists(filename))
                return null;
            DensityDef density = Read(filename);
            if (density.Ranges == null || density.Ranges.Length == 0)
                return null;
            ColorMapTable<double> mapColor = new ColorMapTable<double>();
            int count = density.Ranges.Length;
            if (count > 1)
                for (int i = 0; i < count - 1; i++)
                {
                    mapColor.Items.Add(new ColorMapItem<double>(density.Ranges[i].minValue, density.Ranges[i].maxValue,
                        Color.FromArgb(255, density.Ranges[i].RGB_r, density.Ranges[i].RGB_g, density.Ranges[i].RGB_b)));
                }
            int lastItemIndex = count - 1;
            mapColor.Items.Add(new ColorMapItem<double>(density.Ranges[lastItemIndex].minValue, density.Ranges[lastItemIndex].maxValue + 1,
                       Color.FromArgb(255, density.Ranges[lastItemIndex].RGB_r, density.Ranges[lastItemIndex].RGB_g, density.Ranges[lastItemIndex].RGB_b)));

            return mapColor.Items == null || mapColor.Items.Count == 0 ? null : mapColor;
        }

        public static ColorMapTable<double> GetColorMapTable(DensityRange[] ranges)
        {
            if (ranges == null || ranges.Length == 0)
                return null;
            ColorMapTable<double> mapColor = new ColorMapTable<double>();
            int count = ranges.Length;
            if (count > 1)
                for (int i = 0; i < count - 1; i++)
                {
                    mapColor.Items.Add(new ColorMapItem<double>(ranges[i].minValue, ranges[i].maxValue,
                        Color.FromArgb(255, ranges[i].RGB_r, ranges[i].RGB_g, ranges[i].RGB_b)));
                }
            int lastItemIndex = count - 1;
            mapColor.Items.Add(new ColorMapItem<double>(ranges[lastItemIndex].minValue, ranges[lastItemIndex].maxValue + 1,
                       Color.FromArgb(255, ranges[lastItemIndex].RGB_r, ranges[lastItemIndex].RGB_g, ranges[lastItemIndex].RGB_b)));
            return mapColor.Items == null || mapColor.Items.Count == 0 ? null : mapColor;
        }
    }
}
