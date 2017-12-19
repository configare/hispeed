using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Drawing;

namespace GeoDo.RSS.UI.AddIn.L2ColorTable
{
    public class L2ColorTableParser : IL2ColorTableParser,IDisposable
    {
        private XmlNode _rootNode = null;
        private XmlDocument _doc = null;
        //
        protected string _name = null;
        protected string[] _applyfor = null;
        protected string _description = null;

        public L2ColorTableParser(string filename)
        {
            if (string.IsNullOrEmpty(filename) || !File.Exists(filename))
                throw new FileNotFoundException(filename);
            _doc = new XmlDocument();
            _doc.Load(filename);
            _rootNode = _doc.DocumentElement;
            _name = NodeHandler.NodeAtt2String(_rootNode, "name");
            string applyfor = NodeHandler.NodeAtt2String(_rootNode, "applyfor");
            _applyfor  = applyfor.Split(',');
            _description = NodeHandler.NodeAtt2String(_rootNode, "description");
        }

        public string Name
        {
            get { return _name; }
        }

        public string[] ApplyFor
        {
            get { return _applyfor; }
        }

        public string Description
        {
            get { return _description; }
        }

        public string GetFileInnerText()
        {
            return _rootNode.InnerXml;
        }

        public string GetMatchString()
        {
            return NodeHandler.NodeAtt2String(_rootNode, "matchstring");
        }

        public string GetAllIndexXml()
        {
            XmlNode node = _rootNode.SelectSingleNode("BandValueRanges");
            return node != null ? node.InnerXml : string.Empty;
        }

        public string GetAllColorTable()
        {
            XmlNode node = _rootNode.SelectSingleNode("ColorTables");
            return node != null ? node.InnerXml : string.Empty;
        }

        public string[] GetDatasetNames()
        {
            XmlNode node = _rootNode.SelectSingleNode("BandValueRanges");
            if (node == null || node.ChildNodes == null || node.ChildNodes.Count == 0)
                return null;
            List<string> datasets = new List<string>();
            foreach (XmlNode chNode in node.ChildNodes)
            {
                string dsstring =   NodeHandler.NodeAtt2String(chNode, "datasets");
                if (string.IsNullOrEmpty(dsstring))
                    continue;
                string[] ds = dsstring.Split(',');
                foreach (string dsss in ds)
                {
                    string[] dsssss = dsss.Split(':');
                    if(!datasets.Contains(dsssss[0]))
                        datasets.Add(dsssss[0]);
                }
            }
            return datasets.Count>0? datasets.ToArray():null ;
        }

        public string[] GetBandValueRanges()
        {
            XmlNode node = _rootNode.SelectSingleNode("BandValueRanges");
            if (node == null || node.ChildNodes == null || node.ChildNodes.Count == 0)
                return null;
            string[] retNames  = new string[node.ChildNodes.Count];
            int i = 0;
            foreach (XmlNode chNode in node.ChildNodes)
            {
                retNames[i++] = NodeHandler.NodeAtt2String(chNode, "name");
            }
            return retNames;
        }

        public string[] GetBandValueRanges(out string[] tips)
        {
            tips = null;
            XmlNode node = _rootNode.SelectSingleNode("BandValueRanges");
            if (node == null || node.ChildNodes == null || node.ChildNodes.Count == 0)
                return null;
            string[] retNames = new string[node.ChildNodes.Count];
            int i = 0;
            tips = new string[node.ChildNodes.Count];
            foreach (XmlNode chNode in node.ChildNodes)
            {
                retNames[i] = NodeHandler.NodeAtt2String(chNode, "name");
                tips[i] = retNames[i] + ":" +
                    NodeHandler.NodeAtt2String(chNode, "datasets");
                i++;
            }
            return retNames;
        }

        public string GetBandValueTips(string name)
        {
            XmlNode node = _rootNode.SelectSingleNode("BandValueRanges");
            if (node == null || node.ChildNodes == null || node.ChildNodes.Count == 0)
                return null;
            string[] retNames = new string[node.ChildNodes.Count];
            int i = 0;
            foreach (XmlNode chNode in node.ChildNodes)
            {
                if (name.ToUpper() == NodeHandler.NodeAtt2String(chNode, "name").ToUpper())
                {
                    return name + ":" + NodeHandler.NodeAtt2String(chNode, "datasets");
                }
            }
            return string.Empty;
        }

        public string[] GetColorTables()
        {
            XmlNode node = _rootNode.SelectSingleNode("ColorTables");
            if (node == null || node.ChildNodes == null || node.ChildNodes.Count == 0)
                return null;
            string[] retNames = new string[node.ChildNodes.Count];
            int i = 0;
            foreach (XmlNode chNode in node.ChildNodes)
            {
                retNames[i++] = NodeHandler.NodeAtt2String(chNode, "name");
            }
            return retNames;
        }

        public string[] GetColorTables(out string[] tips)
        {
            tips = null;
            XmlNode node = _rootNode.SelectSingleNode("ColorTables");
            if (node == null || node.ChildNodes == null || node.ChildNodes.Count == 0)
                return null;
            string[] retNames = new string[node.ChildNodes.Count];
            int i = 0;
            tips = new string[node.ChildNodes.Count];
            foreach (XmlNode chNode in node.ChildNodes)
            {
                retNames[i] = NodeHandler.NodeAtt2String(chNode, "name");
                tips[i] = retNames[i] + ":" +
                      NodeHandler.NodeAtt2String(chNode, "applyfor");
                i++;
            }
            return retNames;
        }

        public string GetColorTableTips(string name)
        {
            XmlNode node = _rootNode.SelectSingleNode("ColorTables");
            if (node == null || node.ChildNodes == null || node.ChildNodes.Count == 0)
                return null;
            string[] retNames = new string[node.ChildNodes.Count];
            int i = 0;
            foreach (XmlNode chNode in node.ChildNodes)
            {
                if (name.ToUpper() == NodeHandler.NodeAtt2String(chNode, "name").ToUpper())
                {
                    return name + ":" + NodeHandler.NodeAtt2String(chNode, "applyfor");
                }
            }
            return string.Empty;
        }

        public string GetBandValueRangeInnerText(string name)
        {
            XmlNode node = _rootNode.SelectSingleNode("BandValueRanges");
            if (node == null || node.ChildNodes == null || node.ChildNodes.Count == 0)
                return null;
            foreach (XmlNode chNode in node.ChildNodes)
            {
                string ds = NodeHandler.NodeAtt2String(chNode, "name");
                if (ds.ToUpper() == name.ToUpper())
                    return chNode.InnerXml;
            }
            return string.Empty;
        }

        public string GetColorTableInnerText(string name)
        {
            XmlNode node = _rootNode.SelectSingleNode("ColorTables");
            if (node == null || node.ChildNodes == null || node.ChildNodes.Count == 0)
                return null;
            foreach (XmlNode chNode in node.ChildNodes)
            {
                string ds = NodeHandler.NodeAtt2String(chNode, "name");
                if (ds.ToUpper() == name.ToUpper())
                    return chNode.InnerXml;
            }
            return string.Empty;
        }

        public BandValueColorPair[] GetBandValueColorPairByIndexSpacename(string indexSpacename)
        {
            XmlNode node = _rootNode.SelectSingleNode("BandValueRanges");
            if (node == null || node.ChildNodes == null || node.ChildNodes.Count == 0)
                return null;
            foreach (XmlNode chNode in node.ChildNodes)
            {
                if (indexSpacename.ToUpper() == NodeHandler.NodeAtt2String(chNode, "name").ToUpper())
                {
                    string ds = NodeHandler.NodeAtt2String(chNode, "datasets");
                    string[] parts = ds.Split(',');
                    return GetBandValueColorPair(parts[0].Split(':')[0]);
                }
            }
            return null;
        }

        public BandValueColorPair[] GetBandValueTransColorPair(string datasetName)
        {
            XmlNode node = GetBandValueNode(datasetName);
            if (node == null || node.ChildNodes == null || node.ChildNodes.Count == 0)
                return null;
            string name = NodeHandler.NodeAtt2String(node, "name");
            List<BandValueIndexPair> pairs = new List<BandValueIndexPair>();
            L2BandValueIndexPairParser p = new L2BandValueIndexPairParser();
            foreach (XmlNode chNode in node.ChildNodes)
            {
                string v = NodeHandler.NodeAtt2String(chNode, "values");
                string idx = NodeHandler.NodeAtt2String(chNode, "indexes");
                Int16 tV = (Int16)NodeHandler.NodeAtt2Int(chNode, "transValue");
                Color transRGB = NodeHandler.NodeAtt2Color(chNode, "transRGB");
                string description = NodeHandler.NodeAtt2String(chNode, "description");    
                bool isDisplay = false;
                Boolean.TryParse(NodeHandler.NodeAtt2String(chNode, "Display"), out isDisplay);
                BandValueIndexPair[] ps = p.Parse(v, idx, tV, isDisplay, transRGB, description);
                if (ps != null && ps.Length > 0)
                    pairs.AddRange(ps);
            }

            return GetBandValueTransColorPair(pairs.ToArray(), name);
        }

        private BandValueColorPair[] GetBandValueTransColorPair(BandValueIndexPair[] bandValueIndexPairs, string name)
        {
            if (bandValueIndexPairs == null || bandValueIndexPairs.Length == 0)
                return null;
            XmlNode node = GetColorTableNode(name);
            if (node == null || node.ChildNodes == null || node.ChildNodes.Count == 0)
                return null;
            List<BandValueColorPair> pairs = new List<BandValueColorPair>();
            foreach (XmlNode chNode in node.ChildNodes)
            {
                int idx = NodeHandler.NodeAtt2Int(chNode, "index");
                Color rgb = NodeHandler.NodeAtt2Color(chNode, "rgb");
                //bool isDisplay = false;
                //Boolean.TryParse(NodeHandler.NodeAtt2String(chNode, "display"), out isDisplay);
                //Int16 transValue = (Int16)NodeHandler.NodeAtt2Int(chNode, "transValue");
                BandValueIndexPair vpair = null;
                foreach (BandValueIndexPair p in bandValueIndexPairs)
                {
                    if (p.Index == idx)
                    {
                        vpair = p;
                        break;
                    }
                }
                if (vpair != null)
                {
                    pairs.Add(new BandValueColorPair(vpair.MinValue, vpair.MaxValue, rgb, vpair.IsDisplay, vpair.TransValue, vpair.TransRGB, vpair.Description));
                }
            }
            return pairs.Count > 0 ? pairs.ToArray() : null;
        }

        public BandValueColorPair[] GetBandValueColorPair(string datasetName)
        {
            XmlNode node = GetBandValueNode(datasetName);
            if (node == null || node.ChildNodes == null || node.ChildNodes.Count == 0)
                return null;
            string name = NodeHandler.NodeAtt2String(node, "name");
            List<BandValueIndexPair> pairs = new List<BandValueIndexPair>();
            L2BandValueIndexPairParser p = new L2BandValueIndexPairParser();
            foreach (XmlNode chNode in node.ChildNodes)
            {
                string v = NodeHandler.NodeAtt2String(chNode, "values");
                string idx = NodeHandler.NodeAtt2String(chNode, "indexes");
                BandValueIndexPair[] ps = p.Parse(v, idx);
                if (ps != null && ps.Length > 0)
                    pairs.AddRange(ps);
            }
            return GetBandValueColorPair(pairs.ToArray(), name);
        }

        private BandValueColorPair[] GetBandValueColorPair(BandValueIndexPair[] bandValueIndexPairs,string name)
        {
            if (bandValueIndexPairs == null || bandValueIndexPairs.Length == 0)
                return null;
            XmlNode node = GetColorTableNode(name);
            if (node == null || node.ChildNodes == null || node.ChildNodes.Count == 0)
                return null;
            List<BandValueColorPair> pairs = new List<BandValueColorPair>();
            foreach (XmlNode chNode in node.ChildNodes)
            {
                int idx = NodeHandler.NodeAtt2Int(chNode, "index");
                Color rgb = NodeHandler.NodeAtt2Color(chNode, "rgb");
                BandValueIndexPair vpair = null;
                foreach (BandValueIndexPair p in bandValueIndexPairs)
                {
                    if (p.Index == idx)
                    {
                        vpair = p;
                        break;
                    }
                }
                if (vpair != null)
                {
                    pairs.Add(new BandValueColorPair(vpair.MinValue, vpair.MaxValue, rgb));
                }
            }
            return pairs.Count > 0 ? pairs.ToArray() : null;
        }

        private XmlNode GetColorTableNode(string name)
        {
            XmlNode node = _rootNode.SelectSingleNode("ColorTables");
            if (node == null || node.ChildNodes == null || node.ChildNodes.Count == 0)
                return null;
            foreach (XmlNode chNode in node.ChildNodes)
            {
                string ds = NodeHandler.NodeAtt2String(chNode, "applyfor");
                string[] parts = ds.Split(',');
                foreach (string p in parts)
                {
                    if (p.ToUpper().Contains(name.ToUpper()))
                        return chNode;
                }
            }
            return null;
        }

        private XmlNode GetBandValueNode(string datasetName)
        {
            XmlNode node = _rootNode.SelectSingleNode("BandValueRanges");
            if (node == null || node.ChildNodes == null || node.ChildNodes.Count == 0)
                return null;
            foreach (XmlNode chNode in node.ChildNodes)
            {
                string ds = NodeHandler.NodeAtt2String(chNode, "datasets");
                string[] parts = ds.Split(',');
                foreach (string p in parts)
                {
                    if (p.ToUpper().Contains(datasetName.ToUpper()))
                        return chNode;
                }
            }
            return null;
        }

        #region IDisposable 成员

        public void Dispose()
        {
            _rootNode = null;
            _doc = null;
        }

        #endregion

        public BandValueColorPair[] GetColorsByColorTableName(string colorTable)
        {
            XmlNode node = _rootNode.SelectSingleNode("ColorTables");
            if (node == null || node.ChildNodes == null || node.ChildNodes.Count == 0)
                return null;
            XmlNode colorTableNode = null;
            foreach (XmlNode chNode in node.ChildNodes)
            {
                if (colorTable.ToUpper() == NodeHandler.NodeAtt2String(chNode, "name").ToUpper())
                {
                    colorTableNode = chNode;
                    break;
                }
            }
            if (colorTableNode == null)
                return null;
           List<BandValueColorPair> pairs = new List<BandValueColorPair>();
           foreach (XmlNode chNode in colorTableNode.ChildNodes)
           {
               int idx = NodeHandler.NodeAtt2Int(chNode, "index");
               Color rgb = NodeHandler.NodeAtt2Color(chNode, "rgb");
               pairs.Add(new BandValueColorPair(idx, idx, rgb));
           }
            return pairs.ToArray();
        }
    }
}
