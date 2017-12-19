using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Remoting;
using System.Reflection;
using System.Xml.Linq;
using System.Xml;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Diagnostics;
using System.Drawing;

namespace GeoDo.RSS.Layout
{
    public static class LayoutToFile
    {
        public static Func<string, IDataFrame, XmlDocument, XmlNode> DataFrame2XmlNodeConverter;

        public static XmlDocument SaveToXmlDocument(ILayoutTemplate template, ILayout layout)
        {
            XmlDocument doc = new XmlDocument();
            XmlNode node = GetRootElement(template, doc, layout);
            if (node == null)
                return null;
            doc.AppendChild(node);
            return doc;
        }

        public static XmlDocument SaveToXmlDocument(ILayout layout)
        {
            XmlDocument doc = new XmlDocument();
            XmlNode node = GetRootElement(null, doc, layout);
            if (node == null)
                return null;
            doc.AppendChild(node);
            return doc;
        }

        public static void SaveToFile(string fname, ILayout layout)
        {
            XmlDocument doc = new XmlDocument();
            XmlNode node = GetRootElement(null, doc, layout);
            if (node == null)
                return;
            //
            if (DataFrame2XmlNodeConverter != null)
            {
                //vector host
                XmlNode vectorNode = GetVectorMcdFileNode(fname, doc, layout);
                if (vectorNode != null && !string.IsNullOrWhiteSpace(vectorNode.InnerXml))
                    node.AppendChild(vectorNode);
                else
                {
                    XmlNode dfEle = GetDataFrames(doc, layout);
                    node.AppendChild(dfEle);
                }

            }
            doc.AppendChild(node);
            //
            doc.Save(fname);
        }

        private static XmlNode GetDataFrames(XmlDocument doc, ILayout layout)
        {
            IElement[] eles = layout.QueryElements((e) => { return e is IDataFrame; });
            if (eles == null || eles.Length == 0)
                return null;
            XmlElement dfNodes = doc.CreateElement("DataFrames");
            StringBuilder strBuilder = new StringBuilder();
            foreach (IDataFrame df in eles)
            {
                XElement dfData = df.Data as XElement;
                if (dfData != null && !dfData.IsEmpty)
                {
                    strBuilder.AppendLine(dfData.ToString());
                }
            }
            if (strBuilder.Length != 0)
                dfNodes.InnerXml = strBuilder.ToString();
            return dfNodes;
        }

        private static XmlNode GetVectorMcdFileNode(string dstFileName, XmlDocument doc, ILayout layout)
        {
            IElement[] eles = layout.QueryElements((e) => { return e is IDataFrame; });
            if (eles == null || eles.Length == 0)
                return null;
            XmlNode dfNodes = doc.CreateElement("DataFrames");
            foreach (IDataFrame df in eles)
            {
                XmlNode dfNd = DataFrame2XmlNodeConverter(dstFileName, df, doc);
                if (dfNd != null)
                    dfNodes.AppendChild(dfNd);
            }
            return dfNodes;
        }

        private static XmlNode GetRootElement(ILayoutTemplate template, XmlDocument doc, ILayout layout)
        {
            XmlNode node = doc.CreateElement("Layout");
            object[] atts = GetAttsFromObject(doc, node, layout, layout);
            if (atts == null || atts.Length == 0)
                return null;
            foreach (object att in atts)
                node.Attributes.Append(att as XmlAttribute);
            List<IElement> elements = layout.Elements;
            CreatNodesFromElements(template, doc, node, elements, layout);
            return node;
        }

        private static XmlAttribute[] GetAttsFromObject(XmlDocument doc, XmlNode node, object obj, ILayout layout)
        {
            PropertyInfo[] pInfos = obj.GetType().GetProperties();
            if (pInfos == null || pInfos.Length == 0)
                return null;
            List<XmlAttribute> xmlAttList = new List<XmlAttribute>();
            //获取类型的名字空间以及类型名称
            XmlAttribute type = doc.CreateAttribute("type");
            type.Value = obj.GetType().Namespace + ".dll" + ":" + obj.GetType().FullName;
            xmlAttList.Add(type);

            foreach (PropertyInfo pInfo in pInfos)
            {
                PersistAttribute patt = GetPersistattribute(pInfo);
                if (patt == null)
                    continue;
                XmlAttribute xmlAtt = CreateXmlAttribute(doc, pInfo, patt, obj, layout);
                xmlAttList.Add(xmlAtt);
            }
            return xmlAttList != null ? xmlAttList.ToArray() : null;
        }

        private static void CreatNodesFromElements(ILayoutTemplate template, XmlDocument doc, XmlNode node, List<IElement> elements, ILayout layout)
        {
            object[] attSubs;
            XmlNode subNode;
            XmlAttribute illuAttr = doc.CreateAttribute("type");
            foreach (IElement ele in elements)
            {
                string[] names = ele.GetType().ToString().Split('.');
                subNode = doc.CreateElement(names[names.Length - 1]);
                //保存经纬网格
                if (ele is IDataFrame)
                {
                    (ele as IDataFrame).SyncAttrbutes();
                    XElement gridXml = (ele as IDataFrame).GetGridXml();
                    if (gridXml != null)
                        subNode.InnerXml = gridXml.ToString();
                    else
                    {
                        subNode.InnerXml = GetGridXmlFromTemplate(template, (ele as IDataFrame).Name);
                    }
                }
                //获取对象被序列化的属性
                attSubs = GetAttsFromObject(doc, subNode, ele, layout);
                if (attSubs == null || attSubs.Length == 0)
                    continue;
                foreach (object attSub in attSubs)
                    subNode.Attributes.Append(attSub as XmlAttribute);
                node.AppendChild(subNode);
                IElementGroup eleGroup = ele as ElementGroup;
                if (eleGroup == null)
                    continue;
                if (eleGroup.Elements == null || eleGroup.Elements.Count == 0)
                    continue;
                CreatNodesFromElements(template, doc, subNode, eleGroup.Elements, layout);
            }
        }

        private static string GetGridXmlFromTemplate(ILayoutTemplate template, string dfName)
        {
            if (template == null || template.Layout == null || string.IsNullOrEmpty(dfName))
                return string.Empty;
            IElement[] eles = template.Layout.QueryElements((ele) => { return ele is IDataFrame && (ele as IDataFrame).Name == dfName; });
            if (eles == null || eles.Length == 0)
                return string.Empty;
            XElement gridEle = (eles[0] as IDataFrame).GeoGridXml;
            return gridEle != null ? gridEle.ToString() : string.Empty;
        }

        private static PersistAttribute GetPersistattribute(PropertyInfo pInfo)
        {
            object[] objs = pInfo.GetCustomAttributes(typeof(PersistAttribute), false);
            if (objs == null || objs.Length == 0)
                return null;
            return objs[0] as PersistAttribute;
        }

        private static XmlAttribute CreateXmlAttribute(XmlDocument doc, PropertyInfo pInfo, PersistAttribute patt, object obj, ILayout layout)
        {
            XmlAttribute att = doc.CreateAttribute(pInfo.Name.ToLower());
            //获取属性的类型 例如："GeoDo.RSS.Layout.enumLayoutUnit:Pixel"
            // string illuminateName = pInfo.PropertyType.Name.ToString() + ":"; 
            if (patt.AttType == enumAttType.ValueType)
            {
                if (obj.GetType().InvokeMember(pInfo.Name, BindingFlags.GetProperty, null, obj, null) != null)
                    att.Value = obj.GetType().InvokeMember(pInfo.Name, BindingFlags.GetProperty, null, obj, null).ToString();
            }
            else
            {
                if (obj.GetType().InvokeMember(pInfo.Name, BindingFlags.GetProperty, null, obj, null) != null)
                {
                    if (pInfo.Name.ToLower() == "size" && layout.Unit == enumLayoutUnit.Pixel)
                    {
                        SizeF size = (SizeF)pInfo.GetValue(obj, null);
                        att.Value = GetBinaryString(size);
                    }
                    else if (pInfo.Name.ToLower() == "location" && layout.Unit == enumLayoutUnit.Pixel)
                    {
                        PointF loc = (PointF)pInfo.GetValue(obj, null);
                        att.Value = GetBinaryString(loc);
                    }
                    else
                        att.Value = GetBinaryString(obj.GetType().InvokeMember(pInfo.Name, BindingFlags.GetProperty, null, obj, null));
                }
            }
            return att;
        }

        //对象到二进制流，再转为64位字符串
        private static string GetBinaryString(object value)
        {
            string result = null;
            using (Stream st = new MemoryStream())
            {
                result = SerializeObj(value, st);
            }
            return result;
        }

        private static string SerializeObj(object value, Stream st)
        {
            IFormatter formatter = (IFormatter)new BinaryFormatter();
            formatter.Serialize(st, value);
            st.Position = 0;
            st.Flush();
            using (BinaryReader br = new BinaryReader(st))
            {
                byte[] cache = br.ReadBytes((int)st.Length);
                return Convert.ToBase64String(cache, 0, cache.Length);
            }
        }
    }
}
