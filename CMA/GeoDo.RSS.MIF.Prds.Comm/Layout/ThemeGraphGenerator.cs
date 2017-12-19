using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.Layout.GDIPlus;
using GeoDo.RSS.Layout;
using GeoDo.RSS.Core.UI;
using System.IO;
using GeoDo.RSS.Layout.Elements;
using System.Drawing;
using GeoDo.RSS.Core.DF;

namespace GeoDo.RSS.MIF.Prds.Comm
{
    public class ThemeGraphGenerator : IThemeGraphGenerator
    {
        protected ISmartSession _session;
        protected IGxdDocument _doc = null;
        protected string _fname;
        public bool IsFitToTemplateWidth = true;

        public ThemeGraphGenerator(ISmartSession session)
        {
            _session = session;
        }

        public string DateString { get; set; }
        public string TimesCount { get; set; }

        public void Generate(string dataFileName, string templateName, int[] aoi, string extInfos, string outIdentify, params object[] options)
        {
            _doc = null;
            _fname = null;
            ILayoutTemplate t = GetTemplateByName(dataFileName, templateName);
            if (t == null)
                return;
            if (IsFitToTemplateWidth)
            {
                using (IRasterDataProvider prd = GeoDataDriver.Open(dataFileName) as IRasterDataProvider)
                {
                    if (prd != null)
                    {
                        FitTemplateWidth(t.Layout, prd.Width, prd.Height);
                    }
                }
            }
            _fname = GetOutputGxdFileName(dataFileName, outIdentify, extInfos);
            _doc = GetDocument(dataFileName, aoi, t, options != null && options.Length > 0 ? options[0] : null);
            TrySetAttributesOfElements(_doc);
        }

        /// <summary>
        /// 适应模版宽度，等比例缩放图像，高度空白区域自动填充。
        /// </summary>
        /// <param name="temp">专题模版</param>
        /// <param name="width">原图像宽度</param>
        /// <param name="height">原图像高度</param>
        protected void FitTemplateWidth(ILayout layout, int width, int height)
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
                    eles[i].Name.Contains("Date") ||
                eles[i].Name.Contains("Day"))
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

        protected void FitTemplateWidth(ILayout layout, float width, float height)
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
                    eles[i].Name.Contains("Date") ||
                eles[i].Name.Contains("Day"))
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

        public string Save()
        {
            if (_doc == null)
                return null;
            _doc.SaveAs(_fname);
            return _fname;
        }

        private void TrySetAttributesOfElements(IGxdDocument doc)
        {
            ILayout layout = doc.GxdTemplateHost.LayoutTemplate.Layout;

            ApplyAttributesOfLayout(layout);

            //图廓
            IBorder border = layout.GetBorder();
            ApplyAttributesOfLayoutBorder(border);
            //数据框
            IElement[] dfs = layout.QueryElements((e) => { return e is IDataFrame; });
            ApplyAttributesOfDataFrame(doc.DataFrames, dfs, layout);
            //图列
            ApplyAttributesOfElement(layout.QueryElements((e) => { return e != null; }));
        }

        protected virtual void ApplyAttributesOfLayout(ILayout layout)
        {
        }

        private void ApplyAttributesOfElement(IElement[] eles)
        {
            if (eles == null || eles.Length == 0)
                return;
            foreach (IElement e in eles)
                ApplyAttributesOfElement(e.Name, e);
        }

        private void ApplyAttributesOfDataFrame(List<IGxdDataFrame> list, IElement[] dfs, ILayout layout)
        {
            if (list == null || dfs == null || list.Count == 0 || dfs.Length == 0 || list.Count != dfs.Length)
                return;
            for (int i = 0; i < dfs.Length; i++)
                ApplyAttributesOfDataFrame(list[i], dfs[i] as IDataFrame, layout);
        }

        protected virtual void ApplyAttributesOfElement(string name, IElement ele)
        {
        }

        protected virtual void ApplyAttributesOfDataFrame(IGxdDataFrame gxdDataFrame, IDataFrame dataFrame, ILayout layout)
        {
        }

        protected virtual void ApplyAttributesOfLayoutBorder(IBorder border)
        {
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
                else if (sate.Contains("FY2"))
                    sate = sate.Replace("FY2", "FY-2");
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
            if (!string.IsNullOrWhiteSpace(TimesCount))
                vars.Add("{ValidDays}", "有效天数:" + TimesCount);
            else
                vars.Add("{ValidDays}", "");

            if (!IsVector(fileName))
            {
                using (RasterDataProvider rdd = GeoDataDriver.Open(fileName) as RasterDataProvider)
                {
                    if (rdd != null)
                    {
                        string resolutionStr = Math.Round(rdd.ResolutionX, 4).ToString();
                        vars.Add("{resolution}", resolutionStr);
                        if (rdd.SpatialRef == null)
                        {
                            vars["{resolution}"] += "度";
                            vars.Add("{projection}", "等经纬度");
                        }
                        else if (rdd.SpatialRef.GeographicsCoordSystem == null)
                            vars.Add("{projection}", "");
                        else if (rdd.SpatialRef.ProjectionCoordSystem == null)
                        {
                            vars["{resolution}"] += "度";
                            vars.Add("{projection}", "等经纬度");
                        }
                        else
                        {
                            string targatName = string.Empty;
                            string projectName = rdd.SpatialRef.ProjectionCoordSystem.Name.Name;
                            GetProjectName(projectName, out targatName);
                            vars.Add("{projection}", targatName);
                        }
                    }
                }
            }
            temp.ApplyVars(vars);
        }

        private bool IsVector(string filename)
        {
            string[] vectorExt = new string[] { ".SHP" };
            if (vectorExt.Contains(Path.GetExtension(filename).ToUpper()))
                return true;
            return false;
        }

        private void GetProjectName(string projectName, out string targatName)
        {
            switch (projectName)
            {
                case "Polar Stereographic": targatName = "极射赤面投影";
                    break;
                case "Albers Conical Equal Area": targatName = "阿尔伯斯等面积投影";
                    break;
                case "Lambert Conformal Conic": targatName = "兰伯托";
                    break;
                case "Mercator": targatName = "墨卡托";
                    break;
                case "Hammer": targatName = "Hammer";
                    break;
                default: targatName = "";
                    break;
            }
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

        public static string FindTemplate(string templateName)
        {
            if (string.IsNullOrEmpty(templateName))
                return null;
            string path = AppDomain.CurrentDomain.BaseDirectory + "LayoutTemplate";
            if (!Directory.Exists(path))
                return null;
            string[] files = Directory.GetFiles(path, "*" + templateName + ".gxt", SearchOption.AllDirectories);
            if (files == null || files.Length == 0)
                return null;
            //如果有多个文件匹配，则应用搜索到的第一个文件
            string full = Path.GetFullPath(files[0]);
            return full;
        }
    }
}
