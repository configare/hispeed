using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using CodeCell.Bricks.Runtime;

namespace CodeCell.AgileMap.Core
{
    public unsafe class PixelConflictor : IConflictor, IDisposable
    {
        private ConflictDefinition _conflictDef = null;
        //private Control _control = null;
        private IMapRuntimeHost _host = null;
        private EventHandler _resizeHandler = null;
        private Bitmap _bitmap = null;
        private Graphics _graphics = null;
        private bool _isAllowChangePosition = true;

        public PixelConflictor(ConflictDefinition conflictDef, IMapRuntimeHost host)
        {
            _conflictDef = conflictDef;
            _host = host;
            _resizeHandler = new EventHandler(_control_SizeChanged);
            _host.OnCanvasSizeChanged += _resizeHandler;
            BuildGrids();
        }

        void _control_SizeChanged(object sender, EventArgs e)
        {
            BuildGrids();
        }

        private void BuildGrids()
        {
            if (_bitmap != null)
            {
                _bitmap.Dispose();
            }
 
            int height = _host.CanvasSize.Height;
            int width = _host.CanvasSize.Width;

            //画布太小
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

        #region IConflictor Members

        public bool IsAllowChangePosition
        {
            get { return _isAllowChangePosition; }
            set { _isAllowChangePosition = value; }
        }

        public bool Enabled
        {
            get { return _conflictDef.Enabled; }
            set { _conflictDef.Enabled = value; }
        }

        public unsafe void Reset()
        {
            if(_graphics != null)
                _graphics.Clear(Color.Black);
        }

        public void Save()
        {
            //_bitmap.Save("d:\\" + Environment.TickCount.ToString() + ".bmp", ImageFormat.Bmp);
        }

        //public void DrawTestRectangleF(PointF pt, SizeF size, Color color)
        //{
        //    using (Pen p = new Pen(color))
        //    {
        //        _graphics.DrawRectangle(p, pt.X, pt.Y, size.Width, size.Height);
        //    }
        //}

        public unsafe bool IsConflicted(float x, float y, SizeF size)
        {
            int br = (int)y;
            int er = (int)(y + size.Height);
            int bc = (int)x;
            int ec = (int)(x + size.Width);
            if (_bitmap == null) //for web
                return false;
            er = Math.Min(er, _bitmap.Height - 1);
            ec = Math.Min(ec, _bitmap.Width - 1);
            if (br < 0 || bc < 0 || er < 0 || ec < 0)
                return true;
            //是冲突的,2011-1-18 可能是调试时写的以下一行
            //_graphics.DrawRectangle(Pens.Green, x, y, size.Width, size.Height);
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
            //未冲突
            TryAdjustConflictedRect(ref size);
            //这里没考虑文本绘制的位置是左对齐、右对齐、上对齐等各种不同形式。
            _graphics.FillRectangle(Brushes.Red, x - size.Width / 2, y - size.Height / 2, size.Width, size.Height);
            return false;
        }

        private void TryAdjustConflictedRect(ref SizeF size)
        {
            if (_conflictDef != null && _conflictDef.Enabled)
            {
                size = new SizeF(Math.Max(_conflictDef.GridSize.Width, size.Width),
                    Math.Max(_conflictDef.GridSize.Height, size.Height));
            }
        }

        public unsafe bool IsConflicted(PointF point, SizeF size)
        {
            return IsConflicted(point.X, point.Y, size);
        }

        public unsafe void HoldPosition(float x, float y, SizeF size)
        {
            if (_graphics != null)//for web
            {
                _graphics.DrawRectangle(Pens.White, x, y, size.Width, size.Height);
                _graphics.FillRectangle(Brushes.Red, x, y, size.Width, size.Height);
            }
        }

        public unsafe void HoldPosition(PointF point, SizeF size)
        {
            HoldPosition(point.X, point.Y, size);
        }

        public void HoldPosition(float x, float y, SizeF size, Matrix transfrom)
        {
            if (_graphics == null)//for web
                return;
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

        #endregion

        #region IDisposable Members

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
            _host.OnCanvasSizeChanged -= _resizeHandler;
            _resizeHandler = null;
            //_control.SizeChanged -= _resizeHandler;
            _conflictDef = null;
            //_control = null;
            _host= null;
        }

        #endregion
    }
}
