using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using GeoDo.RSS.Core.DrawEngine;
using CodeCell.AgileMap.Core;

namespace GeoDo.RSS.Core.UI
{
    public interface IAOIProvider:IDisposable
    {
        bool IsEmpty();
        int[] GetIndexes();
        int[] GetBitmapIndexes();
        int[] GetBitmapIndexes(Feature feature);
        Size BitmapSize { get; }
        Rectangle GetRasterRect();
        CoordEnvelope GetGeoRect();
        CoordEnvelope GetPrjRect();
        CoordEnvelope GetMinGeoRect();
        AOIItem[] GetAOIItems();
        void Reset();
    }
}
