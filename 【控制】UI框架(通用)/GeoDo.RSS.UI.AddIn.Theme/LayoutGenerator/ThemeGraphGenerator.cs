using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.Layout.GDIPlus;
using GeoDo.RSS.Layout;
using GeoDo.RSS.Core.UI;
using GeoDo.RSS.Core.DrawEngine;
using System.IO;
using GeoDo.RSS.UI.AddIn.Layout;

namespace GeoDo.RSS.UI.AddIn.Theme
{
    public class ThemeGraphGenerator : IThemeGraphGenerator
    {
        protected ISmartSession _session;
        protected IGxdDocument _doc = null;
        protected string _fname;

        public ThemeGraphGenerator(ISmartSession session)
        {
            _session = session;
        }

        public void Generate(string dataFileName, string templateName, int[] aoi, string extInfos, string outIdentify, params object[] options)
        {
            _doc = null;
            _fname = null;
            ILayoutTemplate t = GetTemplateByName(dataFileName, templateName);
            if (t == null)
                return;
            _fname = GetOutputGxdFileName(dataFileName, outIdentify, extInfos);
            _doc = GetDocument(dataFileName, aoi, t, options != null && options.Length > 0 ? options[0] : null);
        }

        public string Save()
        {
            if (_doc == null)
                return null;
            _doc.SaveAs(_fname);
            return _fname;
        }

        private string GetOutputGxdFileName(string dataFileName, string outIdentify, string extInfos)
        {
            RasterIdentify rstIdentify = new RasterIdentify(dataFileName);
            rstIdentify.SubProductIdentify = outIdentify;
            rstIdentify.ExtInfos = extInfos;
            return rstIdentify.ToWksFullFileName(".gxd");
        }

        private IGxdDocument GetDocument(string dataFileName, int[] aoi, ILayoutTemplate t, object options)
        {
            IGxdDocument doc = new GxdDocument(new GxdTemplateHost(t));
            IGxdDataFrame gxdDf = doc.DataFrames.Count > 0 ? doc.DataFrames[0] : null;
            if (gxdDf != null)
            {
                IGxdRasterItem rst = new GxdRasterItem(dataFileName, options);//这里传具体的色标定义标识
                gxdDf.GxdRasterItems.Add(rst);
                //
                TryGeneratreAOISecondaryFile(aoi, rst);
                //
                TryApplyVars(doc, dataFileName);
            }
            return doc;
        }

        //应用变量
        private void TryApplyVars(IGxdDocument doc, string fileName)
        {
            ILayoutTemplate temp = doc.GxdTemplateHost.LayoutTemplate;
            RasterIdentify rst = new RasterIdentify(fileName);
            Dictionary<string, string> vars = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(rst.Satellite))
            {
                string sate = rst.Satellite.ToUpper();
                if (sate.Contains("FY3"))
                    sate = sate.Replace("FY3", "FY-3");
                vars.Add("{Satellite}", sate);
            }//changed by wangyu "FY3A"->"FY-3A"2012.10.20
            //vars.Add("{Satellite}", rst.Satellite);
            if (!string.IsNullOrEmpty(rst.Sensor))
                vars.Add("{Sensor}", rst.Sensor);
            if (!string.IsNullOrEmpty(rst.ProductName))
                vars.Add("{Product}", rst.ProductName);
            if (rst.OrbitDateTime != DateTime.MinValue)
            {
                rst.OrbitDateTime.AddHours(8);
                vars.Add("{OrbitDateTime}", rst.OrbitDateTime.AddHours(8).ToString("yyyy年MM月dd日 HH:mm") + " (北京时)");
            }
            //changed by wangyu,add displaying of hour and minutes 2012.10.20
            temp.ApplyVars(vars);
        }

        private void TryGeneratreAOISecondaryFile(int[] aoi, IGxdRasterItem rst)
        {
            string fname = Path.Combine(Path.GetDirectoryName(rst.FileName), Path.GetFileNameWithoutExtension(rst.FileName) + ".aoi");
            if (File.Exists(fname))
                File.Delete(fname);
            if (aoi == null || aoi.Length == 0)
                return;
            using (FileStream sw = new FileStream(fname, FileMode.Create))
            {
                using (BinaryWriter bw = new BinaryWriter(sw))
                {
                    for (int i = 0; i < aoi.Length; i++)
                        bw.Write(aoi[i]);
                }
            }
        }

        private ILayoutTemplate GetTemplateByName(string fname, string templateName)
        {
            ILayoutTemplate t = LayoutTemplate.FindTemplate(templateName);
            if (t == null)
            {
                if (fname.Contains("_DBLV_"))
                    t = LayoutTemplate.FindTemplate("缺省二值图模版");
            }
            return t;
        }
    }
}
