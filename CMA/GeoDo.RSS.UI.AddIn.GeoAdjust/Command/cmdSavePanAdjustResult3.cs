using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using GeoDo.RSS.Core.UI;
using GeoDo.RSS.Core.RasterDrawing;
using System.Windows.Forms;
using System.IO;
using GeoDo.RSS.UI.AddIn.DataPro;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.Core.CA;

namespace GeoDo.RSS.UI.AddIn.GeoAdjust
{
    [Export(typeof(ICommand)), ExportMetadata("VERSION", "1")]
    public class cmdSavePanAdjustResult3 : Command
    {
        public cmdSavePanAdjustResult3()
            : base()
        {
            _id = 30203;
            _text = _name = "保存平移校正3";
        }

        public override void  Execute()
        {
            ISmartViewer viewer = _smartSession.SmartWindowManager.ActiveViewer;
            if (viewer == null)
                return;
            ICanvasViewer canViewer = viewer as ICanvasViewer;
            IRasterDrawing rd = canViewer.ActiveObject as IRasterDrawing;
            if (rd == null || rd.RgbProcessorStack == null)
                return;
            List<IRgbProcessor> rgbProcessors = new List<IRgbProcessor>();
            foreach (IRgbProcessor processor in rd.RgbProcessorStack.Processors)
                rgbProcessors.Add(processor);
            rgbProcessors.Reverse();
            int[] selectBands = rd.SelectedBandNos;
            IGeoPanAdjust adjust = rd as IGeoPanAdjust;
            if (rd == null)
                return;
            GeoAdjustHelper helper = new GeoAdjustHelper();
            string fileName = null;
            string dblvFileName = null;
            try
            {
                CoordEnvelope envelopeAfter = new CoordEnvelope(rd.Envelope.MinX,rd.Envelope.MinY, rd.Envelope.Width, rd.Envelope.Height);
                fileName = helper.SaveGeoAdjustByChangeCoordEnvelope(envelopeAfter, rd.DataProviderCopy);
                //调整对应的判识结果文件
                dblvFileName = AdjustDBLVFile(envelopeAfter, rd.DataProviderCopy);
                adjust.Cancel();
                if (File.Exists(fileName))
                {
                    OpenFileFactory.Open(fileName);
                    IRasterDrawing draw = _smartSession.SmartWindowManager.ActiveViewer.ActiveObject as IRasterDrawing;
                    draw.SelectedBandNos = selectBands;
                    if (rgbProcessors != null && rgbProcessors.Count > 0)
                    {
                        draw.RgbProcessorStack.Clear();
                        foreach (IRgbProcessor processor in rgbProcessors)
                            draw.RgbProcessorStack.Process(processor);
                    }
                    //生成多通道合成图
                    LayoutCreater layoutCreater = new LayoutCreater();
                    string mcsiFileName = layoutCreater.CreateMCSI(_smartSession);
                    if (!string.IsNullOrEmpty(mcsiFileName) && File.Exists(mcsiFileName))
                    {
                        OpenFileFactory.Open(mcsiFileName);
                    }
                    //生成二值图
                    if (!string.IsNullOrEmpty(dblvFileName))
                    {
                        string imgFileName = layoutCreater.CreateDBLVLayout(dblvFileName,draw.FileName);
                        if (!string.IsNullOrEmpty(imgFileName) && File.Exists(imgFileName))
                        {
                            OpenFileFactory.Open(imgFileName);
                        }
                    }
                }
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
                    if (dblvFileName != null && File.Exists(dblvFileName))
                        sw.WriteLine(dblvFileName);
                }
            }
        }

        private string AdjustDBLVFile(Core.DF.CoordEnvelope coordEnvelope, IRasterDataProvider rasterDataPrd)
        {
            List<string> fileList = new List<string>();
            bool isDat = false;
            string dir = Path.GetDirectoryName(rasterDataPrd.fileName);
            if (Path.GetExtension(rasterDataPrd.fileName).ToUpper() == ".DAT")
            {
                fileList.AddRange(Directory.GetFiles(dir, "*.ldf"));
                isDat = true;
            }
            else
            {
                fileList.AddRange(Directory.GetFiles(dir, "*.dat"));
                fileList.AddRange(Directory.GetFiles(dir, "*.mvg"));
            }
            if (fileList.Count < 1)
                return null;
            string dblvFileName = null;
            RasterIdentify ri = new RasterIdentify(rasterDataPrd.fileName);
            foreach (string item in fileList)
            {
                RasterIdentify riItem = new RasterIdentify(item);
                if (riItem.OrbitDateTime == ri.OrbitDateTime)
                {
                    if (!isDat)
                    {
                        if (riItem.SubProductIdentify == "DBLV")
                        {
                            dblvFileName = item;
                            break;
                        }
                    }
                    else
                    {
                        dblvFileName = item;
                        break;
                    }
                }
            }
            if (dblvFileName == null)
                return null;
            //找到对应判识结果文件，进行校正
            GeoAdjustHelper adjustTool = new GeoAdjustHelper();
            IRasterDataProvider dblvDataPrd = GeoDataDriver.Open(dblvFileName) as IRasterDataProvider;
            return adjustTool.SaveGeoAdjustByChangeCoordEnvelope(coordEnvelope, dblvDataPrd);
        }
    }
}
