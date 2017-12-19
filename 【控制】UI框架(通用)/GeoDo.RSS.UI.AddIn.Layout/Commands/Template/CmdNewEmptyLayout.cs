using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using GeoDo.RSS.Core.UI;
using GeoDo.RSS.Layout.DataFrm;
using GeoDo.RSS.Layout;
using System.Drawing;
using GeoDo.RSS.Core.DrawEngine;
using GeoDo.RSS.Core.VectorDrawing;
using GeoDo.RSS.Layout.GDIPlus;

namespace GeoDo.RSS.UI.AddIn.Layout
{
    [Export(typeof(ICommand)), ExportMetadata("VERSION", "1")]
    public class CmdNewEmptyLayout : GeoDo.RSS.Core.UI.Command
    {
        public CmdNewEmptyLayout()
            : base()
        {
            _id = 6000;
            _name = "NewEmptyLayout";
            _text = _toolTip = "新建专题图";
        }

        /// <summary>
        /// 新建专题图
        /// </summary>
        /// <param name="argument">专题图标题:[厘米|像素]</param>
        public override void Execute(string argument)
        {
            LayoutViewer lv = new LayoutViewer(argument);
            lv.SetSession(_smartSession);
            _smartSession.SmartWindowManager.DisplayWindow(lv);
            AddDataFrame(lv);
            lv.LayoutHost.Render();
            lv.LayoutHost.ToSuitedSize();
        }

        private void AddDataFrame(LayoutViewer lv)
        {
            // 这里是绘制专题图Frame的，如果需要给专题图增加外边框，一个建议是
            // 将xbank、ybank设置大一些，多出来的部分，可作为外边框，然后增加外边框的
            // 对象，等相关内容，方法类似于 LayoutViewer 的绘制
            // LayoutViewer 在容器中定义了画布，新增加的外框，应该是画布中，
            // 强制绘制的一个多边形对象
            // 新增外边框完成以后，需要至少改进格网的绘制方法，使数字能落在网格和
            // DataFrame 外边框之间
            DataFrame df = new DataFrame(lv.LayoutHost);           
            ILayoutHost host = lv.LayoutHost;
            SizeF sizef = host.LayoutRuntime.Layout.Size;
            float xbank = 1;//cm
            float ybank = 1;//cm
            xbank = host.LayoutRuntime.Centimeter2Pixel(xbank);
            ybank = host.LayoutRuntime.Centimeter2Pixel(ybank);
            host.LayoutRuntime.Pixel2Layout(ref xbank, ref ybank);
            sizef = new SizeF(sizef.Width - 2 * xbank, sizef.Height - 2 * ybank);
            df.Size = sizef;
            df.Location = new PointF(xbank, ybank);
            df.Update(host);
            AttachVectorHost(df);           
            host.LayoutRuntime.Layout.Elements.Add(df);
            //host.ActiveDataFrame = df;
            df.IsLocked = true;
        }

        private void AttachVectorHost(DataFrame df)
        {
            IDataFrameDataProvider provider = df.Provider as IDataFrameDataProvider;
            if (provider == null)
                return;
            ICanvas c = provider.Canvas;
            if (c != null)
            {
                if (c.CanvasSetting != null)
                    if (c.CanvasSetting.RenderSetting != null)
                        c.CanvasSetting.RenderSetting.BackColor = Color.White;
                IVectorHostLayer vhost = new VectorHostLayer(null);
                c.LayerContainer.Layers.Add(vhost as GeoDo.RSS.Core.DrawEngine.ILayer);
                c.SetToChinaEnvelope();
            }
        }
    }
}
