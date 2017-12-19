using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using GeoDo.RSS.Core.UI;
using GeoDo.RSS.Core.RasterDrawing;
using GeoDo.RSS.Core.DF;
using System.Drawing;

namespace GeoDo.RSS.UI.AddIn.Tools
{
    [Export(typeof(ICommand)), ExportMetadata("VERSION", "1")]
    public class CommandColorTable : Command
    {
        private double _bandMinValue;
        private double _bandMaxValue;

        public CommandColorTable()
            : base()
        {
            _name = "CommandColorTable";
            _text = _toolTip = "线性填色";
            _id = 7104;
        }

        public override void Execute()
        {
            Execute(null);
        }

        public override void Execute(string argument)
        {
            ICanvasViewer viewer = _smartSession.SmartWindowManager.ActiveCanvasViewer;
            if (viewer == null)
                return;
            IRasterDrawing drawing = viewer.ActiveObject as IRasterDrawing;
            if (drawing.TileBitmapProvider.TileCountOfLoading > 0)
                return;
            if (drawing.SelectedBandNos.Length > 1)
            {
                MsgBox.ShowInfo("当前影像显示为RGB合成模式,系统将自动调整为灰度显示模式！");
                drawing.SelectedBandNos = new int[] { drawing.SelectedBandNos[0]};
            }
            IProgressMonitor progress = _smartSession.ProgressMonitorManager.DefaultProgressMonitor;
            try
            {
                progress.Reset("正在统计端值...", 100);
                progress.Start(false);
                if (drawing.TileBitmapProvider.TileCountOfLoading > 0)
                    return;
                IRasterBand band = drawing.DataProviderCopy.GetRasterBand(drawing.SelectedBandNos[0]);
                band.ComputeMinMax(out _bandMinValue, out _bandMaxValue, false,
                    (idx, tip) => { progress.Boost(idx); });
            }
            finally
            {
                progress.Finish();
            }
            using (frmColorRampEditor frm = new frmColorRampEditor())
            {
                frm.MinLimitValue = int.MinValue;
                frm.MaxLimitValue = int.MaxValue;
                //frm.OnStatMinValueAndMaxValue += new frmColorRampEditor.StatMinValueAndMaxValueHandler(frm_OnStatMinValueAndMaxValue);
                frm.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
                if (frm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    Color[] Colors = frm.ComputeColors(200);
                    ApplyColorItems(Colors);
                }
            }
        }

        //void frm_OnStatMinValueAndMaxValue(out int minValue, out int maxValue)
        //{
        //    minValue = 0;
        //    maxValue = 0;
        //    ICanvasViewer viewer = _smartSession.SmartWindowManager.ActiveCanvasViewer;
        //    if (viewer == null)
        //        return;
        //    IRasterDrawing drawing = viewer.ActiveObject as IRasterDrawing;
        //    IRasterBand band = drawing.DataProviderCopy.GetRasterBand(drawing.SelectedBandNos[0]);
        //    band.ComputeMinMax(out _bandMinValue, out _bandMaxValue, false,
        //        (idx, tip) =>
        //        {
        //        }
        //        );
        //    if (_bandMinValue < 0)
        //    {
        //        minValue = (int)Math.Floor(_bandMinValue + Math.Abs(_bandMinValue));
        //        maxValue = (int)(_bandMaxValue + Math.Abs(_bandMinValue));
        //    }
        //    else
        //    {
        //        minValue = (int)Math.Floor(_bandMinValue);
        //        maxValue = (int)Math.Ceiling(_bandMaxValue);
        //    }
        //}

        private void ApplyColorItems(Color[] colors)
        {
            ICanvasViewer viewer = _smartSession.SmartWindowManager.ActiveCanvasViewer;
            if (viewer == null)
                return;
            IRasterDrawing drawing = viewer.ActiveObject as IRasterDrawing;
            ColorMapTable<double> colorTable = GetColorTable(colors);
            if (colorTable == null)
                return;
            drawing.ApplyColorMapTable(colorTable);
            ICanvasViewer cv = _smartSession.SmartWindowManager.ActiveCanvasViewer;
            if (cv != null)
                cv.Canvas.Refresh(Core.DrawEngine.enumRefreshType.All);
        }

        private ColorMapTable<double> GetColorTable(Color[] colors)
        {
            ColorMapTable<double> ct = new ColorMapTable<double>();
            int count = colors.Length;
            double span = (_bandMaxValue - _bandMinValue) / count;
            double v = _bandMinValue;
            for (int i = 0; i < count; i++, v += span)
            {
                ct.Items.Add(new ColorMapItem<double>(v, v + span, colors[i]));
            }
            return ct;
        }

        private void GetCurrentBandNos(out int[] bandNos, out int bandCount)
        {
            bandNos = null;
            bandCount = 0;
            ICanvasViewer viewer = _smartSession.SmartWindowManager.ActiveCanvasViewer;
            if (viewer == null)
                return;
            IRasterDrawing drawing = viewer.ActiveObject as IRasterDrawing;
            bandNos = new int[drawing.BandCount];
            bandCount = drawing.BandCount;
            for (int i = 1; i <= bandCount; i++)
                bandNos[i - 1] = i;
        }
    }
}
