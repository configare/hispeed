#region Version Info
/*========================================================================
* 功能概述：
* 
* 创建者：DongW     时间：2013/9/24 14:27:49
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
using System.IO;
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.DF.GDAL.HDF5Universal;
using System.Drawing;


namespace GeoDo.RSS.DF.SeaSurfaceWind
{
    /// <summary>
    /// 类名：SeaSurfaceWindDataValueReverser
    /// 属性描述：
    /// 创建者：DongW   创建日期：2013/9/24 14:27:49
    /// 修改者：             修改日期：
    /// 修改描述：
    /// 备注：
    /// </summary>
    public class SeaSurfaceWindDataValueReverser
    {
        private int _width = 1440;
        private int _height = 720;

        public bool Reverse(string srcFileName,string dstFileName)
        {
            if (string.IsNullOrEmpty(srcFileName) || !File.Exists(srcFileName))
                return false;
            //风速、u分量、v分量、海面温度、降雨量、大气水汽含量、云中液态水含量
            string[] args = new string[] { "datasets=ssw_f16f17rmsm,u_sm,v_sm,"+
            "sst_rm,rain_f16f17rm,wv_f16f17rm,clw_f16f17rm" };
            IRasterDataProvider dataPrd = null;
            IRasterDataProvider dstDataPrd = null;
            try
            {
                dataPrd = HDF5Driver.Open(srcFileName, enumDataProviderAccess.ReadOnly, args) as IRasterDataProvider;
                if (dataPrd == null)
                    return false;
                else
                {
                    try
                    {
                        dstDataPrd = CreatOutRaster(dstFileName, dataPrd);
                        SetBandsValue(dataPrd, dstDataPrd);
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                }
            }
            finally
            {
                if (dataPrd != null)
                    dataPrd.Dispose();
                if (dstDataPrd != null)
                    dstDataPrd.Dispose();
            }            
        }

        private IRasterDataProvider CreatOutRaster(string dstFileName,IRasterDataProvider srcDataProvider)
        {
            if (srcDataProvider == null || srcDataProvider.BandCount < 1)
                return null;
            int bandCount = srcDataProvider.BandCount;
            IRasterDataDriver driver = RasterDataDriver.GetDriverByName("LDF") as IRasterDataDriver;
            CoordEnvelope outEnv = new CoordEnvelope(-179.75,180, -89.75, 90);
            double resX = outEnv.Width/_width;
            double resY = outEnv.Height/_height;
            string mapInfo = outEnv.ToMapInfoString(new Size(_width, _height));
            RasterDataProvider outRaster = driver.Create(dstFileName, _width, _height,srcDataProvider.BandCount, srcDataProvider.DataType, mapInfo) as RasterDataProvider;
            return outRaster;
        }

        private void SetBandsValue(IRasterDataProvider srcDataPrd,IRasterDataProvider dstDataPrd)
        {
            //0-719列为0.25度-180度，720-1439列为180.25度-360度 
            int width = _width / 2;
            long bufferSize = width;
            Double[] databuffer = new double[bufferSize];
            for (int i = 1; i <= dstDataPrd.BandCount; i++)
            {
                for (int row = 0; row < _height; row++)
                { 
                    unsafe    
                    {
                        fixed (double* ptr = databuffer)
                        {
                             IntPtr buffer = new IntPtr(ptr);

                             srcDataPrd.GetRasterBand(i).Read(0, row, width, 1, buffer, enumDataType.Double, width, 1);
                             //for (int j = 0; j < width; j++)
                             //{
                             //    if (double.IsNaN(databuffer[j]))
                             //        databuffer[j] = -9999;
                             //}
                             dstDataPrd.GetRasterBand(i).Write(width, _height - 1 - row, width, 1, buffer, enumDataType.Double, width, 1);
                             srcDataPrd.GetRasterBand(i).Read(width, row, width, 1, buffer, enumDataType.Double, width, 1);
                             //for (int j = 0; j < width; j++)
                             //{
                             //    if (double.IsNaN(databuffer[j]))
                             //        databuffer[j] = -9999;
                             //}
                             dstDataPrd.GetRasterBand(i).Write(0, _height - 1 - row, width, 1, buffer, enumDataType.Double, width, 1);
                        }
                    }
                }
            }
        }
    }
}
