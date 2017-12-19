using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GeoDo.RSS.BlockOper;
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.Core.DrawEngine;
using GeoDo.RSS.Core.RasterDrawing;
using GeoDo.RSS.Core.UI;
using System.Drawing;
using System.IO;
using GeoDo.RSS.Core.CA;

namespace GeoDo.RSS.UI.AddIn.DataPro
{
    [Export(typeof(ICommand))]
    public class CommandCut : GeoDo.RSS.Core.UI.Command
    {
        public CommandCut()
        {
            _id = 4999;
            _name = "裁切";
            _text = "裁切";
            _toolTip = "Cut";
        }

        public override void Execute()
        {
            try
            {
                List<string> _cutfiles = new List<string>();
                base.Execute();
                RasterClip s = new RasterClip();
                BlockDefWithAOI[] envelopes;
                string inputFilename = TryGetInfoFromActiveView(out envelopes);
                string outdir = Path.GetDirectoryName(inputFilename);
                int[] aoiIndex = null;
                List<BlockDefWithAOI> blockList = new List<BlockDefWithAOI>();
                frmImageClip frm = new frmImageClip(_smartSession);
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    _cutfiles.AddRange(frm.InputFiles);
                    _cutfiles.AddRange(frm.CustomSelectFiles);
                    BlockDefWithAOI outEnvelope;
                    Size size;
                    frm.GetArgs(out outEnvelope, out size, out outdir, out inputFilename);
                    envelopes = new BlockDefWithAOI[] { outEnvelope };
                    aoiIndex = frm.GetFeatureAOIIndex();
                    blockList.AddRange(envelopes);
                    if (aoiIndex != null && aoiIndex.Length != 0)
                        blockList[0].AOIIndexes = aoiIndex;
                }
                else
                    return;
                if (blockList[0].AOIIndexes == null)
                    blockList[0].AOIIndexes = _smartSession.SmartWindowManager.ActiveCanvasViewer.AOIProvider.GetIndexes();
                string[] put = s.RasterClipT(inputFilename, blockList.ToArray(), outdir, _smartSession.ProgressMonitorManager.DefaultProgressMonitor, "Cut");
                _cutfiles = _cutfiles.Distinct().ToList();
                foreach (string file in _cutfiles)
                {
                    s.RasterClipT(file, blockList.ToArray(), outdir, _smartSession.ProgressMonitorManager.DefaultProgressMonitor, "Cut");
                }
                ISmartViewer viewer = _smartSession.SmartWindowManager.ActiveViewer;
                int[] selectBands = null;
                List<IRgbProcessor> rgbProcessors = null;
                if (viewer != null)
                {
                    ICanvasViewer canViewer = viewer as ICanvasViewer;
                    if (canViewer == null)
                        return;
                    IRasterDrawing rd = canViewer.ActiveObject as IRasterDrawing;
                    if (rd == null || rd.RgbProcessorStack == null)
                        return;

                    rgbProcessors = new List<IRgbProcessor>();
                    foreach (IRgbProcessor processor in rd.RgbProcessorStack.Processors)
                        rgbProcessors.Add(processor);
                    rgbProcessors.Reverse();
                    selectBands = rd.SelectedBandNos;
                }
                for (int i = 0; i < put.Length; i++)
                {
                    OpenFileToWindows(put[i]);
                    if (selectBands == null)
                    {
                        continue;
                    }
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
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "提示信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private string TryGetInfoFromActiveView(out BlockDefWithAOI[] envelopes)
        {
            envelopes = null;
            ICanvasViewer canViewer = _smartSession.SmartWindowManager.ActiveCanvasViewer;
            if (canViewer == null)
                throw new Exception("未获得激活的数据窗口");
            IAOIProvider aoiProvider = canViewer.AOIProvider;
            if (aoiProvider == null)
                throw new Exception("未从激活的数据窗口中获取感兴趣区域");
            IRasterDrawing drawing = _smartSession.SmartWindowManager.ActiveCanvasViewer.Canvas.PrimaryDrawObject as IRasterDrawing;
            IRasterDataProvider rdp = drawing.DataProvider;
            if (rdp == null)
                throw new Exception("未从激活的数据窗口中获取数据提供者");
            if (rdp.DataIdentify != null && rdp.DataIdentify.IsOrbit)
                throw new NotSupportedException("暂未支持轨道数据的裁切");
            List<BlockDefWithAOI> enves = new List<BlockDefWithAOI>();
            AOIItem[] aoiitems = aoiProvider.GetAOIItems();
            if (aoiitems != null && aoiitems.Length != 0)
            {
                for (int i = 0; i < aoiitems.Length; i++)
                {
                    enves.Add(new BlockDefWithAOI(aoiitems[i].Name, aoiitems[i].GeoEnvelope.MinX, aoiitems[i].GeoEnvelope.MinY, aoiitems[i].GeoEnvelope.MaxX, aoiitems[i].GeoEnvelope.MaxY));
                }
            }
            envelopes = enves.ToArray();
            return rdp.fileName;
        }

        private void OpenFileToWindows(string file)
        {
            OpenFileFactory.Open(file);
        }
    }
}
