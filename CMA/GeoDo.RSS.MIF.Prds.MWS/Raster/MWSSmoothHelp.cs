#region Version Info
/*========================================================================
* 功能概述：
* 
* 创建者：Administrator     时间：2014-3-30 13:02:39
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
using GeoDo.RSS.MIF.Prds.Comm;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.Core.DF;
using System.Drawing;
using System.IO;

namespace GeoDo.RSS.MIF.Prds.MWS
{
    /// <summary>
    /// 类名：MWSSmoothHelp
    /// 属性描述：对积雪产品的中值滤波和插值算法简单实现类
    /// 创建者：lxj   创建日期：2014-3-30 13:02:39
    /// 修改者：             修改日期：
    /// 修改描述：
    /// 备注：
    /// </summary>
    public class MWSSmoothHelp
    {
        private IContextMessage _contextMessage;
        public MWSSmoothHelp()
        {

        }
        /// <summary>
        /// 中值滤波
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public string ComputerMid(string filename, Int16 smoothwindow, string savepath)
        {
            List<RasterMaper> rms = null;
            IRasterDataProvider outRaster = null;
            RasterProcessModel<float, float> rfr = null;
            string midFilterFileName = savepath + "\\"+ Path.GetFileName(filename);
            if (File.Exists(midFilterFileName))
                return midFilterFileName;
            try
            {
                rms = new List<RasterMaper>();
                IRasterDataProvider inRaster1 = GeoDataDriver.Open(filename) as IRasterDataProvider;
                RasterMaper fileIn1 = new RasterMaper(inRaster1, new int[] { 1 });
                rms.Add(fileIn1);
                if(string.IsNullOrEmpty(midFilterFileName))
                    return null;
                else 
                    outRaster = CreateOutRaster(midFilterFileName, enumDataType.Float, rms.ToArray(), inRaster1.ResolutionX);
                RasterMaper fileOut = new RasterMaper(outRaster, new int[] { 1 });
                RasterMaper[] fileIns = rms.ToArray();
                RasterMaper[] fileOuts = new RasterMaper[] { fileOut };
                rfr = new RasterProcessModel<float, float>();
                rfr.SetRaster(fileIns, fileOuts);
                rfr.RegisterCalcModel(new RasterCalcHandler<float, float>((rvInVistor, rvOutVistor, aoi) =>
                {
                    if (rvInVistor[0].RasterBandsData[0] != null)
                    {
                        int dataLength = rvInVistor[0].SizeY * rvInVistor[0].SizeX;
                        float[] outpixel = new float[dataLength];
                        float[] temp = new float[smoothwindow * smoothwindow];
                        int col = rvInVistor[0].SizeX;
                        if (smoothwindow == 5)
                        {
                            for (int i = 0; i < dataLength; i++)
                            {
                                if (i < 2 * col || i % col == 0 || (i - 1) % col == 0 || (i + 1) % col == 0 || (i + 2) % col == 0 || i > dataLength - 2 * col)
                                {
                                    if (rvInVistor[0].RasterBandsData[0][i] == -999.0f)
                                    {
                                        rvOutVistor[0].RasterBandsData[0][i] = -999.0f;
                                    }
                                    else
                                        rvOutVistor[0].RasterBandsData[0][i] = rvInVistor[0].RasterBandsData[0][i];
                                }
                                else
                                {
                                    if (rvInVistor[0].RasterBandsData[0][i] == -999.0f)
                                    {
                                        rvOutVistor[0].RasterBandsData[0][i] = -999.0f;
                                    }
                                    else
                                    {
                                        temp[0] = rvInVistor[0].RasterBandsData[0][i - 2 * col - 2];
                                        temp[1] = rvInVistor[0].RasterBandsData[0][i - 2 * col - 1];
                                        temp[2] = rvInVistor[0].RasterBandsData[0][i - 2 * col];
                                        temp[3] = rvInVistor[0].RasterBandsData[0][i - 2 * col + 1];
                                        temp[4] = rvInVistor[0].RasterBandsData[0][i - 2 * col + 2];
                                        temp[5] = rvInVistor[0].RasterBandsData[0][i - col - 2];
                                        temp[6] = rvInVistor[0].RasterBandsData[0][i - col - 1];
                                        temp[7] = rvInVistor[0].RasterBandsData[0][i - col];
                                        temp[8] = rvInVistor[0].RasterBandsData[0][i - col + 1];
                                        temp[9] = rvInVistor[0].RasterBandsData[0][i - col + 2];
                                        temp[10] = rvInVistor[0].RasterBandsData[0][i - 2];
                                        temp[11] = rvInVistor[0].RasterBandsData[0][i - 1];
                                        temp[12] = rvInVistor[0].RasterBandsData[0][i];
                                        temp[13] = rvInVistor[0].RasterBandsData[0][i + 1];
                                        temp[14] = rvInVistor[0].RasterBandsData[0][i + 2];
                                        temp[15] = rvInVistor[0].RasterBandsData[0][i + col - 2];
                                        temp[16] = rvInVistor[0].RasterBandsData[0][i + col - 1];
                                        temp[17] = rvInVistor[0].RasterBandsData[0][i + col];
                                        temp[18] = rvInVistor[0].RasterBandsData[0][i + col + 1];
                                        temp[19] = rvInVistor[0].RasterBandsData[0][i + col + 2];
                                        temp[20] = rvInVistor[0].RasterBandsData[0][i + 2 * col - 2];
                                        temp[21] = rvInVistor[0].RasterBandsData[0][i + 2 * col - 1];
                                        temp[22] = rvInVistor[0].RasterBandsData[0][i + 2 * col];
                                        temp[23] = rvInVistor[0].RasterBandsData[0][i + 2 * col + 1];
                                        temp[24] = rvInVistor[0].RasterBandsData[0][i + 2 * col + 2];
                                        int count = 0;
                                        for (int n = 0; n < 25; n++)
                                        {
                                            if (temp[n] == -999.0f)
                                                count++;
                                        }
                                        Array.Sort(temp);
                                        if (count >= 12)
                                            rvOutVistor[0].RasterBandsData[0][i] = rvInVistor[0].RasterBandsData[0][i];
                                        else
                                            rvOutVistor[0].RasterBandsData[0][i] = temp[temp.Length / 2];
                                    }
                                }
                            }
                        }
                        else
                        {
                            for (int i = 0; i < dataLength; i++)
                            {

                                if (i < col || i % col == 0 || (i + 1) % col == 0 || i > dataLength - col)
                                {
                                    rvOutVistor[0].RasterBandsData[0][i] = rvInVistor[0].RasterBandsData[0][i];
                                }
                                else
                                {
                                    if (rvInVistor[0].RasterBandsData[0][i] == -999.0f)
                                    {
                                        rvOutVistor[0].RasterBandsData[0][i] = -999.0f;
                                    }
                                    else
                                    {
                                        temp[0] = rvInVistor[0].RasterBandsData[0][i - col - 1];
                                        temp[1] = rvInVistor[0].RasterBandsData[0][i - col];
                                        temp[2] = rvInVistor[0].RasterBandsData[0][i - col + 1];
                                        temp[3] = rvInVistor[0].RasterBandsData[0][i - 1];
                                        temp[4] = rvInVistor[0].RasterBandsData[0][i];
                                        temp[5] = rvInVistor[0].RasterBandsData[0][i + 1];
                                        temp[6] = rvInVistor[0].RasterBandsData[0][i + col - 1];
                                        temp[7] = rvInVistor[0].RasterBandsData[0][i + col];
                                        temp[8] = rvInVistor[0].RasterBandsData[0][i + col + 1];
                                        int count = 0;
                                        for (int n = 0; n < 9; n++)
                                        {
                                            if (temp[n] == -999.0f)
                                                count++;
                                        }
                                        Array.Sort(temp);
                                        if (count >= 4)
                                            rvOutVistor[0].RasterBandsData[0][i] = rvInVistor[0].RasterBandsData[0][i];
                                        else
                                            rvOutVistor[0].RasterBandsData[0][i] = temp[temp.Length / 2];
                                    }
                                }
                            }
                        }
                    }
                }));
                rfr.Excute();
                return midFilterFileName;
            }
            finally
            {
                if (outRaster != null)
                    outRaster.Dispose();
                if (rms != null && rms.Count > 0)
                {
                    foreach (RasterMaper rm in rms)
                    {
                        if (rm.Raster != null)
                            rm.Raster.Dispose();
                    }
                }
            }

        }
        /// <summary>
        /// 双线性插值方法提高原数据文件分辨率
        /// f(i+u,j+v)=(1-u)(1-v)*f(i,j) + (1-v)u*f(i,j+1) + v(1-u)*f(i+1,j) + uv*f(i+1,j+1)
        /// </summary>
        /// <param name="srcFilename">原文件</param>
        /// <param name="factor">分辨率提高比例如0.1度到0.01度，zoom = 10</param>
        /// <returns></returns>                                   
        public string Bilinear(string srcFilename,Int16 zoom,string savepath)
        {
            List<RasterMaper> rms = new List<RasterMaper>();
            IRasterDataProvider outRaster = null;
            IRasterDataProvider inRaster1 = GeoDataDriver.Open(srcFilename) as IRasterDataProvider;
            RasterMaper fileIn1 = new RasterMaper(inRaster1, new int[] { 1 });
            rms.Add(fileIn1);
            string fileBilname = savepath +"\\"+ Path.GetFileName(srcFilename);// GetFileName(new string[] { srcFilename }, _subProductDef.ProductDef.Identify, identify, ".dat", null);
            if (File.Exists(fileBilname))
                return fileBilname;
            outRaster = CreateOutRaster(fileBilname, enumDataType.Float, rms.ToArray(), inRaster1.ResolutionX / zoom);
            float[] inRasterBuffer = new float[inRaster1.Width * inRaster1.Height];
            IRasterBand bandinRaster1 = inRaster1.GetRasterBand(1);
            float[] sd = new float[inRaster1.Width * inRaster1.Height];
            float[] sdSnow = new float[outRaster.Width * outRaster.Height];//输出数组
            unsafe
            {
                fixed (float* pointer = inRasterBuffer)
                {
                    IntPtr ptr = new IntPtr(pointer);
                    bandinRaster1.Read(0, 0, inRaster1.Width, inRaster1.Height, ptr, inRaster1.DataType, inRaster1.Width, inRaster1.Height);
                    for (int j = 0; j < inRaster1.Width * inRaster1.Height; j++)
                    {
                        sd[j] = inRasterBuffer[j];
                    }
                }
            }
            Int32[] index = new Int32[zoom * zoom];
            try
            {
                for (int i = 0; i < inRaster1.Width * inRaster1.Height; i++)
                {
                    if ((i + 1) % inRaster1.Width == 0 || i % inRaster1.Width == 0 || i >= inRaster1.Width * inRaster1.Height - inRaster1.Width || i < inRaster1.Width) //插值后的高分格子落在原低分格子的最后一列或第一列，或最后一行，最后一列
                    {
                        for (int row = 0; row < zoom; row++)
                        {
                            for (int col = 0; col < zoom; col++)
                            {
                                index[col + row * zoom] = (i / inRaster1.Width * zoom * outRaster.Width + i % inRaster1.Width * zoom) + (row * outRaster.Width) + col;
                                //sdSnow[index[col + row * zoom]] = 0.000001f;
                                //sdSnow[index[col + row * zoom]] = -999.0f;
                                sdSnow[index[col + row * zoom]] = sd[i];
                            }
                        }
                    }
                    else
                    {
                        for (int row = 0; row < zoom; row++)
                        {
                            for (int col = 0; col < zoom; col++)
                            {
                                index[col + row * zoom] = (i / inRaster1.Width * zoom * outRaster.Width + i % inRaster1.Width * zoom) + (row * outRaster.Width) + col;
                                //(2)通过u,v所处的象限
                                int xCenter = zoom / 2;
                                int yCenter = zoom / 2;
                                if (col < xCenter && row < yCenter) //第二象限 高分格子所在象元f(i,j) 其余三个象元分别是f(i,j-1),f(i-1,j),f(i-1,j-1)
                                {
                                    float u = 0.5f + (float)(col * 1.0f / zoom);//列
                                    float v = 0.5f + (float)(row * 1.0f / zoom);//行
                                    sdSnow[index[col + row * zoom]] = (1 - u) * (1 - v) * sd[i - inRaster1.Width - 1] + (1 - v) * u * sd[i - inRaster1.Width] + v * (1 - u) * sd[i - 1] + u * v * sd[i];
                                }
                                if (col >= xCenter && row < yCenter) //第一象限 高分格子所在象元f(i,j) 其余三个象元分别是f(i-1,j),f(i-1,j+1),f(i,j+1)
                                {
                                    float u = (float)(col * 1.0f / zoom) - 0.5f; //列  横向 
                                    float v = (float)(row * 1.0f / zoom) + 0.5f;   //行 纵
                                    sdSnow[index[col + row * zoom]] = (1 - u) * (1 - v) * sd[i - inRaster1.Width] + (1 - v) * u * sd[i - inRaster1.Width + 1] + v * (1 - u) * sd[i] + u * v * sd[i + 1];
                                }
                                if (col < xCenter && row >= yCenter) //第三象限 高分格子所在象元f(i,j) 其余三个象元分别是f(i,j-1),f(i+1,j-1),f(i+1,j)
                                {
                                    float u = 0.5f + (float)(col * 1.0f / zoom); //列
                                    float v = (float)(row * 1.0f / zoom) - 0.5f; //行
                                    sdSnow[index[col + row * zoom]] = (1 - u) * (1 - v) * sd[i - 1] + (1 - v) * u * sd[i] + v * (1 - u) * sd[i + inRaster1.Width - 1] + v * u * sd[i + inRaster1.Width];
                                }
                                if (col >= xCenter && row >= yCenter) //第四象限 高分格子所在象元f(i,j) 其余三个象元分别是f(i,j+1),f(i+1,j),f(i+1,j+1)
                                {
                                    float u = (float)(col * 1.0f / zoom) - 0.5f; //列
                                    float v = (float)(row * 1.0f / zoom) - 0.5f;//行
                                    sdSnow[index[col + row * zoom]] = (1 - u) * (1 - v) * sd[i] + (1 - v) * u * sd[i + 1] + v * (1 - u) * sd[i + inRaster1.Width] + u * v * sd[i + inRaster1.Width + 1];
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                PrintInfo(ex.Message);
            }
            try
            {
                unsafe
                {
                    fixed (float* ptr = sdSnow)
                    {
                        IntPtr sdSnowPtr = new IntPtr(ptr);
                        IRasterBand bandoutRaster = outRaster.GetRasterBand(1);
                        bandoutRaster.Write(0, 0, outRaster.Width, outRaster.Height, sdSnowPtr, outRaster.DataType, outRaster.Width, outRaster.Height);
                    }
                }
                //IFileExtractResult res = new FileExtractResult(_subProductDef.Identify, sdBilinearfilename, true);
                return fileBilname;
            }
            finally
            {
                if (outRaster != null)
                    outRaster.Dispose();
                if (rms != null && rms.Count > 0)
                {
                    foreach (RasterMaper rm in rms)
                    {
                        if (rm.Raster != null)
                            rm.Raster.Dispose();
                    }
                }
            }
        }
        //public string MaskChina(string file)
        //{
        //    List<RasterMaper> rms = null;
        //    IRasterDataProvider outRaster = null;
        //    RasterProcessModel<float, float> rfr = null;
        //    string outfile = "";
        //    //中国区掩膜文件
        //    string chinarasterFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SystemData\\ProductArgs\\MWS\\SnowArgFile\\china_raster.dat");
        //    if (string.IsNullOrEmpty(chinarasterFile) || !File.Exists(chinarasterFile))
        //    {
        //        return null;
        //    }
        //    try
        //    {
        //        rms = new List<RasterMaper>();
        //        IRasterDataProvider inRaster = GeoDataDriver.Open(file) as IRasterDataProvider;
        //        RasterMaper fileIn = new RasterMaper(inRaster, new int[] { 1 });
        //        rms.Add(fileIn);
        //        IRasterDataProvider inRaster3 = GeoDataDriver.Open(chinarasterFile) as IRasterDataProvider;
        //        RasterMaper fileIn3 = new RasterMaper(inRaster3, new int[] { 1 });
        //        outfile = Path.GetDirectoryName(file)+ Path.GetFileNameWithoutExtension(file) + "_maskChina." + Path.GetExtension(file);
        //        rms.Add(fileIn3);
        //        outRaster = CreateOutRaster(outfile, enumDataType.Float, rms.ToArray(), inRaster.ResolutionX);
        //        RasterMaper fileOut = new RasterMaper(outRaster, new int[] { 1 });
        //        RasterMaper[] fileIns = rms.ToArray();
        //        RasterMaper[] fileOuts = new RasterMaper[] { fileOut };
        //        rfr = new RasterProcessModel<float, float>();
        //        rfr.SetRaster(fileIns, fileOuts);
        //        rfr.RegisterCalcModel(new RasterCalcHandler<float, float>((rvInVistor, rvOutVistor, aoi) =>
        //        {
        //            if (rvInVistor[0].RasterBandsData[0] != null && rvInVistor[1].RasterBandsData[0] != null)
        //            {
        //                int dataLength = rvOutVistor[0].SizeX * rvOutVistor[0].SizeY;
        //                for (int i = 0; i < dataLength; i++)
        //                {
        //                    //if (Convert.ToSingle(rvInVistor[1].RasterBandsData[0][i]) == 9.38169E-41) //陆地
        //                        rvOutVistor[0].RasterBandsData[0][i] = rvInVistor[0].RasterBandsData[0][i];
        //                    //else
        //                    //    rvOutVistor[0].RasterBandsData[0][i] = 2000f;
        //                }
        //            }
        //        }));
        //        rfr.Excute();
        //    }
        //    finally
        //    {
        //        if (outRaster != null)
        //            outRaster.Dispose();
        //        if (rms != null && rms.Count > 0)
        //        {
        //            foreach (RasterMaper rm in rms)
        //            {
        //                if (rm.Raster != null)
        //                    rm.Raster.Dispose();
        //            }
        //        }
        //    }
        //    return outfile;
        //}

       
        //创建输出删格文件
        protected IRasterDataProvider CreateOutRaster(string outFileName, enumDataType dataType, RasterMaper[] inrasterMaper, float resolution)
        {
            IRasterDataDriver raster = RasterDataDriver.GetDriverByName("MEM") as IRasterDataDriver;
            CoordEnvelope outEnv = null;
            foreach (RasterMaper inRaster in inrasterMaper)
            {
                if (outEnv == null)
                    outEnv = inRaster.Raster.CoordEnvelope;
                else
                    outEnv = outEnv.Intersect(inRaster.Raster.CoordEnvelope);
            }
            float resX, resY;
            if (resolution != 0f)
            {
                resX = resolution;
                resY = resolution;
            }
            else
            {
                resX = inrasterMaper[0].Raster.ResolutionX;
                resY = inrasterMaper[0].Raster.ResolutionY;
            }
            int width = (int)(Math.Round(outEnv.Width / resX));
            int height = (int)(Math.Round(outEnv.Height / resY));
            string mapInfo = outEnv.ToMapInfoString(new Size(width, height));
            if (File.Exists(outFileName))
                File.Delete(outFileName);
            RasterDataProvider outRaster = raster.Create(outFileName, width, height, 1, dataType, mapInfo) as RasterDataProvider;
            return outRaster;
        }
        private void PrintInfo(string info)
        {
            if (_contextMessage != null)
                _contextMessage.PrintMessage(info);
            else
                Console.WriteLine(info);
        }
    }
}
