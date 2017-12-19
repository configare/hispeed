using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.UI;
using System.ComponentModel.Composition;
using GeoDo.RSS.Core.RasterDrawing;
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.BlockOper;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace GeoDo.RSS.UI.AddIn.DataPro
{
    [Export(typeof(ICommand)), ExportMetadata("VERSION", "1")]
    public class cmdSavePanAdjustResult2 : Command
    {
        public cmdSavePanAdjustResult2()
            : base()
        {
            _id = 30103;
            _text = _name = "保存平移校正2";
        }

        public override void  Execute()
        {
            ISmartViewer viewer = _smartSession.SmartWindowManager.ActiveViewer;
            if (viewer == null)
                return;
            ICanvasViewer canViewer = viewer as ICanvasViewer;
            IRasterDrawing rd = canViewer.ActiveObject as IRasterDrawing;
            IGeoPanAdjust adjust = rd as IGeoPanAdjust;
            if (rd == null)
                return;
            PanAdjustTool adjustTool = new PanAdjustTool();
            string fileName = null;
            try
            {
                fileName = adjustTool.SaveGeoAdjust(rd.EnvelopeBeforeAdjusting, rd.DataProviderCopy);
                adjust.Cancel();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            if (fileName!=null&&File.Exists(fileName))
            {
                string adjustTxtDir = AppDomain.CurrentDomain.BaseDirectory + "MonitoringProductArgs\\COMM\\";
                if (!Directory.Exists(adjustTxtDir))
                    Directory.CreateDirectory(adjustTxtDir);
                string txtFileName = Path.Combine(adjustTxtDir, "AdjustSaveFile.txt");
                using(StreamWriter sw = new StreamWriter(txtFileName,true,Encoding.Default))
                {
                    sw.WriteLine(fileName);
                }
            }
        }
    }
}
