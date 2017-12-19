using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.Core.DrawEngine
{
    public interface ICanvasSettingParser:IDisposable
    {
        CanvasSetting Parse(string fname);
    }
}
