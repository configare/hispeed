using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.Core.DF
{
    public interface IOrbitProjectionTransform:IDisposable
    {
        void Transform(float x ,float y,ref int row,ref int col);
        void Transfrom(float[] xs, float[] ys,out int[] rows,out int[] cols);
        void InvertTransform(int row, int col, ref float x, ref float y);
        void InvertTransform(int[] rows, int[] cols, out float[] xs, out float[] ys);
        CoordEnvelope ComputeEnvelope(int bRow, int bCol, int eRow, int eCol);
    }
}
