using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.RasterDrawing;
using GeoDo.RSS.Core.DF;
using System.IO;
using GeoDo.Project;
using System.Drawing.Imaging;
using System.Drawing;
using GeoDo.RSS.Core.UI;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.Layout;
using GeoDo.RSS.Layout.GDIPlus;
using GeoDo.RSS.MIF.Prds.Comm;
using GeoDo.RasterProject;
using GeoDo.FileProject;
using GeoDo.RSS.Core.DrawEngine;

namespace GeoDo.RSS.UI.AddIn.GeoAdjust
{
    public class LayoutCreater
    {
        private string _region = null;

        public string CreateMCSI(ISmartSession session)
        {
            IRasterDrawing drawing = GetRasterDrawingArugment(session);
            if (drawing == null)
                return null;
            string theme = GetLayoutTheme("MCSI");
            if (string.IsNullOrWhiteSpace(theme))
                return null;
            int width, height;
            string bmpFilename = CreateViewBitmap(drawing, session, out width, out height);
            string gxdFilename = GetGxdFilename(drawing.FileName, "MCSI");
            if (string.IsNullOrWhiteSpace(gxdFilename))
                return null;
            //加载模版
            ILayoutTemplate temp = GetTemplateByArg(theme);
            if (temp == null)
                return null;
            ILayout layout = temp.Layout;
            FitSizeToTemplateWidth(layout, width, height);
            //生成文档，并应用变量
            IGxdDocument gxdDoc = GetDocument(bmpFilename, temp);
            TryApplyVars(gxdDoc.GxdTemplateHost.LayoutTemplate, drawing);
            TrySetAttributesOfElements(gxdDoc);
            gxdDoc.SaveAs(gxdFilename);
            //IExtractResult er = new FileExtractResult("MCSI", gxdFilename) as IExtractResult;
            return gxdFilename;

        }

        private IRasterDrawing GetRasterDrawingArugment(ISmartSession session)
        {
            if (session == null)
                return null;
            ICanvasViewer viewer = session.SmartWindowManager.ActiveCanvasViewer;
            if (viewer == null)
                return null;
            ICanvas canvas = viewer.Canvas;
            if (canvas == null)
                return null;
            return canvas.PrimaryDrawObject as IRasterDrawing;
        }

        private void TrySetAttributesOfElements(IGxdDocument doc)
        {
            ILayout layout = doc.GxdTemplateHost.LayoutTemplate.Layout;
            //数据框
            IElement[] dfs = layout.QueryElements((e) => { return e is IDataFrame; });
            ApplyAttributesOfDataFrame(doc.DataFrames, dfs, layout);
        }

        private void ApplyAttributesOfDataFrame(List<IGxdDataFrame> list, IElement[] dfs, ILayout layout)
        {
            if (list == null || dfs == null || list.Count == 0 || dfs.Length == 0 || list.Count != dfs.Length)
                return;
            for (int i = 0; i < dfs.Length; i++)
                ApplyAttributesOfDataFrame(list[i], dfs[i] as IDataFrame, layout);
        }

        protected virtual void ApplyAttributesOfDataFrame(IGxdDataFrame gxdDataFrame, IDataFrame dataFrame, ILayout layout)
        {
            TrySetDataFrameEnvelope(gxdDataFrame, dataFrame, layout);
        }

        #region 尝试设置数据框范围
        private void TrySetDataFrameEnvelope(IGxdDataFrame gxdDataFrame, IDataFrame dataFrame, ILayout layout)
        {
            try
            {
                ThemeGraphRegion region = ThemeGraphRegionSetting.GetThemeGraphRegion("FLD");
                if (region != null && _region != null)
                {
                    foreach (PrjEnvelopeItem item in region.PrjEnvelopeItems)
                    {
                        if (item.Name.ToUpper() == _region)
                        {
                            PrjEnvelope env = item.PrjEnvelope;
                            GxdEnvelope gxdEnv = ToPrjEnvelope(env, gxdDataFrame, dataFrame);
                            if (gxdEnv != null)
                            {
                                FitSizeToTemplateWidth(layout, (float)(gxdEnv.MaxX - gxdEnv.MinX), (float)(gxdEnv.MaxY - gxdEnv.MinY));
                                gxdDataFrame.Envelope = gxdEnv;
                                return;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //PrintInfo(ex.Message);
            }
        }

        protected GxdEnvelope ToPrjEnvelope(PrjEnvelope env, IGxdDataFrame gxdDataFrame, IDataFrame dataFrame)
        {
            if (env == null)
                return null;
            GeoDo.Project.IProjectionTransform tran = GetProjectionTransform(gxdDataFrame.SpatialRef);
            if (tran == null)
                return null;
            double[] xs = new double[] { env.MinX, env.MaxX };
            double[] ys = new double[] { env.MaxY, env.MinY };
            try
            {
                tran.Transform(xs, ys);
                return new GxdEnvelope(xs[0], xs[1], ys[1], ys[0]);
            }
            catch
            {
                return null;
            }
        }

        private Project.IProjectionTransform GetProjectionTransform(string spatialRef)
        {
            if (spatialRef == null || !spatialRef.Contains("PROJCS"))
                return Project.ProjectionTransformFactory.GetDefault();
            else
                return Project.ProjectionTransformFactory.GetProjectionTransform(GeoDo.Project.SpatialReference.GetDefault(), GeoDo.Project.SpatialReferenceFactory.GetSpatialReferenceByWKT(spatialRef, Project.enumWKTSource.EsriPrjFile));
        }
        #endregion

        private IGxdDocument GetDocument(string rasterFileName, ILayoutTemplate temp, params object[] arguments)
        {
            IGxdDocument doc = new GxdDocument(new GxdTemplateHost(temp));
            IGxdDataFrame gxdDf = doc.DataFrames.Count > 0 ? doc.DataFrames[0] : null;
            if (string.IsNullOrEmpty(rasterFileName))
                return null;
            if (gxdDf != null)
            {
                IGxdRasterItem rst = new GxdRasterItem(rasterFileName, null);//这里传具体的色标定义标识
                gxdDf.GxdRasterItems.Add(rst);
            }
            return doc;
        }

        private void TryApplyVars(ILayoutTemplate temp, IRasterDrawing rasterDrawing)
        {
            string fileName = Path.Combine(Path.GetDirectoryName(rasterDrawing.FileName), rasterDrawing.FileName.Replace("_G.", "."));
            DataIdentify did;
            if (File.Exists(fileName))
            {
                using (IRasterDataProvider dataPrd = GeoDataDriver.Open(fileName) as IRasterDataProvider)
                {
                    did = dataPrd.DataIdentify;
                }
            }
            else
                did = rasterDrawing.DataProviderCopy.DataIdentify;
            Dictionary<string, string> vars = new Dictionary<string, string>();
            //
            fileName = Path.GetFileName(fileName).ToUpper();
            if (fileName.Contains("EA"))
            {
                vars.Add("{Region}", "洞庭湖");
                _region = "EA";
            }
            else if (fileName.Contains("EB"))
            {
                vars.Add("{Region}", "鄱阳湖");
                _region = "EB";
            }
            if (!string.IsNullOrEmpty(did.Satellite))
            {
                string sate = did.Satellite.ToUpper();
                if (sate.Contains("FY3"))
                    sate = sate.Replace("FY3", "FY-3");
                vars.Add("{Satellite}", sate);
            }
            if (!string.IsNullOrEmpty(did.Sensor))
                vars.Add("{Sensor}", did.Sensor);
            if (did.OrbitDateTime != DateTime.MinValue)
            {
                vars.Add("{OrbitDateTime}", did.OrbitDateTime.AddHours(8).ToString("yyyy年MM月dd日 HH:mm") + " (北京时)");
            }
            IRasterDataProvider dataProvider = rasterDrawing.DataProviderCopy;// GeoDataDriver.Open(fileName) as IRasterDataProvider; 
            if (dataProvider != null)
            {
                string resolutionStr = Math.Round(dataProvider.ResolutionX, 4).ToString();
                vars.Add("{resolution}", resolutionStr);
                if (dataProvider.SpatialRef == null)
                {
                    vars["{resolution}"] += "度";
                    vars.Add("{projection}", "等经纬度");
                }
                else if (dataProvider.SpatialRef.GeographicsCoordSystem == null)
                    vars.Add("{projection}", "");
                else if (dataProvider.SpatialRef.ProjectionCoordSystem == null)
                {
                    vars["{resolution}"] += "度";
                    vars.Add("{projection}", "等经纬度");
                }
                else
                {
                    string targatName = string.Empty;
                    string projectName = dataProvider.SpatialRef.ProjectionCoordSystem.Name.Name;
                    GetProjectName(projectName, out targatName);
                    vars.Add("{projection}", targatName);
                }
                int[] channels = rasterDrawing.SelectedBandNos;// dataProvider.GetDefaultBands();
                List<string> channelList = new List<string>();
                if (channels != null && channels.Count() > 0)
                {
                    for (int i = 0; i < channels.Length; i++)
                    {
                        channelList.Add(channels[i].ToString());
                        channelList.Add(",");
                    }
                    channelList.RemoveAt(channelList.Count - 1);
                }
                string channelStr = null;
                foreach (string item in channelList)
                    channelStr += item;
                vars.Add("{channel}", channelStr);
            }
            temp.ApplyVars(vars);
        }

        private IGxdDocument CreateDocument(ILayoutTemplate template, object p)
        {
            IGxdDocument doc = new GxdDocument(new GxdTemplateHost(template));
            IGxdDataFrame gxdDf = doc.DataFrames.Count > 0 ? doc.DataFrames[0] : null;
            return doc;
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

        /// <summary>
        /// 适应模版宽度，等比例缩放图像，高度空白区域自动填充。
        /// </summary>
        /// <param name="temp">专题模版</param>
        /// <param name="width">原图像宽度</param>
        /// <param name="height">原图像高度</param>
        private void FitSizeToTemplateWidth(ILayout layout, int width, int height)
        {
            FitSizeToTemplateWidth(layout, (float)width, (float)height);
        }

        protected void FitSizeToTemplateWidth(ILayout layout, float width, float height)
        {
            IElement[] dfs = layout.QueryElements((e) => { return e is IDataFrame; });
            if (dfs == null || dfs.Length == 0)
                return;
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

        private ILayoutTemplate GetTemplateByArg(string templateName)
        {
            ILayoutTemplate t = LayoutTemplate.FindTemplate(templateName);
            if (t == null)
                t = LayoutTemplate.FindTemplate("缺省二值图模版");
            return t;
        }

        private string GetGxdFilename(string fileName, string instanceIdentify)
        {
            if (string.IsNullOrEmpty(fileName))
                return null;
            RasterIdentify rstIdentify = new RasterIdentify(fileName);
            rstIdentify.ProductIdentify = "FLD";
            rstIdentify.SubProductIdentify = instanceIdentify;
            string gxdFilename = rstIdentify.ToWksFileName(".gxd");
            gxdFilename = Path.Combine(Path.GetDirectoryName(fileName), gxdFilename);
            return gxdFilename;
        }


        /// <summary>
        /// 获取专题图模版
        /// </summary>
        /// <returns></returns>
        private string GetLayoutTheme(string themeType)
        {
            if (string.IsNullOrEmpty(themeType))
                return null;
            string theme = string.Empty;
            if (themeType == "MCSI")
            {
                theme = "水情产品多通道合成图";
                return theme;
            }
            else
            {
                theme = "水情二值图模版";
                return theme;
            }
        }

        /// <summary>
        /// 生成视窗图片
        /// </summary>
        /// <param name="rd"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        private string CreateViewBitmap(IRasterDrawing drawing, ISmartSession session, out int width, out int height)
        {
            string bmpFilename = GetTempBmpFilename(drawing.FileName, session);
            string ext = Path.GetExtension(bmpFilename);
            ImageFormat imageFormat = GetImageFormat(ext);
            Size imgSize;
            using (Bitmap img = CreateRasterBitmap(drawing))
            {
                img.Save(bmpFilename, imageFormat);
                imgSize = img.Size;
            }
            width = imgSize.Width;
            height = imgSize.Height;
            WriteWorldFile(drawing.DataProvider.CoordEnvelope, imgSize, drawing.DataProvider.SpatialRef, bmpFilename);
            return bmpFilename;
        }


        private string GetTempBmpFilename(string filename, ISmartSession session)
        {
            string sugFilename = Path.ChangeExtension(Path.GetFileName(filename), ".png");
            string tempPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Temp");
            //return session.TemporalFileManager.NextTemporalFilename(sugFilename, ".png", null);
            return Path.Combine(tempPath, sugFilename);
        }

        private ImageFormat GetImageFormat(string ext)
        {
            switch (ext.ToLower())
            {
                case ".png":
                    return ImageFormat.Png;
                case ".bmp":
                    return ImageFormat.Bmp;
                case ".jpeg":
                case ".jpg":
                    return ImageFormat.Jpeg;
                case ".tif":
                case ".tiff":
                    return ImageFormat.Tiff;
                default:
                    return ImageFormat.Png;
            }
        }

        private Bitmap CreateRasterBitmap(IRasterDrawing drawing)
        {
            if (drawing == null)
                return null;
            Bitmap img = drawing.GetBitmapUseOriginResolution();
            if (IsPixelFormatIndexed(img.PixelFormat))
            {
                Bitmap newimg = img.Clone(new System.Drawing.Rectangle(0, 0, img.Width, img.Height), PixelFormat.Format24bppRgb);
                img.Dispose();
                return newimg;
            }
            else
            {
                return img;
            }
        }

        private static PixelFormat[] indexedPixelFormats = { PixelFormat.Undefined, PixelFormat.DontCare, PixelFormat.Format16bppArgb1555, PixelFormat.Format1bppIndexed, PixelFormat.Format4bppIndexed, PixelFormat.Format8bppIndexed };

        /// <summary>
        ///  判断图片的PixelFormat 是否在 引发异常的 PixelFormat 之中
        ///  无法从带有索引像素格式的图像创建graphics对象
        /// </summary>
        /// <param name="imgPixelFormat"></param>
        /// <returns></returns>
        private static bool IsPixelFormatIndexed(PixelFormat imgPixelFormat)
        {
            foreach (PixelFormat pf in indexedPixelFormats)
            {
                if (pf.Equals(imgPixelFormat)) return true;
            }
            return false;
        }

        private void WriteWorldFile(GeoDo.RSS.Core.DF.CoordEnvelope env, Size imgSize, ISpatialReference spatial, string bmpFilename)
        {
            try
            {
                double minx = env.MinX;
                double maxy = env.MaxY;
                double resolutionX = env.Width / imgSize.Width;
                double resolutionY = env.Height / imgSize.Height;
                WorldFile worldFile = new WorldFile();
                worldFile.CreatWorldFile(resolutionX, -resolutionY, minx, maxy, bmpFilename);
                worldFile.CreatXmlFile(spatial == null ? GeoDo.Project.SpatialReference.GetDefault() : spatial, bmpFilename);
            }
            catch
            {
            }
        }

        public string CreateDBLVLayout(string dblvFileName, string drawFName)
        {
            string templatName = GetLayoutTheme("0SDI");
            if (string.IsNullOrWhiteSpace(templatName))
                return null;
            string resultFilename = GenerateRasterThemeGraphy(dblvFileName, drawFName);
            return resultFilename;
        }

        private string GenerateRasterThemeGraphy(string dstFile, string drawFName)
        {
            string templatName = GetLayoutTheme("0SDI");
            string colorTabelName = "colortablename=" + "FLDDBLV";
            ILayoutTemplate t = GetTemplateByArg(templatName);
            if (t == null)
                return null;
            ILayout layout = t.Layout;
            int width, height;
            GeoDo.RSS.Core.DF.CoordEnvelope envelope = null;
            using (IRasterDataProvider dataPrd = GeoDataDriver.Open(dstFile) as IRasterDataProvider)
            {
                width = dataPrd.Width;
                height = dataPrd.Height;
                envelope = dataPrd.CoordEnvelope;
            }
            FitSizeToTemplateWidth(layout, width, height);
            //生成文档，并应用变量
            TryApplyVars(t, drawFName);
            IGxdDocument gxdDoc = CreateDocument(t, string.IsNullOrEmpty(colorTabelName) ? null : colorTabelName);
            IGxdDataFrame gxdDf = gxdDoc.DataFrames.Count > 0 ? gxdDoc.DataFrames[0] : null;
            if (gxdDf != null)
            {
                string[] arguments = new string[] { string.IsNullOrEmpty(colorTabelName) ? null : colorTabelName };
                IGxdRasterItem rst = new GxdRasterItem(dstFile, colorTabelName, arguments, colorTabelName);//这里传具体的色标定义标识
                gxdDf.GxdRasterItems.Add(rst);
            }
            TrySetAttributesOfElements(gxdDoc);
            string docFname = GetGxdFilename(dstFile, "0SDI");
            gxdDoc.SaveAs(docFname);
            return docFname;
        }

        private void TryApplyVars(ILayoutTemplate t, string drawFName)
        {
            string fileName = drawFName.Replace("_G", "");
            RasterIdentify rst = new RasterIdentify(fileName);
            Dictionary<string, string> vars = new Dictionary<string, string>();
            //
            fileName = Path.GetFileName(fileName).ToUpper();
            if (fileName.Contains("EA"))
            {
                vars.Add("{Region}", "洞庭湖");
                _region = "EA";
            }
            else if (fileName.Contains("EB"))
            {
                vars.Add("{Region}", "鄱阳湖");
                _region = "EB";
            }
            if (!string.IsNullOrEmpty(rst.Satellite))
            {
                string sate = rst.Satellite.ToUpper();
                if (sate.Contains("FY3"))
                    sate = sate.Replace("FY3", "FY-3");
                vars.Add("{Satellite}", sate);
            }
            if (!string.IsNullOrEmpty(rst.Sensor))
                vars.Add("{Sensor}", rst.Sensor);
            if (rst.GenerateDateTime != DateTime.MinValue)
            {
                vars.Add("{OrbitDateTime}", rst.OrbitDateTime.AddHours(8).ToString("yyyy年MM月dd日 HH:mm") + " (北京时)");
            }
            t.ApplyVars(vars);
        }
    }
}
