using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace CodeCell.AgileMap.Core
{
    public interface ILayer : IDisposable, IPersistable
    {
        string Id { get; }
        string Name { get; set; }
        ScaleRange DisplayScaleRange { get; set; }
        IClass Class { get; set; }
        bool Disposed { get; }
        bool VisibleAtScale(int scale);
        bool IsReady { get; }
        bool IsRendered { get; }
    }
}
