using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.Core.RasterDrawing
{
    public class Tile<T>
    {
        protected TileIdentify _id;
        protected int _bandNo;
        protected T[] _data;

        public Tile(TileIdentify id,int bandNo,T[] data)
        {
            _id = id;
            _bandNo = bandNo;
            _data = data;
        }

        public TileIdentify Id
        { 
            get { return _id; } 
        }

        public int BandNo
        {
            get { return _bandNo; }
        }

        public T[] Data
        {
            get { return _data; }
        }
    }
}
