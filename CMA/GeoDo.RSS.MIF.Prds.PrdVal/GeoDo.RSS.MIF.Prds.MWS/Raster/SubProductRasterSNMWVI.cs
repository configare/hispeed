#region Version Info
/*========================================================================
* 功能概述：
* 
* 创建者：Administrator     时间：2013-11-15 10:50:13
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
    /// 类名：SubProductRasterSNMWVI
    /// 属性描述：微波与可见光判识结果融合计算雪深
    /// 创建者：lxj   创建日期：2013-11-15 10:50:13
    /// 修改者：             修改日期：
    /// 修改描述：
    /// 备注：
    /// </summary>
    public class SubProductRasterSNMWVI : CmaMonitoringSubProduct
    {
        private IContextMessage _contextMessage;
        private IArgumentProvider _curArguments = null;
        public SubProductRasterSNMWVI(SubProductDef subProductDef)
            : base(subProductDef)
        {
        }

        public override IExtractResult Make(Action<int, string> progressTracker)
        {
            _curArguments = _argumentProvider;
            if (_curArguments == null)
                return null;
            if (_curArguments.GetArg("AlgorithmName") == null)
                return null;

            if (_curArguments.GetArg("AlgorithmName").ToString() == "MicroVisSNWSDAlgorithm")
            {
                return MicroVisSNWSDAlgorithm(progressTracker);
            }
            return null;
        }

        private IExtractResult MicroVisSNWSDAlgorithm(Action<int, string> progressTracker)
        {

            string inputSDFileName = _argumentProvider.GetArg("RasterSDFile").ToString();
            if (string.IsNullOrEmpty(inputSDFileName) || !File.Exists(inputSDFileName))
            {
                PrintInfo("缺少微波雪深文件");
                return null;
            }
            string inputCLDSNWFileName = "";
            string inputVISNWFileName = "";
            string identify = "";
            string[] snowfname = GetStringArray("RasterVISNWFiles");
            if (progressTracker != null)
            {
                progressTracker(1, "开始计算");
            }
            if (snowfname == null || snowfname.Length <= 0)
            {
                PrintInfo("请选可见光雪判识文件！");
                return null;
            }
            if (snowfname.Length == 1)
            {
                inputVISNWFileName = snowfname[0];
            }
            else
            {
                identify = "SNDB";
                IFileExtractResult vissnowFilename = ComposeVISSNW(snowfname,identify);
                inputVISNWFileName = vissnowFilename.FileName;
            }
            progressTracker(20, "计算完成20%");
            string[] cloudfname = GetStringArray("RasterCLDSNWFiles");
            if (cloudfname == null || cloudfname.Length <= 0)
            {
                PrintInfo("请选可见光云判识文件！");
                return null;
            }
            if (cloudfname.Length == 1)
            {
                inputCLDSNWFileName = cloudfname[0];
            }
            else
            {
                identify = "0CLM";
                IFileExtractResult viscldFilename = ComposeVISSNW(cloudfname,identify);
                inputCLDSNWFileName = viscldFilename.FileName;
            }
            progressTracker(50, "计算完成50%");
            if (string.IsNullOrEmpty(inputVISNWFileName) || !File.Exists(inputVISNWFileName) || string.IsNullOrEmpty(inputCLDSNWFileName) || !File.Exists(inputCLDSNWFileName))
            {
                PrintInfo("缺少可见光雪或者云判识文件");
                return null;
            }
            else
            {
                IExtractResultArray array = new ExtractResultArray("融合雪深");
                IFileExtractResult visFilename = ComputeVIS(inputVISNWFileName, inputCLDSNWFileName);
                array.Add(visFilename);
                progressTracker(70, "计算完成70%");
                string visSNWfilename = visFilename.FileName;
                IFileExtractResult microvisfilename = ComputeSD(inputSDFileName, visSNWfilename);
                array.Add(microvisfilename);
                //中值滤波
                Int16 smoothwindow = 5;
                string microvisName = microvisfilename.FileName;
                IFileExtractResult midSDFilterResult = ComputerMid(microvisName, smoothwindow);//滤波
                array.Add(midSDFilterResult);
                progressTracker(100, "计算完成");
                return array;
            }
        }
        /// <summary>
        /// 中值滤波
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        private IFileExtractResult ComputerMid(string filename, Int16 smoothwindow)
        {
            List<RasterMaper> rms = null;
            IRasterDataProvider outRaster = null;
            RasterProcessModel<float, float> rfr = null;
            //float outResloution = 0.01f;
            string identyString = "MFVI";
          
            try
            {
                rms = new List<RasterMaper>();
                IRasterDataProvider inRaster1 = GeoDataDriver.Open(filename) as IRasterDataProvider;
                RasterMaper fileIn1 = new RasterMaper(inRaster1, new int[] { 1 });
                rms.Add(fileIn1);
                string middelFilterFileName = GetFileName(new string[] { filename }, _subProductDef.ProductDef.Identify, identyString, ".dat", null);
                outRaster = CreateOutRaster(middelFilterFileName, enumDataType.Float, rms.ToArray(), inRaster1.ResolutionX);
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
                        for (int i = 0; i < dataLength; i++)
                        {
                            if (i < 2 * col || i % col == 0 || (i - 1) % col == 0 || (i + 1) % col == 0 || (i + 2) % col == 0 || i > dataLength - 2 * col)
                            {
                                rvOutVistor[0].RasterBandsData[0][i] = rvInVistor[0].RasterBandsData[0][i];
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
                                Array.Sort(temp);
                                rvOutVistor[0].RasterBandsData[0][i] = temp[temp.Length / 2];
                            }
                        }
                    }
                }));
                rfr.Excute();
                IFileExtractResult res = new FileExtractResult(_subProductDef.Identify, middelFilterFileName, true);
                res.SetDispaly(false);
                return res;
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
        /// 融合微波雪深文件 和 可见光雪云辨识文件更新雪深
        /// </summary>
        /// <param name="sdfilname"></param>
        /// <param name="visfilename"></param>
        /// <returns></returns>
        private IFileExtractResult ComputeSD(string sdfilename, string visfilename)
        {
            List<RasterMaper> rms = null;
            IRasterDataProvider outRaster = null;
            IRasterDataProvider outClassRaster = null;//融合分类
            rms = new List<RasterMaper>();
            IRasterDataProvider inRaster1 = GeoDataDriver.Open(sdfilename) as IRasterDataProvider; //微波雪深数据
            RasterMaper fileIn1 = new RasterMaper(inRaster1, new int[] { 1 });
            rms.Add(fileIn1);
            IRasterDataProvider inRaster2 = GeoDataDriver.Open(visfilename) as IRasterDataProvider;//可见云和雪判识结果
            RasterMaper fileIn2 = new RasterMaper(inRaster2, new int[] { 1 });
            rms.Add(fileIn2);
            //林地覆盖率
            string forestFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SystemData\\ProductArgs\\MWS\\SnowArgFile\\china_forest._frac.dat");
            IRasterDataProvider inRaster3 = GeoDataDriver.Open(forestFile) as IRasterDataProvider; 
            RasterMaper fileIn3 = new RasterMaper(inRaster3, new int[] { 1 });
            rms.Add(fileIn3);
            if (inRaster1.ResolutionX != inRaster3.ResolutionX)
            {
                PrintInfo("雪深数据分辨率与辅助数据分辨率0.1度不同！");
                    return null;
            }
            string visFileName = GetFileName(new string[] { sdfilename }, _subProductDef.ProductDef.Identify, "MWVI", ".dat", null);
            outRaster = CreateOutRaster(visFileName, enumDataType.Float, rms.ToArray(), inRaster2.ResolutionX);
            //融合分类数据
            string classFileName = GetFileName(new string[] { sdfilename }, _subProductDef.ProductDef.Identify, "MCVI", ".dat", null);
            outClassRaster = CreateOutRaster(classFileName, enumDataType.Float, rms.ToArray(), inRaster2.ResolutionX);
            double minX = outRaster.CoordEnvelope.MinX;
            double minY = outRaster.CoordEnvelope.MinY;
            double maxX = outRaster.CoordEnvelope.MaxX;
            double maxY = outRaster.CoordEnvelope.MaxY;
            int xLeft = (int)((minX - inRaster1.CoordEnvelope.MinX) / inRaster1.ResolutionX);
            int yUp = (int)((inRaster1.CoordEnvelope.MaxY - maxY) / inRaster1.ResolutionY);
            int xRight = (int)((maxX - inRaster1.CoordEnvelope.MinX) / inRaster1.ResolutionX);
            int yDown = (int)((inRaster1.CoordEnvelope.MaxY - minY) / inRaster1.ResolutionX);
            int xSize = xRight - xLeft;
            int ySize = yDown - yUp;
            float[] buffer = new float[xSize * ySize];
            IRasterBand bandinRaster1 = inRaster1.GetRasterBand(1);
            float[] sd = new float[xSize * ySize];
            //float[] snowClass = new float[xSize * ySize];
            unsafe
            {
                fixed (float* pointer = buffer)
                {
                    IntPtr ptr = new IntPtr(pointer);
                    bandinRaster1.Read(xLeft, yUp, xSize, ySize, ptr, inRaster1.DataType, xSize, ySize);
                    for (int j = 0; j < xSize * ySize; j++)
                    {
                        sd[j] = buffer[j];
                    }
                }
            }
            int visLeftX = (int)((minX - inRaster2.CoordEnvelope.MinX) / inRaster2.ResolutionX);
            int visLeftY = (int)((inRaster2.CoordEnvelope.MaxY - maxY) / inRaster2.ResolutionY);
            int visRightX = (int)((maxX - inRaster2.CoordEnvelope.MinX) / inRaster2.ResolutionX);
            int visRightY = (int)((inRaster2.CoordEnvelope.MaxY - minY) / inRaster2.ResolutionX);
            int visxSize = visRightX - visLeftX;
            int visySize = visRightY - visLeftY;
            Int16[] visbuffer = new Int16[visxSize * visySize];
            IRasterBand bandinRaster2 = inRaster2.GetRasterBand(1);
            Int16[] visSNW = new Int16[visxSize * visySize];
           // float[] sdSnow = new float[outRaster.Width * outRaster.Height];
            float[ ,] sdSnow = new float[2,outRaster.Width * outRaster.Height];//一维放融合雪深，二维放融合分类
            unsafe
            {
                fixed (Int16* pointer = visbuffer)
                {
                    IntPtr ptr = new IntPtr(pointer);
                    bandinRaster2.Read(visLeftX, visLeftY, visxSize, visySize, ptr, inRaster2.DataType, visxSize, visySize);
                    for (int j = 0; j < visxSize * visySize; j++)
                    {
                        visSNW[j] = visbuffer[j];
                    }
                }
            }
            //读取森林覆盖率;
            int xforestLeft = (int)((minX - inRaster3.CoordEnvelope.MinX) / inRaster3.ResolutionX);
            int yforestUp = (int)((inRaster3.CoordEnvelope.MaxY - maxY) / inRaster3.ResolutionY);
            int xforestRight = (int)((maxX - inRaster3.CoordEnvelope.MinX) / inRaster3.ResolutionX);
            int yforestDown = (int)((inRaster3.CoordEnvelope.MaxY - minY) / inRaster3.ResolutionX);
            int xforestSize = xforestRight - xforestLeft;
            int yforestSize = yforestDown - yforestUp;
            Int16[] forestbuffer = new Int16[xforestSize * yforestSize];
            IRasterBand bandinRaster3 = inRaster3.GetRasterBand(1);
            Int16[] forestFrac = new Int16[xforestSize * yforestSize];
            unsafe
            {
                fixed (Int16* pointer = forestbuffer)
                {
                    IntPtr ptr = new IntPtr(pointer);
                    bandinRaster3.Read(xforestLeft, yforestUp, xforestSize, yforestSize, ptr, inRaster3.DataType, xforestSize, yforestSize);
                    for (int j = 0; j < xforestSize * yforestSize; j++)
                    {
                        forestFrac[j] = forestbuffer[j];
                    }
                }
            }
            int zoom = (int)(inRaster1.ResolutionX / inRaster2.ResolutionX);
            try
            {
                for (int j = 0; j < xSize * ySize; j++)   //低分的格子 
                {
                    if (((j + 1) % xSize == 0) && (j != xSize * ySize - 1))      //最后一列并且不是低分最后一个格子
                    {
                        int zoomcha = xSize * zoom - visxSize;
                        Int32[] index = new Int32[zoom * (zoom - zoomcha)];
                        for (int row = 0; row < zoom; row++)  //行还是原来的zoom行
                        {
                            for (int col = 0; col < zoom - zoomcha; col++)  //列比原来少 zoomcha 列 
                            {
                                index[col + row * (zoom - zoomcha)] = (j / xSize * zoom * visxSize + j % xSize * zoom) + (row * visxSize) + col;

                            }
                        }
                        sdSnow = SDsnow(zoom - zoomcha, zoom, index, visSNW, sd[j], sdSnow,forestFrac[j]);
                    }
                    else
                    {
                        if ((j >= xSize * ySize - xSize) && (j != xSize * ySize - 1))    //最后一行并且不是低分最后一个格子
                        {
                            int zoomcha = ySize * zoom - visySize;
                            Int32[] index = new Int32[(zoom - zoomcha) * zoom];
                            for (int row = 0; row < zoom - zoomcha; row++)  //行比原来少 zoomcha行
                            {
                                for (int col = 0; col < zoom; col++)  //列还是原来列 
                                {
                                    index[col + row * zoom] = (j / xSize * zoom * visxSize + j % xSize * zoom) + (row * visxSize) + col;
                                }
                            }
                            sdSnow = SDsnow(zoom, zoom - zoomcha, index, visSNW, sd[j], sdSnow, forestFrac[j]);
                        }
                        else
                        {
                            if (j == xSize * ySize - 1)  //低分的最后一个格子
                            {
                                int zoomcha = ySize * zoom - visySize;
                                Int32[] index = new Int32[(zoom - zoomcha) * (zoom - zoomcha)];
                                for (int row = 0; row < zoom - zoomcha; row++)  //行比原来少 zoomcha行
                                {
                                    for (int col = 0; col < zoom - zoomcha; col++)  //列列比原来少 zoomcha 列
                                    {
                                        index[col + row * (zoom - zoomcha)] = (j / xSize * zoom * visxSize + j % xSize * zoom) + (row * visxSize) + col;
                                    }
                                }
                                sdSnow = SDsnow(zoom - zoomcha, zoom - zoomcha, index, visSNW, sd[j], sdSnow, forestFrac[j]);
                            }
                            else
                            {
                                Int32[] index = new Int32[zoom * zoom];
                                for (int row = 0; row < zoom; row++)
                                {
                                    for (int col = 0; col < zoom; col++)
                                    {
                                        index[col + row * zoom] = (j / xSize * zoom * visxSize + j % xSize * zoom) + (row * visxSize) + col;
                                    }
                                }
                                sdSnow = SDsnow(zoom, zoom, index, visSNW, sd[j], sdSnow, forestFrac[j]);
                            }
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                PrintInfo(ex.Message);
            }
            float[] sdSnow1 = new float[outRaster.Width * outRaster.Height];
            float[] sdclass = new float[outRaster.Width * outRaster.Height];
            IExtractResultArray array = new ExtractResultArray("融合雪深分类");
            for (int index = 0; index < outRaster.Width * outRaster.Height; index++)
            {
                sdSnow1[index] = sdSnow[0, index];
                sdclass[index] = sdSnow[1, index];
            }
            try
            {
                unsafe
                {
                    fixed (float* ptr = sdSnow1)
                    {
                        IntPtr sdSnowPtr = new IntPtr(ptr);
                        IRasterBand bandoutRaster = outRaster.GetRasterBand(1);
                        bandoutRaster.Write(0, 0, outRaster.Width, outRaster.Height, sdSnowPtr, outRaster.DataType, outRaster.Width, outRaster.Height);
                    }
                    fixed (float* ptr = sdclass)
                    {
                        IntPtr sdclassPtr = new IntPtr(ptr);
                        IRasterBand bandoutRaster = outClassRaster.GetRasterBand(1);
                        bandoutRaster.Write(0, 0, outClassRaster.Width, outClassRaster.Height, sdclassPtr, outClassRaster.DataType, outClassRaster.Width, outClassRaster.Height);
                    }
                }
                IFileExtractResult res = new FileExtractResult(_subProductDef.Identify, visFileName, true);
                array.Add(res);
                IFileExtractResult res2 = new FileExtractResult(_subProductDef.Identify, classFileName, true);
                array.Add(res2);
                return res;
            }
            finally
            {
               if (outRaster != null)
                 outRaster.Dispose();
               if (outClassRaster != null)
                 outClassRaster.Dispose();
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
        /// 计算生成数据的雪深数组
        /// </summary>
        /// <param name="zoomcol">列</param>
        /// <param name="zoomrow">行</param>
        /// <param name="index">低分一格子相应高分格子的所有格子索引</param>
        /// <param name="visSNW">高分雪数据</param>
        /// <param name="sdj">原微波每个格子雪深</param>
        /// <param name="sdSnow">计算出来的微波与可见光融合雪深</param>
        /// <param name="forestFracj">森林覆盖率</param>
        /// <returns></returns>
        private float[,] SDsnow(int zoomcol, int zoomrow, Int32[] index, Int16[] visSNW, float sdj, float[,] sdSnow, Int16 forestFracj)
       {
            int[] temp = new int[zoomcol * zoomrow];
            int sumSnow = 0;
            int sumCloud = 0;
            int sumbare = 0;
            for (int p = 0; p < zoomcol * zoomrow; p++)
            {
                temp[p] = visSNW[index[p]];

                if (temp[p] == 1)
                {
                    sumSnow += 1;   //雪
                }
                if (temp[p] == 2)
                {
                    sumCloud += 1;  //云
                }
                if (temp[p] == 0)
                {
                    sumbare += 1;  // 裸土
                }
            }
            if (forestFracj >= 5000)   //如果森林覆盖率大于50%就以微波数据为主
            {
                for (int p = 0; p < zoomcol * zoomrow; p++)
                {
                    sdSnow[0,index[p]] = sdj;
                    if (sdSnow[0, index[p]] > 0.0f)
                        sdSnow[1, index[p]] = 1.0f; //表示林地
                }
            }
            else
            {
                if ((sdj != 0 && sumSnow == zoomcol * zoomrow) || (sdj != 0 && sumCloud == zoomcol * zoomrow))  //纯像元 全是雪 或全是云 
                {
                    for (int p = 0; p < zoomcol * zoomrow; p++)
                    {
                        sdSnow[0,index[p]] = sdj;
                        sdSnow[1, index[p]] = 11.0f; //表示以微波为主；
                    }
                }
                if ((sdj != 0 && sumbare == zoomcol * zoomrow) || (sdj == 0 && sumbare == zoomcol * zoomrow)) //纯像元  微波有雪，可见光裸土或者 微波无，可见光无 
                {
                    for (int p = 0; p < zoomcol * zoomrow; p++)
                    {
                        sdSnow[0,index[p]] = 0.0f;
                        sdSnow[1, index[p]] = 0.0f;//表示可见光一定没雪。微波可有可无
                    }
                }
                if (sdj <= 0.004f && sumSnow == zoomcol * zoomrow)  //纯像元 微波无雪，可见光有雪 
                {
                    for (int p = 0; p < zoomcol * zoomrow; p++)
                    {
                        sdSnow[0,index[p]] = 5.0f;
                        sdSnow[1, index[p]] = 15.0f;//微波无雪，可见光有雪
                    }
                }

                //非纯像元
                if ((sdj != 0 && sdj < 100.0f) && (sumSnow + sumCloud == zoomcol * zoomrow))  //雪+云
                {
                    for (int p = 0; p < zoomcol * zoomrow; p++)
                    {
                        sdSnow[0,index[p]] = sdj;
                        sdSnow[1, index[p]] = 21.0f; //微波有， 可见光为雪+ 云， 以微波为主
                    }
                }
                if ((sdj != 0 && sdj < 100.0f) && (sumSnow + sumbare == zoomcol * zoomrow))  // 雪+ 裸地
                {
                    //雪占比例
                    //float snowPercent = sumSnow/(zoom * zoom) ;
                    for (int p = 0; p < zoomcol * zoomrow; p++)
                    {
                        if (visSNW[index[p]] == 1)
                        {
                            sdSnow[0, index[p]] = sdj * (zoomcol * zoomrow) / sumSnow;
                            sdSnow[1, index[p]] = 22.0f;//可见光为雪+ 裸地，用微波/雪百分比，
                        }
                        else
                        {
                            sdSnow[0, index[p]] = 0.0f;
                            sdSnow[1, index[p]] = 0.0f; //表示裸地
                        }
                    }
                }
                if ((sdj != 0 && sdj < 100.0f) && (sumSnow + sumCloud + sumbare == zoomcol * zoomrow))// 雪+ 云+ 裸地
                {
                    if (sumCloud - sumbare > 0)
                    {
                        for (int p = 0; p < zoomcol * zoomrow; p++)
                        {
                            if (visSNW[index[p]] == 1 || visSNW[index[p]] == 2)
                            {
                                sdSnow[0, index[p]] = sdj * (zoomcol * zoomrow) / (sumSnow + sumCloud);
                                sdSnow[1, index[p]] = 22.0f;
                            }
                            else
                            {
                                sdSnow[0, index[p]] = 0.0f;
                                sdSnow[1, index[p]] = 0.0f;
                            }
                        }
                    }
                    if (sumCloud - sumbare < 0)
                    {
                        for (int p = 0; p < zoomcol * zoomrow; p++)
                        {  
                            if (visSNW[index[p]] == 1)
                            {
                                sdSnow[0, index[p]] = sdj * (zoomcol * zoomrow) / sumSnow;
                                sdSnow[1, index[p]] = 22.0f;
                            }
                            else
                            {
                                sdSnow[0, index[p]] = 0.0f;
                                sdSnow[1, index[p]] = 0.0f;
                            }
                        }
                    }
                }

                if ((sdj != 0 && sdj < 100.0f) && (sumCloud + sumbare == zoomcol * zoomrow)) // 微波有，可见光 云+ 裸地  （最初按无雪）
                {
                    //for (int p = 0; p < zoomcol * zoomrow; p++)            //2013.11.27修改按着云和裸地的比例，云当作雪对待 
                    //{
                    //    sdSnow[index[p]] = 0.0f;
                    //}
                    for (int p = 0; p < zoomcol * zoomrow; p++)
                    {
                        if (visSNW[index[p]] == 2)
                        {
                            sdSnow[0, index[p]] = sdj * (zoomcol * zoomrow) / sumCloud;
                            sdSnow[1, index[p]] = 22.0f;
                        }
                        else
                        {
                            sdSnow[0, index[p]] = 0.0f;
                            sdSnow[1, index[p]] = 0.0f;
                        }
                    }
                }
               
                if (sdj <=0.004f && ((sumSnow + sumCloud == zoomcol * zoomrow) || (sumSnow + sumCloud + sumbare == zoomcol * zoomrow))) // 微波无 ；可见光 雪+ 云  或者可见光 雪+ 云+裸地
                {
                    for (int p = 0; p < zoomcol * zoomrow; p++)
                    {
                        //if (visSNW[index[p]] == 1)    //2014年2月13日以前为这种,可见光为雪时，雪深5.0
                        if (visSNW[index[p]] == 1 || visSNW[index[p]] == 2)//2014.2.13修改可见光云当成雪，雪深为5.0
                        {
                            sdSnow[0, index[p]] = 5.0f;
                            sdSnow[1, index[p]] = 25.0f;
                        }
                        else
                        {
                            sdSnow[0, index[p]] = 0.0f;
                            sdSnow[1, index[p]] = 0.0f;
                        }
                    }
                }
               
            }
            return sdSnow;
        }
        /// <summary>
        /// 输入的可见光雪判识或云判识分别合成一个文件
        /// </summary>
        /// <param name="filenames"></param>
        /// <returns></returns>
        private IFileExtractResult ComposeVISSNW(string[] filenames,string identify)
        {
            List<RasterMaper> rms = new List<RasterMaper>();
            RasterProcessModel<Int16, Int16> rfr = null;
            IRasterDataProvider outRaster = null;
            float resloution = 0.0f;
            try
            {
                for (int i = 0; i < filenames.Length; i++)
                {
                    IRasterDataProvider inRaster = GeoDataDriver.Open(filenames[i]) as IRasterDataProvider;
                    RasterMaper rm = new RasterMaper(inRaster, new int[] { 1 });
                    rms.Add(rm);
                    resloution = inRaster.ResolutionX;
                }
                string visSNWFile = GetFileName(new string[] { filenames[0] }, _subProductDef.ProductDef.Identify, identify, ".dat", null);
                outRaster = CreateOutRasterUnion(visSNWFile,enumDataType.Int16, rms.ToArray(), resloution);
                RasterMaper[] fileIns = rms.ToArray();
                RasterMaper[] fileOuts = new RasterMaper[] { new RasterMaper(outRaster, new int[] { 1 }) };
                rfr = new RasterProcessModel<Int16, Int16>();
                rfr.SetRaster(fileIns, fileOuts);
                rfr.RegisterCalcModel(new RasterCalcHandler<Int16, Int16>((rvInVistor, rvOutVistor, aoi) =>
                    {
                        int dataLength = rvOutVistor[0].RasterBandsData[0].Length;
                        int n = rvInVistor.Count();
                        Int16[,] save = new Int16[n, dataLength];// 第一维是文件的顺序号，第二维是文件的数值
                        Int16[] sumData = new Int16[dataLength];
                        int j = 0; //记录输入文件的序号，从0开始
                        //System.Windows.Forms.MessageBox.Show(Convert.ToString(n));
                        foreach (RasterVirtualVistor<Int16> rv in rvInVistor)
                        {
                            if (rv.RasterBandsData[0] == null)
                                continue;
                            for (int index = 0; index < dataLength; index++)
                            {
                                save[j, index] = rv.RasterBandsData[0][index];
                            }
                            j++;
                        }
                        //Int16[] sumData = new Int16[dataLength];
                        for (int m = 0; m < n; m++)                      //文件号
                        {
                            for (int q = 0; q < dataLength; q++)         //像元号
                            {
                                sumData[q] = Convert.ToInt16(sumData[q] + save[m, q]); //计算这些输文件的值和
                            }
                        }
                        
                        for (int index = 0; index < dataLength; index++)
                        {
                            if (sumData[index] == 0)
                            {
                                rvOutVistor[0].RasterBandsData[0][index] = 0;
                            }
                            else
                            {
                                rvOutVistor[0].RasterBandsData[0][index] = 1;
                            }
                        }
                    }));
                rfr.Excute();
                IFileExtractResult res = new FileExtractResult(_subProductDef.Identify, visSNWFile, true);
                res.SetDispaly(false);
                return res;
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
        /// 可见光雪和云合成一个文件
        /// </summary>
        /// <param name="visSNWfilename"></param>
        /// <param name="cldfilename"></param>
        /// <returns></returns>
        private IFileExtractResult ComputeVIS(string visSNWfilename, string cldfilename)
        {
            List<RasterMaper> rms = null;
            IRasterDataProvider outRaster = null;
            RasterProcessModel<Int16, Int16> rfr = null;
            try
            {
                rms = new List<RasterMaper>();
                IRasterDataProvider inRaster1 = GeoDataDriver.Open(visSNWfilename) as IRasterDataProvider;
                RasterMaper fileIn1 = new RasterMaper(inRaster1, new int[] { 1 });
                rms.Add(fileIn1);
                IRasterDataProvider inRaster2 = GeoDataDriver.Open(cldfilename) as IRasterDataProvider;
                RasterMaper fileIn2 = new RasterMaper(inRaster2, new int[] { 1 });
                rms.Add(fileIn2);
                //中国区掩膜文件
                string chinarasterFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SystemData\\ProductArgs\\MWS\\SnowArgFile\\china_raster.dat");
                if (string.IsNullOrEmpty(chinarasterFile) || !File.Exists(chinarasterFile))
                {
                    PrintInfo("缺少系统中国区栅格文件");
                    return null;
                }
                IRasterDataProvider inRaster3 = GeoDataDriver.Open(chinarasterFile) as IRasterDataProvider;
                RasterMaper fileIn3= new RasterMaper(inRaster3, new int[] { 1 });
                rms.Add(fileIn3);

                string visFileName = GetFileName(new string[] { visSNWfilename }, _subProductDef.ProductDef.Identify, "SVIS", ".dat", null);
                outRaster = CreateOutRaster(visFileName, enumDataType.Int16, rms.ToArray(), inRaster1.ResolutionX);
                RasterMaper fileOut = new RasterMaper(outRaster, new int[] { 1 });
                RasterMaper[] fileIns = rms.ToArray();
                RasterMaper[] fileOuts = new RasterMaper[] { fileOut };
                rfr = new RasterProcessModel<Int16, Int16>();
                rfr.SetRaster(fileIns, fileOuts);
                rfr.RegisterCalcModel(new RasterCalcHandler<Int16, Int16>((rvInVistor, rvOutVistor, aoi) =>
                {
                    if (rvInVistor[0].RasterBandsData[0] != null && rvInVistor[1].RasterBandsData[0] != null && rvInVistor[2].RasterBandsData[0] != null)
                    {
                        int dataLength = rvInVistor[2].RasterBandsData[0].Length;
                        int[] type = new int[dataLength];
                        float[] sdtmp = new float[dataLength];
                        for (int i = 0; i < dataLength; i++)
                        {
                            if (rvInVistor[0].RasterBandsData[0][i] == 1 && (rvInVistor[1].RasterBandsData[0][i] == 0 ||rvInVistor[1].RasterBandsData[0][i] == 1 ))
                            {
                                if (rvInVistor[2].RasterBandsData[0][i] == 1) //如果两个文件都判识出既是云，又是雪就按雪来处理。
                                    rvOutVistor[0].RasterBandsData[0][i] = 1;  // 1 表示雪
                                else
                                    rvOutVistor[0].RasterBandsData[0][i] = 0;
                            }
                            else
                            {
                                if (rvInVistor[0].RasterBandsData[0][i] == 0 && rvInVistor[1].RasterBandsData[0][i] == 1)
                                {
                                    if (rvInVistor[2].RasterBandsData[0][i] == 1)
                                        rvOutVistor[0].RasterBandsData[0][i] = 2;  // 2 表示云
                                    else
                                        rvOutVistor[0].RasterBandsData[0][i] = 0;
                                }
                                else
                                {
                                    rvOutVistor[0].RasterBandsData[0][i] = 0;    // 0 表示裸地
                                }
                            }
                        }
                    }
                }));
                rfr.Excute();
                IFileExtractResult res = new FileExtractResult(_subProductDef.Identify, visFileName, true);
                res.SetDispaly(false);
                return res;
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
       
        private void PrintInfo(string info)
        {
            if (_contextMessage != null)
                _contextMessage.PrintMessage(info);
            else
                Console.WriteLine(info);
        }
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
            RasterDataProvider outRaster = raster.Create(outFileName, width, height, 1, dataType, mapInfo) as RasterDataProvider;
            return outRaster;
        }
        protected IRasterDataProvider CreateOutRasterUnion(string outFileName, enumDataType dataType, RasterMaper[] inrasterMaper, float resolution)
        {
            IRasterDataDriver raster = RasterDataDriver.GetDriverByName("MEM") as IRasterDataDriver;
            CoordEnvelope outEnv = null;
            foreach (RasterMaper inRaster in inrasterMaper)
            {
                if (outEnv == null)
                    outEnv = inRaster.Raster.CoordEnvelope;
                else
                    outEnv = outEnv.Union(inRaster.Raster.CoordEnvelope);
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
            RasterDataProvider outRaster = raster.Create(outFileName, width, height, 1, dataType, mapInfo) as RasterDataProvider;
            return outRaster;
        }
    }
}
