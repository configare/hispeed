#region Version Info
/*========================================================================
* 功能概述：
* 
* 创建者：admin     时间：2013-10-07 15:19:23
* ------------------------------------------------------------------------
* 变更记录：
* 时间：                 修改者：                
* 修改说明：
* 
* ------------------------------------------------------------------------
* ========================================================================
*/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.Core.DF;
using GeoDo.Project;
using GeoDo.RSS.DF.LDF;
using System.IO;

namespace GeoDo.Smart.AutoPrd.Mosaic
{
    /// <summary>
    /// 类名：MosaicProcess
    /// 属性描述：
    /// 创建者：罗战克   创建日期：2013-10-07 15:19:23
    /// 修改者：             修改日期：
    /// 修改描述：
    /// 备注：
    /// </summary>
    public class MosaicProcess
    {
        /// <summary>
        /// 地理数据拼接:地理坐标，投影坐标
        /// 拼接输出ldf
        /// 支持按高度角数据拼接,高度角数据为后缀为 MosaicIdentify(输入参数) 的附加文件，值为扩大了100倍的short类型的高度角值。
        /// 本拼接程序会判断是否有高度角数据： MosaicIdentify(输入参数)，如果有，则拼接时候会依据高度角的大小关系执行是否覆盖操作，如果没有，则按照有无数据执行拼接。
        /// </summary>
        /// <param name="rasterFiles"></param>
        /// <param name="outRasterFile"></param>
        /// <param name="geoHeader"></param>
        /// <param name="bandMaps"></param>
        /// <param name="progress"></param>
        public void Mosaic(string[] rasterFiles, string outRasterFile, GeoInfo geoHeader, string[] bandMaps, string mosiacAngle, Action<int, string> progress)
        {
            int[] bandMap = null;
            if (!GetBandMap(bandMaps, out bandMap))
            {
                if (progress != null)
                    progress(0, "输入参数错误，参数[要拼接的波段]不正确");
                return;
            }
            if (bandMap == null || bandMap.Length == 0)
            {
                if (progress != null)
                    progress(0, "输入参数错误，[要拼接的波段]为空或不正确");
                return;
            }
            int bandCount = bandMap.Length;
            ISpatialReference spatialRef = TryFindSpatialRef(rasterFiles);
            using (IRasterDataProvider outraster = CreateMosaicRaster(outRasterFile, geoHeader, bandCount, spatialRef, mosiacAngle))
            {
                string filename = null;
                IRasterDataProvider raster = null;
                string solarZenithFilename = null;
                IRasterDataProvider solarZenithRaster = null;

                RasterMaper outSolarZenithMap = null;// new RasterMaper(solarZenithRaster, new int[] { 1 });
                string outSolarZenithFile = Path.ChangeExtension(outRasterFile, mosiacAngle);
                IRasterDataProvider outSolarZenithRster = null;
                try
                {
                    for (int f = 0; f < rasterFiles.Length; f++)
                    {
                        filename = rasterFiles[f];
                        solarZenithFilename = Path.ChangeExtension(filename, mosiacAngle);
                        if (progress != null)
                            progress((int)((f + 1) * 100f / rasterFiles.Length), string.Format("{0}/{1}", f + 1, rasterFiles.Length));
                        try
                        {
                            raster = GeoDataDriver.Open(filename) as IRasterDataProvider;
                            if (raster == null)
                                continue;
                            if (File.Exists(outSolarZenithFile))
                            {
                                outSolarZenithRster = GeoDataDriver.Open(outSolarZenithFile, enumDataProviderAccess.Update, null) as IRasterDataProvider;
                                outSolarZenithMap = new RasterMaper(outSolarZenithRster, new int[] { 1 });
                            }

                            RasterMaper rmSz = null;
                            if (File.Exists(solarZenithFilename))
                            {
                                solarZenithRaster = GeoDataDriver.Open(solarZenithFilename) as IRasterDataProvider;
                                rmSz = new RasterMaper(solarZenithRaster, new int[] { 1 });
                            }
                            RasterMaper rmin = new RasterMaper(raster, bandMap);
                            RasterMaper rmout = new RasterMaper(outraster, bandMap);
                            RasterProcessModel<short, short> rp = new RasterProcessModel<short, short>();
                            RasterMaper[] rmIns, rmOuts;
                            if (rmSz != null)
                                rmIns = new RasterMaper[] { rmin, rmSz };
                            else
                                rmIns = new RasterMaper[] { rmin };
                            if (outSolarZenithMap != null)
                                rmOuts = new RasterMaper[] { rmout, outSolarZenithMap };
                            else
                                rmOuts = new RasterMaper[] { rmout };
                            rp.SetRaster(rmIns, rmOuts);
                            rp.RegisterCalcModel(new RasterCalcHandlerFun<short, short>((rasterVistor, target, map) =>
                            {
                                if (rasterVistor[0] == null || rasterVistor[0].RasterBandsData.Length == 0)
                                    return false;
                                if (target[0] == null || target[0].RasterBandsData[0].Length == 0)
                                    return false;
                                if (rasterVistor[0].RasterBandsData[0] == null)
                                    return false;
                                //目标角度数据存在，源角度数据不存在，不做拼接
                                if (target.Length != 1 && rasterVistor.Length == 1)
                                    return false;
                                bool isUpdate = false;
                                //不使用天顶角数据,补数据
                                if (target.Length == 1 || rasterVistor.Length == 1)
                                {
                                    for (int i = 0; i < target[0].RasterBandsData[0].Length; i++)
                                    {
                                        if (rasterVistor[0].RasterBandsData[0][i] == 0)//空值
                                            continue;
                                        if (target[0].RasterBandsData[0][i] == 0)
                                        {
                                            isUpdate = true;
                                            for (int b = 0; b < bandCount; b++)
                                            {
                                                target[0].RasterBandsData[b][i] = rasterVistor[0].RasterBandsData[b][i];
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    for (int i = 0; i < target[0].RasterBandsData[0].Length; i++)
                                    {
                                        if (rasterVistor[0].RasterBandsData[0][i] == 0)//空值
                                            continue;
                                        /*
                                         * 太阳高度表示昼夜状态
                                         * 在晨昏线上的各地太阳高度为0 °，表示正经历昼夜更替；
                                         * 在昼半球上的各地太阳高度大于0°，表示白昼；
                                         * 在夜半球上的各地太阳高度小于0°，表示黑夜。
                                         * 日出、日落时，太阳高度为0
                                         * 即：
                                         * 0°附近在日落或日出
                                         * -90°和0°之间在晚上
                                         * 上午太阳高度逐渐增大，最大时为太阳上中天，即是正午，午后太阳高度又逐渐缩小。
                                         */
                                        //（高度角 = 90 - 天顶角）

                                        //高度角有效范围：
                                        //这里读取的是SolarZenith太阳天顶，有效范围0-180。
                                        //天顶角0-90为白天，小值为接近中午
                                        if (rasterVistor.Length == 1 || rasterVistor[1] == null || rasterVistor[1].RasterBandsData[0] == null ||
                                            target.Length == 1 || target[1] == null)
                                        {
                                            if (target[0].RasterBandsData[0][i] == 0)//如果没有角度数据，则采取补数据的方式（即不覆盖已有数据）
                                            {
                                                isUpdate = true;
                                                for (int b = 0; b < bandCount; b++)//拼接所有通道数据
                                                {
                                                    target[0].RasterBandsData[b][i] = rasterVistor[0].RasterBandsData[b][i];
                                                }
                                            }
                                        }
                                        else if (rasterVistor[1].RasterBandsData[0][i] > 9000 || rasterVistor[1].RasterBandsData[0][i] < 0)//原高度角为夜晚或者不合理
                                        {
                                            continue;
                                        }
                                        else if (target[1].RasterBandsData[0][i] == 0)
                                        {
                                            isUpdate = true;
                                            for (int b = 0; b < bandCount; b++)
                                            {
                                                target[0].RasterBandsData[b][i] = rasterVistor[0].RasterBandsData[b][i];
                                            }
                                            //更新目标高度角数据
                                            target[1].RasterBandsData[0][i] = rasterVistor[1].RasterBandsData[0][i];
                                        }
                                        else if (rasterVistor[1].RasterBandsData[0][i] < target[1].RasterBandsData[0][i])//取高度角小的
                                        {
                                            isUpdate = true;
                                            for (int b = 0; b < bandCount; b++)
                                            {
                                                target[0].RasterBandsData[b][i] = rasterVistor[0].RasterBandsData[b][i];
                                            }
                                            //更新目标高度角数据
                                            target[1].RasterBandsData[0][i] = rasterVistor[1].RasterBandsData[0][i];
                                        }
                                    }
                                }
                                if (isUpdate)
                                    return true;
                                else
                                    return false;
                            }
                            ));
                            int rowCount = rp.CalcRowCount();
                            rp.Excute(0, rowCount);
                        }
                        finally
                        {
                            if (raster != null)
                            {
                                raster.Dispose();
                                raster = null;
                            }
                            if (outSolarZenithRster != null)
                            {
                                outSolarZenithRster.Dispose();
                                outSolarZenithRster = null;
                            }
                        }
                    }
                }
                finally
                {
                    if (outSolarZenithRster != null)
                    {
                        outSolarZenithRster.Dispose();
                        outSolarZenithRster = null;
                    }
                }
            }
        }

        private ISpatialReference TryFindSpatialRef(string[] rasterFiles)
        {
            for (int i = 0; i < rasterFiles.Length; i++)
            {
                string filename = rasterFiles[i];
                using (IRasterDataProvider raster = GeoDataDriver.Open(filename) as IRasterDataProvider)
                {
                    if (raster.SpatialRef != null)
                        return raster.SpatialRef;
                }
            }
            return null;
        }

        private bool GetBandMap(string[] bands, out int[] bandMap)
        {
            bandMap = null;
            List<int> bandMaps = new List<int>();
            int band = -1;
            for (int b = 0; b < bands.Length; b++)
            {
                if (int.TryParse(bands[b], out band))
                {
                    bandMaps.Add(band);
                }
                else
                    return false;
            }
            bandMap = bandMaps.ToArray();
            return true;
        }

        private IRasterDataProvider CreateMosaicRaster(string filename, GeoInfo geoHeader, int bandCount, ISpatialReference spatialRef, string mosiacAngle)
        {
            IRasterDataProvider outRaster = null;
            if (File.Exists(filename))
            {
                outRaster = GeoDataDriver.Open(filename, enumDataProviderAccess.Update, null) as IRasterDataProvider;
            }
            else
            {
                float resX = 0.01f;
                float resY = 0.01f;
                float ltX = 65;
                float ltY = 60;
                int width = 8000;
                int height = 7000;
                if (geoHeader == null)
                    return null;
                resX = geoHeader.ResX;
                resY = geoHeader.ResY;
                ltX = geoHeader.LtX;
                ltY = geoHeader.LtY;
                width = geoHeader.Width;
                height = geoHeader.Height;
                if (spatialRef == null)
                    spatialRef = SpatialReference.GetDefault();
                string dir = Path.GetDirectoryName(filename);
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
                IRasterDataDriver raster = RasterDataDriver.GetDriverByName("LDF") as IRasterDataDriver;
                string bandNames = "";//CreateBandNames(bandcount);
                string[] options = new string[]{
                            "INTERLEAVE=BSQ",
                            "VERSION=LDF",
                            "WITHHDR=TRUE",
                            "SPATIALREF=" + spatialRef.ToProj4String(),
                            "MAPINFO={" + 1 + "," + 1 + "}:{" + ltX + "," + ltY + "}:{" + resX + "," + resY + "}",
                            "BANDNAMES="+ bandNames
                        };
                outRaster = raster.Create(filename, width, height, bandCount, enumDataType.UInt16, options) as RasterDataProvider;
                string outSolarZenithFile = Path.ChangeExtension(filename, mosiacAngle);
                using (RasterDataProvider outSolarZenithRaster = raster.Create(outSolarZenithFile, width, height, 1, enumDataType.UInt16, options) as RasterDataProvider)
                { }
            }
            return outRaster;
        }    

    }
}
