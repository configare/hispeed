using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace CodeCell.AgileMap.Core
{
    public class SimpleConflictor:IConflictor,IDisposable
    {
        private ConflictDefinition _conflictDef = null;
        private IMapRuntimeHost _host = null;
        private bool[,] _grids = null;
        private EventHandler _resizeHandler = null;
        private bool _isAllowChangePosition = true;

        public SimpleConflictor(ConflictDefinition conflictDef, IMapRuntimeHost host)
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
            _grids = null;
            int rowCount = _host.CanvasSize.Height / _conflictDef.GridSize.Height;
            int colCount = _host.CanvasSize.Width / _conflictDef.GridSize.Width;
            //画布太小
            if (rowCount == 0 || colCount == 0)
                return;
            try
            {
                _grids = new bool[rowCount, colCount];
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

        public void Reset()
        {
            if (_grids != null)
            {
                int row = _grids.GetLength(0);
                int col = _grids.GetLength(1);
                for (int i = 0; i < row; i++)
                    for (int j = 0; j < col; j++)
                        _grids[i, j] = false;
            }
        }

        public bool IsConflicted(float x, float y, SizeF size)
        {
            int row1 = (int)(y / _conflictDef.GridSize.Height);
            int col1 = (int)(x / _conflictDef.GridSize.Width);
            if (row1 < 0 || col1 < 0)
                return true;
            if (_grids == null) //for web
                return false;
            row1 = Math.Min(row1, _grids.GetLength(0) - 1);
            col1 = Math.Min(col1, _grids.GetLength(1) - 1);
            //
            int row2 = (int)((y + size.Height) / _conflictDef.GridSize.Height);
            int col2 = (int)((x + size.Height) / _conflictDef.GridSize.Width);
            if (row2 < 0 || col2 < 0)
                return true;
            row2 = Math.Min(row2, _grids.GetLength(0) - 1);
            col2 = Math.Min(col2, _grids.GetLength(1) - 1);
            //
            for (int r = row1; r <= row2; r++)
            {
                for (int c = col1; c <= col2; c++)
                {
                    if (_grids[r, c])
                    {
                        return true;
                    }
                }
            }
            //
            for (int r = row1; r <= row2; r++)
            {
                for (int c = col1; c <= col2; c++)
                {
                    _grids[r, c] = true;
                }
            }
            return false;
        }

        public bool IsConflicted(PointF point, SizeF size)
        {
            return IsConflicted(point.X, point.Y, size);
        }

        public void HoldPosition(float x, float y, SizeF size)
        {
            int row1 = (int)(y / _conflictDef.GridSize.Height);
            int col1 = (int)(x / _conflictDef.GridSize.Width);
            if (row1 < 0 || col1 < 0)
                return;
            row1 = Math.Min(row1, _grids.GetLength(0) - 1);
            col1 = Math.Min(col1, _grids.GetLength(1) - 1);
            //
            int row2 = (int)((y + size.Height) / _conflictDef.GridSize.Height);
            int col2 = (int)((x + size.Height) / _conflictDef.GridSize.Width);
            if (row2 < 0 || col2 < 0)
                return;
            row2 = Math.Min(row2, _grids.GetLength(0) - 1);
            col2 = Math.Min(col2, _grids.GetLength(1) - 1);
            //
            for (int r = row1; r <= row2; r++)
            {
                for (int c = col1; c <= col2; c++)
                {
                    _grids[r, c] = true;
                }
            }
        }

        public void HoldPosition(PointF point, SizeF size)
        {
            HoldPosition(point.X, point.Y, size);
        }

        public void HoldPosition(float x, float y, SizeF size, Matrix transfrom)
        { 
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            _host.OnCanvasSizeChanged -= _resizeHandler;
            _grids = null;
            _conflictDef = null;
            _host = null;
        }

        #endregion
    }
}
