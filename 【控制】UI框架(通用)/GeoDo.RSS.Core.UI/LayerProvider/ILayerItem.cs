using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace GeoDo.RSS.Core.UI
{
    public interface ILayerItem
    {
        string Name { get; set; }
        string Text { get; }
        Image Image { get; }
        enumLayerTypes LayerType { get; }
        bool IsSelected { get; set; }
        bool IsVisible { get; set; }
        bool IsAllowSelectable { get; }
        bool IsAllowVisiable { get; }
        object Tag { get; }
    }
}
