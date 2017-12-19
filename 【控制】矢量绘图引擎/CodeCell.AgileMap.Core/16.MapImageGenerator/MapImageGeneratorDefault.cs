using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using CodeCell.Bricks.Runtime;

namespace CodeCell.AgileMap.Core
{
    public class MapImageGeneratorDefault : IMapImageGenerator, IMapRuntimeHost
    {
        private RectangleF _viewport = RectangleF.Empty;
        protected Size _targetSize = Size.Empty;
        protected IMapRuntime _mapRuntime = null;
        protected ICoordinateTransform _coordTransfrom = null;
        protected IProjectionTransform _projectionTransform = null;
        private ISpatialReference _spatialReference = null;
        private RenderArgs _renderArg = new RenderArgs();
        private OnRenderIsFinishedHandler _renderFinishedNotify = null;
        private EventHandler _onCanvasSizeChanged = null;

        public MapImageGeneratorDefault()
        {
            _mapRuntime = new MapRuntime(this);
            _mapRuntime.ScaleBarArgs.Enabled = false;
            _coordTransfrom = (_mapRuntime as IFeatureRenderEnvironment).CoordinateTransform;
            BuildProjectionTransform();
            SetViewportToDefault();
        }

        public MapImageGeneratorDefault(IMapRuntime mapRuntime)
        {
            _mapRuntime = mapRuntime;
            _mapRuntime.ScaleBarArgs.Enabled = false;
            _coordTransfrom = (_mapRuntime as IFeatureRenderEnvironment).CoordinateTransform;
            BuildProjectionTransform();
            SetViewportToDefault();
        }

        private void BuildProjectionTransform()
        {
            if (_projectionTransform != null)
                _projectionTransform.Dispose();
            _projectionTransform = ProjectionTransformFactory.GetProjectionTransform(new SpatialReference(new GeographicCoordSystem()), _spatialReference);
            if (_mapRuntime != null)
                (_mapRuntime as MapRuntime).ChangeProjectionTransform();
        }

        public RectangleF GetMapImage(RectangleF rectPrj, Size targetSize, ref Image img)
        {
            _targetSize = targetSize;
            if (_onCanvasSizeChanged != null)
                _onCanvasSizeChanged(this, null);
            _viewport = rectPrj;
            ReRender(null, img);
            return _mapRuntime.ActualViewport;
        }

        public void ApplyMap(IMap map)
        {
            _spatialReference = map.MapArguments.TargetSpatialReference;
            //Log.WriterWarning(_spatialReference.ToWKTString());
            BuildProjectionTransform();
            _mapRuntime.Apply(map);
        }

        public Envelope ExtentGeo
        {
            get { throw new NotImplementedException(); }
        }

        public IMap Map
        {
            get { return _mapRuntime.Map; }
        }

        public RectangleF GeoEnvelope2Viewport(Envelope geoEnvelope)
        {
            PointF prjLeftUp = new PointF((float)geoEnvelope.MinX, (float)geoEnvelope.MaxY);
            _projectionTransform.Transform(ref prjLeftUp);
            PointF prjRightDown = new PointF((float)geoEnvelope.MaxX, (float)geoEnvelope.MinY);
            _projectionTransform.Transform(ref prjRightDown);
            return RectangleF.FromLTRB(prjLeftUp.X, -prjLeftUp.Y, prjRightDown.X, -prjRightDown.Y);
        }

        public void SetViewport(Envelope geoEnvelope)
        {
            _viewport = GeoEnvelope2Viewport(geoEnvelope);
        }

        private void SetViewportToDefault()
        {
            Envelope envelope = new Envelope(68.9, 3.14, 141.55, 54);//默认为中国区域
            SetViewport(envelope);
        }

        public ISpatialReference SpatialReference
        {
            get { return _spatialReference; }
            set
            {
                if (_mapRuntime.Map != null)
                {
                    throw new Exception("在已经加载地图的状态下不能修改空间参考。");
                }
                _spatialReference = value;
                BuildProjectionTransform();
            }
        }

        public EventHandler OnCanvasSizeChanged
        {
            get { return _onCanvasSizeChanged; }
            set { _onCanvasSizeChanged = value; }
        }

        public IProjectionTransform ProjectionTransform
        {
            get { return _projectionTransform; }
        }

        public enumCoordinateType CoordinateType
        {
            get { return enumCoordinateType.Projection; }
        }

        private Envelope _focusEnvelope = new Envelope();
        public Envelope FocusEnvelope
        {
            get
            {
                _focusEnvelope.MinX = Math.Min(_viewport.Left, _viewport.Right);
                _focusEnvelope.MaxX = Math.Max(_viewport.Left, _viewport.Right);
                _focusEnvelope.MinY = Math.Min(_viewport.Top, _viewport.Bottom);
                _focusEnvelope.MaxY = Math.Max(_viewport.Top, _viewport.Bottom);
                return _focusEnvelope;
            }
        }

        public Size CanvasSize
        {
            get { return _targetSize; }
        }

        public bool UseDummyMap
        {
            get { return false; }
        }

        public ILocationService LocationService
        {
            get { throw new NotImplementedException(); }
        }

        public void Render()
        {
            Render(null);
        }

        public void ReRender()
        {
            //throw new NotSupportedException();
        }

        public void Render(OnRenderIsFinishedHandler finishNotify)
        {
            throw new NotSupportedException();
        }

        public void ReRender(OnRenderIsFinishedHandler finishNotify)
        {
            throw new NotSupportedException();
        }

        public void ReRender(OnRenderIsFinishedHandler finishNotify, Image img)
        {
            _renderFinishedNotify = finishNotify;
            using (Graphics g = Graphics.FromImage(img))
            {
                if (_mapRuntime == null || _mapRuntime.Map == null)
                {
                    g.Clear(Color.White);
                    return;
                }
                try
                {
                    _renderArg.BeginRender(g);
                    _renderArg.IsReRender = true;
                    g.Clear(_mapRuntime.Map.MapArguments.BackColor);
                    _mapRuntime.Render(_renderArg);
                }
                finally
                {
                    _renderArg.EndRender();
                    //触发渲染完毕的事件通知(异步方式通知)
                    if (_renderFinishedNotify != null)
                    {
                        //_container.BeginInvoke(_renderFinishedNotify);
                        //BeginInvoke(_renderFinishedNotify);
                        _renderFinishedNotify = null;
                    }
                }
            }
        }

        public void RefreshContainer()
        {
            throw new NotSupportedException();
        }

        public void DoBeginInvoke(OnRenderIsFinishedHandler notify)
        {
            throw new NotSupportedException();
        }
    }
}
