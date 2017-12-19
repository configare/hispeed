using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.Project;
using GeoDo.RSS.Core.DF;
using System.Drawing;

namespace GeoDo.RSS.Core.RasterDrawing
{
    internal class OrbitProjection : IProjectionTransform
    {
        private IOrbitProjectionTransform _orbitTran;
        private Size _size;

        public OrbitProjection(IOrbitProjectionTransform orbitTran,Size size)
        {
            _orbitTran = orbitTran;
            _size = size;
        }

        public void InverTransform(double[] xs, double[] ys)
        {
            int[] rows = new int[xs.Length];
            int[] cols = new int[ys.Length];
            for (int i = 0; i < xs.Length; i++)
                cols[i] = (int)xs[i];
            for (int i = 0; i < ys.Length; i++)
                rows[i] = _size.Height - (int)ys[i];
            float[] lons = new float[xs.Length];
            float[] lats = new float[ys.Length];
            _orbitTran.InvertTransform(rows, cols, out lons, out lats);
            for (int i = 0; i < cols.Length; i++)
            {
                xs[i] = lons[i];
                ys[i] = lats[i];
            }
        }

        public void Transform(double[] xs, double[] ys)
        {
            int[] rows, cols;
            float[] lons = new float[xs.Length];
            float[] lats = new float[ys.Length];
            for (int i = 0; i < lons.Length; i++)
                lons[i] = (float)xs[i];
            for (int i = 0; i < lats.Length; i++)
                lats[i] = (float)ys[i];
            _orbitTran.Transfrom(lons, lats, out rows, out cols);
            for (int i = 0; i < cols.Length; i++)
            {
                xs[i] = rows[i];
                ys[i] =  _size.Height -cols[i];
            }
        }

        public void Dispose()
        {
            _orbitTran = null;
        }
    }
}
