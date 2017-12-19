#region Version Info
/*========================================================================
* 功能概述：
* 
* 创建者：admin     时间：2013-09-08 9:29:21
* ------------------------------------------------------------------------
* 变更记录：
* 时间：                 修改者：                
* 修改说明：
* 
* ------------------------------------------------------------------------
* ========================================================================
*/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.UI;
using System.ComponentModel.Composition;
using GeoDo.RSS.Core.RasterDrawing;
using System.Windows.Forms;
using System.Drawing;
using GeoDo.RSS.Core.DF;

namespace GeoDo.RSS.UI.AddIn.L2ColorTable
{
    /// <summary>
    /// 类名：L2ColortableEnhance
    /// 属性描述：定量产品分段填色
    /// 创建者：罗战克   创建日期：2013-09-08 9:29:21
    /// 修改者：             修改日期：
    /// 修改描述：
    /// 备注：
    /// </summary>
    [Export(typeof(ICommand))]
    public class CmdL2ColortableEnhance:Command
    {
        public CmdL2ColortableEnhance()
        {
            _id = 31101;
            _name = "ColortableEnhance";
            _text = "定量产品分段填色";
            _toolTip = "定量产品分段填色";
        }

        public override void Execute()
        {
            Execute("");
        }

        public override void Execute(string argument)
        {
            if (_smartSession == null)
                return;
            ICanvasViewer viewer = _smartSession.SmartWindowManager.ActiveCanvasViewer;
            if (viewer == null)
            {
                MsgBox.ShowInfo("没有激活的影像窗口，启动命令失败");
                return;
            }
            IRasterDrawing drawing = viewer.ActiveObject as IRasterDrawing;
            IProgressMonitor progress = _smartSession.ProgressMonitorManager.DefaultProgressMonitor;

            using (frmL2ColorTable frm = new frmL2ColorTable())
            {
                SetFormStatus(frm);
                frm.SetSession(_smartSession);
                if (frm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    if (RangeIsOK(drawing.DataProvider.DataType, frm.ColorTable))
                    {
                        ColorMapTable<double> colorTables = ToColorMapTable(frm.ColorTable);
                        //drawing.SelectedBandNos = new int[] { frm.SelectBand };
                        drawing.ApplyColorMapTable(colorTables);
                        ICanvasViewer cv = _smartSession.SmartWindowManager.ActiveCanvasViewer;
                        if (cv != null)
                            cv.Canvas.Refresh(Core.DrawEngine.enumRefreshType.RasterLayer);
                    }
                }
            }
        }

        /// <summary>
        /// 最大取255种
        /// </summary>
        /// <param name="bandValueColorPair"></param>
        /// <returns></returns>
        private ColorMapTable<double> ToColorMapTable(BandValueColorPair[] bandValueColorPair)
        {
            if (bandValueColorPair == null)
                return null;
            ColorMapTable<double> ranges = new ColorMapTable<double>();
            int maxCount = Math.Min(bandValueColorPair.Length, 254);
            for (int i = 0; i < maxCount; i++)
            //foreach (BandValueColorPair v in bandValueColorPair)
            {
                BandValueColorPair v = bandValueColorPair[i];
                //[min,max)
                ranges.Items.Add(new ColorMapItem<double>(v.MinValue, v.MinValue == v.MaxValue ? v.MaxValue : v.MaxValue + 1, Color.FromArgb(v.Color.R, v.Color.G, v.Color.B)));
            }
            return ranges;
        }

        private void SetFormStatus(Form frm)
        {
            frm.Owner = _smartSession.SmartWindowManager.MainForm as Form;
            frm.StartPosition = FormStartPosition.Manual;
            frm.Location = _smartSession.SmartWindowManager.ViewLeftUpperCorner;
        }

        public void GetLimitValueOfEditingImage( GeoDo.RSS.Core.DF.enumDataType dataType , out int limitSmallValue, out int limitLargeValue)
        {
            limitSmallValue = limitLargeValue = -1;
            switch (dataType)
            {
                case enumDataType.Byte:
                    limitSmallValue = 0;
                    limitLargeValue = 255;
                    break;
                case enumDataType.Float:
                case enumDataType.Double:
                    limitSmallValue = int.MinValue;
                    limitLargeValue = int.MaxValue;
                    break;
                case enumDataType.Int16:
                    limitSmallValue = Int16.MinValue;
                    limitLargeValue = Int16.MaxValue;
                    break;
                case enumDataType.Int32:
                    limitSmallValue = Int32.MinValue;
                    limitLargeValue = Int32.MaxValue;
                    break;
                case enumDataType.UInt16:
                    limitSmallValue = UInt16.MinValue;
                    limitLargeValue = UInt16.MaxValue;
                    break;
                case enumDataType.UInt32:
                    limitSmallValue = (int)UInt32.MinValue;
                    limitLargeValue = int.MaxValue;
                    break;
                default:
                    break;
            }
        }

        private bool RangeIsOK(GeoDo.RSS.Core.DF.enumDataType dataType, BandValueColorPair[] values)
        {
            if (values == null)
            {
                MsgBox.ShowInfo("选择索引空间不能应用与当前显示影像,请重新选择或者选择\"调色板填色\"功能进行填色。");
                return false;
            }
            //
            int minValue = int.MaxValue;
            int maxValue = int.MinValue;
            foreach (BandValueColorPair v in values)
            {
                if (v.MinValue < minValue)
                    minValue = v.MinValue;
                if (v.MaxValue > maxValue)
                    maxValue = v.MaxValue;
            }
            //
            int limitSmallValue = -1;
            int limitLargeValue = -1;
            GetLimitValueOfEditingImage(dataType, out limitSmallValue, out limitLargeValue);
            //
            if (maxValue < limitSmallValue || minValue > limitLargeValue)
            {
                MsgBox.ShowInfo("选择的\"索引空间\"不能应用与当前显示影像,请重新选择或者选择\"调色板填色\"功能进行填色。");
                return false;
            }
            return true;
        }
    }
}
