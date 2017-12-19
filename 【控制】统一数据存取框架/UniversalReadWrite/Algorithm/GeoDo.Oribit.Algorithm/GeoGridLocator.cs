using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoDo.Oribit.Algorithm
{
    public class GeoGridLocator : IDisposable
    {
        private float[] _lonValues;
        private float[] _latValues;
        private int _width = 0;
        private int _height = 0;

        public GeoGridLocator(float[] lonValues, float[] latValues, int width, int height)
        {
            _lonValues = lonValues;
            _latValues = latValues;
            _width = width;
            _height = height;
        }

        public void Dispose()
        {
            _lonValues = null;
            _latValues = null;
        }
    }
}
