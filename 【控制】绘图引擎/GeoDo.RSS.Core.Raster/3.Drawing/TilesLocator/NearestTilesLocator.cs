using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DrawEngine;
using GeoDo.RSS.Core.DF;
using System.Drawing;

namespace GeoDo.RSS.Core.RasterDrawing
{
    internal class NearestTilesLocator : INearestTilesLocator, IDisposable
    {
        private TileComputer _tileComputer = null;
        private IRasterDataProvider _dataProvider = null;
        private LevelDef[] _leves = null;
        private GeoDo.RSS.Core.DrawEngine.CoordEnvelope _envelope = null;
        private float _originalResolutionX = 0, _originalResolutionY = 0;

        public NearestTilesLocator(IRasterDataProvider dataProvider, GeoDo.RSS.Core.DrawEngine.CoordEnvelope originalEnvelope, float originalResolutionX, float originalResolutionY)
        {
            _envelope = originalEnvelope;
            _dataProvider = dataProvider;
            _originalResolutionX = originalResolutionX;
            _originalResolutionY = originalResolutionY;
        }

        public ITileComputer TileComputer
        {
            get { return _tileComputer; }
        }

        public void ComputeByLevel(ICanvas canvas, LevelDef level, out TileIdentify[] tiles)
        {
            UpdateTileComputer(canvas);
            int beginRow = 0, beginCol = 0, width = 0, height = 0;
            GetRasterRowColOfViewWnd(canvas, out beginRow, out beginCol, out width, out height);
            tiles = level.GetTileIdentifiesByOriginal(beginRow, beginCol, width, height);
        }

        public void ComputeExtand(ICanvas canvas, LevelDef level, out TileIdentify[] tiles)
        {
            double expandX = canvas.CurrentEnvelope.Width  * canvas.CanvasSetting.XExpand;
            double expandY = canvas.CurrentEnvelope.Height * canvas.CanvasSetting.YExpand;
            int beginRow = 0, beginCol = 0, width = 0, height = 0;
            GetRasterRowColOfViewWnd(canvas, expandX,expandY, out beginRow, out beginCol, out width, out height);
            tiles = level.GetTileIdentifiesByOriginal(beginRow, beginCol, width, height);
        }

        public void Compute(ICanvas canvas, out LevelDef level, out TileIdentify[] tiles)
        {
            level = new LevelDef();
            tiles = null;
            UpdateTileComputer(canvas);
            level = GetNearestLevel(canvas);
            if (level.IsEmpty())
                return;
            int beginRow = 0, beginCol = 0, width = 0, height = 0;
            GetRasterRowColOfViewWnd(canvas, out beginRow, out beginCol, out width, out height);
            tiles = level.GetTileIdentifiesByOriginal(beginRow, beginCol, width, height);
        }

        public void GetRasterRowColOfViewWnd(ICanvas canvas, out int beginRow, out int beginCol, out int width, out int height)
        {
            float resolutionX = _originalResolutionX;// _dataProvider.ResolutionX;
            float resolutionY = _originalResolutionY;// _dataProvider.ResolutionY;
            if (resolutionX < float.Epsilon)
                resolutionX = 1;
            if (resolutionY < float.Epsilon)
                resolutionY = 1;
            //raster coord by left-upper 
            beginCol = (int)((canvas.CurrentEnvelope.MinX - _envelope.MinX) / resolutionX);
            beginRow = (int)((_envelope.MaxY - canvas.CurrentEnvelope.MaxY) / resolutionY);
            width = (int)(canvas.CurrentEnvelope.Width / resolutionX);
            height = (int)(canvas.CurrentEnvelope.Height / resolutionY);
        }

        public void GetRasterRowColOfViewWnd(ICanvas canvas, double expandX,double expandY, out int beginRow, out int beginCol, out int width, out int height)
        {
            float resolutionX = _originalResolutionX;// _dataProvider.ResolutionX;
            float resolutionY = _originalResolutionY;// _dataProvider.ResolutionY;
            if (resolutionX < float.Epsilon)
                resolutionX = 1;
            if (resolutionY < float.Epsilon)
                resolutionY = 1;
            //raster coord by left-upper 
            beginCol = (int)((canvas.CurrentEnvelope.MinX - expandX - _envelope.MinX) / resolutionX);
            beginRow = (int)((_envelope.MaxY - (canvas.CurrentEnvelope.MaxY + expandY)) / resolutionY);
            width = (int)((canvas.CurrentEnvelope.Width + 2 * expandX) / resolutionX);
            height = (int)((canvas.CurrentEnvelope.Height + 2 * expandY) / resolutionY);
        }

        public void GetRasterRowColOfViewWnd(ICanvas canvas, out float beginRow, out float beginCol, out float width, out float height)
        {
            float resolutionX = _originalResolutionX;// _dataProvider.ResolutionX;
            float resolutionY = _originalResolutionY;// _dataProvider.ResolutionY;
            if (resolutionX < float.Epsilon)
                resolutionX = 1;
            if (resolutionY < float.Epsilon)
                resolutionY = 1;
            //raster coord by left-upper 
            beginCol = (float)(canvas.CurrentEnvelope.MinX - _envelope.MinX) / resolutionX;
            beginRow = (float)(_envelope.MaxY - canvas.CurrentEnvelope.MaxY) / resolutionY;
            width = (float)canvas.CurrentEnvelope.Width / resolutionX;
            height = (float)canvas.CurrentEnvelope.Height / resolutionY;
        }

        public void GetCenterRowColOfViewWnd(ICanvas canvas, out int centerRow, out int centerCol)
        {
            float resolutionX = _dataProvider.ResolutionX;
            float resolutionY = _dataProvider.ResolutionY;
            if (resolutionX < float.Epsilon)
                resolutionX = 1;
            if (resolutionY < float.Epsilon)
                resolutionY = 1;
            //raster coord by left-upper 
            centerCol = (int)((canvas.CurrentEnvelope.MinX - _envelope.MinX + canvas.CurrentEnvelope.Width / 2) / resolutionX);
            centerRow = (int)((_envelope.MaxY - canvas.CurrentEnvelope.MaxY + canvas.CurrentEnvelope.Height / 2) / resolutionY);
        }

        private LevelDef GetNearestLevel(ICanvas canvas)
        {
            float scale = 1 / (canvas.ResolutionX / _originalResolutionX);
            float dlt = 0;
            float minDlt = float.MaxValue;
            LevelDef retLevel = new LevelDef();
            foreach (LevelDef level in _leves)
            {
                dlt = Math.Abs(level.Scale - scale);
                if (dlt < minDlt)
                {
                    retLevel = level;
                    minDlt = dlt;
                }
            }
            return retLevel;
        }

        public void UpdateTileComputer(ICanvas canvas)
        {
            TileSetting tileSetting = canvas.CanvasSetting != null ? canvas.CanvasSetting.TileSetting : new TileSetting();
            if (_tileComputer == null || _tileComputer.TileSize != tileSetting.TileSize || _tileComputer.SampleRatio != tileSetting.SampleRatio)
            {
                _tileComputer = new TileComputer(tileSetting.TileSize, tileSetting.SampleRatio);
                _leves = _tileComputer.GetLevelDefs(_dataProvider.Width, _dataProvider.Height);
            }
        }

        private void PrintCurrentLevel(LevelDef nearestLevel, ICanvas canvas)
        {
            Console.WriteLine(nearestLevel.ToString());
            Console.Write(" ");
            Console.WriteLine((1 / canvas.ResolutionX).ToString());
        }

        public void Dispose()
        {
        }
    }
}
