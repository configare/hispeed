using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

namespace GeoDo.RSS.Core.VectorDrawing
{
    public unsafe class PixelConflictor : IConflictor, IDisposable
    {
        private Bitmap _bitmap = null;
        private Graphics _graphics = null;
        private Size _canvasSize;

        public PixelConflictor(Size canvasSize)
        {
            _canvasSize = canvasSize;
            BuildGrids();
        }

        public void Reset(Size canvasSize)
        {
            _canvasSize = canvasSize;
            BuildGrids();
        }

        private void BuildGrids()
        {
            if (_bitmap != null)
            {
                _bitmap.Dispose();
            }
            int height = _canvasSize.Height;
            int width = _canvasSize.Width;
            if (height == 0 || width == 0)
                return;
            try
            {
                _bitmap = new Bitmap(width, height, PixelFormat.Format24bppRgb);
                _graphics = Graphics.FromImage(_bitmap);
            }
            catch
            {
                throw new OutOfMemoryException("画布太大,不支持冲突检测。");
            }
        }

        public unsafe void Reset()
        {
            if (_graphics != null)
                _graphics.Clear(Color.Black);
        }

        public unsafe bool IsConflicted(float x, float y, SizeF size)
        {
            int br = (int)y;
            int er = (int)(y + size.Height);
            int bc = (int)x;
            int ec = (int)(x + size.Width);
            er = Math.Min(er, _bitmap.Height - 1);
            ec = Math.Min(ec, _bitmap.Width - 1);
            if (br < 0 || bc < 0 || er < 0 || ec < 0)
                return true;
            BitmapData pdata = null;
            try
            {
                pdata = _bitmap.LockBits(new Rectangle(0, 0, _bitmap.Width, _bitmap.Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
                byte* optr = (byte*)pdata.Scan0.ToPointer();
                byte* ptr = optr;
                for (int r = br; r < er; r++)
                {
                    ptr = optr + r * pdata.Stride;
                    for (int c = bc; c < ec; c++)
                    {
                        byte* b = ptr + c * 3 + 2;//24bits, B8+G8+R8
                        if (*b == 255)
                            return true;
                    }
                }
            }
            finally
            {
                _bitmap.UnlockBits(pdata);
            }
            _graphics.FillRectangle(Brushes.Red, x, y, size.Width, size.Height);
            return false;
        }

        public unsafe bool IsConflicted(PointF point, SizeF size)
        {
            return IsConflicted(point.X, point.Y, size);
        }

        public unsafe void HoldPosition(float x, float y, SizeF size)
        {
            _graphics.DrawRectangle(Pens.White, x, y, size.Width, size.Height);
            _graphics.FillRectangle(Brushes.Red, x, y, size.Width, size.Height);
        }

        public unsafe void HoldPosition(PointF point, SizeF size)
        {
            HoldPosition(point.X, point.Y, size);
        }

        public void HoldPosition(float x, float y, SizeF size, Matrix transfrom)
        {
            Matrix om = _graphics.Transform;
            try
            {
                _graphics.Transform = transfrom;
                HoldPosition(x, y, size);
            }
            finally
            {
                _graphics.Transform = om;
            }
        }

        public void Dispose()
        {
            if (_graphics != null)
            {
                _graphics.Dispose();
                _graphics = null;
            }
            if (_bitmap != null)
            {
                _bitmap.Dispose();
                _bitmap = null;
            }
        }
    }
}
