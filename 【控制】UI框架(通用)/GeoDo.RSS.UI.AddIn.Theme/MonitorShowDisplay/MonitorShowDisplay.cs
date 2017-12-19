using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.UI;
using GeoDo.RSS.Core.DrawEngine;
using GeoDo.RSS.Layout;
using GeoDo.RSS.Core.RasterDrawing;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.Layout.GDIPlus;
using GeoDo.RSS.UI.AddIn.Layout;
using System.IO;
using GeoDo.RSS.Core.DF;
using System.Drawing.Imaging;

namespace GeoDo.RSS.UI.AddIn.Theme
{
    public class MonitorShowDisplay
    {
        /// <summary>
        /// 监测示意图显示/多通道合成图 
        /// </summary>
        /// <param name="argument">template:沙尘监测示意图,监测示意图,DST,MCSI</param>
        /// <returns>专题图文档，IGxdDocument</returns>
        public string DisplayMonitorBitmap(ISmartSession session, string argument, out string subProductIdentify)
        {
            subProductIdentify = null;
            ICanvasViewer viewer = session.SmartWindowManager.ActiveCanvasViewer;
            if (viewer == null)
                return null;
            ICanvas canvas = viewer.Canvas;
            if (canvas == null)
                return null;
            string bitmapFileName = null;
            int width = 0;
            int height = 0;
            bool isOk = GetOrginBmpAndSave(canvas, out bitmapFileName, out width, out height);
            if (!isOk)
                return null;
            CreatWorldFile(bitmapFileName, canvas, width, height);
            string[] parts = argument.Split(',');
            if (parts == null || parts.Length == 0)
                return null;
            subProductIdentify = parts[parts.Length - 1];
            object[] args = new object[] { parts[0] };
            ILayoutTemplate temp = GetTemplateByArg(parts[0]);
            if (temp == null)
                return null;

            ILayout layout = temp.Layout;
            FitSizeToTemplateWidth(layout, width, height);

            string rasterFname = (canvas.PrimaryDrawObject as IRasterDrawing).FileName;
            IGxdDocument gxdDoc = GetDocument(session, rasterFname, bitmapFileName, temp, parts[1]);
            TryApplyVars(gxdDoc.GxdTemplateHost.LayoutTemplate, (canvas.PrimaryDrawObject as IRasterDrawing));

            string fname = GetOutputGxdFileName(rasterFname, parts[2]);
            gxdDoc.SaveAs(fname);
            return fname;
        }

        private void TryApplyVars(ILayoutTemplate temp, IRasterDrawing rs)
        {
            string fileName = rs.FileName;
            RasterIdentify rst = new RasterIdentify(fileName);
            Dictionary<string, string> vars = new Dictionary<string, string>();
            IRasterDataProvider dataProvider = GeoDataDriver.Open(fileName) as IRasterDataProvider;
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
            if (dataProvider != null)
            {
                string resolutionStr = Math.Round(dataProvider.ResolutionX, 4).ToString();
                vars.Add("{resolution}", resolutionStr);
                if (dataProvider.SpatialRef == null)
                    vars.Add("{projection}", "等经纬度");
                else if (dataProvider.SpatialRef.GeographicsCoordSystem == null)
                    vars.Add("{projection}", "");
                else if (dataProvider.SpatialRef.ProjectionCoordSystem == null)
                    vars.Add("{projection}", "等经纬度");
                else
                {
                    string targatName = string.Empty;
                    string projectName = dataProvider.SpatialRef.ProjectionCoordSystem.Name.Name;
                    GetProjectName(projectName, out targatName);
                    vars.Add("{projection}", targatName);
                }
                int[] channels = rs.SelectedBandNos;// dataProvider.GetDefaultBands();
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
        
        /// <summary>
        /// 适应模版宽度，等比例缩放图像，高度空白区域自动填充。
        /// </summary>
        /// <param name="temp">专题模版</param>
        /// <param name="width">原图像宽度</param>
        /// <param name="height">原图像高度</param>
        private void FitSizeToTemplateWidth(ILayout layout, int width, int height)
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
        
        /// <summary>
        /// 适用于原始分辨率
        /// </summary>
        /// <param name="temp"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        private void FitSizeToBmp(ILayoutTemplate temp, int width, int height)
        {
            ILayout layout = temp.Layout;
            IElement[] dfs = layout.QueryElements((e) => { return e is IDataFrame; });
            IDataFrame df = (dfs[0] as IDataFrame);
            if (df.Size.Width == width && df.Size.Height == height)
                return;
            float wP = width - df.Size.Width;
            float hP = height - df.Size.Height;
            df.IsLocked = false;
            df.ApplySize(wP, hP);
            df.IsLocked = true;
            layout.Size = new System.Drawing.SizeF(layout.Size.Width + wP, layout.Size.Height + hP);
            IBorder border = layout.GetBorder();
        }

        private string GetOutputGxdFileName(string dataFileName, string productIdentify)
        {
            RasterIdentify rstIdentify = new RasterIdentify(dataFileName);
            rstIdentify.ProductIdentify = productIdentify;
            rstIdentify.SubProductIdentify = "MCSI";
            return rstIdentify.ToWksFullFileName(".gxd");
        }

        private IGxdDocument GetDocument(ISmartSession session, string rasterFname, string outputFileName, ILayoutTemplate temp, string wndName, params object[] arguments)
        {
            IGxdDocument doc = new GxdDocument(new GxdTemplateHost(temp));
            IGxdDataFrame gxdDf = doc.DataFrames.Count > 0 ? doc.DataFrames[0] : null;
            if (string.IsNullOrEmpty(outputFileName))
                return null;
            if (gxdDf != null)
            {
                IGxdRasterItem rst = new GxdRasterItem(outputFileName, null);//这里传具体的色标定义标识
                gxdDf.GxdRasterItems.Add(rst);
            }
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

        private bool GetBitmapAndSave(ICanvas canvas, out string fileName, out int width, out int height)
        {
            fileName = null;
            width = 0;
            height = 0;
            System.Drawing.Bitmap bitmap = canvas.FullRasterRangeToBitmap();
            if (bitmap == null)
                return false;
            fileName = MifEnvironment.GetFullFileName(Guid.NewGuid().ToString() + ".bmp");
            width = bitmap.Width;
            height = bitmap.Height;
            if (bitmap == null)
                return false;
            bitmap.Save(fileName, System.Drawing.Imaging.ImageFormat.Png);
            return true;
        }

        private bool GetOrginBmpAndSave(ICanvas canvas, out string fileName, out int width, out int height)
        {
            fileName = null;
            width = 0;
            height = 0;
            System.Drawing.Bitmap bitmap = (canvas.PrimaryDrawObject as IRasterDrawing).GetBitmapUseOriginResolution();
            if (bitmap == null)
                return false;
            string ofilename = (canvas.PrimaryDrawObject as IRasterDrawing).FileName;
            fileName = MifEnvironment.GetFullFileName(Guid.NewGuid().ToString() + ".bmp");
            width = bitmap.Width;
            height = bitmap.Height;
            if (bitmap == null)
                return false;
            bitmap.Save(fileName, System.Drawing.Imaging.ImageFormat.Png);
            return true;
        }

        private ILayoutTemplate GetTemplateByArg(string templateName)
        {
            templateName = templateName.Split(':')[1];
            ILayoutTemplate t = LayoutTemplate.FindTemplate(templateName);
            if (t == null)
                t = LayoutTemplate.FindTemplate("缺省二值图模版");
            return t;
        }

        private void CreatWorldFile(string fileName, ICanvas canvas, int width, int height)
        {
            if (string.IsNullOrEmpty(fileName))
                return;
            if (canvas == null)
                return;
            if (canvas.PrimaryDrawObject == null)
                return;
            if (width == 0 || height == 0)
                return;
            WorldFile wf = new WorldFile();
            if (canvas.PrimaryDrawObject.OriginalEnvelope == null)
                return;
            RasterDrawing drawing = canvas.PrimaryDrawObject as RasterDrawing;
            if (drawing == null)
                return;
            if (drawing.DataProvider == null)
                return;
            IRasterDataProvider prd = drawing.DataProvider;
            if (prd.SpatialRef == null)
                return;
            double bmpResolutionX = (prd.CoordEnvelope.MaxX - prd.CoordEnvelope.MinX) / width;
            double bmpResolutionY = (prd.CoordEnvelope.MaxY - prd.CoordEnvelope.MinY) / height;
            wf.CreatWorldFile(bmpResolutionX, -bmpResolutionY, prd.CoordEnvelope.MinX, prd.CoordEnvelope.MaxY, fileName);
            wf.CreatXmlFile(prd.SpatialRef, fileName);
        }
    }
}
