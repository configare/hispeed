using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GeoDo.RSS.Core.DrawEngine;
using GeoDo.RSS.Core.View;
using System.Drawing.Imaging;
using GeoDo.RSS.Core.RasterDrawing;
using GeoDo.RSS.Core.VectorDrawing;
using System.IO;
using GeoDo.RSS.Layout.GDIPlus;

namespace GeoDo.RSS.Layout.DataFrm
{
    public partial class DataFrameDataProvider : UserControl, IDataFrameDataProvider, IDisposable
    {
        protected CanvasHost _canvasHost;
        protected ILayoutRuntime _runtime;
        protected Bitmap _buffer;
        protected ISizableElement _dataFrame;
        protected bool _isUseDefaultBackgroudLayer;
        protected Color _landColor;
        protected Color _seaColor;
        protected string[] _interestRegions;
        private bool _isNeedReRender = true;

        public DataFrameDataProvider(ISizableElement dataFrame, ILayoutRuntime runtime,
            bool isUseDefaultBackgroudLayer,Color landColor,Color seaColor,string[] interestRegions)
        {
            InitializeComponent();
            _dataFrame = dataFrame;
            _runtime = runtime;
            _isUseDefaultBackgroudLayer = isUseDefaultBackgroudLayer;
            _landColor = landColor;
            _seaColor = seaColor;
            _interestRegions = interestRegions;
            SizeChanged += new EventHandler(DataFrameDataProvider_SizeChanged);
            TryReCreateCanvasHost(_isUseDefaultBackgroudLayer);
        }

        void DataFrameDataProvider_SizeChanged(object sender, EventArgs e)
        {
            TryReCreateCanvasHost(_isUseDefaultBackgroudLayer);
        }

        private void TryReCreateCanvasHost(bool isUseDefaultBackgroudLayer)
        {
            if (_runtime == null)
                return;
            if (_canvasHost == null)
            {
                _canvasHost = new CanvasHost();
                _canvasHost.CreateCanvas();
                _canvasHost.Dock = DockStyle.Fill;
                _canvasHost.Canvas.IsDrawScalePercent = false;
                _canvasHost.Canvas.OnEnvelopeChanged += new EventHandler((sender, e) => 
                {
                    _isNeedReRender = true;
                });
                if (_canvasHost.Canvas != null && _canvasHost.Canvas.CanvasSetting != null && _canvasHost.Canvas.CanvasSetting.RenderSetting != null)
                    _canvasHost.Canvas.CanvasSetting.RenderSetting.BackColor = Color.White;
                TryApplyDefaultBackgroudLayer(isUseDefaultBackgroudLayer);
                Controls.Add(_canvasHost);
            }
            _canvasHost.Size = Size;
            if (_buffer == null || _buffer.Width != _canvasHost.Width || _buffer.Height != _canvasHost.Height)
            {
                if (_buffer != null)
                    _buffer.Dispose();
                _buffer = new Bitmap(_canvasHost.Width, _canvasHost.Height, PixelFormat.Format24bppRgb);
            }
        }

        private void TryApplyDefaultBackgroudLayer(bool isUseDefaultBackgroudLayer)
        {
            if (!isUseDefaultBackgroudLayer)
                return;
            string fname = AppDomain.CurrentDomain.BaseDirectory + @"数据引用\基础矢量\矢量模版\海陆模版.shp";
            if (!File.Exists(fname))
            {
                Console.WriteLine("文件\""+fname+"\"未找到,无法应用默认背景。");
                return;
            }
            IBackgroundLayer lyr = null;
            if (_interestRegions == null || _interestRegions.Length == 0)
                lyr = new BackgroundLayer(fname);
            else
                lyr = new BackgroundLayer(fname, _interestRegions);
            (lyr as BackgroundLayer).LandColor = _landColor;
            (lyr as BackgroundLayer).SeaColor = _seaColor;
            _canvasHost.Canvas.LayerContainer.Layers.Add(lyr);
        }

        ICanvas IDataFrameDataProvider.Canvas
        {
            get { return _canvasHost.Canvas; }
        }

        Size _preSize = new Size();
        Bitmap IDataFrameDataProvider.GetBuffer(bool isClearBuffer)
        {
            //判断_canvasHost的长宽变化与否
                //未变化
            if (_preSize.Width == _canvasHost.Width && _preSize.Height == _canvasHost.Height)
            {
                if (!(_isNeedReRender || isClearBuffer))
                    return _buffer;
            }
                //变化了【分为：变化有效和无效】
            if (_canvasHost.Width<1 || _canvasHost.Height<1)
            {
                _canvasHost.Width=_preSize.Width;
                _canvasHost.Height = _preSize.Height;
            }
            _canvasHost.DrawToBitmap(_buffer, new Rectangle(0, 0, _canvasHost.Width, _canvasHost.Height));
            _preSize = new Size(_canvasHost.Width, _canvasHost.Height);
            _isNeedReRender = false;
            return _buffer;
        }

        void IDisposable.Dispose()
        {
            if (_canvasHost != null)
            {
                if (_canvasHost as IDisposable != null)
                    (_canvasHost as IDisposable).Dispose();
                _canvasHost.Dispose();
                _canvasHost = null;
            }
            if (_buffer != null)
            {
                _buffer.Dispose();
                _buffer = null;
            }

            _runtime = null;
            _dataFrame = null;
        }
    }
}
