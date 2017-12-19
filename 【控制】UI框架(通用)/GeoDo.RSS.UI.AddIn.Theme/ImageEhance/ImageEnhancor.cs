using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.UI;
using GeoDo.RSS.Core.RasterDrawing;
using System.IO;
using System.Windows.Forms;
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.Core.CA;
using GeoDo.RSS.MIF.Core;

namespace GeoDo.RSS.UI.AddIn.Theme
{
    public class ImageEnhancor
    {
        private ISmartSession _session = null;

        public ImageEnhancor(ISmartSession session)
        {
            _session = session;
        }

        #region save
        public void SaveImageEnhance()
        {
            if (_session == null)
                return;
            if (_session.SmartWindowManager == null)
                return;
            ICanvasViewer viewer = _session.SmartWindowManager.ActiveViewer as ICanvasViewer;
            if (viewer == null)
                return;
            IRasterDrawing drawing = viewer.ActiveObject as IRasterDrawing;
            if (drawing == null)
                return;
            string enhanceFileName;
            SaveImageEnhance(drawing, out enhanceFileName);
            if (!string.IsNullOrWhiteSpace(enhanceFileName))
                SaveSelectedBands(drawing, enhanceFileName);
        }

        private void SaveImageEnhance(IRasterDrawing drawing,out string enhanceFileName)
        {
            enhanceFileName = null;
            string filename = GetEhanceFilenameFromRaster(drawing);
            if (string.IsNullOrEmpty(filename))
                return;
            string initDir = GetDir(filename);
            if (!Directory.Exists(initDir))
                Directory.CreateDirectory(initDir);
            using (SaveFileDialog dlg = new SaveFileDialog())
            {
                dlg.FileName = Path.GetFileName(filename);
                dlg.Filter = "xml文件(*.xml)|*.xml";
                dlg.InitialDirectory = initDir;
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    enhanceFileName = dlg.FileName;
                    drawing.RgbProcessorStack.SaveTo(enhanceFileName);
                }
            }
        }

        private void SaveSelectedBands(IRasterDrawing drawing, string enhanceFileName)
        {
            if (string.IsNullOrEmpty(enhanceFileName))
                return;
            int[] bands = drawing.SelectedBandNos;
            if (bands == null || bands.Length == 0)
                return;
            string bandString = bands[0].ToString();
            for (int i = 1; i < bands.Length; i++)
                bandString += "," + bands[i].ToString();
            string bandsFname = Path.GetDirectoryName(enhanceFileName) + "\\" + Path.GetFileNameWithoutExtension(enhanceFileName) + ".bands";
            File.WriteAllText(bandsFname, bandString);
        }
        
        private string GetDir(string _enhanceName)
        {
            string dir = AppDomain.CurrentDomain.BaseDirectory + "图像增强方案";
            string[] parts = _enhanceName.Split('_');
            return dir + "\\" + parts[0];
        }

        private string GetEhanceFilenameFromRaster(IRasterDrawing drawing)
        {
            IMonitoringSession msession = _session.MonitoringSession as IMonitoringSession;
            string product = GetProduct(msession);

            return ImageEnhanceFactory.GetEnhanceNameByRasterFileName(drawing.DataProviderCopy, product);
        }

        private string GetProduct(IMonitoringSession session)
        {
            if (session == null)
                return "其它";
            if (session.ActiveMonitoringProduct == null)
                return "其它";
            return session.ActiveMonitoringProduct.Name;
        }
        #endregion

        #region apply
        /// <summary>
        /// 
        /// </summary>
        /// <param name="enhancePath">图像方案的绝对路径</param>
        public void ApplyImageEnhance(string enhancePath)
        {
            if (string.IsNullOrEmpty(enhancePath) || !File.Exists(enhancePath))
                return;
            if (_session.SmartWindowManager == null)
                return;
            ICanvasViewer viewer = _session.SmartWindowManager.ActiveCanvasViewer;
            if (viewer == null)
                return;
            IRasterDrawing drawing = viewer.ActiveObject as IRasterDrawing;
            if (drawing == null)
                return;
            //若为子产品，仍不应用方案
            if (drawing.DataProviderCopy != null)
            {
                string fileName = drawing.DataProviderCopy.fileName;
                string ext = Path.GetExtension(fileName).ToUpper();
                RasterIdentify rid = new RasterIdentify(fileName);
                if (rid.SubProductIdentify != null && ext == ".DAT")
                    return;
            }
            ReadSelectedBands(enhancePath, viewer, drawing);
            ReadImageEnhance(enhancePath, viewer, drawing);
        }

        private void ReadSelectedBands(string argument, ICanvasViewer viewer, IRasterDrawing drawing)
        {
            string bandsFile = Path.GetDirectoryName(argument) + "\\" + Path.GetFileNameWithoutExtension(argument) + ".bands";
            if (!File.Exists(bandsFile))
                return;
            string bandString = File.ReadAllText(bandsFile);
            string[] parts = bandString.Split(',');
            if (parts.Length == 1) //灰度
            {
                drawing.SelectedBandNos = new int[] { int.Parse(bandString) };
                viewer.Canvas.Refresh(Core.DrawEngine.enumRefreshType.RasterLayer);
                return;
            }
            else if (parts.Length == 3) //RGB
            {
                int[] bands = new int[3];
                for (int i = 0; i < 3; i++)
                    bands[i] = int.Parse(parts[i]);
                drawing.SelectedBandNos = bands;
                viewer.Canvas.Refresh(Core.DrawEngine.enumRefreshType.RasterLayer);
            }
        }

        private static void ReadImageEnhance(string argument, ICanvasViewer viewer, IRasterDrawing drawing)
        {
            IRgbProcessorStack stack = drawing.RgbProcessorStack;
            stack.Clear();
            stack.ReadXmlElement(argument);
            viewer.Canvas.Refresh(Core.DrawEngine.enumRefreshType.RasterLayer);
        }
        #endregion
    }
}
