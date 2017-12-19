using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using GeoDo.Project;
using GeoDo.RSS.Core.CA;
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.Core.DrawEngine;
using GeoDo.RSS.DF.GDAL;

namespace GeoDo.RSS.Core.RasterDrawing
{
    public unsafe class RasterDrawing : IRasterDrawing, IPrimaryDrawObject, 
        IGeoPanAdjust,IBitmapInteractiveHelper, IDisposable
    {
        private int[] _selectedBandNos = null;
        private GeoDo.RSS.Core.DrawEngine.CoordEnvelope _envelope = null;
        private GeoDo.RSS.Core.DrawEngine.CoordEnvelope _originalEnvelope = null;
        private IRasterDataProvider _dataProvider = null;
        private IRasterDataProvider _dataProviderCopy = null;
        private IProjectionTransform _projectionTransform = null;
        private ITileBitmapProvider _tileBitmapProvider = null;
        private IRgbProcessorStack _rgbProcessorStack = null;
        private float _originalResolutionX = 0, _originalResolutionY = 0;
        protected ICanvas _canvas = null;
        protected TileSetting _tileSetting = null;
        private Bitmap _bitmap = null;
        private int _preWidth = 0, _preHeight = 0;
        private int _width = 0, _heigth = 0;
        private int _bandCount = 0;
        private ReadPixelHelper _readPixelHelper = null;
        private Color _backColor;
        private IList<ILoadingPrecentSubscriber> _loadingPercentSubscribers = new List<ILoadingPrecentSubscriber>();
        private bool _isGeoPanAdjusting = false;
        private IRgbStretcherProvider _stretcherProvider;
        private string _rgbStretcherArgument;

        public RasterDrawing(string fname, ICanvas canvas,
            IRgbStretcherProvider stretcerProvider, params string[] options)
        {
            _canvas = canvas;
            _stretcherProvider = stretcerProvider;
            SetRgbStretcherArgument(options);
            SetFieldsByCanvas();
            SetDataProviderByFile(fname, options);
            SetFieldsByDataProvider();
            BuildTileBitmapProvider(canvas);
            BuildRgbProcessorStack();
        }

        public RasterDrawing(string fname, ICanvas canvas, int[] selectedBandNos,
            IRgbStretcherProvider stretcerProvider, params string[] options)
        {
            _canvas = canvas;
            _stretcherProvider = stretcerProvider;
            SetRgbStretcherArgument(options);
            SetFieldsByCanvas();
            SetDataProviderByFile(fname, options);
            if (selectedBandNos == null)
                throw new ArgumentNullException("selectedBandNos");
            if (selectedBandNos.Length != 1 && selectedBandNos.Length != 3)
                throw new ArgumentOutOfRangeException("Selected bandcount must is 1 or 3");
            _selectedBandNos = selectedBandNos;
            SetFieldsByDataProvider();
            BuildTileBitmapProvider(canvas);
            BuildRgbProcessorStack();
        }

        private void SetRgbStretcherArgument(params object[] options)
        {
            if (options == null || options.Length == 0)
                return;
            foreach (object obj in options)
            {
                if (obj == null)
                    continue;
                if (obj.ToString().ToLower().Contains("colortablename="))
                {
                    _rgbStretcherArgument = obj.ToString().Replace("colortablename=", "");
                    return;
                }
            }
        }

        private void SetFieldsByCanvas()
        {
            if (_canvas != null && _canvas.CanvasSetting != null)
                _tileSetting = _canvas.CanvasSetting.TileSetting;
            if (_canvas != null && _canvas.CanvasSetting != null && _canvas.CanvasSetting.RenderSetting != null)
                _backColor = _canvas.CanvasSetting.RenderSetting.BackColor;
        }

        private int _firstVisibleTilesCount = 0;
        private Bitmap _firstBitmap = null;
        private int MIN_IMAGE_SIZE = 1248 * 1248;
        public void StartLoading(Action<int, int> progress)
        {
            int times = GetOverviewLoadTimes();
            bool isSync = times == -1;
            _isFirst = false;
            if (times != -1)
            {
                CreateFirstBitmap(progress);
                Application.DoEvents();
            }
            _tileBitmapProvider.TilesLocator.UpdateTileComputer(_canvas);
            LevelDef[] levels = _tileBitmapProvider.TileComputer.GetLevelDefs(_dataProvider.Width, _dataProvider.Height);
            _firstVisibleTilesCount = _tileBitmapProvider.LoadTileBitmaps(levels, isSync);
        }

        public int GetOverviewLoadTimes()
        {
            int w = _canvas.Container.Width;
            int h = _canvas.Container.Height;
            if (_dataProviderCopy.Width * _dataProviderCopy.Height <= MIN_IMAGE_SIZE)
                return -1;
            float sx = w / (float)_dataProviderCopy.Width;
            float sy = h / (float)_dataProviderCopy.Height;
            sx = Math.Min(sx, sy);
            h = (int)(_dataProviderCopy.Height * sx);
            return h * _selectedBandNos.Length;
        }

        private void CreateFirstBitmap(Action<int, int> progress)
        {
            int w = _canvas.Container.Width;
            int h = _canvas.Container.Height;
            float sx = w / (float)_dataProviderCopy.Width;
            float sy = h / (float)_dataProviderCopy.Height;
            sx = Math.Min(sx, sy);
            w = (int)(_dataProviderCopy.Width * sx);
            h = (int)(_dataProviderCopy.Height * sx);
            if (_firstBitmap != null)
                _firstBitmap.Dispose();
            _firstBitmap = _tileBitmapProvider.DataProviderReader.GetOverview(_dataProviderCopy, w, h, _selectedBandNos,
                (p) =>
                {
                    if (progress != null)
                        progress(_selectedBandNos.Length * h, p);
                });
            _bitmap = _firstBitmap.Clone() as Bitmap;
            _envelope = _originalEnvelope;
        }

        public string SpatialRef
        {
            get 
            {
                if (_dataProvider == null || _dataProvider.SpatialRef == null)
                    return null;
                try
                {
                    return _dataProvider.SpatialRef.ToWKTString();
                }
                catch(Exception ex) 
                {
                    Console.WriteLine(ex.Message);
                    return null;
                }
            }
        }

        public bool IsRasterCoord
        {
            get 
            {
                if (_dataProvider == null)
                    return false;
                return _dataProvider.CoordType == enumCoordType.Raster && 
                    !_dataProvider.DataIdentify.IsOrbit; 
            }
        }

        public IRgbStretcherProvider RgbStretcherProvider
        {
            get { return _stretcherProvider; }
        }

        public IList<ILoadingPrecentSubscriber> LoadingSubscribers
        {
            get { return _loadingPercentSubscribers; }
        }

        public Bitmap Bitmap
        {
            get { return _bitmap; }
        }

        public bool IsActive
        {
            get { return _tileBitmapProvider.IsActive; }
        }

        public void Active()
        {
            _tileBitmapProvider.Active();
        }

        public void Deactive()
        {
            _tileBitmapProvider.Deactive();
        }

        private void BuildRgbProcessorStack()
        {
            _rgbProcessorStack = new RgbProcessorStack();
        }

        private void BuildTileBitmapProvider(ICanvas canvas)
        {
            _tileBitmapProvider = new TileBitmapProvider(this, canvas, _dataProvider,
                _tileSetting, _selectedBandNos, _originalEnvelope,
                _originalResolutionX, _originalResolutionY, _stretcherProvider, _rgbStretcherArgument);
        }

        private void SetDataProviderByFile(string fname, params string[] options)
        {
            try
            {
                _dataProvider = GeoDataDriver.Open(fname, enumDataProviderAccess.ReadOnly, options) as IRasterDataProvider;
                _dataProviderCopy = GeoDataDriver.Open(fname, enumDataProviderAccess.ReadOnly, options) as IRasterDataProvider;
                CheckDataProvider();
                _width = _dataProviderCopy.Width;
                _heigth = _dataProviderCopy.Height;
                _bandCount = _dataProviderCopy.BandCount;
                _readPixelHelper = new ReadPixelHelper(_dataProviderCopy);
            }
            catch (Exception ex)
            {
                throw new CreateDataProviderFailedByFileName(fname, ex);
            }
            if (_dataProvider == null)
                throw new CreateDataProviderFailedByFileName(fname);
        }

        private void CheckDataProvider()
        {
            if (_dataProvider.CoordEnvelope == null ||
                EnvelopeIsNaN(_dataProvider.CoordEnvelope))
                throw new ArgumentOutOfRangeException("数据提供者的CoordEnvelope错误！");
        }

        private bool EnvelopeIsNaN(DF.CoordEnvelope evp)
        {
            return double.IsNaN(evp.MinX) ||
                double.IsNaN(evp.MaxX) ||
                double.IsNaN(evp.MinY) ||
                double.IsNaN(evp.MaxY) ||
                //
                double.IsInfinity(evp.MinX) ||
                double.IsInfinity(evp.MaxX) ||
                double.IsInfinity(evp.MinY) ||
                double.IsInfinity(evp.MaxY) ||
                //
                double.IsNegativeInfinity(evp.MinX) ||
                double.IsNegativeInfinity(evp.MaxX) ||
                double.IsNegativeInfinity(evp.MinY) ||
                double.IsNegativeInfinity(evp.MaxY);
        }

        private void SetFieldsByDataProvider()
        {
            SetEnvelope();
            SetOrAdjustSelectedBandNos();
        }

        private void SetEnvelope()
        {
            GeoDo.RSS.Core.DF.CoordEnvelope evp = _dataProvider.CoordEnvelope;
            switch (_dataProvider.CoordType)
            {
                case enumCoordType.Raster:
                case enumCoordType.PrjCoord:
                    if (evp == null)
                        evp = new DF.CoordEnvelope(new DF.CoordPoint(0, 0), _dataProvider.Width, _dataProvider.Height);
                    _envelope = new DrawEngine.CoordEnvelope(evp.MinX, evp.MaxX, evp.MinY, evp.MaxY);
                    if (_dataProvider.SpatialRef != null)
                    {
                        _projectionTransform = ProjectionTransformFactory.GetProjectionTransform(SpatialReference.GetDefault(), _dataProvider.SpatialRef);
                    }
                    _canvas.CoordTransform.DataCoordType = enumDataCoordType.Prj;
                    break;
                case enumCoordType.GeoCoord:
                    //这里未处理其他坐标系统//???
                    _projectionTransform = ProjectionTransformFactory.GetDefault();
                    double[] xs = new double[] { evp.MinX, evp.MaxX };
                    double[] ys = new double[] { evp.MinY, evp.MaxY };
                    _projectionTransform.Transform(xs, ys);
                    _envelope = new DrawEngine.CoordEnvelope(xs[0], xs[1], ys[0], ys[1]);
                    _canvas.CoordTransform.DataCoordType = enumDataCoordType.Geo;
                    break;
            }
            _originalEnvelope = _envelope.Clone();
            _originalResolutionX = (float)(_originalEnvelope.Width / _dataProvider.Width);
            _originalResolutionY = (float)(_originalEnvelope.Height / _dataProvider.Height);
            //
            if (_dataProvider.SpatialRef == null)
                _canvas.CoordTransform.SpatialRefOfViewer = new SpatialReference(new GeographicCoordSystem());
            else
                _canvas.CoordTransform.SpatialRefOfViewer = _dataProvider.SpatialRef.Clone();
        }

        private void SetOrAdjustSelectedBandNos()
        {
            if (_dataProvider.BandCount > 0)
            {
                if (_selectedBandNos == null)
                    _selectedBandNos = new int[] { 1 };
                else
                {
                    for (int i = 0; i < _selectedBandNos.Length; i++)
                        if (_selectedBandNos[i] < 1 || _selectedBandNos[i] > _dataProvider.BandCount)
                            _selectedBandNos[i] = 1;
                    int bNo = _selectedBandNos[0];
                    for (int i = 1; i < _selectedBandNos.Length; i++)
                        if (bNo != _selectedBandNos[i])
                            return;
                    _selectedBandNos = new int[] { bNo };
                }
            }
        }

        public void TryCreateOrbitPrjection()
        {
            if (_dataProvider.DataIdentify != null && _dataProvider.DataIdentify.IsOrbit)
            {
                _dataProvider.OrbitProjectionTransformControl.Build();
                if (_dataProvider.OrbitProjectionTransformControl.OrbitProjectionTransform != null)
                    _projectionTransform = new OrbitProjection(_dataProvider.OrbitProjectionTransformControl.OrbitProjectionTransform, new Size(_width, _heigth));
            }
        }

        public IProjectionTransform ProjectionTransform
        {
            get { return _projectionTransform; }
        }

        public int BandCount
        {
            get { return _bandCount; }
        }

        public string FileName
        {
            get { return _dataProvider.fileName; }
        }

        public IRgbProcessorStack RgbProcessorStack
        {
            get { return _rgbProcessorStack; }
        }

        public int[] SelectedBandNos
        {
            get { return _selectedBandNos; }
            set
            {
                if (value == null)
                    return;
                if (_selectedBandNos != null && _selectedBandNos.Length == value.Length)
                {
                    for (int b = 0; b < _selectedBandNos.Length; b++)
                        if (_selectedBandNos[b] != value[b])
                            goto setLine;
                    return;
                }
            setLine:
                _dataProvider.ResetStretcher();
                _selectedBandNos = value;
                SetOrAdjustSelectedBandNos();
                _tileBitmapProvider.Reset();
                _tileBitmapProvider.UpdateSelectedBandNos(_selectedBandNos);
            }
        }

        public void Reset()
        {
            SetOrAdjustSelectedBandNos();
            _tileBitmapProvider.Reset();
            _tileBitmapProvider.DataProviderReader.ResetStretcher();
            _tileBitmapProvider.UpdateSelectedBandNos(_selectedBandNos);
        }

        public void ApplyColorMapTable(ColorMapTable<double> oColorTable)
        {
            if (SelectedBandNos == null || _bandCount == 0)
                return;
            IColorMapTableGetter getter = ColorMapTableGetterFactory.GetColorTableGetter(_dataProvider.DataType, oColorTable);
            if (getter == null)
                return;
            _tileBitmapProvider.DataProviderReader.SetColorMapTable(getter);
            if (SelectedBandNos.Length > 1)
                SelectedBandNos = new int[] { 1 };
            else
            {
                _tileBitmapProvider.Reset();
                _tileBitmapProvider.UpdateSelectedBandNos(_selectedBandNos);
            }
        }

        public GeoDo.RSS.Core.DrawEngine.CoordEnvelope Envelope
        {
            get { return _envelope; }
        }

        public GeoDo.RSS.Core.DrawEngine.CoordEnvelope OriginalEnvelope
        {
            get { return _originalEnvelope; }
        }

        public Size Size
        {
            get
            {
                if (_dataProvider == null)
                    return Size.Empty;
                return new Size(_dataProvider.Width, _dataProvider.Height);
            }
        }

        public float OriginalResolutionX
        {
            get { return _originalResolutionX; }
        }

        public float Scale
        {
            get
            {
                if (_canvas == null)
                    return 1f;
                return (float)(_originalResolutionX / (_envelope.Width / _canvas.Container.Width));
            }
        }

        public void Screen2Raster(float screenX, float screenY, out int row, out int col)
        {
            if (_canvas == null)
            {
                row = col = 0;
                return;
            }
            float beginRow, beginCol, width, height;
            _tileBitmapProvider.TilesLocator.GetRasterRowColOfViewWnd(_canvas, out beginRow, out beginCol, out width, out height);
            float resX = width / (float)_canvas.Container.Width;
            float resY = height / (float)_canvas.Container.Height;
            col = (int)(beginCol + resX * screenX + 0.5f);
            row = (int)(beginRow + resY * screenY + 0.5f);
        }

        public void Screen2Raster(int screenX, int screenY, out int row, out int col)
        {
            if (_canvas == null)
            {
                row = col = 0;
                return;
            }
            int beginRow, beginCol, width, height;
            _tileBitmapProvider.TilesLocator.GetRasterRowColOfViewWnd(_canvas, out beginRow, out beginCol, out width, out height);
            float resX = width / (float)_canvas.Container.Width;
            float resY = height / (float)_canvas.Container.Height;
            col = (int)(beginCol + resX * screenX);
            row = (int)(beginRow + resY * screenY);
        }

        public void Screen2Raster(float screenX, float screenY, out float row, out float col)
        {
            if (_canvas == null)
            {
                row = col = 0;
                return;
            }
            int beginRow, beginCol, width, height;
            _tileBitmapProvider.TilesLocator.GetRasterRowColOfViewWnd(_canvas, out beginRow, out beginCol, out width, out height);
            float resX = width / (float)_canvas.Container.Width;
            float resY = height / (float)_canvas.Container.Height;
            col = beginCol + resX * screenX;
            row = beginRow + resY * screenY;
        }

        public void Raster2Screen(int row, int col, out int screenX, out int screenY)
        {
            if (_canvas == null)
            {
                screenX = screenY = 0;
                return;
            }
            int beginRow, beginCol, width, height;
            _tileBitmapProvider.TilesLocator.GetRasterRowColOfViewWnd(_canvas, out beginRow, out beginCol, out width, out height);
            float resX = width / (float)_canvas.Container.Width;
            float resY = height / (float)_canvas.Container.Height;
            screenX = -(int)((beginCol - col) / resX);
            screenY = -(int)((beginRow - row) / resY);
        }

        public void Raster2Screen(float row, float col, out float screenX, out float screenY)
        {
            if (_canvas == null)
            {
                screenX = screenY = 0;
                return;
            }
            int beginRow, beginCol, width, height;
            _tileBitmapProvider.TilesLocator.GetRasterRowColOfViewWnd(_canvas, out beginRow, out beginCol, out width, out height);
            float resX = width / (float)_canvas.Container.Width;
            float resY = height / (float)_canvas.Container.Height;
            screenX = -((beginCol - col) / resX);
            screenY = -((beginRow - row) / resY);
        }

        public void Raster2Geo(int row, int col, out double geoX, out double geoY)
        {
            if (_dataProvider.DataIdentify != null && _dataProvider.DataIdentify.IsOrbit)
            {
                if (_dataProvider.OrbitProjectionTransformControl.OrbitProjectionTransform != null)
                {
                    float x = 0, y = 0;
                    _dataProvider.OrbitProjectionTransformControl.OrbitProjectionTransform.InvertTransform(row, col, ref x, ref y);
                    geoX = x;
                    geoY = y;
                    return;
                }
            }
            else
            {
                double prjX, prjY;
                Raster2Prj(row, col, out prjX, out prjY);
                Prj2Geo(prjX, prjY, out geoX, out geoY);
                return;
            }
            geoX = 0;
            geoY = 0;
        }

        public void Geo2Prj(double geoX, double geoY, out double prjX, out double prjY)
        {
            if (_projectionTransform == null)
            {
                prjX = prjY = 0;
                return;
            }
            double[] xs = new double[] { geoX };
            double[] ys = new double[] { geoY };
            //try
            //{
                _projectionTransform.Transform(xs, ys);
            //}
            //catch(Exception ex )
            //{
            //    Console.WriteLine("RasterDrawing.Geo2Prj:"+ex.Message);
            //}
            prjX = xs[0];
            prjY = ys[0];
        }

        public void Prj2Geo(double prjX, double prjY, out double geoX, out double geoY)
        {
            if (_projectionTransform == null)
            {
                geoX = geoY = 0;
                return;
            }
            double[] xs = new double[] { prjX };
            double[] ys = new double[] { prjY };
            try
            {
                _projectionTransform.InverTransform(xs, ys);
            }
            catch (Exception ex)
            {
                Console.WriteLine("RasterDrawing.Prj2Geo:" + ex.Message);
            }
            geoX = xs[0];
            geoY = ys[0];
        }

        public void Raster2Prj(int row, int col, out double prjX, out double prjY)
        {
            prjX = _originalEnvelope.MinX + col * _originalResolutionX;
            prjY = _originalEnvelope.MaxY - row * _originalResolutionY;
        }

        public void Raster2Prj(float row, float col, out double prjX, out double prjY)
        {
            prjX = _originalEnvelope.MinX + col * _originalResolutionX;
            prjY = _originalEnvelope.MaxY - row * _originalResolutionY;
        }

        public unsafe void Raster2Prj(PointF* rasterPoint)
        {
            rasterPoint->X = (float)(_originalEnvelope.MinX + rasterPoint->X * _originalResolutionX);
            rasterPoint->Y = (float)(_originalEnvelope.MaxY - rasterPoint->Y * _originalResolutionY);
        }

        public void Prj2Raster(double prjX, double prjY, out int row, out int col)
        {
            col = (int)((prjX - _originalEnvelope.MinX) / _originalResolutionX);
            row = (int)((_originalEnvelope.MaxY - prjY) / _originalResolutionY);
        }

        public IRasterDataProvider DataProvider
        {
            get { return _dataProvider; }
        }

        public IRasterDataProvider DataProviderCopy
        {
            get { return _dataProviderCopy; }
        }

        public ITileBitmapProvider TileBitmapProvider
        {
            get { return _tileBitmapProvider; }
        }

        public Bitmap GetBitmap(IDrawArgs drawArgs)
        {
            DrawByTiles(drawArgs);
            ApplyImageProcessors();
            //_bitmap.Save("d:\\1.bmp", ImageFormat.Bmp);
            return _bitmap;
        }

        private void ApplyImageProcessors()
        {
            if (_rgbProcessorStack == null || _rgbProcessorStack.Count == 0)
                return;
            //_rgbProcessorStack.Apply(GetAOI(), _bitmap);
            _rgbProcessorStack.Apply(null, _bitmap);//根据气象局遥感室业务人员的要求，不需要对感兴趣区域做增强。
        }

        private int[][] GetAOI()
        {
            if (_canvas.AOIGetter == null)
                return null;
            int[] aoi = _canvas.AOIGetter();
            if (aoi == null)
                return null;
            return new int[][] { aoi };
        }

        private bool _isFirst = true;
        private void DrawByTiles(IDrawArgs drawArgs)
        {
            LevelDef nearestLevel;
            Rectangle rect;
            int bRow, bCol, rows, cols;
            TileIdentify[] tiles = _tileBitmapProvider.GetVisibleTiles(out nearestLevel, out rect, out  bRow, out  bCol, out rows, out cols);
            if (_isFirst || tiles == null)
            {
                _isFirst = false;
                return;
            }
            if (rows == 0 || cols == 0)
                return;
            if (tiles != null)
            {
                //compute envelope
                DrawEngine.CoordPoint location = new DrawEngine.CoordPoint(_originalEnvelope.MinX + _originalResolutionX * rect.X,
                    _originalEnvelope.MinY + _originalResolutionY * (_dataProvider.Height - (rect.Y + rect.Height)));
                _envelope = new DrawEngine.CoordEnvelope(location,
                    _originalResolutionX * rect.Width, _originalResolutionY * rect.Height);
                //build empty bitmap
                int tileSize = _tileBitmapProvider.TileComputer.TileSize;
                int powerOf2 = _tileSetting.PowerOf2;
                int w = cols << powerOf2; //cols * tileSize
                int h = rows << powerOf2; //rows * tileSize
                if (_bitmap != null && (_preHeight != h || _preWidth != w))
                {
                    _bitmap.Dispose();
                    _bitmap = null;
                }
                if (_bitmap == null)
                    _bitmap = new Bitmap(w, h, PixelFormat.Format24bppRgb);
                _preWidth = w;
                _preHeight = h;
                //
                using (Graphics g = Graphics.FromImage(_bitmap))
                {
                    foreach (TileIdentify t in tiles)
                    {
                        TileBitmap tb = _tileBitmapProvider.GetTileBitmap(t);
                        if (tb.IsEmpty())
                            continue;
                        int x = (t.Col - bCol) << powerOf2; //(t.Col - bCol) * tileSize
                        int y = (t.Row - bRow) << powerOf2; //(t.Row - bRow) * tileSize;
                        //tb.Bitmap.Save("f:\\x" + t.Row.ToString() + "_" + t.Col.ToString()+".bmp", ImageFormat.Bmp);
                        g.DrawImage(tb.Bitmap, x, y);
                    }
                }
            }
        }

        public Color GetColorAt(int screenX, int screenY)
        {
            if (_canvas == null)
                return Color.Empty;
            double prjX, prjY;
            _canvas.CoordTransform.Screen2Prj(screenX, screenY, out prjX, out prjY);
            return GetColorAtPrj(prjX, prjY);
        }

        public Color GetColorAtPrj(double prjX, double prjY)
        {
            if (_canvas == null || _bitmap==null)
                return Color.Empty;
            int x = (int)((prjX - _envelope.MinX) / (_envelope.Width / _bitmap.Width));
            int y = (int)((_envelope.MaxY - prjY) / (_envelope.Height / _bitmap.Height));
            if (x < 0 || y < 0 || x >= _bitmap.Width || y >= _bitmap.Height)
                return _backColor;
            return _bitmap.GetPixel(x, y);
        }

        public unsafe void ReadPixelValues(int x, int y, double* buffer)
        {
            if (x < 0 || y < 0 || x >= _width || y >= _heigth)
                return;
            if (_dataProviderCopy is GDALRasterDataProvider)
            {
                IntPtr ptr = new IntPtr(buffer);
                for (int i = 1; i <= _bandCount; i++, ptr = IntPtr.Add(ptr, sizeof(double)))
                {
                    lock (_dataProviderCopy)
                        _dataProviderCopy.GetRasterBand(i).Read(x, y, 1, 1, ptr, enumDataType.Double, 1, 1);
                }
            }
            else
            {
                _readPixelHelper.Read(x, y, buffer);
            }
        }
        #region 几何精校正
        private GeoDo.RSS.Core.DF.CoordEnvelope _envelopeBeforeAdjusting;
        private GeoDo.RSS.Core.DF.CoordEnvelope _envelopeBeforeAdjustingCopy;
        private bool _isHasUnsavedGeoAdjusted = false;
        GeoDo.RSS.Core.DF.CoordEnvelope IGeoPanAdjust.EnvelopeBeforeAdjusting
        {
            get { return _envelopeBeforeAdjustingCopy; }
        }

        void IGeoPanAdjust.Start()
        {
            if (!_isGeoPanAdjusting)
            {
                _envelopeBeforeAdjusting = _dataProvider.CoordEnvelope.Clone();
                _envelopeBeforeAdjustingCopy = _dataProviderCopy.CoordEnvelope.Clone();
                _isGeoPanAdjusting = true;
                _isHasUnsavedGeoAdjusted = false;
            }
        }

        bool IGeoPanAdjust.IsAdjusting
        {
            get { return _isGeoPanAdjusting; }
        }

        bool IGeoPanAdjust.IsHasUnsavedGeoAdjusted
        {
            get { return _isHasUnsavedGeoAdjusted; }
        }

        void IGeoPanAdjust.Cancel()
        {
            IUpdateCoordEnvelope update = _dataProvider as IUpdateCoordEnvelope;
            if (update != null)
            {
                update.Update(_envelopeBeforeAdjusting);
                SetEnvelope();
                update.IsStoreHeaderChanged = false;
            }
            if (_dataProviderCopy != null)
            {
                update = _dataProviderCopy as IUpdateCoordEnvelope;
                if (update != null)
                    update.Update(_envelopeBeforeAdjustingCopy);
            }
            _isHasUnsavedGeoAdjusted = false;
        }

        void IGeoPanAdjust.Save()
        {
            IUpdateCoordEnvelope update = _dataProvider as IUpdateCoordEnvelope;
            if (update != null)
            {
                update.IsStoreHeaderChanged = true;
            }
            _isGeoPanAdjusting = false;
            _isHasUnsavedGeoAdjusted = false;
        }

        void IGeoPanAdjust.ApplyAdjust(double offsetGeoX, double offsetGeoY)
        {
            GeoDo.RSS.Core.DF.CoordEnvelope evp = new DF.CoordEnvelope(
                _dataProvider.CoordEnvelope.MinX + offsetGeoX,
                _dataProvider.CoordEnvelope.MaxX + offsetGeoX,
                _dataProvider.CoordEnvelope.MinY + offsetGeoY,
                _dataProvider.CoordEnvelope.MaxY + offsetGeoY);
            IUpdateCoordEnvelope update = _dataProvider as IUpdateCoordEnvelope;
            if (update != null)
            {
                update.Update(evp);
                SetEnvelope();
            }
            if (_dataProviderCopy != null)
            {
                update = _dataProviderCopy as IUpdateCoordEnvelope;
                if (update != null)
                    update.Update(evp);
            }
            _isHasUnsavedGeoAdjusted = true;
        }

        void IGeoPanAdjust.Stop(bool isSave)
        {
            if (isSave)
                (this as IGeoPanAdjust).Save();
            _isGeoPanAdjusting = false;
            _isHasUnsavedGeoAdjusted = false;
        }
        #endregion

        public static float EstimateRequiredMemory(string fname, out int memoryOfTile, out int tileCount)
        {
            return (new MemEstimatorOfRasterDrawing()).Estimate(fname,out memoryOfTile,out tileCount);
        }

        public void Dispose()
        {
            if (_readPixelHelper != null)
            {
                _readPixelHelper.Dispose();
                _readPixelHelper = null;
            }
            if (_tileBitmapProvider != null)
            {
                _tileBitmapProvider.Dispose();
                _tileBitmapProvider = null;
            }
            if (_bitmap != null)
            {
                _bitmap.Dispose();
                _bitmap = null;
            }
            if (_dataProviderCopy != null)
            {
                IUpdateCoordEnvelope u = _dataProviderCopy as IUpdateCoordEnvelope;
                if (u != null)
                    u.IsStoreHeaderChanged = false;
                _dataProviderCopy.Dispose();
                _dataProviderCopy = null;
            }
            if (_dataProvider != null)
            {
                _dataProvider.Dispose();
                _dataProvider = null;
            }
            if (_firstBitmap != null)
            {
                _firstBitmap.Dispose();
                _firstBitmap = null;
            }
            _envelope = null;
            _canvas = null;
        }
    }
}
