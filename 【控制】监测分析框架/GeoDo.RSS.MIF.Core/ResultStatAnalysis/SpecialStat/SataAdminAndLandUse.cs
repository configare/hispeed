using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;
using System.Drawing;
using System.Threading.Tasks;

namespace GeoDo.RSS.MIF.Core
{
    internal class SataAdminAndLandUse<T>
    {
        public Dictionary<string, Dictionary<string, double>> StatArea(string rasterFile, Func<T, bool> filter)
        {
            using (IRasterDataProvider dataProvider = GeoDataDriver.Open(rasterFile) as IRasterDataProvider)
            {
                return StatArea(dataProvider, null, filter);
            }
        }

        public Dictionary<string, Dictionary<string, double>> StatArea(IRasterDataProvider dataProvider, Func<T, bool> filter)
        {
            return StatArea(dataProvider, null, filter);
        }

        public Dictionary<string, Dictionary<string, double>> StatArea(string rasterFile, int[] aoi, Func<T, bool> filter)
        {
            using (IRasterDataProvider dataProvider = GeoDataDriver.Open(rasterFile) as IRasterDataProvider)
            {
                return StatArea(dataProvider, aoi, filter);
            }
        }

        public Dictionary<string, Dictionary<string, double>> StatArea(IRasterDataProvider dataProvider, int[] aoi, Func<T, bool> filter)
        {
            Size size = new Size(dataProvider.Width, dataProvider.Height);
            string[] adminNames = null, landuseTypes = null;
            byte[] adminRaster = null;
            byte[] landuseRaster = null;
            //adminRaster = GetAdminTypeRaster(dataProvider, size, out adminNames);
            //landuseRaster = GetLanduseTypeRaster(dataProvider, size, out landuseTypes);
            Action act1 = new Action(() => { adminRaster = GetAdminTypeRaster(dataProvider, size, out adminNames); });
            Action act2 = new Action(() => { landuseRaster = GetLanduseTypeRaster(dataProvider, size, out landuseTypes); });
            Parallel.Invoke(act1, act2);
            double[,] result = new double[adminNames.Length, landuseTypes.Length];
            if (aoi != null)
                StatAreaUseAOI(adminRaster, landuseRaster, dataProvider, result, aoi, filter);
            else
                StatArea(adminRaster, landuseRaster, dataProvider, result, filter);
            Dictionary<string, Dictionary<string, double>> retResult = new Dictionary<string, Dictionary<string, double>>();
            for (int iAdmin = 0; iAdmin < adminNames.Length; iAdmin++)
            {
                Dictionary<string, double> landuseResult = new Dictionary<string, double>(landuseTypes.Length);
                for (int iLanduse = 0; iLanduse < landuseTypes.Length; iLanduse++)
                {
                    landuseResult.Add(landuseTypes[iLanduse], result[iAdmin, iLanduse]);
                }
                retResult.Add(adminNames[iAdmin], landuseResult);
            }
            return retResult;
        }

        private void StatArea(byte[] adminRaster, byte[] landuseRaster, IRasterDataProvider dataProvider, double[,] result, Func<T, bool> filter)
        {
            ArgumentProvider arg = new ArgumentProvider(dataProvider, null);
            int row = 0;
            int width = dataProvider.Width;
            float maxLat = (float)dataProvider.CoordEnvelope.MaxY;
            float res = dataProvider.ResolutionY;
            using (IRasterPixelsVisitor<T> v = new RasterPixelsVisitor<T>(arg))
            {
                v.VisitPixel(new int[] { 1 },
                    (idx, bandValues) =>
                    {
                        if (filter(bandValues[0]))
                        {
                            //计数
                            //result[adminRaster[idx], landuseRaster[idx]]++;
                            //精确面积
                            row = idx / width;
                            result[adminRaster[idx], landuseRaster[idx]] += RasterOperator<UInt16>.ComputePixelArea(row, maxLat, res);
                        }
                    }
                );
            }
        }

        private void StatAreaUseAOI(byte[] adminRaster, byte[] landuseRaster, IRasterDataProvider dataProvider, double[,] result, int[] aoi, Func<T, bool> filter)
        {
            ArgumentProvider arg = new ArgumentProvider(dataProvider, null) { AOI = aoi };
            using (IRasterPixelsVisitor<T> v = new RasterPixelsVisitor<T>(arg))
            {
                Rectangle aoiRect = AOIHelper.ComputeAOIRect(aoi, new Size(dataProvider.Width, dataProvider.Height));
                v.VisitPixel(aoiRect, aoi, new int[] { 1 },
                    (idx, bandValues) =>
                    {
                        if (filter(bandValues[0]))
                            result[adminRaster[idx], landuseRaster[idx]]++;
                    }
                );
            }
        }

        private byte[] GetAdminTypeRaster(IRasterDataProvider dataProvider, Size size, out string[] adminNames)
        {
            VectorAOITemplate temp = VectorAOITemplateFactory.GetAOITemplate("省级行政区");
            return temp.GetRaster(GetEnvelope(dataProvider), size, "NAME", out adminNames);
        }

        private byte[] GetLanduseTypeRaster(IRasterDataProvider dataProvider, Size size, out string[] landuseTypes)
        {
            VectorAOITemplate temp = VectorAOITemplateFactory.GetAOITemplate("土地利用类型");
            return temp.GetRaster(GetEnvelope(dataProvider), size, "NAME", out landuseTypes);
        }

        private CodeCell.AgileMap.Core.Envelope GetEnvelope(IRasterDataProvider dataProvider)
        {
            return new CodeCell.AgileMap.Core.Envelope(dataProvider.CoordEnvelope.MinX,
                dataProvider.CoordEnvelope.MinY,
                dataProvider.CoordEnvelope.MaxX,
                dataProvider.CoordEnvelope.MaxY);
        }

        #region weight

        public Dictionary<string, Dictionary<string, double>> StatArea(string rasterFile, Func<T, int, int> weight)
        {
            using (IRasterDataProvider dataProvider = GeoDataDriver.Open(rasterFile) as IRasterDataProvider)
            {
                return StatArea(dataProvider, null, weight);
            }
        }

        public Dictionary<string, Dictionary<string, double>> StatArea(IRasterDataProvider dataProvider, Func<T, int, int> weight)
        {
            return StatArea(dataProvider, null, weight);
        }

        public Dictionary<string, Dictionary<string, double>> StatArea(string rasterFile, int[] aoi, Func<T, int, int> weight)
        {
            using (IRasterDataProvider dataProvider = GeoDataDriver.Open(rasterFile) as IRasterDataProvider)
            {
                return StatArea(dataProvider, aoi, weight);
            }
        }

        public Dictionary<string, Dictionary<string, double>> StatArea(IRasterDataProvider dataProvider, int[] aoi, Func<T, int, int> weight)
        {
            Size size = new Size(dataProvider.Width, dataProvider.Height);
            string[] adminNames = null, landuseTypes = null;
            byte[] adminRaster = null;
            byte[] landuseRaster = null;
            //adminRaster = GetAdminTypeRaster(dataProvider, size, out adminNames);
            //landuseRaster = GetLanduseTypeRaster(dataProvider, size, out landuseTypes);
            Action act1 = new Action(() => { adminRaster = GetAdminTypeRaster(dataProvider, size, out adminNames); });
            Action act2 = new Action(() => { landuseRaster = GetLanduseTypeRaster(dataProvider, size, out landuseTypes); });
            Parallel.Invoke(act1, act2);
            double[,] result = new double[adminNames.Length, landuseTypes.Length];
            if (aoi != null)
                StatAreaUseAOI(adminRaster, landuseRaster, dataProvider, result, aoi, weight);
            else
                StatArea(adminRaster, landuseRaster, dataProvider, result, weight);
            Dictionary<string, Dictionary<string, double>> retResult = new Dictionary<string, Dictionary<string, double>>();
            for (int iAdmin = 0; iAdmin < adminNames.Length; iAdmin++)
            {
                Dictionary<string, double> landuseResult = new Dictionary<string, double>(landuseTypes.Length);
                for (int iLanduse = 0; iLanduse < landuseTypes.Length; iLanduse++)
                {
                    landuseResult.Add(landuseTypes[iLanduse], result[iAdmin, iLanduse]);
                }
                retResult.Add(adminNames[iAdmin], landuseResult);
            }
            return retResult;
        }

        private void StatArea(byte[] adminRaster, byte[] landuseRaster, IRasterDataProvider dataProvider, double[,] result, Func<T, int, int> weight)
        {
            ArgumentProvider arg = new ArgumentProvider(dataProvider, null);
            int row = 0;
            int width = dataProvider.Width;
            float maxLat = (float)dataProvider.CoordEnvelope.MaxY;
            float res = dataProvider.ResolutionY;
            int weightValue = 0;
            using (IRasterPixelsVisitor<T> v = new RasterPixelsVisitor<T>(arg))
            {
                v.VisitPixel(new int[] { 1 },
                    (idx, bandValues) =>
                    {
                        weightValue = weight(bandValues[0], idx);
                        {
                            //计数
                            //result[adminRaster[idx], landuseRaster[idx]] += weightValue;
                            //精确面积
                            row = idx / width;
                            result[adminRaster[idx], landuseRaster[idx]] += weightValue * RasterOperator<UInt16>.ComputePixelArea(row, maxLat, res);
                        }
                    }
                );
            }
        }

        private void StatAreaUseAOI(byte[] adminRaster, byte[] landuseRaster, IRasterDataProvider dataProvider, double[,] result, int[] aoi, Func<T, int, int> weight)
        {
            ArgumentProvider arg = new ArgumentProvider(dataProvider, null) { AOI = aoi };
            using (IRasterPixelsVisitor<T> v = new RasterPixelsVisitor<T>(arg))
            {
                Rectangle aoiRect = AOIHelper.ComputeAOIRect(aoi, new Size(dataProvider.Width, dataProvider.Height));
                v.VisitPixel(aoiRect, aoi, new int[] { 1 },
                    (idx, bandValues) =>
                    {
                        result[adminRaster[idx], landuseRaster[idx]] += weight(bandValues[0], idx);
                    }
                );
            }
        }

        #endregion
    }
}
