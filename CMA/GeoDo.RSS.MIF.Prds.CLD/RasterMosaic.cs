using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.MIF.Core;
using System.Diagnostics;

namespace GeoDo.RSS.MIF.Prds.CLD
{
    /// <summary>
    /// 目前本类的目标填充值，依据云参数产品固化了，随后这部分会优化掉，填充值应该由外面传入。
    /// </summary>
    public class RasterMosaic
    {
        private Dictionary<string, string> _spatialValues = null;
        private string _fillValues = null;
        private string _sensor = "";

        public RasterMosaic()
        {
        }

        public string Sensor
        {
            set
            {
                _sensor = value;
            }
            get
            {
                return _sensor;
            }
        }

        /// <summary>
        /// 特别值键值对
        /// key：输入值，待替换值
        /// value：输出值，替换后的值
        /// </summary>
        /// <param name="spatialValues"></param>
        public void SetSpatialValues(Dictionary<string, string> spatialValues)
        {
            _spatialValues = spatialValues;
        }

        /// <summary>
        /// 填充值，投影区域外，为输出拼接文件的默认背景填充值
        /// </summary>
        /// <param name="fillValues"></param>
        public void SetDstFillValues(string fillValues)
        {
            _fillValues = fillValues;
        }

        /// <summary>
        /// 将源拼接至目标
        /// 目前默认实现为源和目标数据类型一致
        /// </summary>
        /// <param name="src"></param>
        /// <param name="dst"></param>
        /// <returns></returns>
        public bool Mosaic(IRasterDataProvider src, IRasterDataProvider dst)
        {
            //Stopwatch sw = new Stopwatch();
            //sw.Start();
            try
            {
                RasterMaper[] fileIns = new RasterMaper[] { new RasterMaper(src, new int[] { 1 }) };
                RasterMaper[] fileOuts = new RasterMaper[] { new RasterMaper(dst, new int[] { 1 }) };
                enumDataType datatype = src.DataType;
                switch (datatype)
                {
                    case enumDataType.Byte:
                        return Mosaic<byte>(fileIns, fileOuts, (a, b) => { return a == b; });
                    case enumDataType.Double:
                        return Mosaic<double>(fileIns, fileOuts, (a, b) => { return a == b; });
                    case enumDataType.Float:
                        return Mosaic<float>(fileIns, fileOuts, (a, b) => { return a == b; });
                    case enumDataType.Int16:
                        return Mosaic<Int16>(fileIns, fileOuts, (a, b) => { return a == b; });
                    case enumDataType.Int32:
                        return Mosaic<Int32>(fileIns, fileOuts, (a, b) => { return a == b; });
                    case enumDataType.Int64:
                        return Mosaic<Int64>(fileIns, fileOuts, (a, b) => { return a == b; });
                    case enumDataType.UInt16:
                        return Mosaic<UInt16>(fileIns, fileOuts, (a, b) => { return a == b; });
                    case enumDataType.UInt32:
                        return Mosaic<UInt32>(fileIns, fileOuts, (a, b) => { return a == b; });
                    case enumDataType.UInt64:
                        return Mosaic<UInt64>(fileIns, fileOuts, (a, b) => { return a == b; });
                    case enumDataType.Unknow:
                        break;
                    case enumDataType.Atypism:
                        break;
                    case enumDataType.Bits:
                        break;
                    default:
                        break;
                }
                return true;
            }
            catch (Exception ex)
            {
                LogFactory.WriteLine(_sensor + "MosaicError", src + "," + ex.Message);
                return false;
            }
            finally
            {
                //sw.Stop();
                //LogFactory.WriteLine("拼接时间", sw.ElapsedMilliseconds + "秒");
            }
        }

        /// <summary>
        /// 读取时候，会按照srcs的第一个数据类型来读取数据
        /// </summary>
        /// <param name="srcs"></param>
        /// <param name="dst"></param>
        /// <returns></returns>
        public bool Mosaic(IRasterDataProvider[] srcs, IRasterDataProvider dst)
        {
            //Stopwatch sw = new Stopwatch();
            //sw.Start();
            try
            {
                int bandCount = srcs[0].BandCount;
                RasterMaper[] fileIns = new RasterMaper[srcs.Length];
                int[] bandMap = null;
                for (int i = 0; i < srcs.Length; i++)
                {
                    bandMap = new int[bandCount];
                    for (int b = 0; b < bandCount; b++)
                    {
                        bandMap[b] = b + 1;
                    }
                    fileIns[i] = new RasterMaper(srcs[i], bandMap);
                }
                RasterMaper[] fileOuts = new RasterMaper[] { new RasterMaper(dst, bandMap) };
                enumDataType datatype = srcs[0].DataType;
                switch (datatype)
                {
                    case enumDataType.Byte:
                        return Mosaic<byte>(fileIns, fileOuts, (a, b) => { return a == b; });
                    case enumDataType.Double:
                        return Mosaic<double>(fileIns, fileOuts, (a, b) => { return a == b; });
                    case enumDataType.Float:
                        return Mosaic<float>(fileIns, fileOuts, (a, b) => { return a == b; });
                    case enumDataType.Int16:
                        return Mosaic<Int16>(fileIns, fileOuts, (a, b) => { return a == b; });
                    case enumDataType.Int32:
                        return Mosaic<Int32>(fileIns, fileOuts, (a, b) => { return a == b; });
                    case enumDataType.Int64:
                        return Mosaic<Int64>(fileIns, fileOuts, (a, b) => { return a == b; });
                    case enumDataType.UInt16:
                        return Mosaic<UInt16>(fileIns, fileOuts, (a, b) => { return a == b; });
                    case enumDataType.UInt32:
                        return Mosaic<UInt32>(fileIns, fileOuts, (a, b) => { return a == b; });
                    case enumDataType.UInt64:
                        return Mosaic<UInt64>(fileIns, fileOuts, (a, b) => { return a == b; });
                    case enumDataType.Unknow:
                        break;
                    case enumDataType.Atypism:
                        break;
                    case enumDataType.Bits:
                        break;
                    default:
                        break;
                }
                return true;
            }
            catch (Exception ex)
            {
                LogFactory.WriteLine(_sensor + "MosaicError", dst.fileName + "," + ex.Message);
                return false;
            }
            finally
            {
                //sw.Stop();
                //LogFactory.WriteLine("拼接时间", sw.ElapsedMilliseconds + "秒");
            }
        }

        public static  string GetDefaultNullValue(enumDataType datatype)
        {
            switch (datatype)
            {
                case enumDataType.Byte:
                    return "126";
                case enumDataType.Double:
                    return "-32767";
                case enumDataType.Float:
                    return "-32767";
                case enumDataType.Int16:
                    return "-32767";
                case enumDataType.Int32:
                    return "-32767";
                case enumDataType.Int64:
                    return "-32767";
                case enumDataType.UInt16:
                    return UInt16.MaxValue.ToString();
                case enumDataType.UInt32:
                    return UInt16.MaxValue.ToString();
                case enumDataType.UInt64:
                    return UInt16.MaxValue.ToString();
                case enumDataType.Unknow:
                case enumDataType.Atypism:
                case enumDataType.Bits:
                default:
                    break;
            }
            return null;
        }

        private bool ConverValueToT<T>(object value, out T spValue)
        {
            spValue = default(T);
            try
            {
                IConvertible obj = spValue as IConvertible;
                TypeCode tc = obj.GetTypeCode();
                spValue = (T)Convert.ChangeType(value, tc);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private Dictionary<T, T> ConvertSpatialValues<T>()
        {
            Dictionary<T, T> spValues = null;
            if (_spatialValues != null && _spatialValues.Count != 0)
            {
                spValues = new Dictionary<T, T>();
                T kv;
                T vv;
                foreach (string key in _spatialValues.Keys)
                {
                    ConverValueToT<T>(key, out kv);
                    ConverValueToT<T>(_spatialValues[key], out vv);
                    spValues.Add(kv, vv);
                }
            }
            return spValues;
        }

        private bool Mosaic<T>(RasterMaper[] fileIns, RasterMaper[] fileOuts, Func<T, T, bool> equalityComparer)
        {
            Dictionary<T, T> spValues = ConvertSpatialValues<T>();
            T nullvalue = default(T);
            bool hasnullvalue = false;
            if (!string.IsNullOrWhiteSpace(_fillValues))
            {
                hasnullvalue = ConverValueToT<T>(_fillValues, out nullvalue);
            }
            RasterProcessModel<T, T> rfr = new RasterProcessModel<T, T>();
            rfr.SetRaster(fileIns, fileOuts);
            rfr.RegisterCalcModel(new RasterCalcHandlerFun<T, T>((inRasters, outRasters, aoi) =>
            {
                bool isUpdate = false;
                for (int f = 0; f < inRasters.Length; f++)
                {
                    if (inRasters[f] == null || inRasters[f].RasterBandsData.Length == 0)
                        continue;
                    if (outRasters[0] == null || outRasters[0].RasterBandsData[0].Length == 0)
                        continue;
                    if (inRasters[f].RasterBandsData[0] == null)
                        continue;
                    int bandCount = outRasters[0].RasterBandsData.Length;
                    for (int i = 0; i < outRasters[0].RasterBandsData[0].Length; i++)
                    {
                        //以下为拼接逻辑，目前逻辑是覆盖目标数据。
                        for (int b = 0; b < bandCount; b++)                                       //拼接所有波段
                        {
                            if (equalityComparer(inRasters[f].RasterBandsData[b][i], nullvalue))  //EqualityComparer<T>.Default.Equals(T,T);
                                continue;
                            if (spValues.ContainsKey(inRasters[f].RasterBandsData[b][i]))         //输入值为特殊值
                            {
                                if (equalityComparer(outRasters[0].RasterBandsData[b][i], nullvalue) || spValues.ContainsKey(outRasters[0].RasterBandsData[b][i]))       //输出也为特殊值，通过设置的键值对设置目标值
                                {
                                    outRasters[0].RasterBandsData[b][i] = spValues[inRasters[f].RasterBandsData[b][i]];
                                    isUpdate = true;
                                }
                            }
                            else
                            {
                                outRasters[0].RasterBandsData[b][i] = inRasters[f].RasterBandsData[b][i];//目前是后者数据覆盖目标数据的模式，故此处没有检查是否已处理的逻辑
                                isUpdate = true;
                            }
                        }
                    }
                }
                return isUpdate;    //如果为false，则不会执行对目标数据的更新写入操作
            }));
            if (hasnullvalue)
                rfr.Excute(nullvalue);
            else
                rfr.Excute();
            return true;
        }

        private bool MosaicFun<T>(RasterVirtualVistor<T>[] inRasters, RasterVirtualVistor<T>[] outRasters, int[] aoi)
        {
            //EqualityComparer<T>.Default.Equals(
            return false;
        }

        //以下为一些辅助帮助类
        public static IRasterDataProvider CreateRaster(string outFileName, CoordEnvelope env, float resolutionX, float resolutionY, int bandCount,
            IRasterDataProvider referProvider)
        {
            //int bandCount = referProvider.BandCount;
            //CoordEnvelope outEnv = referProvider.CoordEnvelope;
            //float resX = referProvider.ResolutionX;
            //float resY = referProvider.ResolutionY;
            int width = (int)(Math.Round(env.Width / resolutionX));
            int height = (int)(Math.Round(env.Height / resolutionY));
            Project.ISpatialReference spatialRef = referProvider.SpatialRef;
            enumDataType datatype = referProvider.DataType;
            List<string> options = new List<string>();
            options.Add("INTERLEAVE=BSQ");
            options.Add("VERSION=LDF");
            options.Add("WITHHDR=TRUE");
            options.Add("SPATIALREF=" + spatialRef.ToProj4String());
            options.Add("MAPINFO={" + 1 + "," + 1 + "}:{" + env.MinX + "," + env.MaxY + "}:{" + resolutionX + "," + resolutionY + "}"); //=env.ToMapInfoString(new Size(width, height));
            string hdrfile = HdrFile.GetHdrFileName(referProvider.fileName);
            if (!string.IsNullOrWhiteSpace(hdrfile) && File.Exists(hdrfile))
            {
                HdrFile hdr = HdrFile.LoadFrom(hdrfile);
                if (hdr != null && hdr.BandNames != null)
                    options.Add("BANDNAMES=" + string.Join(",", hdr.BandNames));
            }
            CheckAndCreateDir(Path.GetDirectoryName(outFileName));
            IRasterDataDriver raster = RasterDataDriver.GetDriverByName("LDF") as IRasterDataDriver;
            RasterDataProvider outRaster = raster.Create(outFileName, width, height, bandCount, datatype, options.ToArray()) as RasterDataProvider;
            return outRaster;
        }

        protected static void CheckAndCreateDir(string dir)
        {
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
        }
    }
}
