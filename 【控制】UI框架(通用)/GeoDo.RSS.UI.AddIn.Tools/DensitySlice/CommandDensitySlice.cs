using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using GeoDo.RSS.Core.UI;
using GeoDo.RSS.Core.RasterDrawing;
using GeoDo.RSS.Core.DF;
using System.Drawing;
using System.Windows.Forms;

namespace GeoDo.RSS.UI.AddIn.Tools
{
    [Export(typeof(ICommand)), ExportMetadata("VERSION", "1")]
    public class CommandDensitySlice : Command
    {
        private IRasterDrawing _drawing = null;

        public CommandDensitySlice()
            : base()
        {
            _name = "CommandDensitySlice";
            _text = _toolTip = "密度分割";
            _id = 7110;
        }

        /// <param name="argument">间隔数值类型，密度分割文件，选择波段，最小值，间隔值
        ///                        间隔数值类型：1：int 2:float,默认：int</param>
        public override void Execute()
        {
            ICanvasViewer viewer = _smartSession.SmartWindowManager.ActiveCanvasViewer;
            if (viewer == null)
                return;
            _drawing = viewer.ActiveObject as IRasterDrawing;
            IProgressMonitor progress = _smartSession.ProgressMonitorManager.DefaultProgressMonitor;
            using (DensitySliceForm frm = new DensitySliceForm(_drawing))
            {
                frm.UpdateDensSliceEvent += new DensitySliceForm.UpdateDensSlice(frm_UpdateDensSliceEvent);
                frm.Progress = progress;
                SetFormStatus(frm);
                if (frm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    _drawing.SelectedBandNos = new int[] { frm.SelectBand };
                    _drawing.ApplyColorMapTable(frm.ColorTable);
                    ICanvasViewer cv = _smartSession.SmartWindowManager.ActiveCanvasViewer;
                    if (cv != null)
                        cv.Canvas.Refresh(Core.DrawEngine.enumRefreshType.All);
                }
            }
        }

        public override void Execute(string argument)
        {
            ICanvasViewer viewer = _smartSession.SmartWindowManager.ActiveCanvasViewer;
            if (viewer == null)
                return;
            _drawing = viewer.ActiveObject as IRasterDrawing;
            IProgressMonitor progress = _smartSession.ProgressMonitorManager.DefaultProgressMonitor;
            using (DensitySliceForm frm = new DensitySliceForm(_drawing))
            {
                frm.UpdateDensSliceEvent += new DensitySliceForm.UpdateDensSlice(frm_UpdateDensSliceEvent);
                frm.Progress = progress;
                if (_drawing.SelectedBandNos.Length == 1)
                    frm.SelectBand = _drawing.SelectedBandNos[0];
                SetFormStatus(frm);
                if (!string.IsNullOrEmpty(argument))
                    InitFrmByArugment(frm, argument);
                if (frm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    _drawing.SelectedBandNos = new int[] { frm.SelectBand };
                    _drawing.ApplyColorMapTable(frm.ColorTable);
                    ICanvasViewer cv = _smartSession.SmartWindowManager.ActiveCanvasViewer;
                    if (cv != null)
                        cv.Canvas.Refresh(Core.DrawEngine.enumRefreshType.All);
                }
            }
        }

        private void InitFrmByArugment(DensitySliceForm frm, string argument)
        {
            string[] split = argument.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (split.Length < 4)
                return;
            frm.InitByArugment(split[0], int.Parse(split[1]), float.Parse(split[2]), float.Parse(split[3]));
        }

        private void SetFormStatus(DensitySliceForm frm)
        {
            frm.Owner = _smartSession.SmartWindowManager.MainForm as Form;
            frm.StartPosition = FormStartPosition.Manual;
            frm.Location = _smartSession.SmartWindowManager.ViewLeftUpperCorner;
        }

        void frm_UpdateDensSliceEvent(int band, ColorMapTable<double> colorMap)
        {
            _drawing.SelectedBandNos = new int[] { band };
            _drawing.ApplyColorMapTable(colorMap);
            ICanvasViewer cv = _smartSession.SmartWindowManager.ActiveCanvasViewer;
            if (cv != null)
                cv.Canvas.Refresh(Core.DrawEngine.enumRefreshType.All);
        }
    }
}
