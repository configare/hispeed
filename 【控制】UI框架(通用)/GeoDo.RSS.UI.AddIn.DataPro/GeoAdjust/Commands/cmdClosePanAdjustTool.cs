using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.UI;
using System.ComponentModel.Composition;
using GeoDo.RSS.Core.RasterDrawing;
using System.Windows.Forms;

namespace GeoDo.RSS.UI.AddIn.DataPro
{
    [Export(typeof(ICommand)), ExportMetadata("VERSION", "1")]
    public class cmdClosePanAdjustTool : Command
    {
        public cmdClosePanAdjustTool()
            : base()
        {
            _id = 30006;
            _text = _name = "退出平移校正";
        }

        public override void Execute()
        {
            Execute("GeoAdjustByPan");
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
                IGeoPanAdjust adjust = rd as IGeoPanAdjust;
                if (adjust != null && adjust.IsHasUnsavedGeoAdjusted)
                {
                    DialogResult ret = MsgBox.ShowQuestionYesNoCancel("对当前影像的平移校正结果未保存,请确认是否保存？\n按【是】保存。\n按【否】不保存。\n按【取消】返回。");
                    if (ret == DialogResult.Cancel)
                        return;
                    else if (ret == DialogResult.Yes)
                        adjust.Save();
                    else
                        adjust.Cancel();
                }
            }
            _smartSession.UIFrameworkHelper.SetVisible(argument, false);
            _smartSession.UIFrameworkHelper.SetLockBesideX(argument, false);
        }
    }
}
