using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeCell.AgileMap.Core;
using System.Drawing;

namespace GeoDo.RSS.UI.AddIn.Tools
{
    public class ExportPixelsByFeatures:IExportPixelsByFeatures
    {
        public class PixelFeatures
        {
            public int[] RasterIndexes;
            public double[] FeatureValues;
        }

        public PixelFeatures Export(Envelope geoEnvelope, Size rasterSize, string shpFile, 
            string fieldName, Func<Feature,double, bool> filter)
        {
            using (IVectorFeatureDataReader dr = VectorDataReaderFactory.GetUniversalDataReader(shpFile) as IVectorFeatureDataReader)
            {
                Feature[] fets = dr.FetchFeatures();
                if (fets == null || !(fets[0].Geometry is ShapePoint))
                    return null;
                ShapePoint pt;
                int row=0, col = 0;
                double spanY = geoEnvelope.Height / rasterSize.Height ;
                double spanX = geoEnvelope.Width / rasterSize.Width ;
                Dictionary<int,double> pixelFets = new Dictionary<int,double>();
                double fetValue;
                foreach (Feature fet in fets)
                {
                    pt = fet.Geometry as ShapePoint;
                    row = (int)((geoEnvelope.MaxY - pt.Y) / spanY);
                    col = (int)((pt.X - geoEnvelope.MinX) / spanX);
                    if (row < 0 || row >= rasterSize.Height)
                        continue;
                    if (col < 0 || col >= rasterSize.Width)
                        continue;
                    fetValue = ToDouble(fet.GetFieldValue(fieldName));
                    int key = row * rasterSize.Width + col; 
                    if (filter != null)
                    {
                        if (filter(fet, fetValue))
                        {   
                            if (!pixelFets.ContainsKey(key))
                                pixelFets.Add(key, fetValue);
                        }
                    }
                    else
                    {
                        pixelFets.Add(key, fetValue);
                    }
                }
                if (pixelFets.Count == 0)
                    return null;
                PixelFeatures ret = new PixelFeatures();
                ret.FeatureValues = pixelFets.Values.ToArray();
                ret.RasterIndexes = pixelFets.Keys.ToArray();
                return ret;
            }
        }

        private double ToDouble(string v)
        {
            if (string.IsNullOrEmpty(v))
                return 0;
            double retV;
            if (double.TryParse(v, out retV))
                return retV;
            return 0;
        }
    }
}
