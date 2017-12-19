using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace CodeCell.Bricks.ModelFabric
{
    internal class ZoomEditor:IEventHandler
    {
        enum ZoomType
        {
            Zoom,
            Pan,
            None
        }

        private IModelEditor _modelEditor = null;
        private ZoomType _zoomType = ZoomType.Pan;
        private Point _beginPoint = Point.Empty;
        private Point _prePoint = Point.Empty;
        private float _scaleStep = 0.1f;

        public ZoomEditor()
        {
        }

        #region IEventHandler 成员

        public void Handle(object sender, enumEventType eventType, object eventArg, EventHandleStatus status)
        {
            if(_modelEditor == null)
                _modelEditor = sender  as IModelEditor ;
            switch (eventType)
            {
                case enumEventType.MouseWheel:
                    MouseWheel(eventArg as MouseEventArgs, status);
                    break;
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

        private void MouseWheel(MouseEventArgs e, EventHandleStatus status)
        {
            float scale = _modelEditor.Scale;
            if (e.Delta < 0)
                scale -= _scaleStep;
            else
                scale += _scaleStep;
            _modelEditor.Scale = scale;
            _modelEditor.Render();
        }

        private void MouseUp(MouseEventArgs e, EventHandleStatus status)
        {
            _zoomType = ZoomType.None;
            _beginPoint = Point.Empty;
            _prePoint = Point.Empty;
        }

        private void MouseMove(MouseEventArgs e, EventHandleStatus status)
        {
            if (e.Button == MouseButtons.Left && !_prePoint.IsEmpty && _zoomType == ZoomType.Pan)
            {
                int offsetX = (int)((e.X - _prePoint.X) / _modelEditor.Scale);
                int offsetY = (int)((e.Y - _prePoint.Y) / _modelEditor.Scale);
                 (_modelEditor as IInternalTransformControl).Offset(offsetX, offsetY);
                status.Handled = true;
                _prePoint = e.Location;
                _modelEditor.Render();
            }
        }

        private void MouseDown(MouseEventArgs e, EventHandleStatus status)
        {
            if (e.Button == MouseButtons.Left)
            {
                _beginPoint = e.Location;
                _prePoint = _beginPoint;
                _zoomType = ZoomType.Pan;
                status.Handled = true;
            }
        }

        #endregion
    }
}
