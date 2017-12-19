using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GeoDo.RSS.Core.UI;
using GeoDo.RSS.Core.RasterDrawing;
using GeoDo.RSS.Core.DF;
using System.Drawing.Imaging;
using GeoDo.RSS.MIF.Core;

namespace GeoDo.RSS.UI.AddIn.Windows
{
    public partial class OverviewContent : UserControl, IToolWindowContent
    {
        private ISmartSession _session;
        private Bitmap _buffer;
        private int[] _defaultBands;
        private Rectangle _boxBounds;
        private Point _prePoint;
        private Point _bPoint;
        private bool _isBoxMoving = false;
        private double _resX;
        private double _resY;
        private int _offsetX;
        private int _offsetY;
        private GeoDo.RSS.Core.DrawEngine.CoordEnvelope _preMovingEnvelope;
        private EventHandler _viewerEnvelopeChanged;
        private Brush _boxFillBrush = new SolidBrush(Color.FromArgb(32, 255, 0, 0));

        public OverviewContent()
        {
            InitializeComponent();
            DoubleBuffered = true;
            Paint += new PaintEventHandler(OverviewContent_Paint);
            SizeChanged += new EventHandler(OverviewContent_SizeChanged);
            MouseDown += new MouseEventHandler(OverviewContent_MouseDown);
            MouseMove += new MouseEventHandler(OverviewContent_MouseMove);
            MouseUp += new MouseEventHandler(OverviewContent_MouseUp);
            DoubleClick += new EventHandler(OverviewContent_DoubleClick);
            _viewerEnvelopeChanged = new EventHandler(RefreshOverview);
        }

        void OverviewContent_DoubleClick(object sender, EventArgs e)
        {
            using (ColorDialog dlg = new ColorDialog())
            {
                dlg.AllowFullOpen = true;
                dlg.FullOpen = true;
                dlg.Color = this.BackColor;
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    this.BackColor = dlg.Color;
                }
            }
        }

        void OverviewContent_MouseDown(object sender, MouseEventArgs e)
        {
            if (_boxBounds.Contains(e.Location) && e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                _prePoint = e.Location;
                _bPoint = e.Location;
                _isBoxMoving = true;
            }
        }

        void OverviewContent_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isBoxMoving)
            {
                _offsetX = e.X - _prePoint.X;
                _offsetY = e.Y - _prePoint.Y;
                this.Invalidate();
                _prePoint = e.Location;
            }
        }

        void OverviewContent_MouseUp(object sender, MouseEventArgs e)
        {
            if (_isBoxMoving)
            {
                _offsetX = e.X - _bPoint.X;
                _offsetY = e.Y - _bPoint.Y;
                UpdateCanvasEnvelope(_offsetX, _offsetY);
                _offsetX = 0;
                _offsetY = 0;
                _prePoint = e.Location;
            }
            _prePoint = Point.Empty;
            _bPoint = Point.Empty;
            _preMovingEnvelope = null;
            _isBoxMoving = false;
        }

        private void UpdateCanvasEnvelope(int offsetX, int offsetY)
        {
            ICanvasViewer viewer = _session.SmartWindowManager.ActiveCanvasViewer;
            if (viewer == null || viewer.ActiveObject == null)
                return;
            IRasterDrawing drawing = viewer.ActiveObject as IRasterDrawing;
            if (drawing == null || drawing.DataProvider == null)
                return;
            GeoDo.RSS.Core.DrawEngine.CoordEnvelope evp = viewer.Canvas.CurrentEnvelope;
            Core.DrawEngine.CoordEnvelope newevp = new Core.DrawEngine.CoordEnvelope(
               new Core.DrawEngine.CoordPoint(evp.MinX + _offsetX * _resX, evp.MinY - _offsetY * _resY),
               evp.Width, evp.Height);
            viewer.Canvas.CurrentEnvelope = newevp;
            viewer.Canvas.Refresh(Core.DrawEngine.enumRefreshType.All);
        }

        void OverviewContent_SizeChanged(object sender, EventArgs e)
        {
            this.Invalidate();
        }

        void OverviewContent_Paint(object sender, PaintEventArgs e)
        {
            ICanvasViewer viewer = _session.SmartWindowManager.ActiveCanvasViewer;
            if (viewer == null || viewer.ActiveObject == null)
                return;
            IRasterDrawing drawing = viewer.ActiveObject as IRasterDrawing;
            if (drawing == null || drawing.DataProvider == null)
                return;
            HandleBoxMoving(drawing, viewer);
            DrawOverview(e);
            DrawViewportBox(viewer, drawing, e);
        }

        private void HandleBoxMoving(IRasterDrawing drawing, ICanvasViewer viewer)
        {
            if (!_isBoxMoving)
                return;
            if (_preMovingEnvelope == null)
                _preMovingEnvelope = viewer.Canvas.CurrentEnvelope.Clone();
            _preMovingEnvelope = new Core.DrawEngine.CoordEnvelope(
                new Core.DrawEngine.CoordPoint(_preMovingEnvelope.MinX + _offsetX * _resX, _preMovingEnvelope.MinY - _offsetY * _resY),
                _preMovingEnvelope.Width, _preMovingEnvelope.Height);
        }

        private void DrawOverview(PaintEventArgs e)
        {
            if (_buffer == null)
                UpdateBuffer();
            if (_buffer == null)
                return;
            e.Graphics.DrawImage(_buffer, 0, 0);
        }

        private void DrawViewportBox(ICanvasViewer viewer, IRasterDrawing drawing, PaintEventArgs e)
        {
            GeoDo.RSS.Core.DrawEngine.CoordEnvelope evp = viewer.Canvas.CurrentEnvelope;
            if (_isBoxMoving)
                evp = _preMovingEnvelope;
            double x1 = (evp.MinX - drawing.OriginalEnvelope.MinX) / _resX;
            double y1 = (drawing.OriginalEnvelope.MaxY - evp.MaxY) / _resY;
            double width = evp.Width / _resX;
            double height = evp.Height / _resY;
            if (width < 3 && height < 3)
            {
                e.Graphics.FillEllipse(Brushes.Red, new Rectangle((int)x1, (int)y1, 3, 3));
            }
            else
            {
                _boxBounds = new Rectangle((int)x1, (int)y1, (int)width, (int)height);
                e.Graphics.FillRectangle(_boxFillBrush, _boxBounds);
                e.Graphics.DrawRectangle(Pens.Red, _boxBounds);
            }
        }

        public void Apply(ISmartSession session)
        {
            _session = session;
            _session.SmartWindowManager.OnActiveWindowChanged += new OnActiveWindowChangedHandler(ActiveWindowChanged);
            if (_session.SmartWindowManager.ActiveCanvasViewer != null)
            {
                ActiveWindowChanged(null, null, _session.SmartWindowManager.ActiveCanvasViewer);
            }
        }

        void ActiveWindowChanged(object sender, ISmartWindow oldWindow, ISmartWindow newWindow)
        {
            UnattachEvent(oldWindow);
            if (newWindow == null || !(newWindow is ICanvasViewer))
            {
                FreeBuffer();
                this.Refresh();
                return;
            }
            FreeBuffer();
            ICanvasViewer cv = newWindow as ICanvasViewer;
            cv.Canvas.OnEnvelopeChanged += _viewerEnvelopeChanged;
            this.Invalidate();
        }

        private void FreeBuffer()
        {
            if (_buffer != null)
            {
                _buffer.Dispose();
                _buffer = null;
            }
        }

        private void UnattachEvent(ISmartWindow oldWindow)
        {
            ICanvasViewer cv = oldWindow as ICanvasViewer;
            if (cv != null)
                cv.Canvas.OnEnvelopeChanged -= _viewerEnvelopeChanged;
        }

        void RefreshOverview(object sender, EventArgs e)
        {
            this.Invalidate();
        }

        private void UpdateBuffer()
        {
            ICanvasViewer viewer = _session.SmartWindowManager.ActiveCanvasViewer;
            if (viewer == null || viewer.ActiveObject == null)
                return;
            IRasterDrawing drawing = viewer.ActiveObject as IRasterDrawing;
            if (drawing == null || drawing.DataProvider == null)
                return;
            using (IRasterDataProvider prd = GeoDataDriver.Open(drawing.FileName) as IRasterDataProvider)
            {
                IOverviewGenerator ov = prd as IOverviewGenerator;
                if (ov == null)
                    return;
                Size size = ov.ComputeSize(200);
                if (_buffer != null)
                    _buffer.Dispose();
                _defaultBands = GetDefaultBands(prd);
                if (_defaultBands == null || _defaultBands.Length == 0)
                    return;
                if (_defaultBands.Length == 3)
                    _buffer = new Bitmap(size.Width, size.Height, PixelFormat.Format24bppRgb);
                else if (_defaultBands.Length == 1)
                {
                    _buffer = new Bitmap(size.Width, size.Height, PixelFormat.Format8bppIndexed);
                    _buffer.Palette = BitmapBuilderFactory.GetDefaultGrayColorPalette();
                }
                else
                    return;
                //
                ov.Generate(_defaultBands, ref _buffer);
                //                
                _resX = drawing.OriginalEnvelope.Width / _buffer.Width;
                _resY = drawing.OriginalEnvelope.Height / _buffer.Height;
            }
        }

        private int[] GetDefaultBands(IRasterDataProvider dataProvider)
        {
            int[] defaultBands = dataProvider.GetDefaultBands();
            if (dataProvider.BandCount > 0)
            {
                if (defaultBands == null)
                {
                    if (dataProvider.BandCount < 3)
                        defaultBands = new int[] { 1 };
                    else
                        defaultBands = new int[] { 1, 2, 3 };
                }
                else
                {
                    for (int i = 0; i < defaultBands.Length; i++)
                        if (defaultBands[i] < 1 || defaultBands[i] > dataProvider.BandCount)
                            defaultBands[i] = 1;
                }
            }
            if (defaultBands != null)
                Array.Reverse(defaultBands);
            return defaultBands;
        }

        public void Free()
        {
            if (_buffer != null)
            {
                _buffer.Dispose();
                _buffer = null;
            }
        }
    }
}
