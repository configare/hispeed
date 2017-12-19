using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DrawEngine;
using System.Drawing;
using System.Runtime.InteropServices;

namespace GeoDo.RSS.Core.RasterDrawing
{
    public class MemoryRaster<T>:IMemoryRaster<T>
    {
        private string _identify;
        private T[] _data;
        private Size _size;
        private CoordEnvelope _envelope;
        private int _sizeOfDataType;
        private bool _isGeoCoordinate;

        public MemoryRaster(string identify,T[] data,Size size,
            int sizeOfDataType,CoordEnvelope envelope,bool isGeoCoordinate)
        {
            _identify = identify;
            _data = data;
            _size = size;
            _envelope = envelope;
            _sizeOfDataType = sizeOfDataType;
            _isGeoCoordinate = isGeoCoordinate;
        }

        public bool IsGeoCoordinate
        {
            get { return _isGeoCoordinate; }
        }

        public string Identify
        {
            get { return _identify; }
        }

        public T[] Data
        {
            get { return _data; }
        }

        public Size Size
        {
            get { return _size; }
        }

        public int SizeOfDataType
        {
            get { return _sizeOfDataType; }
        }

        public CoordEnvelope Envelope
        {
            get { return _envelope; }
        }

        public void Dispose()
        {
            _data = null;
        }
    }
}
