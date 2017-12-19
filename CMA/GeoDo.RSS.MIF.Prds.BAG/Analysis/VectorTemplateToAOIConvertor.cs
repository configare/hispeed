using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.MIF.Core;
using CodeCell.AgileMap.Core;
using System.Drawing;
using System.Drawing.Imaging;

namespace GeoDo.RSS.MIF.Prds.BAG
{
    public class VectorTemplateToAOIConvertor
    {
        public static int[] GetAOIByVectorTemplate(IRasterDataProvider dataProvider, string shpTemplateName, string primaryFieldName)
        {
            string shpFullname = VectorAOITemplate.FindVectorFullname(shpTemplateName);
            if (String.IsNullOrEmpty(shpFullname))
                return null;
            //step2:读取矢量
            Feature[] features = GetFeatures(shpFullname);
            if (features == null || features.Length == 0)
                return null;
            //step3:矢量栅格化
            Dictionary<string, Color> nameColors = new Dictionary<string, Color>();
            using (Bitmap bitmap = VectorsToBitmap(dataProvider, features, primaryFieldName, out nameColors))
            {
                int[] aoi;
                Color color;
                string name;
                List<int> items = new List<int>();
                foreach (Feature fea in features)
                {
                    name = fea.GetFieldValue(primaryFieldName);
                    if (String.IsNullOrEmpty(name))
                        continue;
                    color = nameColors[name];
                    aoi = GetAOIByFeature(bitmap, color);
                    if (aoi != null)
                        items.AddRange(aoi);
                }
                return items != null ? items.ToArray() : null;
            }
        }

        private static Feature[] GetFeatures(string fname)
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

        private static Bitmap VectorsToBitmap(IRasterDataProvider prd, CodeCell.AgileMap.Core.Feature[] features, string shpPrimaryField, out Dictionary<string, Color> nameColors)
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

        private static int[] GetAOIByFeature(Bitmap bitmap, Color color)
        {
            using (IBitmap2RasterConverter c = new Bitmap2RasterConverter())
            {
                return c.ToRaster(bitmap, color);
            }
        }

        private static Envelope GetEnvelop(IRasterDataProvider prd)
        {
            Size size = new System.Drawing.Size();
            size.Width = prd.Width;
            size.Height = prd.Height;
            return new Envelope(prd.CoordEnvelope.MinX, prd.CoordEnvelope.MinY, prd.CoordEnvelope.MaxX, prd.CoordEnvelope.MaxY);
        }

        private static Dictionary<ShapePolygon, Color> GetVectorColors(Feature[] features, string shpPrimaryField, out Dictionary<string, Color> nameColors)
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

        public static int[] GetAOIByFeature(IRasterDataProvider dataProvider, Feature feature, string shpPrimaryField)
        {
            Dictionary<string, Color> nameColors = new Dictionary<string, Color>();
            using (Bitmap bitmap = VectorsToBitmap(dataProvider, new Feature[]{feature}, shpPrimaryField, out nameColors))
            {
                int[] aoi;
                Color color;
                string name;
                List<int> items = new List<int>();
                name = feature.GetFieldValue(shpPrimaryField);
                if (String.IsNullOrEmpty(name))
                    return null;
                color = nameColors[name];
                aoi = GetAOIByFeature(bitmap, color);
                if (aoi != null)
                    items.AddRange(aoi);
                return items != null ? items.ToArray() : null;
            }
        }

        public static Dictionary<string, int[]> GetFeatureAOIIndex(string shpFileName, string primaryFieldName,CoordEnvelope envelope,Size size)
        {
            string shpFullname = VectorAOITemplate.FindVectorFullname(shpFileName);
            if (String.IsNullOrEmpty(shpFullname))
                return null;
            Feature[] features = GetFeatures(shpFullname);
            if (features == null || features.Length == 0)
                return null;
            Dictionary<string, int[]> result = new Dictionary<string, int[]>();
            int featuresLength = features.Length;
            List<int> tempInt = new List<int>();
            using (VectorAOIGenerator vg = new VectorAOIGenerator())
            {
                int[] aoi = null;
                string currentFiledValue = string.Empty;
                for (int i = 0; i < featuresLength; i++)
                {
                    aoi = vg.GetAOI(new ShapePolygon[] { features[i].Geometry as ShapePolygon }, new Envelope(envelope.MinX, envelope.MinY, envelope.MaxX, envelope.MaxY), size);
                    if (aoi == null || aoi.Length == 0)
                        continue;
                    currentFiledValue = features[i].GetFieldValue(primaryFieldName);
                    if (result.ContainsKey(currentFiledValue))
                    {
                        tempInt.AddRange(result[currentFiledValue]);
                        tempInt.AddRange(aoi);
                        result[currentFiledValue] = tempInt.ToArray();
                    }
                    else
                        result.Add(currentFiledValue, aoi);
                    tempInt.Clear();
                }
                return result.Count == 0 ? null : result;
            }
        }
    }
}
