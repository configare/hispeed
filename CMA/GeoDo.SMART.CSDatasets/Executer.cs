using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.Core.DF;
using System.IO;
using System.Drawing;

namespace GeoDo.SMART.CSDatasets
{
    public class Executer<T>
    {
        private Func<int, T[], bool> _boolFunc;

        public Executer()
        {

        }
        private static string[] AngleNames = { "SolarZenith", "SolarAzimuth", "SensorZenith", "SensorAzimuth" };

        private void ConsoleWrite(int progress, string context)
        {
            Console.WriteLine(progress + "%" + " : " + context);
        }

        public bool ProcessCSD(string outDir, string[] filenames, int[] modelCalcBands, int[] orderCalcBands, Dictionary<int, int> outBands, string calcModel)
        {
            List<RasterMaper> rms = new List<RasterMaper>();
            List<int> outbandsSrc = new List<int>();
            outbandsSrc.AddRange(orderCalcBands);
            foreach (int key in outBands.Keys)
                outbandsSrc.Add(outBands[key]);
            try
            {
                enumDataType datatype = enumDataType.Atypism;
                IRasterDataProvider rdp = null;
                RasterMaper rdpRm = null;
                for (int i = 0; i < filenames.Length; i++)
                {
                    rdp = GeoDataDriver.Open(filenames[i]) as IRasterDataProvider;
                    if (rdp != null)
                    {
                        datatype = rdp.DataType;
                        rdpRm = new RasterMaper(rdp, outbandsSrc.ToArray());
                        rms.Add(rdpRm);
                    }
                }
                string outFileName = outDir + "\\" + GetFileName(filenames, ".ldf");

                IExtractFuncProvider<T> prd = ExtractFuncProviderFactory.CreateExtractFuncProvider<T>(modelCalcBands, calcModel, null);
                _boolFunc = prd.GetBoolFunc();
                int inVisitorCount = rms.Count;
                int inVisitorBand = orderCalcBands.Length;
                int resultInFileIndex = 0;
                using (IRasterDataProvider outRaster = CreateOutRaster(outFileName, rms.ToArray(), datatype, outBands.Count))
                {
                    RasterMaper[] fileIns = rms.ToArray();
                    RasterMaper[] fileOuts = new RasterMaper[] { new RasterMaper(outRaster, outBands.Keys.ToArray()) };
                    //创建处理模型
                    RasterProcessModel<T, T> rfr = null;
                    rfr = new RasterProcessModel<T, T>(ConsoleWrite);
                    rfr.SetRaster(fileIns, fileOuts);
                    List<T> curValues = new List<T>();
                    List<T> nextValues = new List<T>();
                    List<T> indexValues = new List<T>();
                    rfr.RegisterCalcModel(new RasterCalcHandler<T, T>((rvInVistor, rvOutVistor, aoi) =>
                    {
                        int dataLength = rvOutVistor[0].SizeY * rvOutVistor[0].SizeX;

                        if (rvInVistor[0].RasterBandsData == null)
                            return;
                        for (int index = 0; index < dataLength; index++)
                        {
                            curValues.Clear();
                            for (int inBnad = 0; inBnad < inVisitorBand; inBnad++)
                            {
                                if (rvInVistor[0].RasterBandsData[inBnad] == null)
                                    return;
                                curValues.Add(rvInVistor[0].RasterBandsData[inBnad][index]);
                            }
                            resultInFileIndex = 0;

                            for (int inFile = 1; inFile < inVisitorCount; inFile++)
                            {
                                try
                                {
                                    if (rvInVistor[inFile].RasterBandsData == null)
                                        return;
                                    for (int inBnad = 0; inBnad < inVisitorBand; inBnad++)
                                    {
                                        if (rvInVistor[inFile].RasterBandsData[inBnad] == null)
                                            return;
                                        nextValues.Add(rvInVistor[inFile].RasterBandsData[inBnad][index]);
                                    }
                                    indexValues.AddRange(curValues);
                                    indexValues.AddRange(nextValues);
                                    if (_boolFunc != null)
                                    {
                                        if (!_boolFunc(index, indexValues.ToArray()))
                                        {
                                            resultInFileIndex = inFile;
                                            curValues.Clear();
                                            curValues.AddRange(nextValues);
                                        }
                                    }
                                }
                                finally
                                {
                                    indexValues.Clear();
                                    nextValues.Clear();
                                }
                            }
                            for (int outBand = 0; outBand < outBands.Count; outBand++)
                            {
                                rvOutVistor[0].RasterBandsData[outBand][index] = rvInVistor[resultInFileIndex].RasterBandsData[inVisitorBand + outBand][index];
                            }
                        }
                    }));
                    //执行
                    rfr.Excute();
                    Console.WriteLine("已完成!");
                }
            }
            finally
            {
            }
            return true;
        }
        /// <summary>
        /// 修改原始方法-增加角度文件计算
        /// 修改说明：    
        /// 修改创建输出文件：原输出一个文件，现输出包含角度多个文件
        /// 修改输出文件赋值部分
        /// </summary>
        /// <param name="isangle">是否假如角度文件进行计算</param>
        /// <param name="outDir">输出文件夹</param>
        /// <param name="filenames">输入文件-主ldf文件</param>
        /// <param name="modelCalcBands"></param>
        /// <param name="orderCalcBands"></param>
        /// <param name="outBands"></param>
        /// <param name="calcModel"></param>
        /// <returns></returns>
        public bool AngleProcessCSD(string outDir, string[] filenames, int[] modelCalcBands, int[] orderCalcBands, Dictionary<int, int> outBands, string calcModel)
        {
            IRasterDataProvider outRaster = null;
            List<RasterMaper> listoutrastermap = new List<RasterMaper>();
            enumDataType angletype = enumDataType.Int16;
            List<RasterMaper> rms = new List<RasterMaper>();
            List<RasterMaper> rmsangle = new List<RasterMaper>();//角度输入文件rm

            List<int> outbandsSrc = new List<int>();
            List<int> listoutindex = new List<int>();
            //输入文件角度对应键值对 分别为影像文件序号，影像文件对应的角度文件序号
            Dictionary<int, List<int>> _dicangle = new Dictionary<int, List<int>>();
            outbandsSrc.AddRange(orderCalcBands);
            foreach (int key in outBands.Keys)
                outbandsSrc.Add(outBands[key]);
            try
            {
                enumDataType datatype = enumDataType.Atypism;
                IRasterDataProvider rdp = null;
                RasterMaper rdpRm = null;
                //rmlist增加常规影像文件
                for (int i = 0; i < filenames.Length; i++)
                {
                    rdp = GeoDataDriver.Open(filenames[i]) as IRasterDataProvider;
                    if (rdp != null)
                    {
                        datatype = rdp.DataType;
                        rdpRm = new RasterMaper(rdp, outbandsSrc.ToArray());
                        rms.Add(rdpRm);
                    }
                }
                IExtractFuncProvider<T> prd = ExtractFuncProviderFactory.CreateExtractFuncProvider<T>(modelCalcBands, calcModel, null);
                _boolFunc = prd.GetBoolFunc();
                int inVisitorCount = rms.Count;
                int inVisitorBand = orderCalcBands.Length;
                int resultInFileIndex = 0;
                #region 输出文件准备
                string outFileName = outDir + "\\" + GetFileName(filenames, ".ldf");
                outRaster = CreateOutRaster(outFileName, rms.ToArray(), datatype, outBands.Count);
                #endregion
                RasterMaper[] fileIns = rms.ToArray();
                RasterMaper[] fileOuts = new RasterMaper[] { new RasterMaper(outRaster, outBands.Keys.ToArray()) };
                //创建处理模型
                RasterProcessModel<T, T> rfr = null;
                rfr = new RasterProcessModel<T, T>(ConsoleWrite);
                rfr.SetRaster(fileIns, fileOuts);
                List<T> curValues = new List<T>();
                List<T> nextValues = new List<T>();
                List<T> indexValues = new List<T>();
                rfr.RegisterCalcModel(new RasterCalcHandler<T, T>((rvInVistor, rvOutVistor, aoi) =>
                {
                    int dataLength = rvOutVistor[0].SizeY * rvOutVistor[0].SizeX;

                    if (rvInVistor[0].RasterBandsData == null)
                        return;
                    for (int index = 0; index < dataLength; index++)
                    {
                        curValues.Clear();
                        for (int inBnad = 0; inBnad < inVisitorBand; inBnad++)
                        {
                            if (rvInVistor[0].RasterBandsData[inBnad] == null)
                                return;
                            curValues.Add(rvInVistor[0].RasterBandsData[inBnad][index]);
                        }
                        resultInFileIndex = 0;

                        for (int inFile = 1; inFile < inVisitorCount; inFile++)
                        {
                            try
                            {
                                if (rvInVistor[inFile].RasterBandsData == null)
                                    return;
                                for (int inBnad = 0; inBnad < inVisitorBand; inBnad++)
                                {
                                    if (rvInVistor[inFile].RasterBandsData[inBnad] == null)
                                        return;
                                    nextValues.Add(rvInVistor[inFile].RasterBandsData[inBnad][index]);
                                }
                                indexValues.AddRange(curValues);
                                indexValues.AddRange(nextValues);
                                if (_boolFunc != null)
                                {
                                    if (!_boolFunc(index, indexValues.ToArray()))
                                    {
                                        resultInFileIndex = inFile;
                                        curValues.Clear();
                                        curValues.AddRange(nextValues);
                                    }
                                }
                            }
                            finally
                            {
                                indexValues.Clear();
                                nextValues.Clear();
                            }
                        }
                        for (int outBand = 0; outBand < outBands.Count; outBand++)
                        {
                            rvOutVistor[0].RasterBandsData[outBand][index] = rvInVistor[resultInFileIndex].RasterBandsData[inVisitorBand + outBand][index];
                        }
                        listoutindex.Add(resultInFileIndex);
                    }
                }));
                //执行
                rfr.Excute();

                #region 输入文件准备
                for (int i = 0; i < filenames.Length; i++)
                {
                    string[] anglefiles = GetAngleFilesByMainFile(filenames[i]);
                    for (int j = 0; j < anglefiles.Length; j++)
                    {
                        if (!File.Exists(anglefiles[j]))
                        {
                            continue;
                        }
                        IRasterDataProvider rdtemp = GeoDataDriver.Open(anglefiles[j]) as IRasterDataProvider; ;
                        RasterMaper rmtemp = new RasterMaper(rdtemp, new int[] { 1 });
                        rmsangle.Add(rmtemp);
                        List<int> itemangleindex = new List<int>();
                        itemangleindex.Add(rmsangle.Count - 1);
                        if (!_dicangle.Keys.Contains(i))//新增主键
                        {
                            _dicangle.Add(i, itemangleindex);
                        }
                        else//已经包含主键
                        {
                            _dicangle[i].Add(rmsangle.Count - 1);
                        }
                    }
                }
                #endregion
                #region 输出文件准备
                string[] outanglefiles = GetAngleFilesByMainFile(outFileName);
                for (int i = 0; i < outanglefiles.Length; i++)
                {
                    IRasterDataProvider itemraster = CreateOutRaster(outanglefiles[i], rmsangle.ToArray(), angletype, 1);//角度文件只有一个波段
                    listoutrastermap.Add(new RasterMaper(itemraster, new int[] { 1 }));
                }
                RasterMaper[] fileInsangle = rmsangle.ToArray();
                RasterMaper[] fileOutsangle = listoutrastermap.ToArray();
                #endregion
                //创建处理模型
                RasterProcessModel<UInt16, UInt16> rfrangle = new RasterProcessModel<UInt16, UInt16>(); ;
                rfrangle.SetRaster(fileInsangle, fileOutsangle);
                int totalindex = 0;
                rfrangle.RegisterCalcModel(new RasterCalcHandler<UInt16, UInt16>((rvInVistor, rvOutVistor, aoi) =>
                {
                    int dataLength = rvOutVistor[0].SizeY * rvOutVistor[0].SizeX;
                    for (int i = 0; i < dataLength; i++)
                    {
                        int rasterindex = listoutindex[totalindex + i];//当前点的合成数据所在文件序号
                        List<int> listangleindexs = _dicangle[rasterindex];
                        for (int outangleindex = 0; outangleindex < listangleindexs.Count; outangleindex++)
                        {
                            int _angleindex = listangleindexs[outangleindex];
                            rvOutVistor[outangleindex].RasterBandsData[0][i] = rvInVistor[_angleindex].RasterBandsData[0][i];
                        }
                    }
                    totalindex += dataLength;
                }

            ));
                rfrangle.Excute();
                Console.WriteLine("全部生成完成！");
            }
            finally
            {
                if (outRaster != null)
                {
                    outRaster.Dispose();
                }
                if (listoutrastermap.Count > 0)
                {
                    for (int i = 0; i < listoutrastermap.Count; i++)
                    {
                        listoutrastermap[i].Raster.Dispose();//是否可以这样释放
                    }
                }
            }
            return true;
        }
        /// <summary>
        /// 根据影像文件获取角度文件
        /// </summary>
        /// <param name="rasterfile">影像文件名称</param>
        /// <returns>有效的角度文件</returns>
        private string[] GetAngleFilesByMainFile(string rasterfile)
        {
            List<string> _listitemanglefiles = new List<string>();
            //太阳天顶角
            //太阳方位角
            //卫星天顶角 
            //卫星方位角
            //采用有序循环保证循环顺序不变
            for (int i = 0; i < AngleNames.Length; i++)
            {
                string _itemname = rasterfile.Insert(rasterfile.LastIndexOf('.') + 1, "#.").Replace("#", AngleNames[i]);
                //此处未验证文件是否存在，是为了节省代码创建文件时候获取新文件名
                _listitemanglefiles.Add(_itemname);
            }
            return _listitemanglefiles.ToArray();
        }

        public IRasterDataProvider CreateOutRaster(string outFileName, RasterMaper[] inrasterMaper, enumDataType dataType, int bandCount)
        {
            IRasterDataDriver raster = RasterDataDriver.GetDriverByName("LDF") as IRasterDataDriver;
            GeoDo.RSS.Core.DF.CoordEnvelope outEnv = null;
            foreach (RasterMaper inRaster in inrasterMaper)
            {
                if (outEnv == null)
                    outEnv = inRaster.Raster.CoordEnvelope;
                else
                    outEnv = outEnv.Union(inRaster.Raster.CoordEnvelope);
            }
            float resX = inrasterMaper[0].Raster.ResolutionX;
            float resY = inrasterMaper[0].Raster.ResolutionY;
            int width = (int)(Math.Round(outEnv.Width / resX));
            int height = (int)(Math.Round(outEnv.Height / resY));
            string mapInfo = outEnv.ToMapInfoString(new Size(width, height));
            RasterDataProvider outRaster = raster.Create(outFileName, width, height, bandCount, dataType, mapInfo) as RasterDataProvider;
            return outRaster;
        }

        public string GetFileName(string[] files, string format)
        {
            RasterIdentify rid = new RasterIdentify(files);
            return rid.ToWksFileName(format);
        }

    }
}
