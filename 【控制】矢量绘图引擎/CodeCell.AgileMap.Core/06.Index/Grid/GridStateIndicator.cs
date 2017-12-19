using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace CodeCell.AgileMap.Core
{
    /// <summary>
    /// 左下角为坐标原点
    /// 向上Y增大(+)
    /// 向右X增大(+)
    /// </summary>
    public class GridStateIndicator:IDisposable
    {
        private int _width = 0;
        private int _height = 0;
        private Envelope _fullEnvelope = null;
        private GridDefinition _gridDef = null;
        private byte[] _loadedFlags = null;
        //容差
        protected double _tolerance = 0;
        //
        private const int cstUnFlag = 0;
        private const int cstFlaging = 1;
        private const int cstFlaged = 2;
        //
        private bool _unFlagedIsEmpty = false;

        public GridStateIndicator(Envelope fullEnvelope, GridDefinition gridDefinition)
        {
            _fullEnvelope = fullEnvelope;
            _gridDef = gridDefinition;
            float  fwidth = (int)Math.Ceiling((_fullEnvelope.Width / _gridDef.SpanX));
            float fheight = (int)Math.Ceiling((_fullEnvelope.Height / _gridDef.SpanY));
            while (_width * _height > 360 * 180) //64800块
            {
                _gridDef.SpanX *= 10;
                _gridDef.SpanY *= 10;
                fwidth = (int)Math.Ceiling((_fullEnvelope.Width / _gridDef.SpanX));
                fheight = (int)Math.Ceiling((_fullEnvelope.Height / _gridDef.SpanY));
            }
            _width = (int)Math.Ceiling(fwidth);
            _height = (int)Math.Ceiling(fheight);
            _loadedFlags = new byte[_width * _height];
            _tolerance = _fullEnvelope.Width / 1000000d;
        }

        public int Width
        {
            get { return _width; }
        }

        public int Height
        {
            get { return _height; }
        }

        public Envelope FullEnvelope
        {
            get { return _fullEnvelope; }
        }

        public int GetGridNo(ShapePoint pt)
        {
            int row = (int)((pt.Y - _fullEnvelope.MinY) / _gridDef.SpanY);
            int col = (int)((pt.X - _fullEnvelope.MinX) / _gridDef.SpanX);
            return row * _width + col;
        }

        public int[] GetGridNos(Envelope envelope)
        {
            int bc = (int)((envelope.MinX - _fullEnvelope.MinX) / _gridDef.SpanX);
            int ec = (int)((envelope.MaxX - _fullEnvelope.MinX) / _gridDef.SpanX);
            int br = (int)((envelope.MinY - _fullEnvelope.MinY) / _gridDef.SpanY);
            int er = (int)((envelope.MaxY - _fullEnvelope.MinY) / _gridDef.SpanY);
            br = Math.Max(0, br);
            er = Math.Min(_height - 1, er);
            bc = Math.Max(0, bc);
            ec = Math.Min(_width - 1, ec);
            List<int> gridNos = new List<int>();
            for (int r = br; r <= er; r++)
                for (int c = bc; c <= ec; c++)
                    gridNos.Add(r * _width + c);
            return gridNos.Count > 0 ? gridNos.ToArray() : null;
        }

        public void GetGridNoRange(Envelope envelope, out int bRow, out int eRow, out int bCol, out int eCol)
        {
            int bc = (int)((envelope.MinX - _fullEnvelope.MinX) / _gridDef.SpanX);
            int ec = (int)((envelope.MaxX - _fullEnvelope.MinX) / _gridDef.SpanX);
            int br = (int)((envelope.MinY - _fullEnvelope.MinY) / _gridDef.SpanY);
            int er = (int)((envelope.MaxY - _fullEnvelope.MinY) / _gridDef.SpanY);
            bRow = Math.Max(0, br);
            eRow = Math.Min(_height - 1, er);
            bCol = Math.Max(0, bc);
            eCol = Math.Min(_width - 1, ec);
        }
 
        public int GetUnFlagedCount()
        {
            if (_unFlagedIsEmpty)
                return 0;
            int n =0;
            for (int i = 0; i < _loadedFlags.Length; i++)
                if (_loadedFlags[i] == cstUnFlag || _loadedFlags[i] == cstFlaging)
                    n++;
            if (n == 0)
                _unFlagedIsEmpty = true;
            return n;
        }

        public void Flaging(int gridNo)
        {
            _loadedFlags[gridNo] = cstFlaging;
        }

        public void Flaged(int gridNo)
        {
            _loadedFlags[gridNo] = cstFlaged;
        }

        public void Flaged(int[] gridNos)
        {
            for (int i = 0; i < gridNos.Length; i++)
                _loadedFlags[gridNos[i]] = cstFlaged;
        }

        public void UnFlaged(int gridNo)
        {
            _loadedFlags[gridNo] = cstUnFlag;
            _unFlagedIsEmpty = false;
        }

        public void Flaging(int[] gridNos)
        {
            for (int i = 0; i < gridNos.Length; i++)
                _loadedFlags[gridNos[i]] = cstFlaging;
        }

        public Envelope GetEnvelope(int gridNo)
        {
            int r = (int)(gridNo / _width);
            int c = gridNo - r * _width;
            Envelope evp = new Envelope(_fullEnvelope.MinX + c * _gridDef.SpanX - _tolerance, _fullEnvelope.MinY + r * _gridDef.SpanY - _tolerance, 0, 0);
            evp.MaxX = Math.Min(evp.MinX + _gridDef.SpanX,_fullEnvelope.MaxX) + _tolerance;
            evp.MaxY = Math.Min(evp.MinY + _gridDef.SpanY,_fullEnvelope.MaxY) + _tolerance;
            return evp ;
        }

        public void GetDiffGridNos(Envelope envelope, List<int> retGridNos)
        {
            int bc = (int)((envelope.MinX - _fullEnvelope.MinX) / _gridDef.SpanX);
            int ec = (int)((envelope.MaxX - _fullEnvelope.MinX) / _gridDef.SpanX);
            int br = (int)((envelope.MinY - _fullEnvelope.MinY) / _gridDef.SpanY);
            int er = (int)((envelope.MaxY - _fullEnvelope.MinY) / _gridDef.SpanY);
            br = Math.Max(0, br);
            er = Math.Min(_height - 1, er);
            bc = Math.Max(0, bc);
            ec = Math.Min(_width - 1, ec);
            List<int> gridNos = new List<int>();
            for (int r = br; r <= er; r++)
            {
                for (int c = bc; c <= ec; c++)
                {
                    int idx = r*_width + c ;
                    if (_loadedFlags[idx] == cstUnFlag)
                    {
                        retGridNos.Add(idx);
                    }
                }
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            _loadedFlags = null;
        }

        #endregion
    }
}
