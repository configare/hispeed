using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using CodeCell.AgileMap.Core;
using GeoDo.RSS.Core.DF;
using System.Drawing.Imaging;

namespace GeoDo.RSS.MIF.Core
{
    public class AOITemplateStat<T> : IAOITemplateStat<T>
    {
        public StatResultItem[] CountByVector(string rasterFileName, string shpFullname, string shpPrimaryField, Func<T, bool> filter)
        {
            using (IRasterDataProvider rasterPrd = GeoDataDriver.Open(rasterFileName) as IRasterDataProvider)
            {
                return CountByVector(rasterPrd, shpFullname, shpPrimaryField, filter);
            }
        }

        public StatResultItem[] CountByVector(IRasterDataProvider raster, string shpFullname, string shpPrimaryField, Func<T, bool> filter)
        {
            if (filter == null)
                return null;
            if (String.IsNullOrEmpty(shpFullname))
                return null;
            //step2:读取矢量
            CodeCell.AgileMap.Core.Feature[] features = GetFeatures(shpFullname);
            if (features == null || features.Length == 0)
                return null;
            double lon = raster.CoordEnvelope.Center.X;
            double lat = raster.CoordEnvelope.Center.Y;
            //step3:矢量栅格化
            Dictionary<string, Color> nameColors = new Dictionary<string, Color>();
            using (Bitmap bitmap = VectorsToBitmap(raster, features, shpPrimaryField, out nameColors))
            {
                int[] aoi;
                Color color;
                int count;
                string name;
                List<StatResultItem> items = new List<StatResultItem>();
                IRasterOperator<T> oper = new RasterOperator<T>();
                foreach (Feature fea in features)
                {
                    name = fea.GetFieldValue(shpPrimaryField);
                    if (String.IsNullOrEmpty(name))
                        continue;
                    color = nameColors[name];
                    aoi = GetAOIByFeature(bitmap, color);
                    if (aoi == null)
                        count = 0;
                    else
                        count = oper.Count(raster, aoi, filter);
                    StatResultItem it = new StatResultItem();
                    it.Name = name;
                    it.Code = fea.OID.ToString();
                    double d = AreaCountHelper.CalcArea(lon, lat, raster.ResolutionX, raster.ResolutionY) * count * Math.Pow(10, -6);
                    it.Value = Math.Round(d, 3);
                    items.Add(it);
                }
                return items != null ? items.ToArray() : null;
            }
        }

        public StatResultItem[] CountByVector(IRasterDataProvider raster, Dictionary<string, int[]> aoi, Func<T, bool> filter)
        {
            if (filter == null)
                return null;
            if (aoi == null || aoi.Count == 0)
                return null;
            double lon = raster.CoordEnvelope.Center.X;
            double lat = raster.CoordEnvelope.Center.Y;
            int count = 0;
            List<StatResultItem> items = new List<StatResultItem>();
            IRasterOperator<T> oper = new RasterOperator<T>();
            foreach (string fea in aoi.Keys)
            {
                if (aoi[fea] == null)
                    count = 0;
                else
                    count = oper.Count(raster, aoi[fea], filter);
                StatResultItem it = new StatResultItem();
                it.Name = fea;
                double d = AreaCountHelper.CalcArea(lon, lat, raster.ResolutionX, raster.ResolutionY) * count * Math.Pow(10, -6);
                it.Value = Math.Round(d, 3);
                items.Add(it);
            }
            return items != null ? items.ToArray() : null;
        }

        public StatResultItem[] CountByVector(string rasterFileName, Dictionary<string, int[]> aoi, Func<T, bool> filter)
        {
            IRasterDataProvider rasterPrd = GeoDataDriver.Open(rasterFileName) as IRasterDataProvider;
            return CountByVector(rasterPrd, aoi, filter);
        }

        public StatResultItem[] CountByRaster(string rasterFileName, string templateName, Func<T, bool> filter)
        {
            IRasterDataProvider prd = GeoDataDriver.Open(rasterFileName) as IRasterDataProvider;
            return CountByRaster(prd, templateName, filter);
        }

        public StatResultItem[] CountByRaster(IRasterDataProvider raster, string templateName, Func<T, bool> filter)
        {
            switch (templateName)
            {
                case "China_XjRaster":
                    return CountByXJRaster(raster, filter);
                case "China_LandRaster":
                    return CountByLandRaster(raster, filter);
            }
            return null;
        }

        private StatResultItem[] CountByXJRaster(IRasterDataProvider raster, Func<T, bool> filter)
        {
            IRasterDictionaryTemplate<int> temp = RasterDictionaryTemplateFactory.CreateXjRasterTemplate();
            Dictionary<int, string> paris = temp.CodeNameParis;
            if (paris == null)
                return null;
            double lon = raster.CoordEnvelope.Center.X;
            double lat = raster.CoordEnvelope.Center.Y;
            IRasterOperator<T> oper = new RasterOperator<T>();
            List<StatResultItem> items = new List<StatResultItem>();
            int[] aoi;
            int count;
            foreach (int value in paris.Keys)
            {
                aoi = temp.GetAOIByKey(value, raster.CoordEnvelope.MinX, raster.CoordEnvelope.MaxX, raster.CoordEnvelope.MinY, raster.CoordEnvelope.MaxY, new Size(raster.Width, raster.Height));
                if (aoi == null)
                    continue;
                count = oper.Count(raster, aoi, filter);
                if (count == 0)
                    continue;
                StatResultItem it = new StatResultItem();
                it.Name = paris[value];
                it.Value = count;
                double d = AreaCountHelper.CalcArea(lon, lat, raster.ResolutionX, raster.ResolutionY) * count * Math.Pow(10, -6);
                it.Value = Math.Round(d, 3);
                items.Add(it);
            }
            return items != null ? items.ToArray() : null;
        }

        private StatResultItem[] CountByLandRaster(IRasterDataProvider raster, Func<T, bool> filter)
        {
            IRasterDictionaryTemplate<byte> temp = RasterDictionaryTemplateFactory.CreateLandRasterTemplate();
            Dictionary<byte, string> paris = temp.CodeNameParis;
            if (paris == null)
                return null;
            double lon = raster.CoordEnvelope.Center.X;
            double lat = raster.CoordEnvelope.Center.Y;
            IRasterOperator<T> oper = new RasterOperator<T>();
            List<StatResultItem> items = new List<StatResultItem>();
            int[] aoi;
            int count;
            foreach (string value in paris.Values)
            {
                aoi = temp.GetAOI(value, raster.CoordEnvelope.MinX, raster.CoordEnvelope.MaxX, raster.CoordEnvelope.MinY, raster.CoordEnvelope.MaxY, new Size(raster.Width, raster.Height));
                if (aoi == null)
                    continue;
                count = oper.Count(raster, aoi, filter);
                if (count == 0)
                    continue;
                StatResultItem it = new StatResultItem();
                it.Name = value;
                it.Value = count;
                double d = AreaCountHelper.CalcArea(lon, lat, raster.ResolutionX, raster.ResolutionY) * count * Math.Pow(10, -6);
                it.Value = Math.Round(d, 3);
                items.Add(it);
            }
            return items != null ? items.ToArray() : null;
        }

        public int[] GetAOIByFeature(Bitmap bitmap, Color color)
        {
            using (IBitmap2RasterConverter c = new Bitmap2RasterConverter())
            {
                return c.ToRaster(bitmap, color);
            }
        }

        public Bitmap VectorsToBitmap(IRasterDataProvider prd, CodeCell.AgileMap.Core.Feature[] features, string shpPrimaryField, out Dictionary<string, Color> nameColors)
        {
            using (IVector2BitmapConverter c = new Vector2BitmapConverter())
            {
                Dictionary<ShapePolygon, Color> vectors = GetVectorColors(features, shpPrimaryField, out nameColors);
                Bitmap bmp = new Bitmap(prd.Width, prd.Height, PixelFormat.Format24bppRgb);
                Envelope envelop = GetEnvelop(prd);
                c.ToBitmap(vectors, Color.Black, envelop, new Size(prd.Width, prd.Height), ref bmp);
                return bmp;
            }
        }

        private Envelope GetEnvelop(IRasterDataProvider prd)
        {
            Size size = new System.Drawing.Size();
            size.Width = prd.Width;
            size.Height = prd.Height;
            return new Envelope(prd.CoordEnvelope.MinX, prd.CoordEnvelope.MinY, prd.CoordEnvelope.MaxX, prd.CoordEnvelope.MaxY);
        }

        private Dictionary<ShapePolygon, Color> GetVectorColors(Feature[] features, string shpPrimaryField, out Dictionary<string, Color> nameColors)
        {
            Dictionary<ShapePolygon, Color> vectorColors = new Dictionary<ShapePolygon, Color>();
            nameColors = new Dictionary<string, Color>();
            int count = features.Count();
            Random random = new Random(1);
            Color color;
            for (int i = 0; i < count; i++)
            {
                color = Color.FromArgb(random.Next(255), random.Next(255), random.Next(255));
                if (features[i].GetFieldValue(shpPrimaryField) == null)
                    continue;
                if (!nameColors.Keys.Contains(features[i].GetFieldValue(shpPrimaryField))
                  && !String.IsNullOrEmpty(features[i].GetFieldValue(shpPrimaryField)))
                    nameColors.Add(features[i].GetFieldValue(shpPrimaryField), color);
                if (!vectorColors.Keys.Contains(features[i].Geometry as ShapePolygon))
                    vectorColors.Add(features[i].Geometry as ShapePolygon, color);
            }
            return vectorColors;
        }

        public Feature[] GetFeatures(string fname)
        {
            if (fname == null)
                return null;
            IVectorFeatureDataReader dr = VectorDataReaderFactory.GetUniversalDataReader(fname) as IVectorFeatureDataReader;
            if (dr == null)
                return null;
            try
            {
                Feature[] fets = dr.FetchFeatures();
                if (fets == null || fets.Length == 0)
                    return null;
                List<Feature> features = new List<Feature>();
                foreach (Feature fet in fets)
                {
                    if (fet.Geometry is ShapePolygon)
                        features.Add(fet);
                }
                return features != null ? features.ToArray() : null;
            }
            finally
            {
                dr.Dispose();
            }
        }

        #region weight

        public StatResultItem[] CountByVector(string rasterFileName, string shpFullname, string shpPrimaryField, Func<T, int, int> weight)
        {
            using (IRasterDataProvider rasterPrd = GeoDataDriver.Open(rasterFileName) as IRasterDataProvider)
            {
                return CountByVector(rasterPrd, shpFullname, shpPrimaryField, weight);
            }
        }

        public StatResultItem[] CountByVector(IRasterDataProvider raster, string shpFullname, string shpPrimaryField, Func<T, int, int> weight)
        {
            if (weight == null)
                return null;
            if (String.IsNullOrEmpty(shpFullname))
                return null;
            //step2:读取矢量
            CodeCell.AgileMap.Core.Feature[] features = GetFeatures(shpFullname);
            if (features == null || features.Length == 0)
                return null;
            double lon = raster.CoordEnvelope.Center.X;
            double lat = raster.CoordEnvelope.Center.Y;

            //step3:矢量栅格化
            Dictionary<string, Color> nameColors = new Dictionary<string, Color>();
            using (Bitmap bitmap = VectorsToBitmap(raster, features, shpPrimaryField, out nameColors))
            {
                int[] aoi;
                Color color;
                int count;
                string name;
                List<StatResultItem> items = new List<StatResultItem>();
                IRasterOperator<T> oper = new RasterOperator<T>();
                foreach (Feature fea in features)
                {
                    name = fea.GetFieldValue(shpPrimaryField);
                    if (String.IsNullOrEmpty(name))
                        continue;
                    color = nameColors[name];
                    aoi = GetAOIByFeature(bitmap, color);
                    if (aoi == null)
                        count = 0;
                    else
                        count = oper.Count(raster, aoi, weight);
                    StatResultItem it = new StatResultItem();
                    it.Name = name;
                    it.Code = fea.OID.ToString();
                    double d = AreaCountHelper.CalcArea(lon, lat, raster.ResolutionX, raster.ResolutionY) * count * Math.Pow(10, -6);
                    it.Value = Math.Round(d, 3);
                    items.Add(it);
                }
                return items != null ? items.ToArray() : null;
            }
        }

        public StatResultItem[] CountByVector(IRasterDataProvider raster, Dictionary<string, int[]> aoi, Func<T, int, int> weight)
        {
            if (weight == null)
                return null;
            if (aoi == null || aoi.Count == 0)
                return null;
            double lon = raster.CoordEnvelope.Center.X;
            double lat = raster.CoordEnvelope.Center.Y;
            int count = 0;
            List<StatResultItem> items = new List<StatResultItem>();
            IRasterOperator<T> oper = new RasterOperator<T>();
            foreach (string fea in aoi.Keys)
            {
                if (aoi[fea] == null)
                    count = 0;
                else
                    count = oper.Count(raster, aoi[fea], weight);
                StatResultItem it = new StatResultItem();
                it.Name = fea;
                double d = AreaCountHelper.CalcArea(lon,lat,raster.ResolutionX, raster.ResolutionY) * count * Math.Pow(10, -6);
                it.Value = Math.Round(d, 3);
                items.Add(it);
            }
            return items != null ? items.ToArray() : null;
        }

        public StatResultItem[] CountByVector(string rasterFileName, Dictionary<string, int[]> aoi, Func<T, int, int> weight)
        {
            IRasterDataProvider rasterPrd = GeoDataDriver.Open(rasterFileName) as IRasterDataProvider;
            return CountByVector(rasterPrd, aoi, weight);
        }

        public StatResultItem[] CountByRaster(string rasterFileName, string templateName, Func<T, int, int> weight)
        {
            IRasterDataProvider prd = GeoDataDriver.Open(rasterFileName) as IRasterDataProvider;
            return CountByRaster(prd, templateName, weight);
        }

        public StatResultItem[] CountByRaster(IRasterDataProvider raster, string templateName, Func<T, int, int> weight)
        {
            switch (templateName)
            {
                case "China_XjRaster":
                    return CountByXJRaster(raster, weight);
                case "China_LandRaster":
                    return CountByLandRaster(raster, weight);
            }
            return null;
        }

        private StatResultItem[] CountByXJRaster(IRasterDataProvider raster, Func<T, int, int> weight)
        {
            IRasterDictionaryTemplate<int> temp = RasterDictionaryTemplateFactory.CreateXjRasterTemplate();
            Dictionary<int, string> paris = temp.CodeNameParis;
            if (paris == null)
                return null;
            double lon = raster.CoordEnvelope.Center.X;
            double lat = raster.CoordEnvelope.Center.Y;
            IRasterOperator<T> oper = new RasterOperator<T>();
            List<StatResultItem> items = new List<StatResultItem>();
            int[] aoi;
            int count;
            foreach (int value in paris.Keys)
            {
                aoi = temp.GetAOIByKey(value, raster.CoordEnvelope.MinX, raster.CoordEnvelope.MaxX, raster.CoordEnvelope.MinY, raster.CoordEnvelope.MaxY, new Size(raster.Width, raster.Height));
                if (aoi == null)
                    continue;
                count = oper.Count(raster, aoi, weight);
                if (count == 0)
                    continue;
                StatResultItem it = new StatResultItem();
                it.Name = paris[value];
                it.Value = count;
                double d = AreaCountHelper.CalcArea(lon,lat,raster.ResolutionX, raster.ResolutionY) * count * Math.Pow(10, -6);
                it.Value = Math.Round(d, 3);
                items.Add(it);
            }
            return items != null ? items.ToArray() : null;
        }

        private StatResultItem[] CountByLandRaster(IRasterDataProvider raster, Func<T, int, int> weight)
        {
            IRasterDictionaryTemplate<byte> temp = RasterDictionaryTemplateFactory.CreateLandRasterTemplate();
            Dictionary<byte, string> paris = temp.CodeNameParis;
            if (paris == null)
                return null;
            double lon = raster.CoordEnvelope.Center.X;
            double lat = raster.CoordEnvelope.Center.Y;
            IRasterOperator<T> oper = new RasterOperator<T>();
            List<StatResultItem> items = new List<StatResultItem>();
            int[] aoi;
            int count;
            foreach (string value in paris.Values)
            {
                aoi = temp.GetAOI(value, raster.CoordEnvelope.MinX, raster.CoordEnvelope.MaxX, raster.CoordEnvelope.MinY, raster.CoordEnvelope.MaxY, new Size(raster.Width, raster.Height));
                if (aoi == null)
                    continue;
                count = oper.Count(raster, aoi, weight);
                if (count == 0)
                    continue;
                StatResultItem it = new StatResultItem();
                it.Name = value;
                it.Value = count;
                double d = AreaCountHelper.CalcArea(lon,lat,raster.ResolutionX, raster.ResolutionY) * count * Math.Pow(10, -6);
                it.Value = Math.Round(d, 3);
                items.Add(it);
            }
            return items != null ? items.ToArray() : null;
        }

        #endregion
    }
}
