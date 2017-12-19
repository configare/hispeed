using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;

namespace GeoDo.RSS.MIF.Prds.VGT
{
    public static class WriteExpCoefficient
    {
        public static XmlNode _bootNode = null;
        public static XmlDocument _doc = null;

        public static bool SaveToFile(object expParas, string filename)
        {
            if (string.IsNullOrEmpty(filename) || expParas == null)
                return false;
            if (expParas is VGTExpCoefficientCollection)
                return SaveToFile(expParas as VGTExpCoefficientCollection, filename);
            return true;
        }

        public static bool SaveToFile(VGTExpCoefficientCollection expCollection, string filename)
        {
            string path = Path.GetDirectoryName(filename);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            if (File.Exists(filename))
                File.Delete(filename);
            try
            {
                _doc = new XmlDocument();
                CreateBootXML("ExpInfos");
                _bootNode = _doc.DocumentElement;
                WriteNode(_bootNode, "EdgesFile", expCollection.EgdesFilename);
                XmlNode expItemsNode = WriteNode(_bootNode, "ExpItems", "");
                CreateExpItems(expItemsNode, expCollection);
                _doc.Save(filename);
                return true;
            }
            finally
            {
                _doc = null;
                _bootNode = null;
            }
        }

        private static void CreateExpItems(XmlNode pNode, VGTExpCoefficientCollection expCollection)
        {
            WriteNode(pNode, "ExpItem", expCollection.Exps);
        }

        private static void WriteNode(XmlNode pNode, string nodeName, VGTExpCoefficientItem[] expItems)
        {
            if (expItems == null)
                return;
            foreach (VGTExpCoefficientItem expitem in expItems)
            {
                XmlNode subNode = WriteNode(pNode, nodeName, string.Empty);
                WriteAttribue(subNode, "name", expitem.Name);
                WriteAttribue(subNode, "num", expitem.Num.ToString());
                WriteAttribue(subNode, "A", expitem.APara.ToString());
                WriteAttribue(subNode, "B", expitem.BPara.ToString());
                WriteAttribue(subNode, "C", expitem.CPara.ToString());
            }
        }
        
        private static void CreateBootXML(string nodeName)
        {
            XmlNode node = _doc.CreateNode(XmlNodeType.XmlDeclaration, string.Empty, string.Empty);
            _doc.AppendChild(node);
            node = _doc.CreateNode(XmlNodeType.Element, nodeName, string.Empty);
            _doc.AppendChild(node);
        }

        private static XmlNode WriteNode(XmlNode pNode, string nodeName, string value)
        {
            XmlNode node = _doc.CreateNode(XmlNodeType.Element, nodeName, string.Empty);
            node.InnerText = value;
            pNode.AppendChild(node);
            return node;
        }

        private static void WriteAttribue(XmlNode node, string attributeName, string attributeValue)
        {
            XmlAttribute att = _doc.CreateAttribute(attributeName);
            att.Value = attributeValue;
            node.Attributes.Append(att);
        }
    }
}
