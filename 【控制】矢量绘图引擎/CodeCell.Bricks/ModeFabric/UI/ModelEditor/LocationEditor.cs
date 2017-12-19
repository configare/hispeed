using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace CodeCell.Bricks.ModelFabric
{
    internal class LocationEditor:IEventHandler
    {
        enum enumOprType
        {
            Move,
            Anchor,
            None
        }

        private IModelEditor _modelEditor = null;
        private enumOprType _oprType = enumOprType.None;
        private ActionElement _currentElement = null;
        private Point _prePoint = Point.Empty;
        private Point _startPoint = Point.Empty;
        private int _currentAnchorIndex = -1;

        public LocationEditor()
        {
        }

        #region IEventHandler 成员

        public void Handle(object sender, enumEventType eventType, object eventArg,EventHandleStatus status)
        {
            if (_modelEditor == null)
                _modelEditor = sender as IModelEditor;
            switch (eventType)
            {
                case enumEventType.MouseDown:
                    MouseDown(eventArg as MouseEventArgs, status);
                    break;
                case enumEventType.MouseMove:
                    MouseMove(eventArg as MouseEventArgs, status);
                    break;
                case enumEventType.MouseUp:
                    MouseUp(eventArg as MouseEventArgs, status);
                    break;
            }
        }

        private void MouseUp(MouseEventArgs e, EventHandleStatus status)
        {
            _prePoint = Point.Empty;
            _startPoint = Point.Empty;
            _currentElement = null;
            _currentAnchorIndex = -1;
            _oprType = enumOprType.None;
        }

        private void MouseMove(MouseEventArgs e, EventHandleStatus status)
        {
            if (e.Button == MouseButtons.Left && _oprType != enumOprType.None && !_prePoint.IsEmpty && _currentElement != null)
            {
                Point viewPoint = _modelEditor.ToViewCoord(e.Location);
                status.Handled = true;
                float offsetX = viewPoint.X - _prePoint.X;
                float offsetY = viewPoint.Y - _prePoint.Y;
                _prePoint = viewPoint;
                PointF loc = _currentElement.Location;
                SizeF size = _currentElement.Size;
                int minAdjust = 32;
                switch (_oprType)
                {
                    case enumOprType.Move:
                        loc.X += offsetX;
                        loc.Y += offsetY;
                        _currentElement.Location = loc;
                        _modelEditor.Render();
                        break;
                    case enumOprType.Anchor:
                        switch (_currentAnchorIndex)
                        {
                            case 0:
                                if (viewPoint.X > loc.X + size.Width - minAdjust || viewPoint.Y > loc.Y + size.Height - minAdjust)
                                {
                                    loc = PointF.Empty;
                                    break;
                                }
                                loc = viewPoint;
                                size = new SizeF(size.Width - offsetX, size.Height - offsetY);
                                break;
                            case 1:
                                if (viewPoint.Y > loc.Y + size.Height - minAdjust)
                                {
                                    loc = PointF.Empty;
                                    break;
                                }
                                loc = new PointF(loc.X, loc.Y + offsetY);
                                size = new SizeF(size.Width, size.Height - offsetY);
                                break;
                            case 2:
                                if (viewPoint.X < loc.X + minAdjust || viewPoint.Y > loc.Y  + size.Height - minAdjust)
                                {
                                    loc = PointF.Empty;
                                    break;
                                }
                                loc = new PointF(loc.X, loc.Y + offsetY);
                                size = new SizeF(size.Width + offsetX, size.Height - offsetY);
                                break;
                            case 3:
                                if (viewPoint.X < loc.X + minAdjust)
                                {
                                    loc = PointF.Empty;
                                    break;
                                }
                                size = new SizeF(size.Width + offsetX, size.Height);
                                break;
                            case 4:
                                if (viewPoint.X < loc.X + minAdjust || viewPoint.Y < loc.Y + minAdjust)
                                {
                                    loc = PointF.Empty;
                                    break;
                                }
                                size = new SizeF(size.Width + offsetX, size.Height + offsetY);
                                break;
                            case 5:
                                if (viewPoint.Y < loc.Y + minAdjust)
                                {
                                    loc = PointF.Empty;
                                    break;
                                }
                                size = new SizeF(size.Width, size.Height + offsetY);
                                break;
                            case 6:
                                if (viewPoint.X > loc.X + size.Width - minAdjust || viewPoint.Y < loc.Y + minAdjust)
                                {
                                    loc = PointF.Empty;
                                    break;
                                }
                                loc = new PointF(viewPoint.X, loc.Y);
                                size = new SizeF(size.Width - offsetX, size.Height + offsetY);
                                break;
                            case 7:
                                if (viewPoint.X > loc.X + size.Width - minAdjust)
                                {
                                    loc = PointF.Empty;
                                    break;
                                }
                                loc = new PointF(viewPoint.X, loc.Y);
                                size = new SizeF(size.Width - offsetX, size.Height);
                                break;
                        }
                        if (!loc.IsEmpty)
                        {
                            _currentElement.Location = loc;
                            _currentElement.Size = size;
                        }
                        _modelEditor.Render();
                        break;
                }
            }
        }

        private void MouseDown(MouseEventArgs e, EventHandleStatus status)
        {
            Point viewPoint = _modelEditor.ToViewCoord(e.Location);
            if (e.Button == MouseButtons.Left)
            {
                IInternalSelection sel = _modelEditor.BindingEnvironment as IInternalSelection;
                //是否点中了锚点
                _currentElement = sel.GetActionElementAt(viewPoint,out _currentAnchorIndex);
                if (_currentElement != null && _currentAnchorIndex != -1)
                {
                    _startPoint = viewPoint;
                    _prePoint = _startPoint;
                    _oprType = enumOprType.Anchor;
                    status.Handled = true;
                }
                else//点中了除锚点外的其它区域
                {
                    _currentElement = sel.Select(viewPoint);
                    if (_currentElement != null)
                    {
                        _startPoint = viewPoint;
                        _prePoint = _startPoint;
                        _oprType = enumOprType.Move;
                        status.Handled = true;
                    }
                    _modelEditor.Render();
                }
            }
        }

        #endregion
    }
}
