using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;

namespace GeoDo.RSS.MIF.Prds.DRT
{
    public static class WriteExpCoefficient
    {
        public static XmlNode _bootNode = null;
        public static XmlDocument _doc = null;

        public static bool SaveToFile(object droughtParas, string filename)
        {
            if (string.IsNullOrEmpty(filename) || droughtParas == null)
                return false;
            if (droughtParas is DRTExpCoefficientCollection)
                return SaveToFile(droughtParas as DRTExpCoefficientCollection, filename);
            if (droughtParas is TVDIParaClass)
                return SaveToFile(droughtParas as TVDIParaClass, filename);
            return true;
        }

        public static bool SaveToFile(DRTExpCoefficientCollection expCollection, string filename)
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

        private static void CreateExpItems(XmlNode pNode, DRTExpCoefficientCollection expCollection)
        {
            WriteNode(pNode, "ExpItem", expCollection.Exps);
        }

        private static void WriteNode(XmlNode pNode, string nodeName, DRTExpCoefficientItem[] expItems)
        {
            if (expItems == null)
                return;
            foreach (DRTExpCoefficientItem expitem in expItems)
            {
                XmlNode subNode = WriteNode(pNode, nodeName, string.Empty);
                WriteAttribue(subNode, "name", expitem.Name);
                WriteAttribue(subNode, "num", expitem.Num.ToString());
                WriteAttribue(subNode, "A", expitem.APara.ToString());
                WriteAttribue(subNode, "B", expitem.BPara.ToString());
                WriteAttribue(subNode, "C", expitem.CPara.ToString());
            }
        }

        public static bool SaveToFile(TVDIParaClass tvdiParas, string filename)
        {
            string path = Path.GetDirectoryName(filename);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            if (File.Exists(filename))
                File.Delete(filename);
            try
            {
                _doc = new XmlDocument();
                CreateBootXML("TVDI");
                _bootNode = _doc.DocumentElement;
                CreateTVDIFile(tvdiParas);
                _doc.Save(filename);
                return true;
            }
            finally
            {
                _doc = null;
                _bootNode = null;
            }
        }

        private static void CreateTVDIFile(TVDIParaClass tvdiParas)
        {
            WriteNode(_bootNode, "Length", tvdiParas.Length.ToString());
            WriteNode(_bootNode, "Flimit", tvdiParas.FLimit.ToString());
            WriteNode(_bootNode, "HGYZ", tvdiParas.HGYZ.ToString());
            WriteNode(_bootNode, "LSTFreq", tvdiParas.LstFreq.ToString());
            WriteNode(_bootNode, "Zoom", tvdiParas.Zoom.ToString());
            WriteArgumentItem(_bootNode, "LST", tvdiParas.LstFile);
            WriteArgumentItem(_bootNode, "NDVI", tvdiParas.NdviFile);
        }

        private static void WriteArgumentItem(XmlNode pNode, string nodeName, ArgumentItem filePara)
        {
            if (filePara == null)
                return;
            XmlNode fileParaRoot = WriteNode(pNode, nodeName, string.Empty);
            WriteAttribue(fileParaRoot, "max", filePara.Max.ToString());
            WriteAttribue(fileParaRoot, "min", filePara.Min.ToString());
            WriteNode(fileParaRoot, "Band", filePara.Band.ToString());
            WriteNode(fileParaRoot, "Zoom", filePara.Zoom.ToString());
            WriteNode(fileParaRoot, "Invaild", filePara.Invaild);
            WriteNode(fileParaRoot, "Cloudy", filePara.Cloudy.ToString());
        }

        private static void WriteNode(XmlNode pNode, string nodeName, int[] invaild)
        {
            string invaildStr = string.Empty;
            if (invaild == null)
                return;
            foreach (int i in invaild)
                invaildStr += i.ToString() + ";";
            WriteNode(pNode, "Invaild", invaildStr);
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
