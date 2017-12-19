using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.Oribit.Algorithm
{
    public unsafe interface IOribitDataCoordLocator
    {
        void CoordLocate(float[] lonsBuffer,float[] latsBuffer, int width, int height, float lon,float lat, ref int row, ref int col);
    }
}
