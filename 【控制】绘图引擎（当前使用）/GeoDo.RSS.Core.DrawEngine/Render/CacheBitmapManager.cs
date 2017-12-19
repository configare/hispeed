using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

namespace GeoDo.RSS.Core.DrawEngine
{
    public class CacheBitmapManager:IDisposable
    {
        private CacheBitmap _rasterCacheBitmap = null;
        private CacheBitmap _vectorCacheBitmap = null;
        private CacheBitmap _flyCacheBitmap = null;

        public CacheBitmapManager(Color rasterFillColor,Color vectorFillColor,Color flyFillColor,
                                                      IDrawArgs rasterDrawArgs,IDrawArgs vectorDrawArgs,IDrawArgs flyDrawArgs,InterpolationMode interpolationMode)
        {
            _rasterCacheBitmap = new CacheBitmap(PixelFormat.Format24bppRgb, rasterFillColor,rasterDrawArgs,interpolationMode);
            _vectorCacheBitmap = new CacheBitmap(PixelFormat.Format32bppArgb,vectorFillColor,vectorDrawArgs,interpolationMode);
            _flyCacheBitmap = new CacheBitmap(PixelFormat.Format32bppArgb,flyFillColor,flyDrawArgs,interpolationMode);
        }

        public CacheBitmap RasterCacheBitmap
        {
            get { return _rasterCacheBitmap; }
        }

        public CacheBitmap VectorCacheBitmap
        {
            get { return _vectorCacheBitmap; }
        }

        public CacheBitmap FlyCacheBitmap
        {
            get { return _flyCacheBitmap; }
        }

        public void Reset(int width, int height)
        {
            _rasterCacheBitmap.Reset(width, height);
            _vectorCacheBitmap.Reset(width, height);
            _flyCacheBitmap.Reset(width, height);
        }

        public void Dispose()
        {
            _rasterCacheBitmap.Dispose();
            _vectorCacheBitmap.Dispose();
            _flyCacheBitmap.Dispose();
            _rasterCacheBitmap = null;
            _vectorCacheBitmap = null;
            _flyCacheBitmap = null;
        }
    }
}
