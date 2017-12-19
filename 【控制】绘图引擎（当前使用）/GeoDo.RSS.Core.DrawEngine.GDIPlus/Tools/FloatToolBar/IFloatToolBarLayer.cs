using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.Core.DrawEngine.GDIPlus
{
    public interface IFloatToolBarLayer:IToolboxLayer
    {
        bool IsEnabled { get; set; }
        bool IsAutoHide { get; set; }
        List<FloatToolItem> ToolItems { get; }
        Action<FloatToolItem> ToolItemClicked { get; set; }
    }
}
