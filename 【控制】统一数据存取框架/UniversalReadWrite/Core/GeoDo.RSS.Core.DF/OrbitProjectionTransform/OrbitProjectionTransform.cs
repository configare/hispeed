using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using GeoDo.Oribit.Algorithm;

namespace GeoDo.RSS.Core.DF
{
    public class OrbitProjectionTransform:IOrbitProjectionTransform,IDisposable
    {
        private NearestSearcher _nearestSearcher;
        private int _scale;
        private float[] _lons;
        private float[] _lats;
        private Size _size;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lons"></param>
        /// <param name="lats"></param>
        /// <param name="scale">
        /// geo=>raster: return row*scale,col*scale
        /// raster=>geo: input row / scale,col / scale
        /// eg:MERSI 250m 使用 MERSI 1000m的经纬度数据,scale = 4
        /// </param>
        public OrbitProjectionTransform(float[] lons, float[] lats,Size size, int scale)
        {
            _lons = lons;
            _lats = lats;
            _size = size;
            _scale = scale;
        }

        public void Transform(float x, float y, ref int row, ref int col)
        {
            if(_nearestSearcher == null)
                _nearestSearcher = new NearestSearcher(_lons, _lats, _size.Width, _size.Height);
            _nearestSearcher.Cal(x, y, ref col, ref row);
            col *= _scale;
            row *= _scale;
        }

        public void Transfrom(float[] xs, float[] ys, out int[] rows, out int[] cols)
        {
            if (_nearestSearcher == null)
                _nearestSearcher = new NearestSearcher(_lons, _lats, _size.Width, _size.Height);
            rows = new int[xs.Length];
            cols = new int[ys.Length];
            if (_scale != 1)
            {
                for (int i = 0; i < xs.Length; i++)
                {
                    _nearestSearcher.Cal(xs[i], ys[i], ref rows[i], ref cols[i]);
                    rows[i] *= _scale;
                    cols[i] *= _scale;
                }
            }
            else
            {
                for (int i = 0; i < xs.Length; i++)
                    _nearestSearcher.Cal(xs[i], ys[i], ref rows[i], ref cols[i]);
            }
        }

        public CoordEnvelope ComputeEnvelope(int bRow, int bCol, int eRow, int eCol)
        {
            if (_lons == null || _lats == null || _lats.Length == 0 || _lons.Length == 0)
                return null;
            float minX = float.MaxValue;
            float minY = float.MaxValue;
            float maxX = float.MinValue;
            float maxY = float.MinValue;
            int row = 0, col = 0;
            int idx = 0;
            int idx2 = 0;
            for (int r = bRow; r < eRow; r++)
            {
                row = r / _scale;
                if (row < 0 || row >= _size.Height)
                    continue;
                idx = row * _size.Width;
                for (int c = bCol; c < eCol; c++)
                {
                    col = c / _scale;
                    if (col < 0 || col >= _size.Width)
                        continue;
                    idx2 = idx + col;
                    if (_lons[idx2] < minX)
                        minX = _lons[idx2];
                    if (_lons[idx2] > maxX)
                        maxX = _lons[idx2];
                    if (_lats[idx2] < minY)
                        minY = _lats[idx2];
                    if (_lats[idx2] > maxY)
                        maxY = _lats[idx2];
                }
            }
            if (minX == float.MinValue || minY == float.MinValue || maxX == float.MinValue || maxY == float.MinValue)
                return null;
            return new CoordEnvelope(minX, maxX, minY, maxY);
        }

        public void InvertTransform(int row, int col, ref float x, ref float y)
        {
            row /= _scale;
            col /= _scale;
            if (row < 0 || row >= _size.Height || col < 0 || col >= _size.Width)
                return;
            x = _lons[row * _size.Width + col];
            y = _lats[row * _size.Width + col];
        }

        public void InvertTransform(int[] rows, int[] cols, out float[] xs, out float[] ys)
        {
            xs = new float[rows.Length];
            ys = new float[cols.Length];
            for (int i = 0; i < rows.Length; i++)
            {
                rows[i] /= _scale;
                cols[i] /= _scale;
                if (rows[i] < 0 || rows[i] >= _size.Height || cols[i] < 0 || cols[i] >= _size.Width)
                    continue;
                xs[i] = _lons[rows[i]* _size.Width + cols[i]];
                ys[i] = _lats[rows[i] * _size.Width + cols[i]];
            }
        }

        public void Dispose()
        {
            if (_nearestSearcher != null)
            {
                _nearestSearcher.Dispose();
                _nearestSearcher = null;
            }
            _lats = null;
            _lons = null;
        }
    }
}
