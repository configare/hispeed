using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;

namespace GeoDo.RSS.Layout
{
    public interface ILayoutHost : IDisposable, IElementsEditOperator
    {
        /// <summary>
        /// 活动数据框
        /// </summary>
        IDataFrame ActiveDataFrame { get; set; }
        Control Container { get; }
        Size CanvasSize { get; }
        float DPI { get; }
        EventHandler CanvasSizeChanged { get; set; }
        ILayoutRuntime LayoutRuntime { get; }
        ILayoutTemplate Template { get; }
        void Render();
        void Render(bool strongRefreshData);
        void ToSuitedSize();
        void ToSuitedSize(ILayout layout);
        /// <summary>
        /// 1:1
        /// </summary>
        /// <returns></returns>
        Bitmap ExportToBitmap(PixelFormat pixelFormat);
        Bitmap ExportToBitmap(PixelFormat pixelFormat, Size size);
        /// <summary>
        /// 实际缩放尺寸
        /// </summary>
        /// <returns></returns>
        Bitmap SaveToBitmap();
        IControlTool CurrentTool { get; set; }
        ISelectedEditBoxManager SelectedEditBoxManager { get; }
        void ApplyTemplate(ILayoutTemplate template);
        void ApplyArguments(params object[] arguments);
        ILayoutTemplate ToTemplate();
        void SetActiveDataFrame2CurrentTool();
        void SaveAsDocument(string gxdfilename);
        void ApplyGxdDocument(IGxdDocument doc);
        //
        EventHandler OnElementIsDragDroped { get; set; }
    }
}
