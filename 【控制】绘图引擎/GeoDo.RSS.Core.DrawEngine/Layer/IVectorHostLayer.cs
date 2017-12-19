using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace GeoDo.RSS.Core.DrawEngine
{
    public interface IVectorHostLayer
    {
        object Map { get; }
        void ChangeMap(string mcdfname);
        void AddData(string fname, object arguments);
        void ClearAll();
        object MapRuntime { get; }
        void Apply(string mcdfile);
        void Set(object canvas);
        EventHandler SomeDataIsArrivedHandler { get; set; }
        bool IsEnableDummyRender { get; set; }
    }
}
