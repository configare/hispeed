using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;

namespace GeoDo.RSS.Layout.GDIPlus
{
    public class GxdDocument : GxdItem, IGxdDocument
    {
        protected List<IGxdDataFrame> _dataFrames = new List<IGxdDataFrame>();
        protected IGxdTemplateHost _templateHost;
        protected string _fullPath;
        public static Func<IDataFrame, GxdVectorHost> GxdVectorHostGettter;
        public static Func<IDataFrame, GxdEnvelope> GxdEnvelopeGetter;
        public static Action<IDataFrame, GxdDataFrame> GxDataFrameRasterItemsSetter;
        public static Action<string,IGxdDataFrame, ILayoutHost> GxdAddDataFrameExecutor;
   
        public GxdDocument()
        {
            InitReplace();
        }

        /*
         *     GxdTemplateHost template = GetGxdTemplate(doc.Root.Element("GxdLaytouTemplate"));
            List<IGxdDataFrame> dataFrames = GetGxdDataFrames(doc.Root.Element("GxdDataFrames"));
         */
        public GxdDocument(IGxdTemplateHost templateHost, List<IGxdDataFrame> dataFrames)
        {
            InitReplace();
            _templateHost = templateHost;
            if (dataFrames != null && dataFrames.Count > 0)
                _dataFrames.AddRange(dataFrames);
        }

        public GxdDocument(IGxdTemplateHost templateHost)
        {
            InitReplace();
            _templateHost = templateHost;
            if (_templateHost != null)
            {
                if (_templateHost.LayoutTemplate != null && _templateHost.LayoutTemplate.Layout != null)
                {
                    IElement[] eles = _templateHost.LayoutTemplate.Layout.QueryElements((e) => { return e is IDataFrame; });
                    if (eles == null || eles.Length == 0)
                        return;
                    foreach (IDataFrame df in eles)
                    {
                        GxdDataFrame gxddf = new GxdDataFrame(
                            df.Name,
                            GxdEnvelopeGetter(df),
                            GetDfVectorHostFromXml(df),
                            df.SpatialRef, 
                            df.GetGridXml(),
                            GetDocumentableLayersXml(df)
                            );
                        _dataFrames.Add(gxddf);
                    }
                }
            }
        }

        private XElement GetDocumentableLayersXml(IDataFrame df)
        {
            return df.GetDocumentableLayersHostXml();
        }

        private IGxdVectorHost GetDfVectorHostFromXml(IDataFrame df)
        {
            if (df.Data == null)
            {
                return null;
            }
            else
            {
                XDocument doc = XDocument.Load(new MemoryStream(Encoding.UTF8.GetBytes(df.Data.ToString())));
                return new GxdVectorHost(doc.Root.Element("Map").ToString());
            }
        }

        public string FullPath
        {
            get { return _fullPath; }
            set { _fullPath = value; }
        }

        public IGxdTemplateHost GxdTemplateHost
        {
            get { return _templateHost; }
        }

        public List<IGxdDataFrame> DataFrames
        {
            get { return _dataFrames; }
        }

        public override XElement ToXml()
        {
            XElement ele = new XElement("GxDocument");
            ele.SetValue(_templateHost != null ? _templateHost.ToXml().ToString() : string.Empty);
            ele.SetElementValue("GxdDataFrames", GetDataFrames());
            return ele;
        }

        private object GetDataFrames()
        {
            if (_dataFrames == null || _dataFrames.Count == 0)
                return string.Empty;
            StringBuilder sb = new StringBuilder();
            foreach (IGxdDataFrame df in _dataFrames)
                sb.Append(df.ToXml().ToString());
            return sb.ToString();
        }

        private Dictionary<string, string> _replaceWords = new Dictionary<string, string>();

        private void InitReplace()
        {
            _replaceWords.Add("&lt;", "<");
            _replaceWords.Add("&gt;", ">");
            _replaceWords.Add("&amp;lt;", "<");
            _replaceWords.Add("&amp;gt;", ">");
            _replaceWords.Add("&amp;amp;lt;", "<");
            _replaceWords.Add("&amp;amp;gt;", ">");
        }

        public void SaveAs(string fname)
        {
            XDocument doc = new XDocument();
            doc.Add(ToXml());
            doc.Save(fname);
            string text = File.ReadAllText(fname);
            foreach (string key in _replaceWords.Keys)
            {
                text = text.Replace(key, _replaceWords[key]);
            }
            //text = text.Replace("&lt;", "<");
            //text = text.Replace("&gt;", ">");
            //text = text.Replace("&amp;lt;", "<");
            //text = text.Replace("&amp;gt;", ">");
            //int n = 1;
            //string p1 = "&amp;";
            //string p2 = "&amp;";
            //while (text.Contains("&amp;amp"))
            //{
            //    string s1 = p1;
            //    string s2 = p2;
            //    for (int i = 0; i < n; i++)
            //    {
            //        s1 += "amp;";
            //        s2 += "amp;";
            //        text = text.Replace(s1 + "lt;", "<");
            //        text = text.Replace(s2 + "gt;", ">");
            //    }
            //    n++;
            //}
            File.WriteAllText(fname, text);
            CorrectFilePath(fname);
        }

        private void CorrectFilePath(string docFileName)
        {
            XDocument doc = XDocument.Load(docFileName);
            if (_templateHost.LayoutTemplate.FullPath != null && docFileName != null)
                CorrectRelativePath(docFileName, _templateHost.LayoutTemplate.FullPath, doc.Root);
            doc.Save(docFileName);
        }

        private void CorrectRelativePath(string docFileName, string templateFullPath, XElement ele)
        {
            UpdateXElement(ele, docFileName, templateFullPath);
        }

        private void UpdateXElement(XElement xElement, string docFileName, string templateFullPath)
        {
            if (xElement.Name == "DataSource")
            {
                xElement.SetAttributeValue("fileurl", CorrectFilePath(docFileName, xElement.Attribute("fileurl").Value, templateFullPath));
            }
            var eles = xElement.Elements();
            if (eles != null)
                foreach (XElement ele in eles)
                    UpdateXElement(ele, docFileName, templateFullPath);
        }

        private string CorrectFilePath(string docFileName, string fileUrl, string templateFullPath)
        {
            string dir = CodeCell.Bricks.Runtime.RelativePathHelper.GetFullPath(templateFullPath, fileUrl);
            return CodeCell.Bricks.Runtime.RelativePathHelper.GetRelativePath(docFileName, dir);
        }

        public static IGxdDocument GenerateFrom(ILayoutHost host)
        {
            if (host == null || host.LayoutRuntime == null || host.LayoutRuntime.Layout == null)
                return null;
            //
            ILayoutTemplate template = new LayoutTemplate(host.LayoutRuntime.Layout);
            GxdTemplateHost gxdTemplate = new GxdTemplateHost(template);
            //
            IGxdDocument gxdoc = new GxdDocument(gxdTemplate);
            //
            IElement[] dfs = host.LayoutRuntime.QueryElements((ele) => { return ele is IDataFrame; },false);
            if (dfs != null)
            {
                gxdoc.DataFrames.Clear();
                foreach (IDataFrame df in dfs)
                {
                    GxdDataFrame gxdDf = new GxdDataFrame(df.Name, GetDfEnvelope(df), 
                        GetDfVectorHost(df),df.SpatialRef,df.GetGridXml(),df.GetDocumentableLayersHostXml());
                    if (GxDataFrameRasterItemsSetter != null)
                        GxDataFrameRasterItemsSetter(df, gxdDf);
                    gxdoc.DataFrames.Add(gxdDf);
                }
            }
            return gxdoc;
        }

        private static IGxdVectorHost GetDfVectorHost(IDataFrame df)
        {
            if (GxdVectorHostGettter != null)
                return GxdVectorHostGettter(df);
            return null;
        }

        private static GxdEnvelope GetDfEnvelope(IDataFrame df)
        {
            if (GxdEnvelopeGetter != null)
                return GxdEnvelopeGetter(df);
            return null;
        }

        public static IGxdDocument LoadFrom(string fname)
        {
            return (new GxdDocumentParser()).Parse(fname);
        }

    }
}
