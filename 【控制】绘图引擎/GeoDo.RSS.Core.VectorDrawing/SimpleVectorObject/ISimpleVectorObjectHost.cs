using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeCell.AgileMap.Core;

namespace GeoDo.RSS.Core.VectorDrawing
{
    public interface ISimpleVectorObjectHost:IDisposable
    {
        ISimpleVectorObject[] GetAllVectorObjects();
        Feature[] GetAllFeatures();
        void Add(ISimpleVectorObject obj);
        void Remove(Func<ISimpleVectorObject, bool> where);
        void RemoveFeature(int OID);
        LabelDef LabelSetting { get; }
        ISymbol Symbol { get; set; }
    }
}
