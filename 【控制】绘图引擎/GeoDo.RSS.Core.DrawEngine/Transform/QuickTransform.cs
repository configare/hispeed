using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Runtime.InteropServices;

namespace GeoDo.RSS.Core.DrawEngine
{
    /*
     * 屏幕坐标和投影坐标之间的关系是线性关系
     * 因此,可以通过计算直线的k和b,
     * 将矩阵变换转换为一元一次方程的求解(即为一次乘法和一次加法)
     * y = kx +b
     * =>
     * x = y/k -bk
     */
    public class QuickTransform
    {
        //Projection Coord => Screen Coord
        private float _kx;
        private float _ky;
        private float _bx;
        private float _by;
        //Screen Coord      => Projection
        private float _inverKx;
        private float _inverKy;
        private float _inverBx;
        private float _inverBy;

        public QuickTransform()
        {
        }

        public QuickTransform(double kx, double ky, double bx, double by)
        {
            Reset(kx, ky, bx, by);
        }

        public void Reset(double kx, double ky, double bx, double by)
        {
            _kx = (float)kx;
            _ky = (float)ky;
            _bx = (float)bx;
            _by = (float)by;
            _inverKx = 1 / (float)kx;
            _inverKy = 1 / (float)ky;
            _inverBx = -_bx * _inverKx;
            _inverBy = -_by * _inverKy;
        }

        /// <summary>
        /// projection coordinate => screen coordinate
        /// </summary>
        /// <param name="valx"></param>
        /// <param name="valy"></param>
        public void Transform(ref double valx, ref double valy)
        {
            valx = _kx * valx + _bx;
            valy = _ky * valy + _by;
        }

        public void Transform(ref float valx, ref float valy)
        {
            valx = _kx * valx + _bx;
            valy = _ky * valy + _by;
        }

        public unsafe void Transform(PointF* pointf)
        {
            pointf->X = _kx * pointf->X + _bx;
            pointf->Y = _ky * pointf->Y + _by;
        }

        /// <summary>
        /// screen coordinate => projection coordinate
        /// </summary>
        /// <param name="valx"></param>
        /// <param name="valy"></param>
        public void InverTransform(ref double valx, ref double valy)
        {
            valx = _inverKx * valx + _inverBx;
            valy = _inverKy * valy + _inverBy;
        }
    }
}
