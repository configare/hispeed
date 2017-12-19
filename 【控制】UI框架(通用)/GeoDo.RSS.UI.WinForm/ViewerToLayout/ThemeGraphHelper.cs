using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.UI;
using GeoDo.RSS.Layout;
using GeoDo.RSS.Layout.GDIPlus;
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.MIF.Core;
using System.IO;
//using GeoDo.RSS.Layout.Elements;
using System.Drawing;

namespace GeoDo.RSS.UI.WinForm
{
    public class ThematicGraphHelper
    {
        protected ISmartSession _session;
        private bool _isNeedFitToTemplateWidth = true;
        private int[] _aoi = null;//thematic 
        private string _templateName = null;
        private ILayoutTemplate _template;

        public ThematicGraphHelper(ISmartSession session, string templateName)
        {
            _session = session;
            _templateName = templateName;
            _template = GetTemplateByName(templateName);
        }

        public bool IsNeedFitToTemplateWidth
        {
            get { return _isNeedFitToTemplateWidth; }
            set { _isNeedFitToTemplateWidth = value; }
        }

        public int[] AOI
        {
            get { return _aoi; }
            set { _aoi = value; }
        }

        public ILayoutTemplate Template
        {
            get { return _template; }
        }

        private ILayoutTemplate GetTemplateByName(string templateName)
        {
            ILayoutTemplate t = LayoutTemplate.FindTemplate(templateName);
            return t;
        }

        //internal void SetOrbitTimes(string[] orbitTimes)
        //{
        //    if (_template == null)
        //        return;
        //    ILayout layout = _template.Layout;
        //    IElement[] dfs = layout.QueryElements((e) => { return e is ILegendElement; });
        //    List<LegendItem> lg = new List<LegendItem>();
        //    for (int i = 0; i < orbitTimes.Length; i++)
        //    {
        //        lg.Add(new LegendItem(orbitTimes[i]));
        //    }
        //    ILegendElement legend = dfs[0] as ILegendElement;
        //    legend.LegendItems = lg.ToArray();
        //}

        public string CreateThematicGraphic(string dataFileName, string theme, string gxdFilename, string extInfos, params object[] options)
        {
            _template = LayoutTemplate.LoadTemplateFrom(theme);
            if (_template == null)
                return null;
            if (_isNeedFitToTemplateWidth)
            {
                using (IRasterDataProvider prd = GeoDataDriver.Open(dataFileName) as IRasterDataProvider)
                {
                    if (prd != null)
                    {
                        FitTemplateWidth(_template.Layout, prd.Width, prd.Height);
                    }
                }
            }
            IGxdDocument _doc = GetDocument(dataFileName, _aoi, _template, options != null && options.Length > 0 ? options[0] : null);
            if (_doc == null)
                return null;
            _doc.SaveAs(gxdFilename);
            return gxdFilename;
        }

        /// <summary>
        /// 适应模版宽度，等比例缩放图像，高度空白区域自动填充。
        /// </summary>
        /// <param name="temp">专题模版</param>
        /// <param name="width">原图像宽度</param>
        /// <param name="height">原图像高度</param>
        private void FitTemplateWidth(ILayout layout, int width, int height)
        {
            IElement[] dfs = layout.QueryElements((e) => { return e is IDataFrame; });
            IDataFrame df = (dfs[0] as IDataFrame);
            if (df.Size.Width == width && df.Size.Height == height)
                return;
            float dfNewW = df.Size.Width;
            float sc = width / dfNewW;
            float dfNewH = height / sc;

            float yOffset = dfNewH - df.Size.Height;
            df.IsLocked = false;
            df.ApplySize(0f, yOffset);
            df.IsLocked = true;
            layout.Size = new System.Drawing.SizeF(layout.Size.Width, layout.Size.Height + yOffset);
            List<IElement> eles = layout.Elements;
            for (int i = 0; i < eles.Count; i++)
            {
                if (eles[i].Name == "标题" ||
                    eles[i].Name.Contains("Time") ||
                    eles[i].Name.Contains("Date"))
                    continue;
                if (eles[i] is IBorder ||
                    eles[i] is IDataFrame)
                    continue;
                if (eles[i] is ISizableElement)
                {
                    (eles[i] as ISizableElement).ApplyLocation(0f, yOffset);
                }
            }
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
            RasterIdentify identify = new RasterIdentify(fileName);
            Dictionary<string, string> vars = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(identify.Satellite))
            {
                string sate = identify.Satellite.ToUpper();
                if (sate.Contains("FY3"))
                    sate = sate.Replace("FY3", "FY-3");
                vars.Add("{Satellite}", sate);
            }
            if (!string.IsNullOrEmpty(identify.Sensor))
                vars.Add("{Sensor}", identify.Sensor);
            if (!string.IsNullOrEmpty(identify.ProductName))
                vars.Add("{Product}", identify.ProductName);
            if (string.IsNullOrWhiteSpace(DateString))
            {
                if (identify.OrbitDateTime != DateTime.MinValue)
                {
                    identify.OrbitDateTime.AddHours(8);
                    vars.Add("{OrbitDateTime}", identify.OrbitDateTime.AddHours(8).ToString("yyyy年MM月dd日 HH:mm") + " (北京时)");
                }
                if (identify.MaxOrbitDate != DateTime.MaxValue && identify.MinOrbitDate != DateTime.MinValue)
                {
                    DateTime minTime = identify.MinOrbitDate.AddHours(8);
                    DateTime maxTime = identify.MaxOrbitDate.AddHours(8);
                    vars.Add("{MinOrbitDateTime~MaxOrbitDateTime}", minTime.ToString("yyyy年MM月dd日 HH:mm") + " ~ " + maxTime.ToString("yyyy年MM月dd日 HH:mm"));
                }
            }
            else
            {
                vars.Add("{OrbitDateTime}", DateString);
                vars.Add("{MinOrbitDateTime~MaxOrbitDateTime}", DateString);
            }
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

        public string DateString { get; set; }
    }
}
