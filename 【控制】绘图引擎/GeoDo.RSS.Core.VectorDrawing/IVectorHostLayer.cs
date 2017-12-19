using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeCell.AgileMap.Core;

namespace GeoDo.RSS.Core.VectorDrawing
{
    public interface IVectorHostLayer
    {
        IMap Map { get; }
        void ChangeMap(string mcdfname);
        void AddData(string fname, object arguments);
    }
}
