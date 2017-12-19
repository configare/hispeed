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

namespace GeoDo.RSS.UI.AddIn.DataPro
{
    [Export(typeof(ICommand))]
    public class CommandClip : GeoDo.RSS.Core.UI.Command
    {
        public CommandClip()
        {
            _id = 4200;
            _name = "分幅";
            _text = "分幅";
            _toolTip = "Clip";
        }

        public override void Execute()
        {
            try
            {
                base.Execute();
                RasterClip s = new RasterClip();
                BlockDef[] envelopes;
                string inputFilename = TryGetInfoFromActiveView(out envelopes);
                string outdir = Path.GetDirectoryName(inputFilename);
                if (string.IsNullOrWhiteSpace(inputFilename) || envelopes == null || envelopes.Length == 0 || envelopes.Length == 1)
                {
                    frmImageClip frm = new frmImageClip(_smartSession);
                    if (frm.ShowDialog() == DialogResult.OK)
                    {
                        BlockDef outEnvelope;
                        Size size;
                        frm.GetArgs(out outEnvelope, out size, out outdir, out inputFilename);
                        envelopes = new BlockDef[] { outEnvelope };
                    }
                    else
                        return;
                }
                List<BlockDef> blockList = new List<BlockDef>();
                for (int i = 0; i < envelopes.Length; i++)
                {
                    blockList.Add(envelopes[i]);
                }
                string[] put = s.RasterClipT(inputFilename, blockList.ToArray(), outdir, _smartSession.ProgressMonitorManager.DefaultProgressMonitor,"Clip");
                for (int i = 0; i < put.Length; i++)
                {
                    OpenFileToWindows(put[i]);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "提示信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        public override void Execute(string argument)
        {
            base.Execute(argument);
        }

        private string TryGetInfoFromActiveView(out BlockDef[] envelopes)
        {
            envelopes = null;
            ICanvasViewer canViewer = _smartSession.SmartWindowManager.ActiveCanvasViewer;
            if (canViewer == null)
                throw new Exception("未获得激活的数据窗口");
            IAOIProvider aoiProvider = canViewer.AOIProvider;
            if (aoiProvider == null)
                throw new Exception("未从激活的数据窗口中获取感兴趣区域");
            IRasterDrawing drawing = canViewer.Canvas.PrimaryDrawObject as IRasterDrawing;
            IRasterDataProvider rdp = drawing.DataProvider;
            if (rdp == null)
                throw new Exception("未从激活的数据窗口中获取数据提供者");
            if (rdp.DataIdentify != null && rdp.DataIdentify.IsOrbit)
                throw new NotSupportedException("暂未支持轨道数据的裁切");
            List<BlockDef> enves = new List<BlockDef>();
            AOIItem[] aoiitems = aoiProvider.GetAOIItems();
            if (aoiitems != null && aoiitems.Length != 0)
            {
                for (int i = 0; i < aoiitems.Length; i++)
                {
                    enves.Add(new BlockDef(aoiitems[i].Name, aoiitems[i].GeoEnvelope.MinX, aoiitems[i].GeoEnvelope.MinY, aoiitems[i].GeoEnvelope.MaxX, aoiitems[i].GeoEnvelope.MaxY));
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
