using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DrawEngine;
using System.Drawing;

namespace GeoDo.RSS.Layout.GDIPlus
{
    public interface IDataFrameDataProvider
    {
        ICanvas Canvas { get; }
        Bitmap GetBuffer(bool isClearBuffer);
    }
}
