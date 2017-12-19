#region Version Info
/*========================================================================
* 功能概述：
* 
* 创建者：admin     时间：2013-10-21 12:58:36
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
using GeoDo.RSS.Core.DF;
using System.IO;
using System.Runtime.InteropServices;

namespace GeoDo.Smart.GlobalMosaic
{
    /// <summary>
    /// 类名：ToChinaCenter
    /// 属性描述：
    /// 创建者：admin   创建日期：2013-10-21 12:58:36
    /// 修改者：             修改日期：
    /// 修改描述：
    /// 备注：
    /// </summary>
    class ToChinaCenter
    {
        public void Tran()
        {
            int center = 180;
            string inputFileName = @"E:\fy3c\FY3C_VIRR_20131015_1000M_global_CH6.tiff";
            using (IRasterDataProvider inRaster = GeoDataDriver.Open(inputFileName) as IRasterDataProvider)
            {
                int bandCount = inRaster.BandCount;
                string outputFName = Path.ChangeExtension(inputFileName, ".180.tiff");
                using (IRasterDataProvider outRaster = CreatOutputRaster(outputFName, inRaster, bandCount, null))
                {
                    int off = 18;//左右丢失的数据（黑边）
                    //0-15000,18000
                    int leftPiex = 18000;
                    int rightPiex = 18000;//
                    byte[] band = new byte[(leftPiex - off) * 18000];
                    GCHandle handle = GCHandle.Alloc(band, GCHandleType.Pinned);
                    for (int b = 0; b < bandCount; b++)
                    {
                        inRaster.GetRasterBand(b + 1).Read(off, 0, leftPiex - off, 18000, handle.AddrOfPinnedObject(), enumDataType.Byte, leftPiex - off, 18000);
                        outRaster.GetRasterBand(b + 1).Write(rightPiex - off, 0, leftPiex - off, 18000, handle.AddrOfPinnedObject(), enumDataType.Byte, leftPiex - off, 18000);
                    }
                    handle.Free();
                    band = new byte[(rightPiex - off) * 18000];
                    handle = GCHandle.Alloc(band, GCHandleType.Pinned);
                    for (int b = 0; b < bandCount; b++)
                    {
                        inRaster.GetRasterBand(b + 1).Read(leftPiex - off, 0, rightPiex - off, 18000, handle.AddrOfPinnedObject(), enumDataType.Byte, rightPiex - off, 18000);
                        outRaster.GetRasterBand(b + 1).Write(off, 0, rightPiex - off, 18000, handle.AddrOfPinnedObject(), enumDataType.Byte, rightPiex - off, 18000);
                    }
                    handle.Free();
                }
            }
        }

        private IRasterDataProvider CreatOutputRaster(string outputFName, IRasterDataProvider inRaster, int bandCount, Action<int, string> progressCallback)
        {
            if (inRaster == null)
                return null;
            if (progressCallback != null)
                progressCallback(1, "开始创建目标文件...");
            IRasterDataDriver driver = GeoDataDriver.GetDriverByName("GDAL") as IRasterDataDriver;
            List<string> options = new List<string>();
            options.AddRange(new string[]{"DRIVERNAME=GTiff",
                    "TFW=YES",
                    "TILED=YES"});
            if (inRaster.SpatialRef != null)
                options.Add("WKT=" + inRaster.SpatialRef.ToWKTString());
            if (inRaster.CoordEnvelope != null)
            {
                options.Add("GEOTRANSFORM=" + string.Format("{0},{1},{2},{3},{4},{5}", 0, inRaster.ResolutionX, 0, inRaster.CoordEnvelope.MaxY, 0, -inRaster.ResolutionY));
            }
            IRasterDataProvider prdWrite = driver.Create(outputFName, inRaster.Width, inRaster.Height, bandCount, enumDataType.Byte, options.ToArray()) as IRasterDataProvider;
            if (progressCallback != null)
                progressCallback(20, "目标文件创建完成.");
            return prdWrite;
        }
    }
}
