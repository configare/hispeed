using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.IO;
using Telerik.WinControls.UI.Docking;
using GeoDo.RSS.Core.UI;
using GeoDo.RSS.Core.DrawEngine;
using GeoDo.RSS.Core.CA;
using GeoDo.RSS.Core.RasterDrawing;
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.Core.VectorDrawing;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.Core.DrawEngine.GDIPlus;

namespace GeoDo.RSS.UI.AddIn.CanvasViewer
{
    public partial class CanvasViewer : ToolWindow, ICanvasViewer,
        IDisposable, IRgbArgEditorEnvironmentSupport, ICanvasHost,
        IIsLayerable, ICanvasViewerMenuHandlerManager,
        ICurrentRasterInteractiver
    {
        private IPickColorIsFinished _pickColorIsFinished = null;
        private EventHandler _onCoordEnvelopeChanged = null;
        private bool _isPrimaryLinkWnd = false;
        private ISmartSession _session = null;
        private CanvasViewerLayerProviderFacader _layerProvider = null;
        private List<EventHandler> _canvasRefreshSubscribers = new List<EventHandler>();
        private CanvasViewerInitializer _initializer;
        private IAOIProvider _aoiProvider;
        private EventHandler _onWindowClosed;
        private CurrentRasterInteractiver _rasterInteractiver;

        public CanvasViewer(ISmartSession session)
        {
            InitializeComponent();
            Text = Guid.NewGuid().ToString();
            Name = Text;
            canvasHost1.Load += new EventHandler(canvasHost1_Load);
            _session = session;
        }

        public CanvasViewer(string text, ISmartSession session)
            : base(text)
        {
            InitializeComponent();
            canvasHost1.Load += new EventHandler(canvasHost1_Load);
            _session = session;
        }

        void canvasHost1_Load(object sender, EventArgs e)
        {
            canvasHost1.Canvas.Container.MouseMove += new MouseEventHandler(Container_MouseMove);
            canvasHost1.Canvas.Container.MouseDown += new MouseEventHandler(Container_MouseDown);
            canvasHost1.Canvas.OnEnvelopeChanged += new EventHandler((senser, arg) =>
            {
                if (_onCoordEnvelopeChanged != null)
                    _onCoordEnvelopeChanged(this, null);
            });
            canvasHost1.Canvas.Container.Paint += new PaintEventHandler(Container_Paint);
            //
            _initializer = new CanvasViewerInitializer(canvasHost1.Canvas, _session);
            _aoiProvider = new AOIProvider(canvasHost1.Canvas);
            _rasterInteractiver = new CurrentRasterInteractiver(this);
            //
            canvasHost1.Canvas.AOIGetter = () =>
            {
                return _aoiProvider.GetBitmapIndexes();
            };
            canvasHost1.Canvas.MaskGetter = () =>
            {
                return AOIHelper.Reverse(_aoiProvider.GetBitmapIndexes(), _aoiProvider.BitmapSize);
            };
            //
            CreateVectorHost();
        }

        private void CreateVectorHost()
        {
            ICanvas c = canvasHost1.Canvas;
            if (c != null)
            {
                IVectorHostLayer host = new VectorHostLayer(null);
                c.LayerContainer.Layers.Add(host as GeoDo.RSS.Core.DrawEngine.ILayer);
            }
        }

        void Container_Paint(object sender, PaintEventArgs e)
        {
            foreach (EventHandler evt in _canvasRefreshSubscribers)
                evt(this, null);
        }

        public EventHandler OnWindowClosed
        {
            get { return _onWindowClosed; }
            set { _onWindowClosed = value; }
        }

        public IAOIProvider AOIProvider
        {
            get { return _aoiProvider; }
        }

        public Bitmap ActiveDrawing
        {
            get
            {
                if (_session.SmartWindowManager.ActiveCanvasViewer == null)
                    return null;
                if (_session.SmartWindowManager.ActiveViewer.ActiveObject == null)
                    return null;
                IRasterDrawing drawing = _session.SmartWindowManager.ActiveViewer.ActiveObject as IRasterDrawing;
                if (drawing == null)
                    return null;
                return drawing.Bitmap;
            }
        }

        public object RgbProcessorArgEditorEnvironment
        {
            get { return this; }
        }

        public bool IsPrimaryLinkWnd
        {
            get { return _isPrimaryLinkWnd; }
            set { _isPrimaryLinkWnd = value; }
        }

        public ICanvas Canvas
        {
            get
            {
                if (canvasHost1 == null)
                    return null;
                return canvasHost1.Canvas;
            }
        }

        public string Title
        {
            get { return Text; }
        }

        public object ActiveObject
        {
            get
            {
                if (canvasHost1 == null || canvasHost1.Canvas == null)
                    return null;
                return canvasHost1.Canvas.PrimaryDrawObject;
            }
        }

        public void To(GeoDo.RSS.Core.DrawEngine.CoordEnvelope viewport)
        {
            canvasHost1.Canvas.CurrentEnvelope = viewport;
            canvasHost1.Canvas.Refresh(enumRefreshType.All);
        }

        public EventHandler OnCoordEnvelopeChanged
        {
            get { return _onCoordEnvelopeChanged; }
            set { _onCoordEnvelopeChanged = value; }
        }

        private Cursor _preCursor = null;
        public void StartPickColor(IPickColorIsFinished onPickColorIsFinished)
        {
            _pickColorIsFinished = onPickColorIsFinished;
            _preCursor = canvasHost1.Cursor;
            canvasHost1.Cursor = Cursors.Cross;
            canvasHost1.Focus();
        }

        void Container_MouseDown(object sender, MouseEventArgs e)
        {
            if (_pickColorIsFinished == null)
                return;
            _pickColorIsFinished.PickColorFinished(GetColorAt(e.Location));
            _pickColorIsFinished = null;
            canvasHost1.Cursor = _preCursor;

        }

        void Container_MouseMove(object sender, MouseEventArgs e)
        {
            if (_pickColorIsFinished == null)
                return;
            _pickColorIsFinished.PickColorFinished(GetColorAt(e.Location));
        }

        private Color GetColorAt(Point point)
        {
            IRasterDrawing drawing = this.ActiveObject as IRasterDrawing;
            if (drawing == null)
                return Color.Empty;
            return drawing.GetColorAt(point.X, point.Y);
        }

        public object LayerProvider
        {
            get
            {
                if (_layerProvider != null)
                    return _layerProvider;
                else
                {
                    _layerProvider = new CanvasViewerLayerProviderFacader(this);
                    return _layerProvider;
                }
            }
        }

        public List<EventHandler> CanvasRefreshSubscribers
        {
            get { return _canvasRefreshSubscribers; }
        }

        public IAOIContainerLayer AOIContainerLayer
        {
            get
            {
                return canvasHost1.Canvas.LayerContainer.GetByName("感兴趣区域") as IAOIContainerLayer;
            }
        }

        public ISelectedAOILayer SelectedAOILayer
        {
            get
            {
                return canvasHost1.Canvas.LayerContainer.GetByName("选中的感兴趣区域") as ISelectedAOILayer;
            }
        }

        public void DisposeViewer()
        {
            (this as IDisposable).Dispose();
        }

        private void RemoveCanvasViewEnv()
        {
            if (canvasHost1.Canvas.Container != null)
            {
                canvasHost1.Canvas.Container.MouseMove -= new MouseEventHandler(Container_MouseMove);
                canvasHost1.Canvas.Container.MouseDown -= new MouseEventHandler(Container_MouseDown);
                canvasHost1.Canvas.Container.Paint -= new PaintEventHandler(Container_Paint);
            }
            canvasHost1.Canvas.OnEnvelopeChanged -= new EventHandler((senser, arg) =>
            {
                if (_onCoordEnvelopeChanged != null)
                    _onCoordEnvelopeChanged(this, null);
            });
            canvasHost1.Canvas.AOIGetter = null;
            canvasHost1.Canvas.MaskGetter = null;
            canvasHost1.Canvas.Dispose();
            GC.Collect();
        }

        void IDisposable.Dispose()
        {
            if (_initializer != null)
            {
                _initializer.Dispose();
                _initializer = null;
            }
            if (_aoiProvider != null)
            {
                _aoiProvider.Dispose();
                _aoiProvider = null;
            }
            if (canvasHost1 != null && canvasHost1.Canvas != null)
            {
                RemoveCanvasViewEnv();
            }
            this.Controls.Remove(canvasHost1);
            canvasHost1.Load -= new EventHandler(canvasHost1_Load);
            canvasHost1.DisposeView();
            canvasHost1 = null;
            _session = null;
            _onCoordEnvelopeChanged = null;
            _pickColorIsFinished = null;
        }

        IEnumerable<object[]> ICanvasViewerMenuHandlerManager.Handlers
        {
            get { return _initializer.Handlers; }
        }

        void ICanvasViewerMenuHandlerManager.Register(string identify, string menuItem, Action<object, Dictionary<string, object>> action, string argProviderUI)
        {
            _initializer.Register(identify, menuItem, action, argProviderUI);
        }

        void ICanvasViewerMenuHandlerManager.UnRegister(string identify)
        {
            _initializer.UnRegister(identify);
        }

        double ICurrentRasterInteractiver.GetAvgBandValueInAOI(int bandNo)
        {
            return _rasterInteractiver.GetAvgBandValueInAOI(bandNo);
        }

        double[] ICurrentRasterInteractiver.GetBandValuesInAOI(int bandNo)
        {
            return _rasterInteractiver.GetBandValuesInAOI(bandNo);
        }

        double ICurrentRasterInteractiver.GetMaxBandValueInAOI(int bandNo)
        {
            return _rasterInteractiver.GetMaxBandValueInAOI(bandNo);
        }

        double ICurrentRasterInteractiver.GetMinBandValueInAOI(int bandNo)
        {
            return _rasterInteractiver.GetMinBandValueInAOI(bandNo);
        }

        void ICurrentRasterInteractiver.StartInteractive(int[] bandNos, Action<double[]> bandValuesNotifier)
        {
            _rasterInteractiver.StartInteractive(bandNos, bandValuesNotifier);
        }

        void ICurrentRasterInteractiver.StartAOIDrawing(Action drawFinishedNotifiter)
        {
            IPencilToolLayer aoiLayer = _initializer.StartAOIDrawing(enumPencilType.FreeCurve);
            if (aoiLayer != null)
            {
                aoiLayer.PencilIsFinished += new Action<GeometryOfDrawed>((v) =>
                {
                    drawFinishedNotifiter();
                    _initializer._aoiContainer.Reset();
                });
            }
        }

        ILabelService ICurrentRasterInteractiver.LabelService
        {
            get { return _rasterInteractiver.LabelService; }
        }

        void ICurrentRasterInteractiver.StartAOIDrawing(string shapeType, Action<int[], CodeCell.AgileMap.Core.Shape> drawFinishedNotifier)
        {
            IPencilToolLayer aoiLayer = null;
            if (shapeType == "FreeCurve")
                aoiLayer = _initializer.StartAOIDrawing(enumPencilType.FreeCurve);
            else if (shapeType == "Polygon")
                aoiLayer = _initializer.StartAOIDrawing(enumPencilType.Polygon);
            else if (shapeType == "Rectangle")
                aoiLayer = _initializer.StartAOIDrawing(enumPencilType.Rectangle);
            else if (shapeType == "Circle")
                aoiLayer = _initializer.StartAOIDrawing(enumPencilType.Circle);
            else
                aoiLayer = _initializer.StartAOIDrawing(enumPencilType.FreeCurve);
            if (aoiLayer != null)
            {
                aoiLayer.PencilIsFinished += new Action<GeometryOfDrawed>((v) =>
                {
                    if (drawFinishedNotifier != null)
                        drawFinishedNotifier(_aoiProvider.GetIndexes(), GetGeoGeometry(v));
                    _initializer._aoiContainer.Reset();
                });
            }
        }

        private CodeCell.AgileMap.Core.Shape GetGeoGeometry(GeometryOfDrawed v)
        {
            return DrawedGeometry2Polygon.ToPolygon(this.canvasHost1.Canvas, v);
        }

        ICanvas ICanvasHost.Canvas
        {
            get { return this.canvasHost1.Canvas; }
        }

        void ICanvasHost.DrawToBitmap(Bitmap buffer, Rectangle rect)
        {
            this.canvasHost1.DrawToBitmap(buffer, rect);
        }

        public void TryFinishPencilTool()
        {
            PencilToolLayer pencilTool = Canvas.CurrentViewControl as PencilToolLayer;
            Canvas.CurrentViewControl = new DefaultControlLayer();
        }

        public double GetMaxAvgBandValueInAOI(int bandNo, double percent)
        {
            return _rasterInteractiver.GetMaxAvgBandValueInAOI(bandNo, percent);
        }

        public double GetMinAvgBandValueInAOI(int bandNo, double percent)
        {
            return _rasterInteractiver.GetMinAvgBandValueInAOI(bandNo, percent);
        }
    }
}
