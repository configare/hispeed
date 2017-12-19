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
using GeoDo.RSS.Core.CA;

namespace GeoDo.Smart.GlobalMosaic
{
    /// <summary>
    /// 类名：Class1
    /// 属性描述：
    /// 创建者：admin   创建日期：2013-10-07 15:19:23
    /// 修改者：             修改日期：
    /// 修改描述：
    /// 备注：
    /// </summary>
    class Process
    {
        private IRasterDataProvider CreateMosaicRaster(string filename, GeoHeader geoHeader)
        {
            IRasterDataProvider outRaster = null;
            if (File.Exists(filename))
            {
                outRaster = GeoDataDriver.Open(filename, enumDataProviderAccess.Update, null) as IRasterDataProvider;
            }
            else
            {
                int bandCount = 3;
                float resX = 0.01f;
                float resY = 0.01f;
                //下面是全球区域
                //float ltX = -180;
                //float ltY = 90;
                //int width = 36000;
                //int height = 18000;
                //下面是中国区域
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
                ISpatialReference spatialRef = SpatialReference.GetDefault();
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
                string outSolarZenithFile = Path.ChangeExtension(filename, ".SolarZenith.ldf");
                using (RasterDataProvider outSolarZenithRaster = raster.Create(outSolarZenithFile, width, height, 1, enumDataType.UInt16, options) as RasterDataProvider)
                { }
            }
            return outRaster;
        }

        public void Mosaic(string[] rasterFiles, string outrasterFile, GeoHeader geoHeader, Action<int, string> progress)
        {
            using (IRasterDataProvider outraster = CreateMosaicRaster(outrasterFile, geoHeader))
            {
                string filename = null;
                IRasterDataProvider raster = null;
                string solarZenithFilename = null;
                IRasterDataProvider solarZenithRaster = null;

                RasterMaper rmOutSolarZenith = null;// new RasterMaper(solarZenithRaster, new int[] { 1 });
                string outSolarZenithFile = Path.ChangeExtension(outrasterFile, ".SolarZenith.ldf");
                IRasterDataProvider outSolarZenithRster = null;
                try
                {
                    for (int f = 0; f < rasterFiles.Length; f++)
                    {
                        filename = rasterFiles[f];
                        solarZenithFilename = Path.ChangeExtension(filename, ".SolarZenith.ldf");
                        if (progress != null)
                            progress((int)(f * 100f / rasterFiles.Length), string.Format("{0}/{1}", f + 1, rasterFiles.Length));
                        try
                        {
                            raster = GeoDataDriver.Open(filename) as IRasterDataProvider;
                            if (raster == null)
                                continue;
                            if (raster is ILdfDataProvider)
                            {
                                Ldf1Header header = (raster as ILdfDataProvider).Header as Ldf1Header;
                                if (!header.IsDay)
                                {
                                    Console.WriteLine("跳过晚上数据" + Path.GetFileName(filename));
                                    continue;
                                }
                            }

                            if (File.Exists(outSolarZenithFile))
                            {
                                outSolarZenithRster = GeoDataDriver.Open(outSolarZenithFile, enumDataProviderAccess.Update, null) as IRasterDataProvider;
                                rmOutSolarZenith = new RasterMaper(outSolarZenithRster, new int[] { 1 });
                            }

                            RasterMaper rmSz = null;
                            if (File.Exists(solarZenithFilename))
                            {
                                solarZenithRaster = GeoDataDriver.Open(solarZenithFilename) as IRasterDataProvider;
                                rmSz = new RasterMaper(solarZenithRaster, new int[] { 1 });
                            }
                            RasterMaper rmin = new RasterMaper(raster, new int[] { 1, 2, 3 });
                            RasterMaper rmout = new RasterMaper(outraster, new int[] { 1, 2, 3 });
                            RasterProcessModel<short, short> rp = new RasterProcessModel<short, short>();
                            RasterMaper[] rmIns, rmOuts;
                            if (rmSz != null)
                                rmIns = new RasterMaper[] { rmin, rmSz };
                            else
                                rmIns = new RasterMaper[] { rmin };
                            if (rmOutSolarZenith != null)
                                rmOuts = new RasterMaper[] { rmout, rmOutSolarZenith };
                            else
                                rmOuts = new RasterMaper[] { rmout };

                            rp.SetRaster(rmIns, rmOuts);
                            rp.RegisterCalcModel(new RasterCalcHandlerFun<short, short>((rasterVistor, target, map) =>
                            {
                                if (rasterVistor[0] == null || rasterVistor[0].RasterBandsData.Length == 0)
                                    return false;
                                if (target[0] == null || target[0].RasterBandsData[0].Length == 0)
                                    return false;
                                bool isUpdate = false;
                                for (int i = 0; i < target[0].RasterBandsData[0].Length; i++)
                                {
                                    if (rasterVistor[0].RasterBandsData[0] == null)
                                        continue;
                                    if (rasterVistor[0].RasterBandsData[0][i] == 0)
                                        continue;
                                    //高度角判断白天晚上（高度角=90- 天顶角），高度角为0度时候为直射
                                    if (rasterVistor.Length == 1 || rasterVistor[1] == null || rasterVistor[1].RasterBandsData[0] == null ||
                                        target.Length == 1 || target[1] == null ||
                                        rasterVistor[1].RasterBandsData[0][i] < 0 || rasterVistor[1].RasterBandsData[0][i] > 9000)//白天
                                    {
                                        if (target[0].RasterBandsData[0][i] == 0)//如果没有角度数据，则采取补数据的方式（即不覆盖已有数据）
                                        {
                                            isUpdate = true;
                                            target[0].RasterBandsData[0][i] = rasterVistor[0].RasterBandsData[0][i];
                                            target[0].RasterBandsData[1][i] = rasterVistor[0].RasterBandsData[1][i];
                                            target[0].RasterBandsData[2][i] = rasterVistor[0].RasterBandsData[2][i];
                                        }
                                    }
                                    else if (target[1].RasterBandsData[0][i] == 0)
                                    {
                                        isUpdate = true;
                                        target[0].RasterBandsData[0][i] = rasterVistor[0].RasterBandsData[0][i];
                                        target[0].RasterBandsData[1][i] = rasterVistor[0].RasterBandsData[1][i];
                                        target[0].RasterBandsData[2][i] = rasterVistor[0].RasterBandsData[2][i];
                                        //更新目标高度角数据
                                        target[1].RasterBandsData[0][i] = rasterVistor[1].RasterBandsData[0][i];
                                    }
                                    else if (rasterVistor[1].RasterBandsData[0][i] < target[1].RasterBandsData[0][i])
                                    {
                                        isUpdate = true;
                                        target[0].RasterBandsData[0][i] = rasterVistor[0].RasterBandsData[0][i];
                                        target[0].RasterBandsData[1][i] = rasterVistor[0].RasterBandsData[1][i];
                                        target[0].RasterBandsData[2][i] = rasterVistor[0].RasterBandsData[2][i];
                                        //更新目标高度角数据
                                        target[1].RasterBandsData[0][i] = rasterVistor[1].RasterBandsData[0][i];
                                    }

                                }
                                if (isUpdate)
                                    return true;
                                else
                                    return false;
                            }
                            ));
                            rp.Excute();
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

        /*
            byte[] rMap,gMap, bMap;
            string imProcess = @"D:\WorkFolder\Smart二期\1.联机开发@Smart\SMART\bin\Release\图像增强方案\其它\test2.xml";
            c.ToRgbMap(imProcess, out rMap, out gMap, out bMap);
            return;
         * */
        private static IRgbProcessor[] TryGetRgbProcess(string filename)
        {
            RgbProcessorStack processor = new RgbProcessorStack();
            processor.ReadXmlElement(filename);
            IRgbProcessor[] ps = processor.Processors.ToArray();
            return ps;
        }

        public void RgbProcess(IRgbProcessor[] ps, ref byte r, ref byte g, ref byte b)
        {
            if (ps == null || ps.Length == 0)
                return;
            int count = ps.Length;
            IRgbProcessor p = null;
            for (int i = count - 1; i >= 0; i--)//由于是按照先进后出的Stat形式保存的，故应反序以此处理
            {
                p = ps[i];
                if (p is RgbProcessorByPixel)
                {
                    (p as RgbProcessorByPixel).ProcessRGB(ref b, ref g, ref r);
                }
            }
        }
    }
}
