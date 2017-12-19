using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.MIF.Core;

namespace GeoDo.RSS.MIF.Prds.Comm.Raster
{

    /// <summary>
    /// 中值滤波和插值方法类
    /// </summary>
    public class BiliNearAndSmoothHelper
    {
        private IContextMessage _contextMessage;
        /// <summary>
        /// 构造函数
        /// </summary>
        public BiliNearAndSmoothHelper()
        {
        }
        #region 公共方法
        public void SmoothComputer(string srcFilename, Int16 smoothwindow, string savepathname)
        {
            using (IRasterDataProvider inRaster1 = GeoDataDriver.Open(srcFilename) as IRasterDataProvider)
            {
                enumDataType type = inRaster1.DataType;
                switch (type)
                {
                    case enumDataType.Byte: ComputerMid<Byte>(srcFilename, smoothwindow, savepathname); break;
                    case enumDataType.Double: ComputerMid<Double>(srcFilename, smoothwindow, savepathname); break;
                    case enumDataType.Float: ComputerMid<Single>(srcFilename, smoothwindow, savepathname); break;
                    case enumDataType.Int16: ComputerMid<Int16>(srcFilename, smoothwindow, savepathname); break;
                    case enumDataType.Int32: ComputerMid<Int32>(srcFilename, smoothwindow, savepathname); break;
                    case enumDataType.Int64: ComputerMid<Int64>(srcFilename, smoothwindow, savepathname); break;
                    case enumDataType.UInt16: ComputerMid<UInt16>(srcFilename, smoothwindow, savepathname); break;
                    case enumDataType.UInt32: ComputerMid<UInt32>(srcFilename, smoothwindow, savepathname); break;
                    case enumDataType.UInt64: ComputerMid<UInt64>(srcFilename, smoothwindow, savepathname); break;
                    default: break;



                }
            }
        }
        /// <summary>
        /// 插值处理
        /// </summary>
        public void AndBiliNear(string srcFilename, Int16 zoom, string savepathname)
        {
            using (IRasterDataProvider inRaster1 = GeoDataDriver.Open(srcFilename) as IRasterDataProvider)
            {
                enumDataType type = inRaster1.DataType;
                switch (type)
                {
                    case enumDataType.Byte: Bilinear<Byte>(srcFilename, zoom, savepathname); break;
                    case enumDataType.Double: Bilinear<Double>(srcFilename, zoom, savepathname); break;
                    case enumDataType.Float: Bilinear<Single>(srcFilename, zoom, savepathname); break;
                    case enumDataType.Int16: Bilinear<Int16>(srcFilename, zoom, savepathname); break;
                    case enumDataType.Int32: Bilinear<Int32>(srcFilename, zoom, savepathname); break;
                    case enumDataType.Int64: Bilinear<Int64>(srcFilename, zoom, savepathname); break;
                    case enumDataType.UInt16: Bilinear<UInt16>(srcFilename, zoom, savepathname); break;
                    case enumDataType.UInt32: Bilinear<UInt32>(srcFilename, zoom, savepathname); break;
                    case enumDataType.UInt64: Bilinear<UInt64>(srcFilename, zoom, savepathname); break;
                    default: break;
                }

            }
        }
        #endregion

        #region 私有方法
        /// <summary>
        /// 双线性插值方法提高原数据文件分辨率
        /// f(i+u,j+v)=(1-u)(1-v)*f(i,j) + (1-v)u*f(i,j+1) + v(1-u)*f(i+1,j) + uv*f(i+1,j+1)
        /// </summary>
        /// <typeparam name="T">原始文件数据类型</typeparam>
        /// <param name="srcFilename">原文件</param>
        /// <param name="zoom">分辨率提高比例如0.1度到0.01度，zoom = 10</param>
        /// <param name="savepathname">另存结果文件名称</param>
        /// <returns></returns>                                   
        private string Bilinear<T>(string srcFilename, Int16 zoom, string savepathname)
        {
            List<RasterMaper> rms = new List<RasterMaper>();
            IRasterDataProvider outRaster = null;
            IRasterDataProvider inRaster1 = GeoDataDriver.Open(srcFilename) as IRasterDataProvider;
            RasterMaper fileIn1 = new RasterMaper(inRaster1, new int[] { 1 });
            rms.Add(fileIn1);
            string fileBilname = savepathname;
            outRaster = CreateOutRaster(fileBilname, inRaster1.DataType, rms.ToArray(), inRaster1.ResolutionX / zoom, zoom);
            T[] inRasterBuffer = new T[inRaster1.Width * inRaster1.Height];
            IRasterBand bandinRaster1 = inRaster1.GetRasterBand(1);
            T[] sd = new T[inRaster1.Width * inRaster1.Height];
            T[] sdSnow = new T[outRaster.Width * outRaster.Height];//输出数组
            unsafe
            {
                GCHandle h = GCHandle.Alloc(inRasterBuffer, GCHandleType.Pinned);
                IntPtr ptr = h.AddrOfPinnedObject();

                bandinRaster1.Read(0, 0, inRaster1.Width, inRaster1.Height, ptr, inRaster1.DataType, inRaster1.Width, inRaster1.Height);
                for (int j = 0; j < inRaster1.Width * inRaster1.Height; j++)
                {
                    sd[j] = inRasterBuffer[j];
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
                                    float result = (1 - u) * (1 - v) * ConvertTtoFloat<T>(sd[i - inRaster1.Width - 1]) + (1 - v) * u * ConvertTtoFloat<T>(sd[i - inRaster1.Width]) + v * (1 - u) * ConvertTtoFloat<T>(sd[i - 1]) + u * v * ConvertTtoFloat<T>(sd[i]);
                                    sdSnow[index[col + row * zoom]] = ConvertFloatToT<T>(result);
                                }
                                if (col >= xCenter && row < yCenter) //第一象限 高分格子所在象元f(i,j) 其余三个象元分别是f(i-1,j),f(i-1,j+1),f(i,j+1)
                                {
                                    float u = (float)(col * 1.0f / zoom) - 0.5f; //列  横向 
                                    float v = (float)(row * 1.0f / zoom) + 0.5f;   //行 纵
                                    float result = (1 - u) * (1 - v) * ConvertTtoFloat<T>(sd[i - inRaster1.Width]) + (1 - v) * u * ConvertTtoFloat<T>(sd[i - inRaster1.Width + 1]) + v * (1 - u) * ConvertTtoFloat<T>(sd[i]) + u * v * ConvertTtoFloat<T>(sd[i + 1]);
                                    sdSnow[index[col + row * zoom]] = ConvertFloatToT<T>(result);
                                }
                                if (col < xCenter && row >= yCenter) //第三象限 高分格子所在象元f(i,j) 其余三个象元分别是f(i,j-1),f(i+1,j-1),f(i+1,j)
                                {
                                    float u = 0.5f + (float)(col * 1.0f / zoom); //列
                                    float v = (float)(row * 1.0f / zoom) - 0.5f; //行
                                    float result = (1 - u) * (1 - v) * ConvertTtoFloat<T>(sd[i - 1]) + (1 - v) * u * ConvertTtoFloat<T>(sd[i]) + v * (1 - u) * ConvertTtoFloat<T>(sd[i + inRaster1.Width - 1]) + v * u * ConvertTtoFloat<T>(sd[i + inRaster1.Width]);
                                    sdSnow[index[col + row * zoom]] = ConvertFloatToT<T>(result);
                                }
                                if (col >= xCenter && row >= yCenter) //第四象限 高分格子所在象元f(i,j) 其余三个象元分别是f(i,j+1),f(i+1,j),f(i+1,j+1)
                                {
                                    float u = (float)(col * 1.0f / zoom) - 0.5f; //列
                                    float v = (float)(row * 1.0f / zoom) - 0.5f;//行
                                    float result = (1 - u) * (1 - v) * ConvertTtoFloat<T>(sd[i]) + (1 - v) * u * ConvertTtoFloat<T>(sd[i + 1]) + v * (1 - u) * ConvertTtoFloat<T>(sd[i + inRaster1.Width]) + u * v * ConvertTtoFloat<T>(sd[i + inRaster1.Width + 1]);
                                    sdSnow[index[col + row * zoom]] = ConvertFloatToT<T>(result);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //PrintInfo(ex.Message);
            }
            try
            {
                unsafe
                {
                    GCHandle h = GCHandle.Alloc(sdSnow, GCHandleType.Pinned);
                    try
                    {

                        IntPtr sdSnowPtr = h.AddrOfPinnedObject();
                        IRasterBand bandoutRaster = outRaster.GetRasterBand(1);
                        bandoutRaster.Write(0, 0, outRaster.Width, outRaster.Height, sdSnowPtr, outRaster.DataType, outRaster.Width, outRaster.Height);
                    }
                    catch (Exception ex)
                    {
                    }
                    finally
                    {
                        h.Free();
                    }
                }
                //IFileExtractResult res = new FileExtractResult(_subProductDef.Identify, sdBilinearfilename, true);
                return fileBilname;
            }
            finally
            {
                if (outRaster != null)
                    outRaster.Dispose();
                if (inRaster1 != null)
                {
                    inRaster1.Dispose();
                    fileIn1.Raster.Dispose();
                }
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
        /// 中值滤波
        /// </summary>
        /// <typeparam name="T">原始数据类型</typeparam>
        /// <param name="filename">原始数据名称</param>
        /// <param name="smoothwindow">平滑度：此处只能是5*5</param>
        /// <param name="savepath">另存文件名称</param>
        /// <returns></returns>
        private string ComputerMid<T>(string filename, Int16 smoothwindow, string savepath)
        {
            List<RasterMaper> rms = null;
            IRasterDataProvider inRaster1 = null;
            IRasterDataProvider outRaster = null;
            RasterProcessModel<T, T> rfr = null;
            RasterMaper[] fileIns = null;
            RasterMaper[] fileOuts = null;
            RasterMaper fileOut = null;
            string midFilterFileName = savepath;
            if (File.Exists(midFilterFileName))
                return midFilterFileName;
            try
            {
                rms = new List<RasterMaper>();
                inRaster1 = GeoDataDriver.Open(filename) as IRasterDataProvider;
                RasterMaper fileIn1 = new RasterMaper(inRaster1, new int[] { 1 });
                rms.Add(fileIn1);
                if (string.IsNullOrEmpty(midFilterFileName))
                    return null;
                else
                    outRaster = CreateOutRaster(midFilterFileName, inRaster1.DataType, rms.ToArray(), inRaster1.ResolutionX);
                fileOut = new RasterMaper(outRaster, new int[] { 1 });
                fileIns = rms.ToArray();
                fileOuts = new RasterMaper[] { fileOut };
                rfr = new RasterProcessModel<T, T>();
                rfr.SetRaster(fileIns, fileOuts);
                rfr.RegisterCalcModel(new RasterCalcHandler<T, T>((rvInVistor, rvOutVistor, aoi) =>
                {
                    if (rvInVistor[0].RasterBandsData[0] != null)
                    {
                        int dataLength = rvInVistor[0].SizeY * rvInVistor[0].SizeX;
                        T[] temp = new T[smoothwindow * smoothwindow];
                        int col = rvInVistor[0].SizeX;
                        if (smoothwindow == 5)
                        {
                            for (int i = 0; i < dataLength; i++)
                            {
                                if (i < 2 * col || i % col == 0 || (i - 1) % col == 0 || (i + 1) % col == 0 || (i + 2) % col == 0 || i > dataLength - 2 * col)
                                {
                                    if (rvInVistor[0].RasterBandsData[0][i].Equals(ConvertFloatToT<T>(0f)))
                                    {
                                        rvOutVistor[0].RasterBandsData[0][i] = ConvertFloatToT<T>(0f);
                                    }
                                    else
                                        rvOutVistor[0].RasterBandsData[0][i] = rvInVistor[0].RasterBandsData[0][i];
                                }
                                else
                                {
                                    if (rvInVistor[0].RasterBandsData[0][i].Equals(ConvertFloatToT<T>(0f)))
                                    {
                                        rvOutVistor[0].RasterBandsData[0][i] = ConvertFloatToT<T>(0f);
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
                                            if (temp[n].Equals(ConvertFloatToT<T>(0f)))
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
                                    if (rvInVistor[0].RasterBandsData[0][i].Equals(ConvertFloatToT<T>(0f)))
                                    {
                                        rvOutVistor[0].RasterBandsData[0][i] = ConvertFloatToT<T>(0f);
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
                                            if (temp[n].Equals(ConvertFloatToT<T>(0f)))
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
                {
                    outRaster.Dispose();
                    fileOut = null;

                    fileOuts = null;

                }

                if (inRaster1 != null)
                    inRaster1.Dispose();

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
        /// 类型转换
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        private float ConvertTtoFloat<T>(T t)
        {
            return (float)Convert.ChangeType(t, typeof(float));
        }
        private T ConvertFloatToT<T>(float value)
        {
            return (T)Convert.ChangeType(value, typeof(T));
        }

        private IRasterDataProvider CreateOutRaster(string outFileName, enumDataType dataType, RasterMaper[] inrasterMaper, float resolution)
        {
            return CreateOutRaster(outFileName, dataType, inrasterMaper, resolution, 1);
        }
        private IRasterDataProvider CreateOutRaster(string outFileName, enumDataType dataType, RasterMaper[] inrasterMaper, float resolution, Int16 zoom)
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
            if (inrasterMaper.Length == 1 && zoom != 1)
                outEnv = new CoordEnvelope(outEnv.MinX - resX * zoom / 2, outEnv.MaxX - resX * zoom / 2, outEnv.MinY + resX * zoom / 2, outEnv.MaxY + resX * zoom / 2);
            string mapInfo = outEnv.ToMapInfoString(new Size(width, height));
            if (File.Exists(outFileName))
                File.Delete(outFileName);
            RasterDataProvider outRaster = raster.Create(outFileName, width, height, 1, dataType, mapInfo) as RasterDataProvider;
            return outRaster;
        }
        /// <summary>
        ///提示信息
        /// </summary>
        /// <param name="info"></param>
        private void PrintInfo(string info)
        {
            if (_contextMessage != null)
                _contextMessage.PrintMessage(info);
            else
                Console.WriteLine(info);
        }
        #endregion
    }
}
