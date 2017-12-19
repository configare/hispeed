using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.UI;
using System.ComponentModel.Composition;
using GeoDo.RSS.Core.RasterDrawing;
using System.Windows.Forms;
using System.IO;
using GeoDo.RSS.Core.CA;

namespace GeoDo.RSS.UI.AddIn.DataPro
{
    [Export(typeof(ICommand)), ExportMetadata("VERSION", "1")]
    public class cmdClosePanAdjustTool2 : Command
    {
        public cmdClosePanAdjustTool2()
            : base()
        {
            _id = 30106;
            _text = _name = "退出平移校正";
        }

        public override void Execute()
        {
            Execute("GeoAdjustByPan2");
        }

        public override void Execute(string argument)
        {
            ISmartViewer viewer = _smartSession.SmartWindowManager.ActiveViewer;
            if (viewer != null)
            {
                ICanvasViewer canViewer = viewer as ICanvasViewer;
                if (canViewer == null)
                    return;
                IRasterDrawing rd = canViewer.ActiveObject as IRasterDrawing;
                if (rd == null || rd.RgbProcessorStack == null)
                    return;
                List<IRgbProcessor> rgbProcessors = new List<IRgbProcessor>();
                foreach (IRgbProcessor processor in rd.RgbProcessorStack.Processors)
                    rgbProcessors.Add(processor);
                rgbProcessors.Reverse();
                int[] selectBands = rd.SelectedBandNos;
                IGeoPanAdjust adjust = rd as IGeoPanAdjust;
                if (adjust != null)
                {
                    if (adjust.IsHasUnsavedGeoAdjusted)
                    {
                        DialogResult ret = MsgBox.ShowQuestionYesNoCancel("对当前影像的平移校正结果未保存,请确认是否保存？\n按【是】保存。\n按【否】不保存。\n按【取消】返回。");
                        if (ret == DialogResult.Cancel)
                            return;
                        else if (ret == DialogResult.Yes)
                        {
                            PanAdjustTool adjustTool = new PanAdjustTool();
                            string fileName = null;
                            try
                            {
                                fileName = adjustTool.SaveGeoAdjust(rd.EnvelopeBeforeAdjusting, rd.DataProviderCopy);
                            }
                            catch (Exception e)
                            {
                                MessageBox.Show(e.Message);
                            }
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
                            }
                        }
                        else
                            adjust.Cancel();
                    }
                    else
                    {
                        string dirTXTFname = AppDomain.CurrentDomain.BaseDirectory + "MonitoringProductArgs\\COMM\\AdjustSaveFile.txt";
                        if (File.Exists(dirTXTFname))
                        {
                            string[] fileNames = File.ReadAllLines(dirTXTFname, Encoding.Default);
                            int length = fileNames.Length;
                            string lastFile = fileNames[length - 1];
                            AdjustConfig config = new AdjustConfig();
                            bool IsOpenResult = true;
                            bool.TryParse(config.GetConfigValue("IsOpenResult"), out IsOpenResult);
                            if (IsOpenResult && File.Exists(lastFile))
                            {
                                try
                                {
                                    OpenFileFactory.Open(lastFile);
                                    IRasterDrawing draw = _smartSession.SmartWindowManager.ActiveViewer.ActiveObject as IRasterDrawing;
                                    draw.SelectedBandNos = selectBands;
                                    if (rgbProcessors != null && rgbProcessors.Count > 0)
                                    {
                                        draw.RgbProcessorStack.Clear();
                                        foreach (IRgbProcessor processor in rgbProcessors)
                                            draw.RgbProcessorStack.Process(processor);
                                    }
                                }
                                catch (Exception e)
                                {
                                    MessageBox.Show(e.Message);
                                }
                            }
                            File.Delete(dirTXTFname);
                        }
                    }
                }
            }
            _smartSession.UIFrameworkHelper.SetVisible(argument, false);
            _smartSession.UIFrameworkHelper.SetLockBesideX(argument, false);
        }
    }
}

