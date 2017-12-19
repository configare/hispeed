using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Linq;
using System.Drawing;
using System.Xml;
using System.Reflection;

namespace GeoDo.RSS.Layout
{
    public static class LayoutFromFile
    {
        public static ILayout LoadFromFile(string fname)
        {
            XDocument doc = XDocument.Load(fname);
            ILayout layout;
            XElement root = doc.Root;
            layout = CreateLayoutFromRoot(root);

            IEnumerable<XElement> nodes = root.Elements();
            foreach (XElement node in nodes)
            {
                if (node.Name == "DataFrames")
                    LoadContentOfDataFramework(node, layout);
                else
                    CreateInstanceFromElement(node, layout);
            }
            return layout;
        }

        public static ILayout LoadFromXml(string xmlContent)
        {
            using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(xmlContent)))
            {
                XDocument doc = XDocument.Load(ms);
                ILayout layout;
                XElement root = doc.Root;
                layout = CreateLayoutFromRoot(root);
                IEnumerable<XElement> nodes = root.Elements();
                foreach (XElement node in nodes)
                {
                    if (node.Name == "DataFrames")
                        LoadContentOfDataFramework(node, layout);
                    else
                        CreateInstanceFromElement(node, layout);
                }
                return layout;
            }
        }

        private static void LoadContentOfDataFramework(XElement root, ILayout layout)
        {
            if (root == null || root.Elements("DataFrame") == null)
                return;
            var eles = root.Elements("DataFrame");
            if (eles == null || eles.Count() == 0)
                return;
            IElement[] dfs = layout.QueryElements((e) => { return e is IDataFrame; });
            if (dfs == null || dfs.Length == 0)
                return;
            foreach (XElement dfEle in eles)
            {
                IDataFrame df = GetDataFrame(dfEle, dfs);
                if(df != null)
                    df.Data = dfEle;
            }
        }

        private static IDataFrame GetDataFrame(XElement dfEle, IElement[] dfs)
        {
            string name = null;
            if (dfEle.Attribute("name") != null)
                name = dfEle.Attribute("name").Value;
            foreach (IElement e in dfs)
            {
                if (e.Name != null && name != null && e.Name == name)
                {
                    IDataFrame crtDataFrame = e as IDataFrame;
                    return crtDataFrame;
                }
            }
            return null;
        }

        private static Layout CreateLayoutFromRoot(XElement root)
        {
            string type = root.Attribute("type").Value;
            string[] parts = type.Split(':');
            string fname = AppDomain.CurrentDomain.BaseDirectory + parts[0];
            if (!File.Exists(fname))
                return null;
            Assembly assembly = Assembly.LoadFile(fname);
            if (assembly == null)
                return null;
            object obj = assembly.CreateInstance(parts[1]);  //无法创建只有带参数构造函数的对象
            if (obj == null)
                return null;
            IPersitable persitable = obj as IPersitable;
            persitable.InitByXml(root);
            return persitable as Layout;
        }

        private static void CreateInstanceFromElement(XElement node, IElement element)
        {
            if (node != null && node.Name != null && node.Name == "GeoGridLayer")
                return;
            string type = node.Attribute(XName.Get("type")).Value;
            string[] parts = type.Split(':');
            string fname = AppDomain.CurrentDomain.BaseDirectory + parts[0];
            if (!File.Exists(fname))
                return;
            Assembly assembly = Assembly.LoadFile(fname);
            if (assembly == null)
                return;
            object obj = assembly.CreateInstance(parts[1]);
            IPersitable persitable = obj as IPersitable;
            persitable.InitByXml(node);

            IElementGroup group = element as IElementGroup;
            if (group != null)
                group.Elements.Add(persitable as IElement);

            //递归
            IEnumerable<XElement> elements = node.Elements();
            if (elements == null || elements.Count() == 0)
                return;
            foreach (XElement ele in elements)
            {
                CreateInstanceFromElement(ele, persitable as IElement);
            }
            return;
        }

        //base64 string to object
        public static object Base64StringToObject(string s)
        {
            if (String.IsNullOrEmpty(s))
                return null;
            byte[] cache = Convert.FromBase64String(s);
            Stream sm = new MemoryStream(cache);
            IFormatter fomatter = (IFormatter)new BinaryFormatter();
            return fomatter.Deserialize(sm);
        }
    }
}
