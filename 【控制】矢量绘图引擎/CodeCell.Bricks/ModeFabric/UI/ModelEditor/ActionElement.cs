using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace CodeCell.Bricks.ModelFabric
{
    /// <summary>
    /// 可视化建模时的Action图元
    /// </summary>
    internal class ActionElement:IRenderable,IDisposable
    {
        private string _name = null;
        private ActionInfo _actionInfo = null;
        private BindingPair[] _bindingPairs = null;
        private IAction _action = null;
        //
        private PointF _location = PointF.Empty;
        private SizeF _size = new SizeF(120, 60);
        internal static Font Font = new Font("微软雅黑", 9);
        internal static Brush FillBrush = new SolidBrush(Color.FromArgb(235, 240, 249));
        internal static Pen BorderPen = new Pen(Color.FromArgb(94, 146, 231));
        internal static StringFormat TextStringFormat = StringFormat.GenericDefault;
        internal static Brush BoxFillBrush = new SolidBrush(Color.FromArgb(173,216,230));
        internal static Pen BorderEditingRect = new Pen(Color.Blue);
        internal static Pen BorderBoxPen = new Pen(Color.FromArgb(87,108,115));
        //
        private bool _isSelected = false;
        private RectangleF[] _anchorBoxes = null;
        private const int cstBoxHalfHeight = 5;

        static ActionElement()
        {
            TextStringFormat.Alignment = StringAlignment.Center;
            TextStringFormat.LineAlignment = StringAlignment.Center;
            BorderEditingRect.DashStyle = System.Drawing.Drawing2D.DashStyle.DashDot;
            BorderEditingRect.DashPattern = new float[] { 3,3};
        }

        public ActionElement(ActionInfo actionInfo)
        {
            _name = actionInfo.ActionAttribute.Name;
            _actionInfo = actionInfo;
            _action = _actionInfo.ToAction();
        }

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                if (!_isSelected)
                    _anchorBoxes = null;
            }
        }

        public string Name
        {
            get { return _name; }
            set 
            {
                _name = value;
                _action.Name = _name;
            }
        }

        public BindingPair[] BindingPairs
        {
            get { return _bindingPairs; }
        }

        public IAction Action
        {
            get { return _action; }
        }

        public override bool Equals(object obj)
        {
            return _action.Equals(obj);
        }

        public override int GetHashCode()
        {
            return _action.GetHashCode();
        }

        public void UpdateBindingParis(BindingPair[] bindingPair)
        {
            _bindingPairs = bindingPair;
        }

        public PointF Location
        {
            get { return _location; }
            set { _location = value; }
        }

        public SizeF Size
        {
            get { return _size; }
            set { _size = value; }
        }

        public void ToRectangleF(ref RectangleF rect)
        {
            rect.Location = _location;
            rect.Size = _size;
        }

        public RectangleF ToRectangleF()
        {
            return new RectangleF(_location, _size);
        }

        public void UnitTo(ref RectangleF rect)
        { 
            float l = 0,t = 0,r = 0,b = 0 ;
            l = Math.Min(rect.Left, _location.X);
            t = Math.Min(rect.Top, _location.Y);
            r = Math.Max(rect.Right, _location.X + _size.Width);
            b = Math.Max(rect.Bottom, _location.Y + _size.Height);
            rect.Location = new PointF(l, t);
            rect.Size = new SizeF(r - l, b - t);
        }

        #region IRenderable 成员

        public void Render(RenderArg arg)
        {
            if (_size.IsEmpty)
                return;
            arg.Graphics.FillEllipse(FillBrush, _location.X, _location.Y, _size.Width, _size.Height);
            arg.Graphics.DrawEllipse(BorderPen, _location.X, _location.Y, _size.Width, _size.Height);
            arg.Graphics.DrawString(_name, 
                                                  Font, 
                                                  Brushes.Black, 
                                                  new RectangleF(_location, _size),
                                                  TextStringFormat);
            if (_isSelected)
            {
                DrawAnchorBox(arg);
            }
        }

        private void DrawAnchorBox(RenderArg arg)
        {
            if (_anchorBoxes == null)
            {
                _anchorBoxes = new RectangleF[8];
                for (int i = 0; i < 8; i++)
                    _anchorBoxes[i].Size = new Size(cstBoxHalfHeight * 2, cstBoxHalfHeight * 2);
            }
            //rect 
            arg.Graphics.DrawRectangle(BorderEditingRect, _location.X,_location.Y,_size.Width,_size.Height);
            //anchor
            _anchorBoxes[0].Location = new PointF(_location.X - cstBoxHalfHeight, _location.Y - cstBoxHalfHeight);
            _anchorBoxes[1].Location = new PointF(_location.X + _size.Width / 2 - cstBoxHalfHeight, _location.Y - cstBoxHalfHeight);
            _anchorBoxes[2].Location = new PointF(_location.X +_size.Width - cstBoxHalfHeight, _location.Y - cstBoxHalfHeight);
            _anchorBoxes[6].Location = new PointF(_location.X - cstBoxHalfHeight, _location.Y + _size.Height - cstBoxHalfHeight);
            _anchorBoxes[5].Location = new PointF(_location.X + _size.Width / 2 - cstBoxHalfHeight, _location.Y + _size.Height - cstBoxHalfHeight);
            _anchorBoxes[4].Location = new PointF(_location.X + _size.Width - cstBoxHalfHeight, _location.Y + _size.Height - cstBoxHalfHeight);
            _anchorBoxes[3].Location = new PointF(_location.X + _size.Width - cstBoxHalfHeight, _location.Y + _size.Height /2 - cstBoxHalfHeight);
            _anchorBoxes[7].Location = new PointF(_location.X  - cstBoxHalfHeight, _location.Y + _size.Height /2 - cstBoxHalfHeight);
            //
            for (int i = 0; i < 8; i++)
            {
                arg.Graphics.FillRectangle(BoxFillBrush, _anchorBoxes[i].X, _anchorBoxes[i].Y, _anchorBoxes[i].Width, _anchorBoxes[i].Height);
                arg.Graphics.DrawRectangle(BorderBoxPen, _anchorBoxes[i].X, _anchorBoxes[i].Y, _anchorBoxes[i].Width, _anchorBoxes[i].Height);
            }
        }

        public bool IsHited(Point point)
        {
            if (_size.IsEmpty)
                return false;
           using(Region r  = new Region(new RectangleF(_location, _size)))
           {
               return r.IsVisible(point);
           }
        }

        public bool IsHitedAnchor(Point point, out int anchorIndex)
        {
            anchorIndex = -1;
            if (!_isSelected)
                return false;
            for (int i = 0; i < 8; i++)
            {
                if (_anchorBoxes[i].Contains(point.X, point.Y))
                {
                    anchorIndex = i;
                    return true;
                }
            }
            return false;
        }

        #endregion

        #region IDisposable 成员

        public void Dispose()
        {
            _bindingPairs = null;
        }

        #endregion
    }
}
