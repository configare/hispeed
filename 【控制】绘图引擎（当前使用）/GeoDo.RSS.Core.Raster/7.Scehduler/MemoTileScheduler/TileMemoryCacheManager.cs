using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using GeoDo.RSS.Core.DF;
using System.Runtime.InteropServices;

namespace GeoDo.RSS.Core.RasterDrawing
{
    public class TileMemoryCacheManager : ITileMemoryCacheManager,
        IDisposable, IRequestTileAgent, IMemoryCacheControler
    {
        //虚拟视窗相对于当前视窗的放大倍数
        private int _wndExtandMultiple = 1;
        //瓦片计算对象
        private LevelTileComputer _tileComputer;
        //当前视窗大小
        private int _wndWidth;
        private int _wndHeight;
        //缓存控制
        private const int LIMIT_SIZE_ALLCACHE = 2000 * 2000;//pixels
        private enumCacheType _cacheType = enumCacheType.Nearest3Level;
        //当前视窗对应的栅格范围（行列号）
        private Rectangle _rasterEnvelopeOfWnd = new Rectangle();
        //虚拟视窗(扩大区域)对应的栅格范围(行列号)
        private Rectangle _rasterEnvelopeOfVirtualWnd = new Rectangle();
        //缓存的Tiles(分波段)
        private Dictionary<int, IBandMemoryCache> _bandCaches = new Dictionary<int, IBandMemoryCache>();
        //当前显示波段
        private int[] _bands;
        private ITileDiskCacheManager _diskCacheManager;
        private IRasterDataProvider _dataProvider;
        private CacheSettings _settings;
        private LevelId _currentLevel;
        private delegate void RequestTileHandler(int bandNo, TileId tile);
        private RequestTileHandler _requestTileHandler;

        public TileMemoryCacheManager(CacheSettings settings, Size wndSize, int[] bands, IRasterDataProvider dataProvider)
        {
            _settings = settings;
            _wndExtandMultiple = _settings.WndExtandMultiple;
            _wndWidth = wndSize.Width;
            _wndHeight = wndSize.Height;
            _bands = bands;
            _dataProvider = dataProvider;
            _tileComputer = new LevelTileComputer(_settings.TileSize, _dataProvider.Width, _dataProvider.Height);
            _requestTileHandler = new RequestTileHandler(RequestTileHandlerFuc);
            SetCacheType(new Size(dataProvider.Width, dataProvider.Height));
            CreateAndStartDiskCacheManager();
        }

        private void SetCacheType(Size size)
        {
            int pixels = size.Width * size.Height;
            if (pixels < LIMIT_SIZE_ALLCACHE)
                _cacheType = enumCacheType.All;
        }

        private unsafe void CreateAndStartDiskCacheManager()
        {
            _diskCacheManager = new TileDiskCacheManager(_dataProvider, _settings.CacheDir,
                _dataProvider.fileName, _tileComputer,
                new TileReadedCallback(OnReceiveTile));
            //将第一次设置的读取波段添加到异步读取队列
            for (int i = 0; i < _bands.Length; i++)
            {
                _bandCaches.Add(_bands[i], new BandMemoryCache(_tileComputer, _bands[i], this));
                for (int lv = 0; lv < _tileComputer.Levels.Length; lv++)
                    _diskCacheManager.Enqueue(_bands[i], _tileComputer.Levels[lv]);
            }
            //将剩余波段加入异步读取队列
            for (int b = 1; b <= _dataProvider.BandCount; b++)
            {
                if (Array.IndexOf<int>(_bands, b) >= 0)
                    continue;
                _bandCaches.Add(b, new BandMemoryCache(_tileComputer, b, this));
                for (int lv = 0; lv < _tileComputer.Levels.Length; lv++)
                    _diskCacheManager.Enqueue(b, _tileComputer.Levels[lv]);
            }
        }

        public LevelTileComputer TileComputer
        {
            get { return _tileComputer; }
        }

        public Rectangle RasterEnvelopeOfWnd
        {
            get { return _rasterEnvelopeOfWnd; }
        }

        public int[] Bands
        {
            get { return _bands; }
        }

        public void ChangeBands(int[] bands)
        {
            _bands = bands;
            UpdateCachedTiles();
        }

        public bool IsRunning
        {
            get { return _diskCacheManager.IsRunning; }
        }

        public void Start()
        {
            _diskCacheManager.Start();
        }

        public void Stop()
        {
            _diskCacheManager.Stop();
        }

        public enumCacheType CacheType
        {
            get { return _cacheType; }
        }

        public Rectangle GetCacheWindow(int levelNo)
        {
            LevelId lv = _tileComputer.Levels[levelNo];
            int rasterX = (int)(_rasterEnvelopeOfVirtualWnd.Left * lv.Scale);
            int rasterY = (int)(_rasterEnvelopeOfVirtualWnd.Top * lv.Scale);
            int width = (int)(_rasterEnvelopeOfVirtualWnd.Width * lv.Scale);
            int height = (int)(_rasterEnvelopeOfVirtualWnd.Height * lv.Scale);
            Rectangle lvRect = new Rectangle(0, 0, lv.Width, lv.Height);
            lvRect.Intersect(new Rectangle(rasterX, rasterY, width, height));
            if (lvRect.IsEmpty)
                return Rectangle.Empty;
            int tileSize = _tileComputer.TileSize;
            int bCol = lvRect.Left / tileSize;
            int bRow = lvRect.Top / tileSize;
            int tilesByHeight = (int)Math.Ceiling(lvRect.Height / (float)tileSize);
            int tilesByWidth = (int)Math.Ceiling(lvRect.Width / (float)tileSize);
            return new Rectangle(bCol, bRow, bCol + tilesByWidth, bRow + tilesByHeight);
        }

        public void GetNearLevels(int levelNo, out int lowLevelNo, out int highLevelNo)
        {
            lowLevelNo = Math.Min(-1, levelNo - 1);
            highLevelNo = levelNo + 1;
            if (highLevelNo > _tileComputer.Levels.Length - 1)
                highLevelNo = -1;
        }

        public void ReduceMemory()
        {
            int highLevel, lowLevel;
            GetNearLevels(_currentLevel.No, out lowLevel, out highLevel);
            for (int lv = 0; lv < _tileComputer.Levels.Length; lv++)
            {
                if (lv == _currentLevel.No || lv == lowLevel || lv == highLevel)
                    continue;
                Rectangle rect = GetCacheWindow(lv);
                for (int i = 0; i < _bands.Length; i++)
                {
                    _bandCaches[_bands[i]].ReduceMemory(lv, _tileComputer.Levels[lv].Rows, _tileComputer.Levels[lv].Cols, rect);
                }
            }
        }

        public void UpdateWindowSize(Size size)
        {
            _wndWidth = size.Width;
            _wndHeight = size.Height;
        }

        public void UpdateRasterEnvelope(float rasterScale, int beginX, int beginY, int xSize, int ySize)
        {
            _rasterEnvelopeOfWnd.Location = new Point(beginX, beginY);
            _rasterEnvelopeOfWnd.Width = xSize;
            _rasterEnvelopeOfWnd.Height = ySize;
            //
            if (_wndExtandMultiple > 1)
            {
                float scale = _rasterEnvelopeOfWnd.Width / _wndWidth;
                float extandWidth = (_wndExtandMultiple * _wndWidth - _wndWidth) / 2f;
                float extandHeight = (_wndExtandMultiple * _wndHeight - _wndHeight) / 2f;
                int extandHalfCols = (int)(extandWidth * scale);
                int extandHalfRows = (int)(extandHeight * scale);
                _rasterEnvelopeOfVirtualWnd.Location = new Point(beginX - extandHalfCols, beginY - extandHalfRows);
                _rasterEnvelopeOfVirtualWnd.Width = _rasterEnvelopeOfWnd.Width + 2 * extandHalfCols;
                _rasterEnvelopeOfVirtualWnd.Height = _rasterEnvelopeOfWnd.Height + 2 * extandHalfRows;
            }
            else//virtual window size = actual window size
            {
                _rasterEnvelopeOfVirtualWnd = _rasterEnvelopeOfWnd;
            }
            //计算当前显示级别
            _currentLevel = _tileComputer.GetNearestLevel(rasterScale);
            //
            UpdateCachedTiles();
        }

        private void UpdateCachedTiles()
        {
            for (int i = 0; i < _bands.Length; i++)
            {
                _bandCaches[_bands[i]].Update(_currentLevel.No, _rasterEnvelopeOfWnd, _rasterEnvelopeOfVirtualWnd);
            }
        }

        private void OnReceiveTile(TileData tileData)
        {
            if (tileData == null)
                return;
            _bandCaches[tileData.BandNo].Cache(tileData);
            Console.WriteLine("Receive BandNo:" + tileData.BandNo.ToString() + "," + tileData.Tile.ToString());
        }

        public void Dispose()
        {
            if (_diskCacheManager != null)
            {
                _diskCacheManager.Dispose();
                _diskCacheManager = null;
            }
            _bandCaches.Clear();
            _bandCaches = null;
        }


        public void DoRequest(int bandNo, TileId tile)
        {
            _requestTileHandler.BeginInvoke(bandNo, tile, null, null);
        }

        private void RequestTileHandlerFuc(int bandNo, TileId tile)
        {
            _diskCacheManager.Request(bandNo, new TileId[] { tile });
        }

        public void LoadBySync(int bandNo,LevelId level)
        {
            _diskCacheManager.LoadSync(bandNo, level);
        }

        public void LoadBySync(int levelNo)
        {
            for (int b = 0; b < _bands.Length; b++)
                _diskCacheManager.LoadSync(_bands[b], _tileComputer.Levels[levelNo]);
        }

        public void LoadBySync_MaxLevel()
        {
            LevelId lv = _tileComputer.Levels[_tileComputer.Levels.Length - 1];
            LoadBySync(lv.No);
            for (int b = 0; b < _dataProvider.BandCount; b++)
            {
                if (Array.IndexOf<int>(_bands, b + 1) >= 0)
                    continue;
                _diskCacheManager.Enqueue(b + 1, lv);
            }
        }

        public TileData[][] GetVisibleTileDatas()
        {
            TileData[][] allTiles = new TileData[_bands.Length][];
            for (int i = 0; i < _bands.Length; i++)
            {
                TileData[] tiles = _bandCaches[_bands[i]].GetTiles(_currentLevel.No, _rasterEnvelopeOfWnd);
                allTiles[i] = tiles;
            }
            return allTiles;
        }
    }
}
