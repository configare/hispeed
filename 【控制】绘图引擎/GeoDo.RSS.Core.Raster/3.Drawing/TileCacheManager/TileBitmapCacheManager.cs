using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.Core.RasterDrawing
{
    public class TileBitmapCacheManager : ITileBitmapCacheManager, IDisposable
    {
        private Dictionary<int, TileBitmap> _cachedTiles = new Dictionary<int, TileBitmap>();
        private List<int> _currentVisibleTiles = new List<int>();

        public TileBitmapCacheManager()
        {
        }

        public int Count
        {
            get { return _cachedTiles.Count; }
        }

        public void Clear()
        {
            foreach (int id in _cachedTiles.Keys)
            {
                TileBitmap tb = _cachedTiles[id];
                tb.Dispose();
            }
            _cachedTiles.Clear();
        }

        public void ClearPastTiles(TileIdentify[] visibleTiles)
        {
            //_cachedTiles.Clear();
            
            lock (_cachedTiles)
            {
                List<int> expiredTiles = new List<int>();
                foreach (int id in _cachedTiles.Keys)
                {
                    foreach (TileIdentify tile in visibleTiles)
                    {
                        if (tile.TileNo == id)
                        {
                            goto nxtTile;
                        }
                    }
                    expiredTiles.Add(id);
                nxtTile: ;
                }
                foreach (int id in expiredTiles)
                {
                    //TileBitmap tb = _cachedTiles[id];
                    _cachedTiles.Remove(id);
                    //tb.Dispose();
                }
            }
            
        }

        public void WaitLoading(TileIdentify[] visibleTiles)
        {
            _currentVisibleTiles.Clear();
            foreach (TileIdentify tile in visibleTiles)
                _currentVisibleTiles.Add(tile.TileNo);
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
            return _cachedTiles.ContainsKey(tile.TileNo);
        }

        public void Put(TileBitmap tileBitmap)
        {
            lock (_cachedTiles)
            {
                int key = tileBitmap.Tile.TileNo;
                if (!_cachedTiles.ContainsKey(key) && _currentVisibleTiles.Contains(key))
                    _cachedTiles.Add(key, tileBitmap);
            }
        }

        public void Remove(TileBitmap tileBitmap)
        {
            int key = tileBitmap.Tile.TileNo;
            if (_cachedTiles.ContainsKey(key))
                _cachedTiles.Remove(key);
        }

        public TileBitmap Get(int levelNo, int row, int col)
        {
            int key = TileIdentify.GetTileNo(levelNo, row, col);
            return _cachedTiles.ContainsKey(key) ? _cachedTiles[key] : new TileBitmap();
        }

        public void Dispose()
        {
            Clear();
        }
    }
}
