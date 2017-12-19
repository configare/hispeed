using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using GeoDo.RSS.Core.DrawEngine;
using CodeCell.AgileMap.Core;
using System.Drawing.Imaging;

namespace GeoDo.RSS.Core.VectorDrawing
{
    public static class ExtMethods
    {
        public static Bitmap GetBitmapUseOriResolution(this ICanvas canvas)
        {
            IVectorHostLayer hostLayer = canvas.LayerContainer.VectorHost;
            if (hostLayer == null)
                return null;
            IMapRuntime runtime = hostLayer.MapRuntime as IMapRuntime;
            if (runtime == null)
                return null;
            IPrimaryDrawObject priObj = canvas.PrimaryDrawObject;
            Color oBackcolor = runtime.Map.MapArguments.BackColor;
            try
            {
                Image bitmap = new Bitmap(priObj.Size.Width, priObj.Size.Height, PixelFormat.Format32bppArgb);
                runtime.Map.MapArguments.BackColor = Color.Transparent;
                Envelope imageGeoEnv = PrjToGeoEnv(priObj, priObj.OriginalEnvelope.Clone());
                MapImageGeneratorDefault map = new MapImageGeneratorDefault(runtime);
                runtime.Host = map as CodeCell.AgileMap.Core.IMapRuntimeHost;
                map.GetMapImage(map.GeoEnvelope2Viewport(imageGeoEnv), new Size(priObj.Size.Width, priObj.Size.Height), ref bitmap);
                return bitmap as Bitmap;
            }
            finally
            {
                runtime.Host = hostLayer as CodeCell.AgileMap.Core.IMapRuntimeHost;
                runtime.Map.MapArguments.BackColor = oBackcolor;
            }
            //IRenderLayer renderLayer = layer as IRenderLayer;//GeoGridLayer
            //if (filter(layer) && renderLayer.Visible)
            //{
            //    renderLayer.Render(this, drawArgs);
            //}
        }

        private static Envelope PrjToGeoEnv(IPrimaryDrawObject priObj, CoordEnvelope coordEnvelope)
        {
            double geoMinX, geoMinY, geoMaxX, geoMaxY;
            priObj.Prj2Geo(coordEnvelope.MinX, coordEnvelope.MaxY, out geoMinX, out geoMaxY);
            priObj.Prj2Geo(coordEnvelope.MaxX, coordEnvelope.MinY, out geoMaxX, out geoMinY);
            return new Envelope(Math.Min(geoMinX, geoMaxX), Math.Min(geoMinY, geoMaxY),
                Math.Max(geoMinX, geoMaxX), Math.Max(geoMinY, geoMaxY));
        }
    }
}
