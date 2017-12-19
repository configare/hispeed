using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.RasterTools;
using GeoDo.RSS.Core.DF;
using GeoDo.RasterProject;
using GeoDo.RSS.Core.VectorDrawing;
using GeoDo.RSS.MIF.Core;
using System.Drawing;
using CodeCell.AgileMap.Core;
using System.IO;
using GeoDo.RSS.BlockOper;
using System.Text.RegularExpressions;

namespace GeoDo.RSS.MIF.Prds.CLD
{
    public class CloudParaFileStaticsAOI : CloudParaFileStatics
    {
        private  GeoDo.RSS.Core.VectorDrawing.AOIContainerLayer _aoiContainer = null;
        private string _VectorAOIName = null;

        #region 长时间序列均值统计(考虑AOI问题)
        public string[] FilesSeriesMeanStatAOI(string[] filesL, int bandNumL, string[] fillvalueL, string outDir, Action<int, string> progressCallback)
        {
            if (StatRegionSet.UseVectorAOIRegion)
            {
                string prdName = "";
                string[] parts = Path.GetFileNameWithoutExtension(filesL[0]).Split('_');
                if (parts.Length > 1)
                    prdName = parts[0];
                PrjEnvelope RegionEnv = StatRegionSet.AOIPrjEnvelope;
                _aoiContainer = StatRegionSet.AoiContainer;
                _VectorAOIName = StatRegionSet.AOIName;
                if (RegionEnv == null || RegionEnv.Height <= 0 || RegionEnv.Width <= 0)
                    throw new ArgumentException("矢量AOI区域设置无效或范围不合法！");
                if (progressCallback != null)
                    progressCallback(-1, "利用AOI区域，开始长时间序列均值统计...");
                Dictionary<DateTime, string> filedate = null;
                List<string[]> resultList = SeriesStaticAOI(filesL, bandNumL, fillvalueL, out filedate,progressCallback);
                return OutputExcels(resultList, filedate, _VectorAOIName, outDir, prdName);
            }
            else
                return null;
        }

        public List<string[]> SeriesStaticAOI(string[] filesL, int bandNumL, string[] fillvalueL, out Dictionary<DateTime, string> filedate, Action<int, string> progressCallback)
        {
            int[] aoiIndex;
            Size fileSize;
            int aoilength;
            int count = 0;
            IRasterDataProvider arrayPrd = null;
            List<string[]> resultList = new List<string[]>();
            filedate = new Dictionary<DateTime, string>();
            DateTime t=DateTime.MinValue;
                            try
                {
            foreach (string file in filesL)
            {
                enumDataType datatype = enumDataType.Unknow;
                using (IRasterDataProvider dataPrd = GeoDataDriver.Open(file) as IRasterDataProvider)
                {
                    datatype = dataPrd.DataType;
                    fileSize = new Size(dataPrd.Width, dataPrd.Height);
                    //创建AOI（基于数据的地理范围）
                    aoiIndex = GetAOI(dataPrd.CoordEnvelope, _aoiContainer, fileSize);
                    if (aoiIndex == null || aoiIndex.Length <= 0)
                        throw new ArgumentException("矢量AOI区域设置无效或范围不合法！");
                }
                aoilength = aoiIndex.Length;
                count = 0;
                arrayPrd = null;
                switch (datatype)
                {
                    case enumDataType.Float:
                        float[][] rasterbandsF = new float[1][];
                        {
                            rasterbandsF[0] = GetHistArrayAOI<float>(new string[] { file }, bandNumL, fileSize.Width, fileSize.Height, datatype, aoiIndex, fillvalueL, out count);
                            arrayPrd = new ArrayRasterDataProvider<float>("Array", rasterbandsF, aoilength, 1);
                        }
                        break;
                    case enumDataType.Int16:
                        short[][] rasterbandsS = new short[1][];
                        {
                            rasterbandsS[0] = GetHistArrayAOI<short>(new string[] { file }, bandNumL, fileSize.Width, fileSize.Height, datatype, aoiIndex, fillvalueL, out count);
                            arrayPrd = new ArrayRasterDataProvider<short>("Array", rasterbandsS, aoilength, 1);
                        }
                        break;
                    case enumDataType.Byte:
                        Byte[][] rasterbandsB = new Byte[1][];
                        {
                            rasterbandsB[0] = GetHistArrayAOI<Byte>(new string[] { file }, bandNumL, fileSize.Width, fileSize.Height, datatype, aoiIndex, fillvalueL, out count);
                            arrayPrd = new ArrayRasterDataProvider<Byte>("Array", rasterbandsB, aoilength, 1);
                        }
                        break;
                    default:
                        throw new ArgumentException("暂不支持" + datatype.ToString() + "类型的统计！");
                }
                    if (arrayPrd.Width == 0 || arrayPrd.Height == 0)
                        throw new ArgumentException("创建待统计数据失败！" + file);
                    double[] values;
                    if (CloudParaFileStatics.ComputeMinMaxAvg(arrayPrd, datatype, new int[] { bandNumL }, null, out values, progressCallback))
                    {
                        string date;
                        GetFileTime(file, out date, out t);
                        if (!filedate.ContainsKey(t))
                            filedate.Add(t, date);
                        resultList.Add(new string[] { date, values[1].ToString("f2") });
                        if (progressCallback != null)
                            progressCallback(-1, Path.GetFileName(file)+"统计完成！");
                    }
            }
            return resultList;
                }
                            finally
                            {
                                if (arrayPrd != null)
                                    arrayPrd.Dispose();
                            }

       }

        #endregion


        #region AOI直方图统计
        public  Dictionary<int, RasterQuickStatResult> FilesHistoStatAOI(string[] files, int[] bands, string[] fillValues, string min=null, string max=null,  Action<int, string> progressCallback=null)
        {
            foreach (int b in bands)
            {
                if (!CheckFiles(files, b))
                    throw new ArgumentException("输入文件错误！band" + b + "大小或类型不一致！");
            }
            if (StatRegionSet.UseVectorAOIRegion)
            {
                PrjEnvelope RegionEnv = StatRegionSet.AOIPrjEnvelope;
                _aoiContainer =StatRegionSet.AoiContainer;
                _VectorAOIName = StatRegionSet.AOIName;
                if (RegionEnv == null || RegionEnv.Height <= 0 || RegionEnv.Width<=0)
                    throw new ArgumentException("矢量AOI区域设置无效或范围不合法！");
            }
            CoordEnvelope outerEnv = null;
            Size fileSize ;
            enumDataType datatype = enumDataType.Unknow;
            using (IRasterDataProvider dataPrd = GeoDataDriver.Open(files[0]) as IRasterDataProvider)
            {
                datatype = dataPrd.DataType;
                outerEnv = dataPrd.CoordEnvelope;
                fileSize = new Size(dataPrd.Width, dataPrd.Height);
            }
            //创建AOI（基于数据的地理范围）
            int[] aoi = GetAOI(outerEnv, _aoiContainer, fileSize);
            if (aoi == null || aoi.Length <= 0)
                throw new ArgumentException("矢量AOI区域设置无效或范围不合法！");
            if (progressCallback != null)
                progressCallback(3, "开始读取波段数据...");
            int aoilength = aoi.Length;
            int count = 0;
            IRasterDataProvider arrayPrd = null;
            switch (datatype)
            {
                case enumDataType.Float:
                    float[][] rasterbandsF = new float[bands.Max()][];
                    if (min != null && max != null)
                    {
                        foreach (int b in bands)
                            rasterbandsF[b - 1] = GetHistArrayBetweenAOI<float>(files, b, fileSize.Width, fileSize.Height, datatype, min, max, aoi, fillValues, out count);
                        arrayPrd = new ArrayRasterDataProvider<float>("Array", rasterbandsF, count, 1);
                    }
                    else
                    {
                        foreach (int b in bands)
                            rasterbandsF[b - 1] = GetHistArrayAOI<float>(files, b, fileSize.Width, fileSize.Height, datatype, aoi, fillValues, out count);
                        arrayPrd = new ArrayRasterDataProvider<float>("Array", rasterbandsF, aoilength, 1);
                    }
                    break;
                case enumDataType.Int16:
                    short[][] rasterbandsS = new short[bands.Max()][];
                    if (min != null && max != null)
                    {
                        foreach (int b in bands)
                            rasterbandsS[b - 1] = GetHistArrayBetweenAOI<short>(files, b, fileSize.Width, fileSize.Height, datatype, min, max, aoi, fillValues, out count);
                        arrayPrd = new ArrayRasterDataProvider<short>("Array", rasterbandsS, count, 1);
                    }
                    else
                    {
                        foreach (int b in bands)
                            rasterbandsS[b - 1] = GetHistArrayAOI<short>(files, b, fileSize.Width, fileSize.Height, datatype, aoi, fillValues, out count);
                        arrayPrd = new ArrayRasterDataProvider<short>("Array", rasterbandsS, aoilength, 1);
                    }
                    break;
                case enumDataType.Byte:
                    Byte[][] rasterbandsB = new Byte[bands.Max()][];
                    if (min != null && max != null)
                    {
                        foreach (int b in bands)
                            rasterbandsB[b - 1] = GetHistArrayBetweenAOI<Byte>(files, b, fileSize.Width, fileSize.Height, datatype, min, max, aoi, fillValues, out count);
                        arrayPrd = new ArrayRasterDataProvider<Byte>("Array", rasterbandsB, count, 1);
                    }
                    else
                    {
                        foreach (int b in bands)
                            rasterbandsB[b - 1] = GetHistArrayAOI<Byte>(files, b, fileSize.Width, fileSize.Height, datatype, aoi, fillValues, out count);
                        arrayPrd = new ArrayRasterDataProvider<Byte>("Array", rasterbandsB, aoilength, 1);
                    }
                    break;
                default:
                    throw new ArgumentException("暂不支持" + datatype.ToString() + "类型的统计！");
            }
            try
            {
                if (arrayPrd.Width == 0 || arrayPrd.Height == 0)
                    throw new ArgumentException("创建待统计数据失败！");
                if (progressCallback != null)
                    progressCallback(5, "开始统计波段数据...");
                return DoStat(arrayPrd, bands, null, progressCallback);
            }
            finally
            {
                if (arrayPrd != null)
                    arrayPrd.Dispose();
            }
        }

        protected T[] GetHistArrayAOI<T>(string[] filesL, int bandNumL, int width, int height, enumDataType datatype, int[] aoi, string[] fillValuesStr,out int count)
        {
            count = 0;
            int totalwidth = aoi.Length;
            switch (datatype)
            {
                case enumDataType.Float:
                    {
                        float[] marixl = new float[totalwidth * filesL.Length];
                        float[] fillValues = GetFillValues<float>(fillValuesStr, enumDataType.Float);
                        float[] oriData;
                        for (int i = 0; i < filesL.Length; i++)
                        {
                            using (IRasterDataProvider dataPrd = GeoDataDriver.Open(filesL[i]) as IRasterDataProvider)
                            {
                                oriData = GetDataValue<float>(dataPrd, bandNumL, width, height, 0, 0);
                                for (int j = 0; j < totalwidth; j++)
                                {
                                    float data = oriData[aoi[j]];
                                    if (fillValues != null && fillValues.LongLength > 0 && fillValues.Contains(data))
                                        continue;
                                    marixl[count++] = data;
                                }
                            }
                        }
                        return marixl as T[];
                    }
                case enumDataType.Int16:
                    {
                        short[] marixl = new short[totalwidth * filesL.Length];
                        short[] fillValues = GetFillValues<short>(fillValuesStr, enumDataType.Int16);
                        short[] oriData;
                        for (int i = 0; i < filesL.Length; i++)
                        {
                            using (IRasterDataProvider dataPrd = GeoDataDriver.Open(filesL[i]) as IRasterDataProvider)
                            {
                                oriData = GetDataValue<short>(dataPrd, bandNumL, width, height, 0, 0);
                                for (int j = 0; j < totalwidth; j++)
                                {
                                    short data = oriData[aoi[j]];
                                    if (fillValues != null && fillValues.LongLength > 0 && fillValues.Contains(data))
                                        continue;
                                    marixl[count++] = data;
                                }
                            }
                        }
                        return marixl as T[];
                    }
                case enumDataType.Byte:
                    {
                        Byte[] marixl = new Byte[totalwidth * filesL.Length];
                        Byte[] fillValues = GetFillValues<Byte>(fillValuesStr, enumDataType.Byte);
                        Byte[] oriData;
                        for (int i = 0; i < filesL.Length; i++)
                        {
                            using (IRasterDataProvider dataPrd = GeoDataDriver.Open(filesL[i]) as IRasterDataProvider)
                            {
                                oriData = GetDataValue<Byte>(dataPrd, bandNumL, width, height, 0, 0);
                                for (int j = 0; j < totalwidth; j++)
                                {
                                    Byte data = oriData[aoi[j]];
                                    if (fillValues != null && fillValues.LongLength > 0 && fillValues.Contains(data))
                                        continue;
                                    marixl[count++] = data;
                                }
                            }
                        }
                        return marixl as T[];
                    }
                default:
                    return null;
            }
        }

        protected T[] GetHistArrayBetweenAOI<T>(string[] filesL, int bandNumL, int width, int height, enumDataType datatype, string min, string max,int[] aoi, string[] fillValuesStr,out int count)
        {
            count = 0;
            int totalwidth = aoi.Length;
            switch (datatype)
            {
                case enumDataType.Float:
                    {
                        float minval = float.Parse(min);
                        float maxval = float.Parse(max);
                        float[] marixl = new float[totalwidth * filesL.Length];
                        float[] fillValues = GetFillValues<float>(fillValuesStr, enumDataType.Float);
                        float[] oriData;
                        for (int i = 0; i < filesL.Length; i++)
                        {
                            oriData = GetFileData<float>(filesL[i], bandNumL, width, height, enumDataType.Float);
                            for (int j = 0; j < totalwidth; j++)
                            {
                                float data = oriData[aoi[j]];
                                if (fillValues != null && fillValues.LongLength > 0 && fillValues.Contains(data))
                                    continue;
                                if (data >= minval && data <= maxval)
                                    marixl[count++] = data;
                            }
                        }
                        return marixl as T[];
                    }
                case enumDataType.Int16:
                    {
                        short minval = short.Parse(min);
                        short maxval = short.Parse(max);
                        short[] marixl = new short[totalwidth * filesL.Length];
                        short[] fillValues = GetFillValues<short>(fillValuesStr, enumDataType.Int16);
                        short[] oriData;
                        for (int i = 0; i < filesL.Length; i++)
                        {
                            oriData = GetFileData<short>(filesL[i], bandNumL, width, height, enumDataType.Int16);
                            for (int j = 0; j < totalwidth; j++)
                            {
                                short data = oriData[aoi[j]];
                                if (fillValues != null && fillValues.LongLength > 0 && fillValues.Contains(data))
                                    continue;
                                if (data >= minval && data <= maxval)
                                    marixl[count++] = data;
                            }
                        }
                        return marixl as T[];
                    }
                case enumDataType.Byte:
                    {
                        Byte[] marixl = new Byte[totalwidth * filesL.Length];
                        Byte minval = Byte.Parse(min);
                        Byte maxval = Byte.Parse(max);
                        Byte[] fillValues = GetFillValues<Byte>(fillValuesStr, enumDataType.Byte);
                        Byte[] oriData;
                        for (int i = 0; i < filesL.Length; i++)
                        {
                            oriData = GetFileData<Byte>(filesL[i],bandNumL, width, height,enumDataType.Byte);
                            for (int j = 0; j < totalwidth; j++)
                            {
                                Byte data = oriData[aoi[j]];
                                if (fillValues != null && fillValues.LongLength > 0 && fillValues.Contains(data))
                                    continue;
                                if (data >= minval && data <= maxval)
                                    marixl[count++] = data;
                            }
                        }
                        return marixl as T[];
                    }
                default:
                    return null;
            }
        }
        #endregion

        protected T[] GetFileData<T>(string filesL, int bandNumL, int width, int height, enumDataType datatype)
        {
            switch (datatype)
            {
                case enumDataType.Float:
                    {
                        float[] oriData;
                        using (IRasterDataProvider dataPrd = GeoDataDriver.Open(filesL) as IRasterDataProvider)
                        {
                            oriData = GetDataValue<float>(dataPrd, bandNumL, width, height, 0, 0);
                        }
                        return oriData as T[];
                    }
                case enumDataType.Int16:
                    {
                        short[] oriData;
                        using (IRasterDataProvider dataPrd = GeoDataDriver.Open(filesL) as IRasterDataProvider)
                        {
                            oriData = GetDataValue<short>(dataPrd, bandNumL, width, height, 0, 0);
                        }
                        return oriData as T[];
                    }
                case enumDataType.Byte:
                    {
                        Byte[] oriData;
                        using (IRasterDataProvider dataPrd = GeoDataDriver.Open(filesL) as IRasterDataProvider)
                        {
                            oriData = GetDataValue<Byte>(dataPrd, bandNumL, width, height, 0, 0);
                        }
                        return oriData as T[];
                    }
                default:
                    return null;
            }
        }

        public static int[] GetAOI(CoordEnvelope fileCorEnv, AOIContainerLayer aoiContainer, Size fileSize)
        {
            int[] retAOI = null,aoi = null;
            VectorAOIGenerator vg = new VectorAOIGenerator();
            Envelope fileEnv = new Envelope(fileCorEnv.MinX, fileCorEnv.MinY, fileCorEnv.MaxX, fileCorEnv.MaxY);
            foreach (object obj in aoiContainer.AOIs)
            {
                try
                {
                    aoi = vg.GetAOI(new ShapePolygon[] { (obj as Feature).Geometry as ShapePolygon },fileEnv,fileSize);
                    if (aoi == null)
                        continue;
                    if (retAOI == null)
                        retAOI = aoi;
                    else
                        retAOI = GeoDo.RSS.RasterTools.AOIHelper.Merge(new int[][] { retAOI, aoi });
                }
                catch (System.Exception ex)
                {
                    continue;
                }
            }
            return retAOI;
        }

        #region AOI相关系数计算
        public double FilesCorrelateStatAOI(string[] filesL, int bandNumL, string[] fillvalueL,string[] filesR, int bandNumR, string[] fillvalueR, Action<int, string> progressCallback, out long scL, out long scR)
        {
            scR = 0; scL = 0;
            //int widthl = 0, heightl = 0, lengthl = filesL.Length;
            //int widthr = 0, heightr = 0, lengthr = filesR.Length;
            //int xoffsetl = 0, yoffsetl = 0, xoffsetr = 0, yoffsetr = 0;
            if (StatRegionSet.UseVectorAOIRegion)
            {
                PrjEnvelope RegionEnv = StatRegionSet.AOIPrjEnvelope;
                _aoiContainer = StatRegionSet.AoiContainer;
                _VectorAOIName = StatRegionSet.AOIName;
                if (RegionEnv == null || RegionEnv.Height <= 0 || RegionEnv.Width <= 0)
                    throw new ArgumentException("矢量AOI区域设置无效或范围不合法！");
                //int[] dataPosL, dataPosR;
                //PrjEnvelope dstmainPrjL = null,dstmainPrjR = null;
                //if (CheckRegionIntersect(filesL[0], RegionEnv, out dataPosL, out dstmainPrjL))
                //{
                //    xoffsetl = dataPosL[0];
                //    yoffsetl = dataPosL[1];
                //    widthl = dataPosL[2];
                //    heightl = dataPosL[3];
                //}
                //if (CheckRegionIntersect(filesR[0], RegionEnv, out dataPosR, out dstmainPrjR))
                //{
                //    xoffsetr = dataPosR[0];
                //    yoffsetr = dataPosR[1];
                //    widthr = dataPosR[2];
                //    heightr = dataPosR[3];
                //}
            }
            if (progressCallback != null)
                progressCallback(3, "开始读取波段数据...");
            #region 获取左场数据
            enumDataType datatype = enumDataType.Unknow;
            CoordEnvelope FileEnvL, FileEnvR = null;
            Size fileSizeL, fileSizeR;
            int[] aoiL, aoiR;
            using (IRasterDataProvider dataPrd = GeoDataDriver.Open(filesL[0]) as IRasterDataProvider)
            {
                if (dataPrd == null)
                    throw new FileLoadException(filesL[0] + "打开失败！");
                datatype = dataPrd.DataType;
                FileEnvL = dataPrd.CoordEnvelope;
                fileSizeL = new Size(dataPrd.Width, dataPrd.Height);
            }
            aoiL = GetAOI(FileEnvL, _aoiContainer, fileSizeL);
            if (aoiL == null || aoiL.Length<=0)
                throw new ArgumentException("矢量AOI区域设置无效或范围不合法，左场AOI区域内点数为0！");
            double[] marixl = GetCorrelateArrayAOI(filesL, bandNumL, fileSizeL,fillvalueL, datatype,aoiL, out scL);
            #endregion
            #region 获取右场数据
            using (IRasterDataProvider dataPrd = GeoDataDriver.Open(filesR[0]) as IRasterDataProvider)
            {
                if (dataPrd == null)
                    throw new FileLoadException(filesR[0] + "打开失败！");
                datatype = dataPrd.DataType;
                FileEnvR = dataPrd.CoordEnvelope;
                fileSizeR = new Size(dataPrd.Width, dataPrd.Height);
            }
            aoiR= GetAOI(FileEnvR, _aoiContainer, fileSizeR);
            if (aoiR == null || aoiL.Length <= 0)
                throw new ArgumentException("矢量AOI区域设置无效或范围不合法！");
            double[] marixr = GetCorrelateArrayAOI(filesR, bandNumR, fileSizeR, fillvalueR, datatype, aoiR, out scR);
            #endregion
            //计算相关系数
            CloudParaStat stat = new CloudParaStat();
            if (progressCallback != null)
                progressCallback(50, "开始计算相关系数...");
            double cor = stat.CalculateCorrelationCoefficient(marixl, marixr, scL, scR);//取样本数最小
            if (progressCallback != null)
                progressCallback(100, "相关系数计算完成！");
            return cor;
        }

        protected double[] GetCorrelateArrayAOI(string[] filesL, int bandNumL,Size fileSize, string[] fillValuesStr, enumDataType dataType, int [] aoi, out long validCount)
        {
            int length = filesL.Length;
            int totalcount = aoi.Length;
            validCount = 0;
            double[] validValue;
            switch (dataType)
            {
                case enumDataType.Float:
                    {
                        validValue = new double[totalcount * length];
                        float[] oriData, fillValues;
                        float data;
                        fillValues = GetFillValues<float>(fillValuesStr, enumDataType.Float);
                        for (int i = 0; i < length; i++)
                        {
                            oriData = GetFileData<float>(filesL[i], bandNumL, fileSize.Width, fileSize.Height, enumDataType.Float);
                            for (int j = 0; j < totalcount; j++)
                            {
                                data = oriData[aoi[j]];
                                if (fillValues != null && fillValues.LongLength > 0 && fillValues.Contains(data))
                                    continue;
                                validValue[validCount++] = data;
                            }
                        }
                        return validValue;
                    }
                case enumDataType.Int16:
                    {
                        validValue = new double[totalcount * length];
                        short[] oriData, fillValues;
                        short data;
                        fillValues = GetFillValues<short>(fillValuesStr, enumDataType.Int16);
                        for (int i = 0; i < length; i++)
                        {
                            oriData = GetFileData<short>(filesL[i], bandNumL, fileSize.Width, fileSize.Height, enumDataType.Int16);
                            for (int j = 0; j < totalcount; j++)
                            {
                                data = oriData[aoi[j]];
                                if (fillValues != null && fillValues.LongLength > 0 && fillValues.Contains(data))
                                    continue;
                                validValue[validCount++] = data;
                            }
                        }
                        return validValue;

                    }
                case enumDataType.Byte:
                    {
                        validValue = new double[totalcount * length];
                        Byte[] oriData, fillValues;
                        Byte data;
                        fillValues = GetFillValues<Byte>(fillValuesStr, enumDataType.Byte);
                        for (int i = 0; i < length; i++)
                        {
                            oriData = GetFileData<Byte>(filesL[i], bandNumL, fileSize.Width, fileSize.Height, enumDataType.Byte);
                            for (int j = 0; j < totalcount; j++)
                            {
                                data = oriData[aoi[j]];
                                if (fillValues != null && fillValues.LongLength > 0 && fillValues.Contains(data))
                                    continue;
                                validValue[validCount++] = data;
                            }
                        }
                        return validValue;
                    }
            }
            return null;
        }
        #endregion

        #region AOISVD
        public string[] FilesSVDStatAOI(string[] filesL, int bandNumL, string[] fillvalueL, string[] filesR, int bandNumR, string[] fillvalueR, double leftRatio, double rightRatio, string outDir, Action<int, string> progressCallback, bool LisMicaps = false, bool RisMicaps = false)
        {
            if (!Directory.Exists(outDir))
                Directory.CreateDirectory(outDir);
            if (StatRegionSet.UseVectorAOIRegion)//使用自定义区域
            {
                PrjEnvelope RegionEnv = null;
                string regionName = "all";
                int outaoiwidthL, outaoiheightL, outaoiwidthR, outaoiheightR;
                int[] outaoiIndexL, outaoiIndexR;
                if (StatRegionSet.UseVectorAOIRegion)
                {
                    RegionEnv = StatRegionSet.AOIPrjEnvelope;
                    _aoiContainer = StatRegionSet.AoiContainer;
                    regionName = StatRegionSet.AOIName;
                }
                PrjEnvelope dstleftPrj, dstrightPrj, dstPrjEnv;
                float reslxL, reslyL, reslxR, reslyR;
                int[] dataPosL, dataPosR;
                if (!CheckRegionIntersect(filesL[0], RegionEnv, out dstleftPrj, out reslxL, out reslyL, out dataPosL))
                    throw new ArgumentException("设置的区域没有可分析的左场数据!");
                if (!CheckRegionIntersect(filesR[0], RegionEnv, out dstrightPrj, out  reslxR, out reslyR, out dataPosR))
                    throw new ArgumentException("设置的区域没有可分析的右场数据!");
                dstPrjEnv = PrjEnvelope.Intersect(dstleftPrj, dstrightPrj);
                if (dstPrjEnv == null || dstPrjEnv.Height == 0 || dstPrjEnv.Width==0)
                    throw new ArgumentException("左右场的AOI区域无交集，无法进行统计分析!");
                if (progressCallback != null)
                    progressCallback(0, "开始读取左场数据...");
                CoordEnvelope subFileEnv = new CoordEnvelope(dstleftPrj.MinX, dstleftPrj.MaxX, dstleftPrj.MinY, dstleftPrj.MaxY);
                StatRegionSet.SubRegionEnvLeft = subFileEnv;
                if (leftRatio < 1)
                    leftRatio = 1;
                Size subAOISize = new Size(dataPosL[2], dataPosL[3]);//AOI外包矩形在file中的大小
                outaoiwidthL = (int)(subAOISize.Width / leftRatio);//AOI外包矩形的输出列数
                outaoiheightL = (int)(subAOISize.Height / leftRatio);//AOI外包矩形的输出行数
                Size outAOISize = new Size(outaoiwidthL, outaoiheightL);//输出文件的大小
                outaoiIndexL = GetAOI(subFileEnv, _aoiContainer, outAOISize);//AOI在输出文件中的Index
                if (outaoiIndexL == null || outaoiIndexL.Length <= 0)
                    throw new ArgumentException("左场重采样AOI区域为空！");
                StatRegionSet.SubRegionOutSizeLeft = outAOISize;
                StatRegionSet.SubRegionOutIndexLeft = outaoiIndexL;
                double[,] marixL = GetTimeSeriesResampledDataFromSubRegionOutIndex(filesL, bandNumL, fillvalueL, outaoiIndexL, dataPosL, outAOISize);
                Size leftSize=new Size(marixL.GetLength(1),marixL.GetLength(0));  //左场数据大小
                if (progressCallback != null)
                    progressCallback(5, "开始读取右场数据...");
                subFileEnv = new CoordEnvelope(dstrightPrj.MinX, dstrightPrj.MaxX, dstrightPrj.MinY, dstrightPrj.MaxY);
                StatRegionSet.SubRegionEnvRight = subFileEnv;
                if (rightRatio < 1)
                    rightRatio = 1;
                subAOISize = new Size(dataPosR[2], dataPosR[3]);//AOI外包矩形在file中的大小
                outaoiwidthR = (int)(subAOISize.Width / rightRatio);//AOI外包矩形的输出列数
                outaoiheightR = (int)(subAOISize.Height / rightRatio);//AOI外包矩形的输出行数
                outAOISize = new Size(outaoiwidthR, outaoiheightR);//输出文件的大小
                outaoiIndexR = GetAOI(subFileEnv, _aoiContainer, outAOISize);//AOI在输出文件中的Index
                if (outaoiIndexR== null || outaoiIndexR.Length <= 0)
                    throw new ArgumentException("右场重采样AOI区域为空！");
                StatRegionSet.SubRegionOutSizeRight = outAOISize;
                StatRegionSet.SubRegionOutIndexRight = outaoiIndexR;
                double[,] marixr = GetTimeSeriesResampledDataFromSubRegionOutIndex(filesL, bandNumL, fillvalueL, outaoiIndexL, dataPosL, outAOISize);
                Size rightSize = new Size(marixr.GetLength(1), marixr.GetLength(0)); //右场数据大小

                if (progressCallback != null)
                    progressCallback(15, "完成数据采样及标准化,开始矩阵计算...");
                CloudParaStat stat = new CloudParaStat();
                return stat.AlglibSVDWithAOI(marixL, marixr, outDir, filesL[0], leftSize, rightSize, progressCallback);
            }
            return null;
        }

        #endregion

        #region AOIEOF
        public string[] FilesEOFStatAOI(string[] files, int bandNum, string bandname, string[] fillvalue, string outDir, Action<int, string> progressCallback, double leftRatio = 1)
        {
                string regionName = "all";
                PrjEnvelope envelope = null;
                PrjEnvelope dstleftPrj = null;
                int outaoiwidth, outaoiheight;
                int[] outaoiIndex;
                if (StatRegionSet.UseVectorAOIRegion)
                {
                    if (leftRatio < 1)
                        leftRatio = 1;
                    envelope = StatRegionSet.AOIPrjEnvelope;
                    _aoiContainer = StatRegionSet.AoiContainer;
                    regionName = StatRegionSet.AOIName;
                    if (leftRatio>=1)//将原始文件降采样，然后再计算aoi,再取点；输出分辨率采用原始文件分辨率；
                    {
                        int[] dataPosL;
                        float rawReslxL=1, rawReslyL=1;//,outReslx,outResly;
                        if (!CheckRegionIntersect(files[0], envelope, out dstleftPrj, out rawReslxL, out rawReslyL, out dataPosL))
                        {
                            throw new ArgumentException("重采样AOI区域为空！");
                        }
                        CoordEnvelope subFileEnv = new CoordEnvelope(dstleftPrj.MinX, dstleftPrj.MaxX, dstleftPrj.MinY, dstleftPrj.MaxY);
                        StatRegionSet.SubRegionEnv = subFileEnv;
                        Size subAOISize = new Size(dataPosL[2], dataPosL[3]);
                        outaoiwidth = (int)(subAOISize.Width / leftRatio);
                        outaoiheight = (int)(subAOISize.Height / leftRatio);
                        //outReslx = rawReslxL * (float)leftRatio;
                        //outResly = rawReslyL * (float)leftRatio;
                        Size outAOISize = new Size(outaoiwidth, outaoiheight);
                        outaoiIndex = GetAOI(subFileEnv, _aoiContainer, outAOISize);
                        if (outaoiIndex == null || outaoiIndex.Length <= 0)
                            throw new ArgumentException("重采样AOI区域为空！");
                        StatRegionSet.SubRegionOutSize = outAOISize;
                        StatRegionSet.SubRegionOutIndex = outaoiIndex;
                        double[,] marix = GetTimeSeriesResampledDataFromSubRegionOutIndex(files,bandNum,fillvalue,outaoiIndex,dataPosL,outAOISize);
                        //string[] result = SaveSVDAnaResult(marix, outDir,  subFileEnv,outAOISize, outaoiIndex, 1, false);
                        if (progressCallback != null)
                            progressCallback(15, "完成数据采样及标准化,开始EOF分解计算...");
                        CloudParaStat stat = new CloudParaStat();
                        return stat.AlglibEOF(marix, outDir, bandname, progressCallback, false, null);
                    }
                    else//采样率==1，不需要抽样，直接使用AOIindex获取数据进行分析
                    {
                        return null;
                    }
                }
                return null;
        }

        /// <summary>
        /// 根据自定义的AOI区域获取指定文件的指定范围的标准化后的时间序列二维数据
        /// 行方向为每个时间序列的空间点，列方向为单一点的时间序列
        /// </summary>
        /// <param name="files">时间序列文件</param>
        /// <param name="bandNum">波段名</param>
        /// <param name="fillvalue">波段填充值</param>
        /// <param name="outaoiIndex">AOI在输出栅格中的Index</param>
        /// <param name="dataPosL">AOI外包矩形在文件原始分辨率矩阵中的偏移量及宽高，xoffset,yoffset,xsize,ysize</param>
        /// <param name="outAOISize">AOI外包矩形在输出栅格中的款高</param>
        /// <returns>标准化后的时间序列二维数据</returns>
        private double[,] GetTimeSeriesResampledDataFromSubRegionOutIndex(string[] files, int bandNum, string[] fillvalue, int[] outaoiIndex, int[] dataPosL, Size outAOISize)
        {
            int xoffset, yoffset;
            xoffset = dataPosL[0];
            yoffset = dataPosL[1];
            Size subAOISize = new Size(dataPosL[2], dataPosL[3]);
            int totalwidth = outaoiIndex.Length;
            double[,] marix = new double[files.Length, totalwidth];
            double[] standardmxi;
            for (int i = 0; i < files.Length; i++)
            {
                //standardmxi为抽点后的AOI区域的标准化的栅格矩阵
                standardmxi = GetResampledDataFromSubRegionOutIndex(files[i], bandNum, fillvalue, outaoiIndex, subAOISize, outAOISize, xoffset, yoffset);
                if (standardmxi == null)
                {
                    //continue;
                    throw new ArgumentException("提取"+files[i]+"的AOI信息失败！");
                }
                for (int k = 0; k < totalwidth; k++)
                {
                    marix[i, k] = standardmxi[k];
                }
            }
            return marix;
        }

        /// <summary>
        /// 提取单文件的AOI区域的数据
        /// </summary>
        /// <param name="file">输入文件</param>
        /// <param name="bandNum">波段</param>
        /// <param name="fillvalue">忽略值</param>
        /// <param name="outaoiIndex">AOI在采样后外包矩形中的Index</param>
        /// <param name="subAOISize">AOI外包矩形原始大小</param>
        /// <param name="outAOISize">AOI外包矩形采样后大小</param>
        /// <param name="xoffset">AOI外包矩形在文件中的xoffset</param>
        /// <param name="yoffset">AOI外包矩形在文件中的yoffset</param>
        /// <returns></returns>
        private double[] GetResampledDataFromSubRegionOutIndex(string file, int bandNum, string[] fillvalue, int[] outaoiIndex, Size subAOISize, Size outAOISize, int xoffset, int yoffset)
        {
            bool isNeedReSample = true;
            if (subAOISize == outAOISize)
                isNeedReSample = false;
            double[] standardmxi;
            enumDataType datatype =enumDataType.Unknow;
            int totalwidth = outaoiIndex.Length;
            using (IRasterDataProvider dataPrd = GeoDataDriver.Open(file) as IRasterDataProvider)
            {
                if(dataPrd==null)
                    return null;
                datatype=dataPrd.DataType;
                switch(datatype)
                {
                    case enumDataType.Byte:
                        {
                            Byte[] outDataB = null;
                            Byte []leftoutData =new Byte [totalwidth];
                            Byte[] fillValues = GetFillValues<Byte>(fillvalue, enumDataType.Byte);
                            Byte []oriData=GetDataValue<Byte>(dataPrd, bandNum, subAOISize.Width, subAOISize.Height, xoffset, yoffset);
                            if (isNeedReSample)
                                AnaliysisDataPreprocess.MedianRead(oriData, subAOISize, outAOISize, out outDataB);
                            else
                                outDataB = oriData;
                            for (int i = 0; i < totalwidth;i++ )
                            {
                                Byte data = outDataB[outaoiIndex[i]];
                                if (fillValues != null && fillValues.LongLength > 0 && fillValues.Contains(data))
                                    continue;
                                leftoutData[i] = data;
                            }
                            AnaliysisDataPreprocess.StandardDeviation(leftoutData, out standardmxi);
                            return standardmxi;
                        }
                    case enumDataType.Int16:
                        {
                            short[] outDataB = null;
                            short[] leftoutData = new short[totalwidth];
                            short[] fillValues = GetFillValues<short>(fillvalue, enumDataType.Byte);
                            short[] oriData = GetDataValue<short>(dataPrd, bandNum, subAOISize.Width, subAOISize.Height, xoffset, yoffset);
                            if (isNeedReSample)
                                AnaliysisDataPreprocess.MedianRead(oriData, subAOISize, outAOISize, out outDataB);
                            else
                                outDataB = oriData;
                            for (int i = 0; i < totalwidth; i++)
                            {
                                short data = outDataB[outaoiIndex[i]];
                                if (fillValues != null && fillValues.LongLength > 0 && fillValues.Contains(data))
                                    continue;
                                leftoutData[i] = data;
                            }
                            AnaliysisDataPreprocess.StandardDeviation(leftoutData, out standardmxi);
                            return standardmxi;
                        }
                    case enumDataType.Float:
                        {
                            float[] outDataB = null;
                            float[] leftoutData = new float[totalwidth];
                            float[] fillValues = GetFillValues<float>(fillvalue, enumDataType.Float);
                            float[] oriData = GetDataValue<float>(dataPrd, bandNum, subAOISize.Width, subAOISize.Height, xoffset, yoffset);
                            if (isNeedReSample)
                                AnaliysisDataPreprocess.MedianRead(oriData, subAOISize, outAOISize, out outDataB);
                            else
                                outDataB = oriData;
                            for (int i = 0; i < totalwidth; i++)
                            {
                                float data = outDataB[outaoiIndex[i]];
                                if (fillValues != null && fillValues.LongLength > 0 && fillValues.Contains(data))
                                    continue;
                                leftoutData[i] = data;
                            }
                            AnaliysisDataPreprocess.StandardDeviation(leftoutData, out standardmxi);
                            return standardmxi;
                        }
                    default:
                        return null;
                }
            }
        }

        protected bool CheckAOIIntersectFileSize(string file, double ratio, out float reslx, out float resly, out int[] dataPos)
        {
            dataPos = null;
            using (IRasterDataProvider dataPrd = GeoDataDriver.Open(file) as IRasterDataProvider)
            {
                CoordEnvelope cordenv = dataPrd.CoordEnvelope;
                reslx = dataPrd.ResolutionX;
                resly = dataPrd.ResolutionY;
                if (cordenv != null)
                {
                    PrjEnvelope filePrj = new PrjEnvelope(cordenv.MinX, cordenv.MaxX, cordenv.MinY, cordenv.MaxY);
                    //dstmainPrj = PrjEnvelope.Intersect(env, filePrj);
                    //if (dstmainPrj != null && dstmainPrj.Width > 0 && dstmainPrj.Height > 0)
                    //{
                    //    int xoffset = (int)((dstmainPrj.MinX - cordenv.MinX) / reslx);
                    //    int yoffset = (int)((cordenv.MaxY - dstmainPrj.MaxY) / resly);
                    //    int width = (int)(dstmainPrj.Width / reslx);
                    //    int height = (int)(dstmainPrj.Height / resly);
                    //    dataPos = new int[4] { xoffset, yoffset, width, height };
                    //    return true;
                    //}
                    throw new ArgumentOutOfRangeException(Path.GetFileName(file), "自定义区域与文件不存在相交区域！");
                }
                throw new ArgumentException(Path.GetFileName(file), "文件坐标范围不可用！");
            }
        }

        #endregion

    }
}
