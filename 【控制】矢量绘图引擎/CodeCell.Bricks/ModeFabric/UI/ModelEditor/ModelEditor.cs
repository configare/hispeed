using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Drawing;
using CodeCell.Bricks.Runtime;

namespace CodeCell.Bricks.ModelFabric
{
    public class ModelEditor:IModelEditor,IInternalTransformControl
    {
        private Control _container = null;
        private RenderArg _arg = null;
        private Matrix _tramsform = new Matrix();
        private Matrix _invertTransform = new Matrix();
        private Matrix _identityTransform = new Matrix();
        private Color _backColor = Color.White;
        //
        private float _scale = 1f;
        private float _offsetX = 0;
        private float _offsetY = 0;
        //
        private BindingEnvironment _bindingEnvironment = null;
        //
        private List<IEventHandler> _eventHandlers = null;
        private EventHandleStatus _eventHandleStatus = new EventHandleStatus();
        //
        private RectangleF _fullExtent = RectangleF.Empty;
        private RectangleF _extent = RectangleF.Empty;
        //
        private OnScaleChangedHandler _onScaleChangedHandler = null;
        private OnExtentChangedHandler _onExtentChangedHandler = null;
    
        public ModelEditor(Control container)
        {
            _container = container;
            InitRenderArg();
            InitBindingEnvironment();
            InitEventHandlers();
            AttachEvents();
        }

        public IBindingEnvironment BindingEnvironment
        {
            get { return _bindingEnvironment; }
        }

        public Control Container
        {
            get { return _container; }
        }

        public RectangleF FullExtent
        {
            get { return _fullExtent; }
        }

        public RectangleF Extent
        {
            get { return _extent; }
            set
            {
                _extent = value;
                UpdateScaleAndOffsetByExtent();
            }
        }

        public Color BackColor
        {
            get { return _backColor; }
            set { _backColor = value; }
        }

        public float Scale
        {
            get { return _scale; }
            set 
            {
                if (value < 0f || value > 100f)
                    return;
                _scale = value;
                UpdateTransform();
            }
        }

        public OnScaleChangedHandler OnScaleChanged
        {
            get { return _onScaleChangedHandler; }
            set { _onScaleChangedHandler = value; }
        }

        public OnExtentChangedHandler OnExtentChanged
        {
            get { return _onExtentChangedHandler; }
            set { _onExtentChangedHandler = value; }
        }

        private void UpdateScaleAndOffsetByExtent()
        {
            float scaleX = _container.Height / _extent.Height;
            float scaleY = _container.Width / _extent.Width;
            float scale = Math.Min(scaleX, scaleY);
            _scale = scale;
            _offsetX =  - _extent.Left;
            _offsetY = -_extent.Top;
            UpdateTransform();
        }

        private float _preScale = 1f;
        private void UpdateTransform()
        {
            _tramsform.Reset();
            _tramsform.Scale(_scale, _scale);
            _tramsform.Translate(_offsetX, _offsetY);
            if (_invertTransform != null)
                _invertTransform.Dispose();
            _invertTransform = _tramsform.Clone();
            _invertTransform.Invert();
            try
            {
                //触发比例改变事件
                if (Math.Abs(_preScale - _scale) > float.Epsilon)
                    if (_onScaleChangedHandler != null)
                        _onScaleChangedHandler(this, _scale);
            }
            catch(Exception ex)
            {
                Log.WriterException("ModelEditor", "UpdateTransform->Raise ScaleChanged Event", ex);
            }
            _preScale = _scale;
        }

        public Point ToViewCoord(Point containerPixelCoord)
        { 
            Point[] pts = new Point[]{containerPixelCoord};
            _invertTransform.TransformPoints(pts);
            return pts[0];
        }

        private void InitEventHandlers()
        {
            _eventHandlers = new List<IEventHandler>();
            _eventHandlers.Add(_bindingEnvironment);
            _eventHandlers.Add(new LocationEditor());
            _eventHandlers.Add(new ZoomEditor());
        }

        private void InitBindingEnvironment()
        {
            _bindingEnvironment = new BindingEnvironment(this as IModelEditor);
        }

        private void InitRenderArg()
        {
            _arg = new RenderArg(this as IModelEditor);
        }

        private void AttachEvents()
        {
            _container.Paint += new PaintEventHandler(_container_Paint);
            _container.SizeChanged += new EventHandler(_container_SizeChanged);
            _container.MouseDoubleClick += new MouseEventHandler(_container_MouseDoubleClick);
            _container.AllowDrop = true;
            _container.DragEnter += new DragEventHandler(_container_DragEnter);
            _container.DragDrop += new DragEventHandler(_container_DragDrop);
            _container.MouseWheel += new MouseEventHandler(_container_MouseWheel);
            _container.MouseDown += new MouseEventHandler(_container_MouseDown);
            _container.MouseMove += new MouseEventHandler(_container_MouseMove);
            _container.MouseUp += new MouseEventHandler(_container_MouseUp);
        }

        void _container_MouseUp(object sender, MouseEventArgs e)
        {
            HandleEvent(enumEventType.MouseUp, e);
        }

        void _container_MouseMove(object sender, MouseEventArgs e)
        {
            HandleEvent(enumEventType.MouseMove, e);
        }

        void _container_MouseDown(object sender, MouseEventArgs e)
        {
            HandleEvent(enumEventType.MouseDown, e);
        }

        void _container_MouseWheel(object sender, MouseEventArgs e)
        {
            HandleEvent(enumEventType.MouseWheel, e);
        }

        private void HandleEvent(enumEventType eventType, object eventArg)
        {
            if (_eventHandlers == null || _eventHandlers.Count == 0)
                return;
            _eventHandleStatus.Handled = false;
            try
            {
                foreach (IEventHandler h in _eventHandlers)
                {
                    h.Handle(this, eventType, eventArg, _eventHandleStatus);
                    if (_eventHandleStatus.Handled)
                        return;
                }
            }
            finally 
            {
                _eventHandleStatus.Handled = false;
            }
        }

        void _container_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }

        void _container_DragDrop(object sender, DragEventArgs e)
        {
            IDataObject obj = e.Data;
            if (obj == null)
                return;
            if (obj.GetDataPresent("ActionInfo"))
            {
                ActionInfo info = obj.GetData("ActionInfo") as ActionInfo;
                if (info != null)
                {
                    AddActionInfoToEditor(info, _container.PointToClient(Control.MousePosition));
                }
            }
        }

        private void AddActionInfoToEditor(ActionInfo info, Point point)
        {
            ActionElement ele = new ActionElement(info);
            ele.Location = ToViewCoord(point);
            _bindingEnvironment.Add(ele);
            _container.Invalidate();
        }

        void _container_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            HandleEvent(enumEventType.MouseDoubleClick, e);
        }

        void _container_SizeChanged(object sender, EventArgs e)
        {
            UpdateTransform();
            _container.Invalidate();
        }

        void _container_Paint(object sender, PaintEventArgs e)
        {
            Render(e);
        }

        private void Render(PaintEventArgs e)
        {
            BeginRender(e);
            try
            {
                e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
                _arg.Update(e.Graphics);
                e.Graphics.Clear(_backColor);
                Render(_arg);
            }
            finally 
            {
                EndRender(e);
            }
        }

        private void EndRender(PaintEventArgs e)
        {
            e.Graphics.Transform = _identityTransform;
        }

        private void BeginRender(PaintEventArgs e)
        {
            e.Graphics.Transform = _tramsform;
        }

        private void Render(RenderArg arg)
        {
            if (_bindingEnvironment == null || _bindingEnvironment.ActionElements == null || _bindingEnvironment.ActionElements.Length ==0)
                return;
            if (_bindingEnvironment.ActionElementLinks != null && _bindingEnvironment.ActionElementLinks.Length > 0)
                foreach (IRenderable r in _bindingEnvironment.ActionElementLinks)
                    r.Render(arg);
            _bindingEnvironment.ActionElements[0].ToRectangleF(ref _fullExtent);
            foreach (IRenderable r in _bindingEnvironment.ActionElements)
            {
                (r as ActionElement).UnitTo(ref _fullExtent);
                r.Render(arg);
            }
        }

        public void Render()
        {
            _container.Invalidate();
        }

        #region IDisposable 成员

        public void Dispose()
        {
            if (_tramsform != null)
            {
                _tramsform.Dispose();
                _tramsform = null;
            }
            if (_identityTransform != null)
            {
                _identityTransform.Dispose();
                _identityTransform = null;
            }
            if (_invertTransform != null)
            {
                _invertTransform.Dispose();
                _invertTransform = null;
            }
            if (_arg != null)
            {
                _arg.Dispose();
                _arg = null;
            }
            if (ActionElement.BorderPen != null)
            {
                ActionElement.BorderPen.Dispose();
                ActionElement.BorderPen = null;
            }

            if (ActionElement.FillBrush != null)
            {
                ActionElement.FillBrush.Dispose();
                ActionElement.FillBrush = null;
            }
            if (ActionElement.TextStringFormat != null)
            {
                ActionElement.TextStringFormat.Dispose();
                ActionElement.TextStringFormat = null;
            }
            if (ActionElement.BorderEditingRect != null)
            {
                ActionElement.BorderEditingRect.Dispose();
                ActionElement.BorderEditingRect = null;
            }
            if (ActionElement.BoxFillBrush != null)
            {
                ActionElement.BoxFillBrush.Dispose();
                ActionElement.BoxFillBrush = null;
            }
            if (ActionElement.BorderBoxPen != null)
            {
                ActionElement.BorderBoxPen.Dispose();
                ActionElement.BorderBoxPen = null;
            }
            if (ActionElementLink.PenLink != null)
            {
                ActionElementLink.PenLink.Dispose();
                ActionElementLink.PenLink = null;
            }
            if (ActionElementLink.FillArrow != null)
            {
                ActionElementLink.FillArrow.Dispose();
                ActionElementLink.FillArrow = null;
            }
        }

        #endregion

        public void ToScriptFile(string filename)
        { 
        }

        public void LoadScriptFile(string filename)
        {
            _bindingEnvironment.LoadScriptFile(filename);
        }

        #region IInternalTransformControl 成员

        public void Offset(int offetX, int offsetY)
        {
            _offsetX += offetX;
            _offsetY += offsetY;
            UpdateTransform();
        }

        void IInternalTransformControl.Scale(float scale)
        {
            _scale *= scale;
            UpdateTransform();
        }

        #endregion
    }
}
