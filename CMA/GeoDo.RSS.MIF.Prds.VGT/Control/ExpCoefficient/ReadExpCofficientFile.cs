using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;

namespace GeoDo.RSS.MIF.Prds.VGT
{
    public static class ReadExpCofficientFile
    {
        internal static VGTExpCoefficientCollection LoadExpCoefficientFile(string filename)
        {
            if (string.IsNullOrEmpty(filename) || !File.Exists(filename))
                return null;
            VGTExpCoefficientCollection expInfos = new VGTExpCoefficientCollection();
            XElement root = XElement.Load(filename);
            if (root == null)
                return null;
            XElement edgesRoot = ReadXElement(root, "EdgesFile");
            if (edgesRoot == null)
                return null;
            expInfos.EgdesFilename = ReadAttributeValue(edgesRoot, "file");
            XElement expRoot = ReadXElement(root, "ExpItems");
            if (expRoot == null)
                return null;
            expInfos.Exps = ReadVGTExpItem(expRoot, "ExpItem");
            return expInfos;
        }

        private static VGTExpCoefficientItem[] ReadVGTExpItem(XElement pNode, string nodeName)
        {
            XElement[] indeies = ReadXElements(pNode, nodeName);
            if (indeies == null)
                return null;
            var s = from item in pNode.Elements(nodeName)
                    select new VGTExpCoefficientItem()
                    {
                        Name = item.Attribute("name").Value,
                        Num = int.Parse((item.Attribute("num").Value ?? "0")),
                        APara = Double.Parse((item.Attribute("A").Value ?? "0")),
                        BPara = Double.Parse((item.Attribute("B").Value ?? "0")),
                        CPara = double.Parse((item.Attribute("C").Value?? "0"))
                    };
            return s.ToArray();
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
