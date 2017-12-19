using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.Core.RasterDrawing
{
    public class TileBitmapCacheManager : ITileBitmapCacheManager, IDisposable
    {
        private readonly int MAX_CACHE_TILE_COUNT = 100000;
        private Dictionary<string, TileBitmap> _cachedTiles = new Dictionary<string, TileBitmap>();

        public TileBitmapCacheManager()
        {
        }

        public TileBitmapCacheManager(int maxTilesCount)
        {
            MAX_CACHE_TILE_COUNT = maxTilesCount;
        }

        public int Count
        {
            get { return _cachedTiles.Count; }
        }

        public void Clear()
        {
            while (_cachedTiles.Count > 0)
            {
                TileBitmap tb = _cachedTiles[_cachedTiles.Keys.First()];
                _cachedTiles.Remove(_cachedTiles.Keys.First());
                tb.Dispose();
            }
            _cachedTiles.Clear();
        }

        public IEnumerable<TileBitmap> TileBitmaps
        {
            get { return _cachedTiles.Values; }
        }

        public bool IsEmpty()
        {
            return _cachedTiles.Count == 0;
        }

        public bool IsExist(TileIdentify tile)
        {
            return _cachedTiles.ContainsKey(GetKey(tile));
        }

        public void Put(TileBitmap tileBitmap)
        {
            if (_cachedTiles.Count == MAX_CACHE_TILE_COUNT)
                _cachedTiles.Remove(_cachedTiles.Keys.First());
            string key = GetKey(tileBitmap);
            if (!_cachedTiles.ContainsKey(key))
                _cachedTiles.Add(key, tileBitmap);
        }

        public void Remove(TileBitmap tileBitmap)
        {
            string key = GetKey(tileBitmap);
            if (_cachedTiles.ContainsKey(key))
                _cachedTiles.Remove(key);
        }

        private string GetKey(TileBitmap tileBitmap)
        {
            return string.Format("L{0}R{1}C{2}", tileBitmap.Level.LevelNo, tileBitmap.Tile.Row, tileBitmap.Tile.Col);
        }

        private string GetKey(TileIdentify tile)
        {
            return string.Format("L{0}R{1}C{2}", tile.Level.LevelNo, tile.Row, tile.Col);
        }

        public TileBitmap Get(int levelNo, int row, int col)
        {
            string key = string.Format("L{0}R{1}C{2}", levelNo, row, col);
            return _cachedTiles.ContainsKey(key) ? _cachedTiles[key] : new TileBitmap();
        }

        public void Dispose()
        {
            Clear();
        }
    }
}
