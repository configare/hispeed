using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeCell.AgileMap.Core;

namespace GeoDo.RSS.MIF.Core
{
    public interface ILabelLayer:IDisposable
    {
        ISymbol Symbol { get; set; }
        LabelDef LabelDef { get; }
        CodeCell.AgileMap.Core.Feature[] GetAllFeature();
        void AddFeature(CodeCell.AgileMap.Core.Feature feature);
        void RemoveFeature(CodeCell.AgileMap.Core.Feature feature);
        void RemoveAll();
        void Refresh();
        void SaveToFile(string shpFileName);
    }
}
