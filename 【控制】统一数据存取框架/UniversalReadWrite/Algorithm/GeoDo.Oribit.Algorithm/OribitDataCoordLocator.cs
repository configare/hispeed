using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoDo.Oribit.Algorithm
{
    public class OribitDataCoordLocator : IOribitDataCoordLocator
    {
        public OribitDataCoordLocator()
        {
        }

        public unsafe void CoordLocate(float[] lonsBuffer, float[] latsBuffer, int width, int height, float lon, float lat, ref int row, ref int col)
        {
            fixed (float* lonsPtr = lonsBuffer, latsPtr = latsBuffer)
            { 
                //
            }
        }
    }
}
