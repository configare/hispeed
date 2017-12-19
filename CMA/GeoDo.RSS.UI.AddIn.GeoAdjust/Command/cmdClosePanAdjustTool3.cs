using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using GeoDo.RSS.Core.UI;
using GeoDo.RSS.Core.RasterDrawing;
using GeoDo.RSS.Core.CA;
using System.Windows.Forms;
using System.IO;
using GeoDo.RSS.UI.AddIn.DataPro;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.Core.DF;

namespace GeoDo.RSS.UI.AddIn.GeoAdjust
{
    [Export(typeof(ICommand)), ExportMetadata("VERSION", "1")]
    public class cmdClosePanAdjustTool3:Command
    {
        public cmdClosePanAdjustTool3()
            : base()
        {
            _id = 30206;
            _text = _name = "退出平移校正";
        }

        public override void Execute()
        {
            Execute("GeoAdjustByPan3");
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
                            GeoAdjustHelper adjustTool = new GeoAdjustHelper();
                            string fileName = null;
                            string dblvFileName = null;
                            try
                            {
                                fileName = adjustTool.SaveGeoAdjustByChangeCoordEnvelope(rd.DataProvider.CoordEnvelope, rd.DataProviderCopy);
                                dblvFileName = AdjustDBLVFile(rd.DataProvider.CoordEnvelope, rd.DataProviderCopy);
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
                                    string imgFileName = layoutCreater.CreateDBLVLayout(dblvFileName, draw.FileName);
                                    if (!string.IsNullOrEmpty(imgFileName) && File.Exists(imgFileName))
                                    {
                                        OpenFileFactory.Open(imgFileName);
                                    }
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
                            if (File.Exists(lastFile))
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
            //_smartSession.UIFrameworkHelper.SetVisible(argument, false);
            //_smartSession.UIFrameworkHelper.SetLockBesideX(argument, false);
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
            return adjustTool.SaveGeoAdjustByChangeCoordEnvelope(coordEnvelope,dblvDataPrd);
        }
    }
}
