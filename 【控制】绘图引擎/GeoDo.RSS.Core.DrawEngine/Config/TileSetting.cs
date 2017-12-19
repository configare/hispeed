using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.Core.DrawEngine
{
    public class TileSetting
    {
        private int _tileSize = 512;
        private int _sampleratio = 2;
        private int _powerOf2 = 9;

        public TileSetting()
        {
            
        }

        public TileSetting(int tileSize, int sampleRatio)
        {
            _tileSize = tileSize;
            _sampleratio = sampleRatio;
            //
            _powerOf2 = 0;
            int size = 1;
            while (size != _tileSize)
            {
                size *= 2;
                _powerOf2++;
            }
        }

        public int PowerOf2
        {
            get 
            {
                return _powerOf2;
            }
        }

        public int TileSize
        {
            get { return _tileSize; }
        }

        public int SampleRatio
        {
            get { return _sampleratio; }
        }
    }
}
