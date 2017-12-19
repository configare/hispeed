using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Diagnostics;

namespace GeoDo.RSS.Core.RasterDrawing
{
    public class BandMemoryCache : IBandMemoryCache
    {
        private int _bandNo;
        private IRequestTileAgent _requestAgent;
        private Dictionary<int, Dictionary<int, TileData>> _cache = new Dictionary<int, Dictionary<int, TileData>>();
        private LevelTileComputer _tileComputer;

        public BandMemoryCache(LevelTileComputer tileComputer, int bandNo, IRequestTileAgent requestAgent)
        {
            _tileComputer = tileComputer;
            _bandNo = bandNo;
            _requestAgent = requestAgent;
        }

        public int BandNo
        {
            get { return _bandNo; }
        }

        private TileId[] GetVisibileTiles(int levelNo, Rectangle viewport)
        {
            LevelId lv = _tileComputer.Levels[levelNo];
            //当视窗大小转换为本级分辨率下
            float scale = lv.Scale;
            int bCol = (int)(viewport.Left * scale);
            int bRow = (int)(viewport.Top * scale);
            int width = (int)(viewport.Width * scale);
            int height = (int)(viewport.Height * scale);
            //计算本级和视窗的交集
            Rectangle lvRect = new Rectangle(0, 0, lv.Width, lv.Height);
            lvRect.Intersect(new Rectangle(bCol, bRow, width, height));
            if (lvRect.IsEmpty)
                return null;
            List<TileId> retTiles = new List<TileId>();
            int tileSize = _tileComputer.TileSize;
            bCol = lvRect.Left / tileSize;
            bRow = lvRect.Top / tileSize;
            int tilesByHeight = (int)Math.Ceiling(lvRect.Height / (float)tileSize);
            int tilesByWidth = (int)Math.Ceiling(lvRect.Width / (float)tileSize);
            int idx = 0;
            for (int r = bRow; r < bRow + tilesByHeight; r++)
            {
                idx = r * lv.Cols + bCol;
                for (int c = bCol; c < bCol + tilesByWidth; c++, idx++)
                {
                    retTiles.Add(lv.Tiles[idx]);
                }
            }
            return retTiles.Count > 0 ? retTiles.ToArray() : null;
        }

        public void LoadBySync(int levelNo)
        {
            _requestAgent.LoadBySync(_bandNo, _tileComputer.Levels[levelNo]);
        }

        public void Update(int levelNo, Rectangle rasterEnvelopeOfWnd, Rectangle rasterEnvelopeOfVirtualWnd)
        {
            LevelId lv = _tileComputer.Levels[levelNo];
            TileId[] visibleTiles = GetVisibileTiles(levelNo, rasterEnvelopeOfVirtualWnd);
            //通知异步读取线程优先读取(这里还可以优化)
            Console.WriteLine("---------------- bandNo " + _bandNo.ToString() + "--------------------");
            if (visibleTiles != null)
            {
                for (int i = 0; i < visibleTiles.Length; i++)
                {
                    TileId tile = visibleTiles[i];
                    if (!TileIsEixst(tile))
                    {
                        _requestAgent.DoRequest(_bandNo, tile);
                        Console.WriteLine("Request: " + tile.ToString());
                    }
                    else
                    {
                        Console.WriteLine("In Memeory: " + tile.ToString());
                    }
                }
            }
        }

        public TileData[] GetTiles(int levelNo, Rectangle rasterEnvelope)
        {
            if (levelNo > _tileComputer.Levels.Length - 1)
                return null;
            TileId[] visibleTiles = GetVisibileTiles(levelNo, rasterEnvelope);
            //如果本级没有缓存,则向上找分辨率较粗的一级代替
            if (!_cache.ContainsKey(levelNo))
            {
                //通知异步读取线程优先读取
                for (int i = 0; i < visibleTiles.Length; i++)
                {
                    TileId tile = visibleTiles[i];
                    _requestAgent.DoRequest(_bandNo, tile);
                }
                //找分辨率较粗的一级代替
                GetTiles(levelNo++, rasterEnvelope);
            }
            LevelId lv = _tileComputer.Levels[levelNo];
            Dictionary<int, TileData> lvTiles = _cache[levelNo];
            List<TileData> retTiles = new List<TileData>();
            int idx = 0;
            for (int i = 0; i < visibleTiles.Length; i++)
            {
                TileId tile = visibleTiles[i];
                if (lvTiles.ContainsKey(tile.Index))
                    retTiles.Add(lvTiles[tile.Index]);
                else//如果该块不在内存,则通知异步获取,向上查找较粗分辨率的一层代替
                {
                    TileData data = ToUpperLevel(lv.No, tile.Row, tile.Col);
                    if (data != null)
                        retTiles.Add(data);
                    _requestAgent.DoRequest(_bandNo, _tileComputer.Levels[lv.No].Tiles[idx]);
                }
            }
            return retTiles.Count > 0 ? retTiles.ToArray() : null;
        }

        private TileData ToUpperLevel(int levelNo, int row, int col)
        {
            int maxLevel = _tileComputer.Levels.Length - 1;
            int r = row, c = col;
            int idx = 0;
            while (levelNo <= maxLevel)
            {
                idx = r * _tileComputer.Levels[levelNo].Cols + c;
                if (_cache.ContainsKey(levelNo))
                    if (_cache[levelNo].ContainsKey(idx))
                        return _cache[levelNo][idx];
                r = (int)(r / 2f);
                c = (int)(c / 2f);
                levelNo++;
            }
            return null;
        }

        private object lockObjForCache = new object();
        public void Cache(TileData tileData)
        {
             lock (lockObjForCache)
            {
                //暂时为全部缓存
                int levelNo = tileData.Tile.LevelNo;
                Dictionary<int, TileData> levelCache;
                if (!_cache.ContainsKey(levelNo))
                {
                    levelCache = new Dictionary<int, TileData>();
                    _cache.Add(levelNo, levelCache);
                    levelCache.Add(tileData.Tile.Index, tileData);
                    Console.WriteLine("Cached: " + tileData.Tile.ToString());
                }
                else
                {
                    levelCache = _cache[tileData.Tile.LevelNo];
                    if (!levelCache.ContainsKey(tileData.Tile.Index))
                    {
                        levelCache.Add(tileData.Tile.Index, tileData);
                        Console.WriteLine("Cached: " + tileData.Tile.ToString());
                    }
                }
            }
        }

        public void ReduceMemory(int levelNo,int rows,int cols, Rectangle cacheWindow)
        {
            if (_cache.ContainsKey(levelNo))
            {
                List<int> outsideIdxs = new List<int>();
                int r = 0, c = 0;
                Dictionary<int, TileData> levelCache = _cache[levelNo];
                foreach (int idx in levelCache.Keys)
                {
                    r = idx / cols;
                    c = idx - r * cols;
                    if (cacheWindow.Contains(c, r))
                        outsideIdxs.Add(idx);
                }
                for (int i = 0; i < outsideIdxs.Count; i++)
                {
                    TileData tile = levelCache[outsideIdxs[i]];
                    Console.WriteLine("Will Remove:" + tile.Tile.ToString());
                    levelCache.Remove(outsideIdxs[i]);
                }
             }
        }

        private bool TileIsEixst(TileId tile)
        {
            Dictionary<int, TileData> levelCache = null;
            if (!_cache.ContainsKey(tile.LevelNo))
            {
                levelCache = new Dictionary<int, TileData>();
                _cache.Add(tile.LevelNo, levelCache);
            }
            return _cache[tile.LevelNo].ContainsKey(tile.Index);
        }
    }
}
