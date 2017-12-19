using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.Core.DF;
using System.Drawing;

namespace GeoDo.RSS.MIF.Prds.FIR
{
    /// <summary>
    /// 生成火点强度栅格文件
    /// </summary>
    internal class IntensityRasterGenerator
    {
        public IntensityRasterGenerator()
        {
        }

        public IFileExtractResult Generate(IArgumentProvider argProvider, Dictionary<int, PixelFeature> features)
        {
            if (argProvider == null || features == null)
                return null;
            Size size = new Size(argProvider.DataProvider.Width, argProvider.DataProvider.Height);
            IPixelFeatureMapper<Int16> result = new MemPixelFeatureMapper<Int16>("0FPG", features.Count, size, argProvider.DataProvider.CoordEnvelope, argProvider.DataProvider.SpatialRef);
            foreach (int idx in features.Keys)
            {
                //if (features[idx].IsVertified)
                //    result.Put(idx, features[idx].FirIntensity);
                if (features[idx].IsVertified)
                    result.Put(idx, (Int16)Math.Round(features[idx].SecondPixelArea * 1000));
            }
            RasterIdentify id = new RasterIdentify(argProvider.DataProvider.fileName);
            id.ThemeIdentify = "CMA";
            id.ProductIdentify = "FIR";
            id.SubProductIdentify = "0FPG";
            id.IsOutput2WorkspaceDir = true;
            DataIdentify dataId = argProvider.DataProvider.DataIdentify;
            if (dataId == null)
                dataId = new DataIdentify();
            id.Satellite = dataId.Satellite ?? "Unknow";
            id.Sensor = dataId.Sensor ?? "Unknow";
            id.OrbitDateTime = dataId.OrbitDateTime;
            //writeGFR();
            using (IInterestedRaster<Int16> iir = new InterestedRaster<Int16>(id, size, argProvider.DataProvider.CoordEnvelope.Clone(), argProvider.DataProvider.SpatialRef))
            {
                iir.Put(result);
                return new FileExtractResult("火点强度", iir.FileName);
            }
        }

        /// <summary>
        /// 全球火点LDF转换Dat
        /// </summary>
        public void writeGFR()
        {
            IRasterDataProvider dp = RasterDataDriver.Open(@"L:\新演示数据\01_火情\全球火点\FIR_FREQ_FY3B_VIRR_1D_GBAL_PXXX_1000KM_201304250941.ldf") as IRasterDataProvider;
            Size size = new Size(dp.Width, dp.Height);
            IPixelFeatureMapper<Int16> result = new MemPixelFeatureMapper<Int16>("FIR", dp.Width * dp.Height, size, dp.CoordEnvelope, dp.SpatialRef);
            RasterIdentify id = new RasterIdentify(dp.fileName);
            id.ThemeIdentify = "CMA";
            id.ProductIdentify = "FIR";
            id.SubProductIdentify = "GFRF";
            id.IsOutput2WorkspaceDir = true;
            IArgumentProvider ap = new ArgumentProvider(dp, null);
            RasterPixelsVisitor<UInt16> visitor = new RasterPixelsVisitor<UInt16>(ap);
            visitor.VisitPixel(new int[] { 1 },
                (index, values) =>
                {
                    result.Put(index, (Int16)values[0]);
                });
            using (IInterestedRaster<Int16> iir = new InterestedRaster<Int16>(id, size, dp.CoordEnvelope.Clone(), dp.SpatialRef))
            {
                iir.Put(result);
                string filename = iir.FileName;
            }
        }
    }
}
