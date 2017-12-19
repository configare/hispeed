using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.Core.UI
{
    /// <summary>
    /// 视图窗口(文档窗口)
    /// </summary>
    public interface ISmartViewer:ISmartWindow
    {
        string Name { get; }
        string Title { get; }
        /// <summary>
        /// 视窗内的活动对象,eg:IRasterDrawing,IScaleBar,ITextBoxElement
        /// </summary>
        object ActiveObject { get; }
        void DisposeViewer();
    }
}
