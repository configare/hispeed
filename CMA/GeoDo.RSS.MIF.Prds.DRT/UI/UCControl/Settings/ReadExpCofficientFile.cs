using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;

namespace GeoDo.RSS.MIF.Prds.DRT
{
    public static class ReadExpCofficientFile
    {
        public static DRTExpCoefficientCollection LoadExpCoefficientFile(string filename)
        {
            if (string.IsNullOrEmpty(filename) || !File.Exists(filename))
                return null;
            DRTExpCoefficientCollection expInfos = new DRTExpCoefficientCollection();
            XElement root = XElement.Load(filename);
            if (root == null)
                return null;
            XElement edgesRoot = ReadXElement(root, "EdgesFile");
            if (edgesRoot == null)
                return null;
            expInfos.EgdesFilename = string.IsNullOrEmpty(edgesRoot.Value) ? ReadAttributeValue(edgesRoot, "file") : edgesRoot.Value.ToString();
            XElement expRoot = ReadXElement(root, "ExpItems");
            if (expRoot == null)
                return null;
            expInfos.Exps = ReadDRTExpItem(expRoot, "ExpItem");
            return expInfos;
        }

        private static DRTExpCoefficientItem[] ReadDRTExpItem(XElement pNode, string nodeName)
        {
            XElement[] indeies = ReadXElements(pNode, nodeName);
            if (indeies == null)
                return null;
            var s = from item in pNode.Elements(nodeName)
                    select new DRTExpCoefficientItem()
                    {
                        Name = item.Attribute("name").Value,
                        Num = int.Parse((item.Attribute("num").Value ?? "0")),
                        APara = Double.Parse((item.Attribute("A").Value ?? "0")),
                        BPara = Double.Parse((item.Attribute("B").Value ?? "0")),
                        CPara = double.Parse((item.Attribute("C").Value ?? "0"))
                    };
            return s.ToArray();
        }

        public static TVDIParaClass LoadTVDIParasFile(string filename)
        {
            if (string.IsNullOrEmpty(filename) || !File.Exists(filename))
                return null;
            TVDIParaClass paras = new TVDIParaClass();
            XElement root = XElement.Load(filename);
            if (root == null)
                return null;
            XElement zoomRoot = ReadXElement(root, "Zoom");
            if (zoomRoot != null)
                paras.Zoom = int.Parse(zoomRoot.Value);
            XElement lengthRoot = ReadXElement(root, "Length");
            if (lengthRoot != null)
                paras.Length = int.Parse(lengthRoot.Value);
            XElement flimitRoot = ReadXElement(root, "Flimit");
            if (flimitRoot != null)
                paras.FLimit = float.Parse(flimitRoot.Value);
            XElement LstPcRoot = ReadXElement(root, "LSTFreq");
            if (LstPcRoot != null)
                paras.LstFreq = int.Parse(LstPcRoot.Value);
            XElement hgYZRoot = ReadXElement(root, "HGYZ");
            if (hgYZRoot != null)
                paras.HGYZ = float.Parse(hgYZRoot.Value);
            paras.NdviFile = ReadFileParaParas(root, "NDVI");
            paras.LstFile = ReadFileParaParas(root, "LST");
            return paras;
        }

        private static ArgumentItem ReadFileParaParas(XElement pNode, string nodeName)
        {
            XElement fileParaRoot = ReadXElement(pNode, nodeName);
            if (fileParaRoot == null)
                return null;
            ArgumentItem filePara = new ArgumentItem();
            string str = ReadAttributeValue(fileParaRoot, "max");
            if (!string.IsNullOrEmpty(str))
                filePara.Max = int.Parse(str);
            str = ReadAttributeValue(fileParaRoot, "min");
            if (!string.IsNullOrEmpty(str))
                filePara.Min = int.Parse(str);
            filePara.Band = GetIntXElementValue(fileParaRoot, "Band");
            filePara.Zoom = GetIntXElementValue(fileParaRoot, "Zoom");
            filePara.Cloudy = GetIntXElementValue(fileParaRoot, "Cloudy");
            filePara.Invaild = GetIntArrayXElementValue(fileParaRoot, "Invaild");
            return filePara;
        }

        private static int[] GetIntArrayXElementValue(XElement pNode, string nodeName)
        {
            XElement node = ReadXElement(pNode, nodeName);
            if (node == null)
                return null;
            if (string.IsNullOrEmpty(node.Value))
                return null;
            List<int> intList = new List<int>();
            string[] temps = node.Value.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string temp in temps)
                intList.Add(int.Parse(temp));
            return intList.Count == 0 ? null : intList.ToArray();
        }

        private static int GetIntXElementValue(XElement vtiRoot, string nodeName)
        {
            XElement node = ReadXElement(vtiRoot, nodeName);
            if (node == null)
                return 0;
            return int.Parse(node.Value);
        }

        private static string GetXElementValue(string nodeStr, XElement vtiRoot)
        {
            XElement dirRoot = ReadXElement(vtiRoot, nodeStr);
            if (dirRoot != null)
                return dirRoot.Value;
            return string.Empty;
        }

        private static XElement ReadXElement(XElement node, string nodeName)
        {
            var list = from ele in node.Elements(nodeName)
                       select ele;
            if (list == null || list.Count<XElement>() == 0)
                return null;
            return list.First<XElement>();
        }

        private static XElement[] ReadXElements(XElement node, string nodeName)
        {
            var list = from ele in node.Elements(nodeName)
                       select ele;
            if (list == null || list.Count<XElement>() == 0)
                return null;
            return list.ToArray<XElement>();
        }

        private static string ReadAttributeValue(XElement node, string attributeName)
        {
            if (node == null || string.IsNullOrEmpty(attributeName) || node.Attributes(attributeName).Count() == 0)
                return string.Empty;
            XAttribute att = node.Attributes(attributeName).First<XAttribute>();
            if (att == null)
                return string.Empty;
            return att.Value;
        }

    }
}
