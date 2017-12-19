using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.Core.DF
{
    public interface ICoordTransform:IDisposable
    {
        void Raster2DataCoord(int row, int col, out double coordX, out double coordY);
        void Raster2DataCoord(int row, int col, double[] coord);
        double[] GTs { get; }
    }
}
