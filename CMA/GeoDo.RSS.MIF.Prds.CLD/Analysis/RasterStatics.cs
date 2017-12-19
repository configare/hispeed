using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.Core.DF;
using System.Drawing;
using System.IO;
using GeoDo.RSS.Core.VectorDrawing;

namespace GeoDo.RSS.MIF.Prds.CLD
{
    /// <summary>
    /// 目前本类的目标填充值，依据云参数产品固化了，随后这部分会优化掉，填充值应该由外面传入。
    /// </summary>
    public class RasterStatics
    {
        private Dictionary<string, string> _spatialValues = null;
        private string _fillValues = null;
        private string _invalidValues = "0";
        private double[] _InvalidValuesArray = null;

        public RasterStatics()
        {
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
        /// 无效值，投影区域内
        /// </summary>
        /// <param name="invalidValues"></param>
        public void SetDstInvalidValues(string invalidValues)
        {
            _invalidValues = invalidValues;
        }

        public void SetDstInvalidValues(double[] invalidValues)
        {
            _InvalidValuesArray = invalidValues;
        }

        public bool PeriodicMaxStat(string[] filenames, string outMaxname, Action<int, string> progressCallback = null)
        {
            List<RasterMaper> rms = new List<RasterMaper>();
            IRasterDataProvider outRaster = null; 
            IRasterDataProvider inRaster = null;
            IRasterDataProvider refRaster = null;
            try
            {
                CoordEnvelope outEnv = null;
                int bandcount = 0;
                float xresl = 0;
                float yresl = 0;
                enumDataType datatype = enumDataType.Unknow;
                for (int i = 0; i < filenames.Length; i++)
                {
                    try
                    {
                        inRaster = GeoDataDriver.Open(filenames[i]) as IRasterDataProvider;
                        if (inRaster == null)
                            throw new FileLoadException("文件打开失败或为空！");
                        if (outEnv == null && inRaster.CoordEnvelope != null)
                        {
                            outEnv = inRaster.CoordEnvelope;
                            refRaster = inRaster;
                            bandcount = inRaster.BandCount;
                            xresl = inRaster.ResolutionX;
                            yresl = inRaster.ResolutionY;
                            datatype = inRaster.DataType;
                        }
                        rms.Add(new RasterMaper(inRaster, new int[] { 1 }));
                    }
                    catch (System.Exception ex)
                    {
                        if (progressCallback != null)
                            progressCallback(-5, "\t\t\t\t失败" + ex.Message + ",未参与周期合成！" + filenames[i]);
                    }
                }
                if (bandcount == 0 || xresl == 0 || yresl == 0 || outEnv == null)
                {
                    outEnv = inRaster.CoordEnvelope;
                    refRaster = inRaster;
                    bandcount = inRaster.BandCount;
                    xresl = inRaster.ResolutionX;
                    yresl = inRaster.ResolutionY;
                    datatype = inRaster.DataType;
                }
                if (bandcount == 0 || xresl == 0 || yresl == 0)
                    return false;
                RasterMaper[] fileIns = rms.ToArray();
                if (File.Exists(outMaxname))
                {
                    File.Delete(outMaxname);
                }
                outRaster = RasterMosaic.CreateRaster(outMaxname, outEnv, xresl, yresl, bandcount, refRaster);
                if (progressCallback != null)
                    progressCallback(-1, "\t\t\t\t创建输出文件成功！" + Path.GetFileName(outMaxname));
                SetDefaultInvalidValues(outRaster);
                if (progressCallback != null)
                    progressCallback(-1, "\t\t\t\t输出文件填充无效值成功！" + Path.GetFileName(outMaxname));
                RasterMaper[] fileOuts = new RasterMaper[] { new RasterMaper(outRaster, new int[] { 1 }) };
                switch (datatype)
                {
                    case enumDataType.Byte:
                        return PeriodicMaxStatByte(fileIns, fileOuts, progressCallback);
                    case enumDataType.Float:
                        return PeriodicMaxStatFloat(fileIns, fileOuts, progressCallback);
                    case enumDataType.Int16:
                        return PeriodicMaxStatINT16(fileIns, fileOuts, progressCallback);
                    default:
                        break;
                }
                return true;
            }
            catch (Exception ex)
            {
                if (progressCallback != null)
                    progressCallback(-5, "\t\t\t\t计算出错" + ex.Message+","+outMaxname);
                if (outRaster != null)
                    outRaster.Dispose();
                if (File.Exists(outMaxname))
                    File.Delete(outMaxname);
                return false;
            }
            finally
            {
                if (inRaster != null)
                    inRaster.Dispose();
                if (refRaster != null)
                    refRaster.Dispose();
                if (outRaster != null)
                    outRaster.Dispose();
            }

        }

        public bool PeriodicMinStat(string[] filenames, string outMinname, Action<int, string> progressCallback = null)
        {
            List<RasterMaper> rms = new List<RasterMaper>();
            IRasterDataProvider outRaster = null;
            IRasterDataProvider inRaster = null;
            IRasterDataProvider refRaster = null;
            try
            {
                CoordEnvelope outEnv = null;
                int bandcount = 0;
                float xresl = 0;
                float yresl = 0;
                enumDataType datatype = enumDataType.Unknow;
                for (int i = 0; i < filenames.Length; i++)
                {
                    try
                    {
                        inRaster = GeoDataDriver.Open(filenames[i]) as IRasterDataProvider;
                        if (inRaster == null)
                            throw new FileLoadException("文件打开失败或为空！");
                        if (outEnv == null && inRaster.CoordEnvelope != null)
                        {
                            outEnv = inRaster.CoordEnvelope;
                            refRaster = inRaster;
                            bandcount = inRaster.BandCount;
                            xresl = inRaster.ResolutionX;
                            yresl = inRaster.ResolutionY;
                            datatype = inRaster.DataType;
                        }
                        rms.Add(new RasterMaper(inRaster, new int[] { 1 }));
                    }
                    catch (System.Exception ex)
                    {
                        if (progressCallback != null)
                            progressCallback(-5, "\t\t\t\t失败" + ex.Message + ",未参与周期合成！" + filenames[i]);
                    }
                }
                if (bandcount == 0 || xresl == 0 || yresl == 0 || outEnv == null)
                {
                    outEnv = inRaster.CoordEnvelope;
                    refRaster = inRaster;
                    bandcount = inRaster.BandCount;
                    xresl = inRaster.ResolutionX;
                    yresl = inRaster.ResolutionY;
                    datatype = inRaster.DataType;
                }
                if (bandcount == 0 || xresl == 0 || yresl == 0)
                    return false;
                RasterMaper[] fileIns = rms.ToArray();
                if (File.Exists(outMinname))
                {
                    File.Delete(outMinname);
                }
                outRaster = RasterMosaic.CreateRaster(outMinname, outEnv, xresl, yresl, bandcount, refRaster);
                if (progressCallback != null)
                    progressCallback(-1, "\t\t\t\t创建输出文件成功！" + Path.GetFileName(outMinname));
                SetDefaultInvalidValues(outRaster);
                if (progressCallback != null)
                    progressCallback(-1, "\t\t\t\t输出文件填充无效值成功！" + Path.GetFileName(outMinname));
                RasterMaper[] fileOuts = new RasterMaper[] { new RasterMaper(outRaster, new int[] { 1 }) };
                switch (datatype)
                {
                    case enumDataType.Byte:
                        return PeriodicMinStatByte(fileIns, fileOuts, progressCallback);
                    case enumDataType.Float:
                        return PeriodicMinStatFloat(fileIns, fileOuts, progressCallback);
                    case enumDataType.Int16:
                        return PeriodicMinStatINT16(fileIns, fileOuts, progressCallback);
                    default:
                        break;
                }
                return true;
            }
            catch (Exception ex)
            {
                if (progressCallback != null )
                    progressCallback(-5, "\t\t\t\t计算出错" + ex.Message + "," + outMinname);
                if (outRaster != null)
                    outRaster.Dispose();
                if (File.Exists(outMinname))
                    File.Delete(outMinname);
                return false;
            }
            finally
            {
                if (inRaster != null)
                    inRaster.Dispose();
                if (refRaster != null)
                    refRaster.Dispose();
                if (outRaster != null)
                    outRaster.Dispose();
            }
        }

        public bool PeriodicAvgStat(string[] filenames, string outAvgname, object aoi = null, Action<int, string> progressCallback = null)
        {
            List<RasterMaper> rms = new List<RasterMaper>();
            IRasterDataProvider outRaster = null;
            IRasterDataProvider inRaster = null;
            IRasterDataProvider refRaster = null;
            try
            {
                CoordEnvelope outEnv = null;
                int bandcount = 0;
                float xresl = 0;
                float yresl = 0;
                enumDataType datatype = enumDataType.Unknow;
                for (int i = 0; i < filenames.Length; i++)
                {
                    try
                    {
                        inRaster = GeoDataDriver.Open(filenames[i]) as IRasterDataProvider;
                        if (inRaster == null)
                            throw new FileLoadException("文件打开失败或为空！");
                        if (outEnv == null && inRaster.CoordEnvelope != null)
                        {
                            outEnv = inRaster.CoordEnvelope;
                            refRaster = inRaster;
                            bandcount = inRaster.BandCount;
                            xresl = inRaster.ResolutionX;
                            yresl = inRaster.ResolutionY;
                            datatype = inRaster.DataType;
                        }
                        rms.Add(new RasterMaper(inRaster, new int[] { 1 }));
                    }
                    catch (System.Exception ex)
                    {
                        if (progressCallback != null)
                            progressCallback(-5, "\t\t\t\t失败" + ex.Message + ",未参与周期合成！" + filenames[i]);
                    }
                }
                if (bandcount == 0 || xresl == 0 || yresl == 0 || outEnv == null)
                {
                    outEnv = inRaster.CoordEnvelope;
                    refRaster = inRaster;
                    bandcount = inRaster.BandCount;
                    xresl = inRaster.ResolutionX;
                    yresl = inRaster.ResolutionY;
                    datatype = inRaster.DataType;
                }
                if (bandcount == 0 || xresl == 0 || yresl == 0)
                    return false;
                if (File.Exists(outAvgname))
                {
                    File.Delete(outAvgname);
                }
                RasterMaper[] fileIns = rms.ToArray();
                outRaster = RasterMosaic.CreateRaster(outAvgname, outEnv, xresl, yresl, bandcount, refRaster);
                if (progressCallback != null)
                    progressCallback(-1, "\t\t\t\t创建输出文件成功！" );//+ Path.GetFileName(outAvgname)
                SetDefaultInvalidValues(outRaster);
                //if (progressCallback != null)
                //    progressCallback(-1, "\t\t\t\t输出文件填充无效值成功！" );//+ Path.GetFileName(outAvgname)
                RasterMaper[] fileOuts = new RasterMaper[] { new RasterMaper(outRaster, new int[] { 1 }) };
                switch (datatype)
                {
                    case enumDataType.Byte:
                        return PeriodicAvgStatByte(fileIns, fileOuts,aoi, progressCallback);
                    case enumDataType.Float:
                        return PeriodicAvgStatFloat(fileIns, fileOuts,aoi, progressCallback);
                    case enumDataType.Int16:
                        return PeriodicAvgStatINT16(fileIns, fileOuts, aoi,progressCallback);
                    default:
                        break;
                }
                return true;
            }
            catch (Exception ex)
            {
                if (progressCallback != null)
                    progressCallback(-1, "\t\t\t\t计算出错" + ex.Message );//+ "," + outAvgname
                if (outRaster != null)
                    outRaster.Dispose();
                if (File.Exists(outAvgname))
                    File.Delete(outAvgname);
                return false;
            }
            finally
            {
                if (inRaster != null)
                    inRaster.Dispose();
                if (refRaster != null)
                    refRaster.Dispose();
                if (outRaster != null)
                    outRaster.Dispose();
            }
        }

        #region Int16类型的最大值、最小值、平均值计算
        private bool PeriodicMaxStatINT16(RasterMaper[] fileIns, RasterMaper[] fileOuts, Action<int, string> progressCallback = null)
        {
            Int16 fillValues = 0; 
            bool hasnullvalue = false;
            if (!string.IsNullOrWhiteSpace(_fillValues))
            {
                hasnullvalue = Int16.TryParse(_fillValues, out fillValues);
            }
            Int16 invalidvalue = 0;
            bool hasinvalidvalue = false;
            if (!string.IsNullOrWhiteSpace(_invalidValues))
            {
                hasinvalidvalue = Int16.TryParse(_invalidValues, out invalidvalue);
            }

            RasterProcessModel<Int16, Int16> rfr = new RasterProcessModel<Int16, Int16>();
            rfr.SetRaster(fileIns, fileOuts);
            rfr.RegisterCalcModel(new RasterCalcHandlerFun<Int16, Int16>((inRasters, outRasters, aoi) =>
            {
                bool isUpdate = false;
                if (inRasters[0] == null || inRasters[0].RasterBandsData.Length == 0)
                    return false;
                if (outRasters[0] == null || outRasters[0].RasterBandsData[0].Length == 0)
                    return false;
                if (inRasters[0].RasterBandsData[0] == null)
                    return false;
                int bandCount = outRasters[0].RasterBandsData.Length;
                for (int l = 0; l < inRasters.Length;l++ )
                {
                    if (progressCallback != null)
                        progressCallback(-1, "\t\t\t\t共" + inRasters.Length + "个文件,开始合成第" + (l + 1) + "个...,文件:" + Path.GetFileName(inRasters[l].Raster.fileName));
                    for (int i = 0; i < outRasters[0].RasterBandsData[0].Length; i++)
                    {
                        for (int b = 0; b < bandCount; b++)                                         //所有波段
                        {
                            if (inRasters[l].RasterBandsData[b][i].Equals(fillValues) || inRasters[l].RasterBandsData[b][i].Equals(invalidvalue))
                                continue;
                            if (outRasters[0].RasterBandsData[b][i].Equals(invalidvalue))       //输出也为特殊值，通过设置的键值对设置目标值
                            {
                                outRasters[0].RasterBandsData[b][i] = inRasters[l].RasterBandsData[b][i];
                                isUpdate = true;
                            }
                            else if (outRasters[0].RasterBandsData[b][i] < inRasters[l].RasterBandsData[b][i])
                            {
                                outRasters[0].RasterBandsData[b][i] = inRasters[l].RasterBandsData[b][i];//目前是后者数据覆盖目标数据的模式
                                isUpdate = true;
                            }
                        }
                    }

                }
                return isUpdate;    //如果为false，则不会执行对目标数据的更新写入操作
            }));
            //if (hasnullvalue)
            //    rfr.Excute(nullvalue);
            //else
                rfr.Excute();
            return true;
        }

        private bool PeriodicMinStatINT16(RasterMaper[] fileIns, RasterMaper[] fileOuts, Action<int, string> progressCallback = null)
        {
            Int16 fillValues = 0;
            bool hasnullvalue = false;
            if (!string.IsNullOrWhiteSpace(_fillValues))
            {
                hasnullvalue = Int16.TryParse(_fillValues, out fillValues);
            }
            Int16 invalidvalue = 0;
            bool hasinvalidvalue = false;
            if (!string.IsNullOrWhiteSpace(_invalidValues))
            {
                hasinvalidvalue = Int16.TryParse(_invalidValues, out invalidvalue);
            }
            RasterProcessModel<Int16, Int16> rfr = new RasterProcessModel<Int16, Int16>();
            rfr.SetRaster(fileIns, fileOuts);
            rfr.RegisterCalcModel(new RasterCalcHandlerFun<Int16, Int16>((inRasters, outRasters, aoi) =>
            {
                bool isUpdate = false;
                if (inRasters[0] == null || inRasters[0].RasterBandsData.Length == 0)
                    return false;
                if (outRasters[0] == null || outRasters[0].RasterBandsData[0].Length == 0)
                    return false;
                if (inRasters[0].RasterBandsData[0] == null)
                    return false;
                int bandCount = outRasters[0].RasterBandsData.Length;
                for (int l = 0; l < inRasters.Length; l++)
                {
                    if (progressCallback != null)
                        progressCallback(-1, "\t\t\t\t共" + inRasters.Length + "个文件,开始合成第" + (l + 1) + "个...,文件:" + Path.GetFileName(inRasters[l].Raster.fileName));
                    for (int i = 0; i < outRasters[0].RasterBandsData[0].Length; i++)
                    {
                        for (int b = 0; b < bandCount; b++)                                         //所有波段
                        {
                            if (inRasters[l].RasterBandsData[b][i].Equals(fillValues) || inRasters[l].RasterBandsData[b][i].Equals(invalidvalue))
                                continue;
                            if (outRasters[0].RasterBandsData[b][i].Equals(invalidvalue))       //输出也为特殊值，通过设置的键值对设置目标值
                            {
                                outRasters[0].RasterBandsData[b][i] = inRasters[l].RasterBandsData[b][i];
                                isUpdate = true;
                            }
                            else if (outRasters[0].RasterBandsData[b][i] > inRasters[l].RasterBandsData[b][i])
                            {
                                outRasters[0].RasterBandsData[b][i] = inRasters[l].RasterBandsData[b][i];//目前是后者数据覆盖目标数据的模式
                                isUpdate = true;
                            }
                        }
                    }

                }
                return isUpdate;    //如果为false，则不会执行对目标数据的更新写入操作
            }));
            rfr.Excute();
            return true;
        }

        private bool PeriodicAvgStatINT16(RasterMaper[] fileIns, RasterMaper[] fileOuts, object inaoi = null, Action<int, string> progressCallback = null)
        {
            Int16 fillValues = 0;
            bool hasnullvalue = false;
            if (!string.IsNullOrWhiteSpace(_fillValues))
            {
                hasnullvalue = Int16.TryParse(_fillValues, out fillValues);
            }
            Int16 invalidvalue = 0;
            bool hasinvalidvalue = false;
            if (!string.IsNullOrWhiteSpace(_invalidValues))
            {
                hasinvalidvalue = Int16.TryParse(_invalidValues, out invalidvalue);
            }
            RasterProcessModel<Int16, Int16> rfr = new RasterProcessModel<Int16, Int16>();
            rfr.SetRaster(fileIns, fileOuts);
            if ((inaoi) != null)
            {
                rfr.SetCustomAOI("自定义", inaoi);
            }
            rfr.RegisterCalcModel(new RasterCalcHandlerFun<Int16, Int16>((inRasters, outRasters, aoi) =>
            {
                bool isUpdate = false;
                if (inRasters[0] == null || inRasters[0].RasterBandsData.Length == 0)
                    return false;
                if (outRasters[0] == null || outRasters[0].RasterBandsData[0].Length == 0)
                    return false;
                if (inRasters[0].RasterBandsData[0] == null)
                    return false;
                int bandCount = outRasters[0].RasterBandsData.Length;
                for (int b = 0; b < bandCount; b++)                                         //所有波段
                {
                    if (progressCallback != null)
                        progressCallback(-1, "\t\t\t\t共" + bandCount + "个波段,开始合成第" + (b + 1) + "个波段...");
                    double[] sumValues = new double[outRasters[0].RasterBandsData[0].Length];
                    Int16[] sumCount = new short[outRasters[0].RasterBandsData[0].Length];
                    for (int l = 0; l < inRasters.Length; l++)
                    {
                        if (progressCallback != null)
                            progressCallback(-1, "\t\t\t\t共" + inRasters.Length + "个文件,开始合成第" + (l + 1) + "个...,文件:" + Path.GetFileName(inRasters[l].Raster.fileName));
                        if (aoi == null)
                        {
                            for (int i = 0; i < outRasters[0].RasterBandsData[0].Length; i++)
                            {
                                if (inRasters[l].RasterBandsData[b][i].Equals(fillValues) || inRasters[l].RasterBandsData[b][i].Equals(invalidvalue))
                                    continue;
                                else if (!IsValidValue<Int16>(inRasters[l].RasterBandsData[b][i], enumDataType.Int16))
                                    continue;
                                //else
                                {
                                    sumValues[i] += inRasters[l].RasterBandsData[b][i];//目前是后者数据覆盖目标数据的模式
                                    sumCount[i] += 1;
                                    isUpdate = true;
                                }
                            }
                        }
                        else
                        {
                            for (int k = 0; k < aoi.Length; k++)
                            {
                                int i = aoi[k];
                                if (inRasters[l].RasterBandsData[b][i].Equals(fillValues) || inRasters[l].RasterBandsData[b][i].Equals(invalidvalue))
                                    continue;
                                else if (!IsValidValue<Int16>(inRasters[l].RasterBandsData[b][i], enumDataType.Int16))
                                    continue;
                                //else
                                {
                                    sumValues[i] += inRasters[l].RasterBandsData[b][i];//目前是后者数据覆盖目标数据的模式
                                    sumCount[i] += 1;
                                    isUpdate = true;
                                }
                            }
                        }
                    }
                    for (int i = 0; i < outRasters[0].RasterBandsData[0].Length; i++)
                    {
                        if (sumCount[i]!=0)
                        {
                            outRasters[0].RasterBandsData[b][i] = (Int16)(sumValues[i] / sumCount[i]);
                        }
                    }
                }
                return isUpdate;    //如果为false，则不会执行对目标数据的更新写入操作
            }));
            rfr.Excute();
            return true;
        }

        #endregion

        #region byte类型的最大值、最小值、平均值计算
        private bool PeriodicMaxStatByte(RasterMaper[] fileIns, RasterMaper[] fileOuts, Action<int, string> progressCallback = null)
        {
            Byte fillValues = 0;
            bool hasnullvalue = false;
            if (!string.IsNullOrWhiteSpace(_fillValues))
            {
                hasnullvalue = Byte.TryParse(_fillValues, out fillValues);
            }
            Byte invalidvalue = 0;
            bool hasinvalidvalue = false;
            if (!string.IsNullOrWhiteSpace(_invalidValues))
            {
                hasinvalidvalue = Byte.TryParse(_invalidValues, out invalidvalue);
            }

            RasterProcessModel<Byte, Byte> rfr = new RasterProcessModel<Byte, Byte>();
            rfr.SetRaster(fileIns, fileOuts);
            rfr.RegisterCalcModel(new RasterCalcHandlerFun<Byte, Byte>((inRasters, outRasters, aoi) =>
            {
                bool isUpdate = false;
                if (inRasters[0] == null || inRasters[0].RasterBandsData.Length == 0)
                    return false;
                if (outRasters[0] == null || outRasters[0].RasterBandsData[0].Length == 0)
                    return false;
                if (inRasters[0].RasterBandsData[0] == null)
                    return false;
                int bandCount = outRasters[0].RasterBandsData.Length;
                for (int l = 0; l < inRasters.Length; l++)
                {
                    if (progressCallback != null)
                        progressCallback(-1, "\t\t\t\t共" + inRasters.Length + "个文件,开始合成第" + (l + 1) + "个...,文件:" + Path.GetFileName(inRasters[l].Raster.fileName));
                    for (int i = 0; i < outRasters[0].RasterBandsData[0].Length; i++)
                    {
                        for (int b = 0; b < bandCount; b++)                                         //所有波段
                        {
                            if (inRasters[l].RasterBandsData[b][i].Equals(fillValues) || inRasters[l].RasterBandsData[b][i].Equals(invalidvalue))
                                continue;
                            if (outRasters[0].RasterBandsData[b][i].Equals(invalidvalue))       //输出也为特殊值，通过设置的键值对设置目标值
                            {
                                outRasters[0].RasterBandsData[b][i] = inRasters[l].RasterBandsData[b][i];
                                isUpdate = true;
                            }
                            else if (outRasters[0].RasterBandsData[b][i] < inRasters[l].RasterBandsData[b][i])
                            {
                                outRasters[0].RasterBandsData[b][i] = inRasters[l].RasterBandsData[b][i];//目前是后者数据覆盖目标数据的模式
                                isUpdate = true;
                            }
                        }
                    }

                }
                return isUpdate;    //如果为false，则不会执行对目标数据的更新写入操作
            }));
            //if (hasnullvalue)
            //    rfr.Excute(nullvalue);
            //else
            rfr.Excute();
            return true;
        }

        private bool PeriodicMinStatByte(RasterMaper[] fileIns, RasterMaper[] fileOuts, Action<int, string> progressCallback = null)
        {
            Byte fillValues = 0;
            bool hasnullvalue = false;
            if (!string.IsNullOrWhiteSpace(_fillValues))
            {
                hasnullvalue = Byte.TryParse(_fillValues, out fillValues);
            }
            Byte invalidvalue = 0;
            bool hasinvalidvalue = false;
            if (!string.IsNullOrWhiteSpace(_invalidValues))
            {
                hasinvalidvalue = Byte.TryParse(_invalidValues, out invalidvalue);
            }
            RasterProcessModel<Byte, Byte> rfr = new RasterProcessModel<Byte, Byte>();
            rfr.SetRaster(fileIns, fileOuts);
            rfr.RegisterCalcModel(new RasterCalcHandlerFun<Byte, Byte>((inRasters, outRasters, aoi) =>
            {
                bool isUpdate = false;
                if (inRasters[0] == null || inRasters[0].RasterBandsData.Length == 0)
                    return false;
                if (outRasters[0] == null || outRasters[0].RasterBandsData[0].Length == 0)
                    return false;
                if (inRasters[0].RasterBandsData[0] == null)
                    return false;
                int bandCount = outRasters[0].RasterBandsData.Length;
                for (int l = 0; l < inRasters.Length; l++)
                {
                    if (progressCallback != null)
                        progressCallback(-1, "\t\t\t\t共" + inRasters.Length + "个文件,开始合成第" + (l + 1) + "个...,文件:" + Path.GetFileName(inRasters[l].Raster.fileName));
                    for (int i = 0; i < outRasters[0].RasterBandsData[0].Length; i++)
                    {
                        for (int b = 0; b < bandCount; b++)                                         //所有波段
                        {
                            if (inRasters[l].RasterBandsData[b][i].Equals(fillValues) || inRasters[l].RasterBandsData[b][i].Equals(invalidvalue))
                                continue;
                            if (outRasters[0].RasterBandsData[b][i].Equals(invalidvalue))       //输出也为特殊值，通过设置的键值对设置目标值
                            {
                                outRasters[0].RasterBandsData[b][i] = inRasters[l].RasterBandsData[b][i];
                                isUpdate = true;
                            }
                            else if (outRasters[0].RasterBandsData[b][i] > inRasters[l].RasterBandsData[b][i])
                            {
                                outRasters[0].RasterBandsData[b][i] = inRasters[l].RasterBandsData[b][i];//目前是后者数据覆盖目标数据的模式
                                isUpdate = true;
                            }
                        }
                    }

                }
                return isUpdate;    //如果为false，则不会执行对目标数据的更新写入操作
            }));
            rfr.Excute();
            return true;
        }

        private bool PeriodicAvgStatByte(RasterMaper[] fileIns, RasterMaper[] fileOuts, object inaoi = null, Action<int, string> progressCallback = null)
        {
            Byte fillValues = 0;
            bool hasnullvalue = false;
            if (!string.IsNullOrWhiteSpace(_fillValues))
            {
                hasnullvalue = Byte.TryParse(_fillValues, out fillValues);
            }
            Byte invalidvalue = 0;
            bool hasinvalidvalue = false;
            if (!string.IsNullOrWhiteSpace(_invalidValues))
            {
                hasinvalidvalue = Byte.TryParse(_invalidValues, out invalidvalue);
            }
            RasterProcessModel<Byte, Byte> rfr = new RasterProcessModel<Byte, Byte>();
            rfr.SetRaster(fileIns, fileOuts);
            if ((inaoi  )!= null)
            {
                rfr.SetCustomAOI("自定义", inaoi);
            }
            rfr.RegisterCalcModel(new RasterCalcHandlerFun<Byte, Byte>((inRasters, outRasters, aoi) =>
            {
                bool isUpdate = false;
                if (inRasters[0] == null || inRasters[0].RasterBandsData.Length == 0)
                    return false;
                if (outRasters[0] == null || outRasters[0].RasterBandsData[0].Length == 0)
                    return false;
                if (inRasters[0].RasterBandsData[0] == null)
                    return false;
                int bandCount = outRasters[0].RasterBandsData.Length;
                for (int b = 0; b < bandCount; b++)                                         //所有波段
                {
                    if (progressCallback != null)
                        progressCallback(-1, "\t\t\t\t共" + bandCount + "个波段,开始合成第" + (b + 1) + "个波段...");
                    double[] sumValues = new double[outRasters[0].RasterBandsData[0].Length];
                    Int16[] sumCount = new short[outRasters[0].RasterBandsData[0].Length];
                    for (int l = 0; l < inRasters.Length; l++)
                    {
                        if (progressCallback != null)
                            progressCallback(-1, "\t\t\t\t共" + inRasters.Length + "个文件,开始合成第" + (l + 1) + "个...,文件:" + Path.GetFileName(inRasters[l].Raster.fileName));
                        if (aoi==null)
                        {
                            for (int i = 0; i < outRasters[0].RasterBandsData[0].Length; i++)
                            {
                                if (inRasters[l].RasterBandsData[b][i].Equals(fillValues) || inRasters[l].RasterBandsData[b][i].Equals(invalidvalue))
                                    continue;
                                else if (!IsValidValue<byte>(inRasters[l].RasterBandsData[b][i],enumDataType.Byte))
                                    continue;
                                //else
                                {
                                    sumValues[i] += inRasters[l].RasterBandsData[b][i];//目前是后者数据覆盖目标数据的模式
                                    sumCount[i] += 1;
                                    isUpdate = true;
                                }
                            }
                        } 
                        else
                        {
                            for (int k = 0; k < aoi.Length; k++)
                            {
                                int i = aoi[k];
                                if (inRasters[l].RasterBandsData[b][i].Equals(fillValues) || inRasters[l].RasterBandsData[b][i].Equals(invalidvalue))
                                    continue;
                                else if (!IsValidValue<byte>(inRasters[l].RasterBandsData[b][i], enumDataType.Byte))
                                    continue;
                                //else
                                {
                                    sumValues[i] += inRasters[l].RasterBandsData[b][i];//目前是后者数据覆盖目标数据的模式
                                    sumCount[i] += 1;
                                    isUpdate = true;
                                }
                            }
                        }
                    }
                    for (int i = 0; i < outRasters[0].RasterBandsData[0].Length; i++)
                    {
                        if (sumCount[i] != 0)
                        {
                            outRasters[0].RasterBandsData[b][i] = (Byte)(sumValues[i] / sumCount[i]);
                        }
                        //else
                        //    outRasters[0].RasterBandsData[b][i] = 0;
                    }
                }
                return isUpdate;    //如果为false，则不会执行对目标数据的更新写入操作
            }));
            rfr.Excute();
            return true;
        }

        private bool IsValidValue<T>(T invalue,enumDataType dataType)
        {
            if(_InvalidValuesArray!=null)
            {
                switch (dataType)
                {
                    case enumDataType.Byte:
                        foreach (double value in _InvalidValuesArray)
                        {
                            if (invalue.Equals((byte)value))
                                return false;
                        }
                        break;
                    case enumDataType.Double:
                        foreach (double value in _InvalidValuesArray)
                        {
                            if (invalue.Equals((double)value))
                                return false;
                        }
                        break;
                    case enumDataType.Float:
                        foreach (double value in _InvalidValuesArray)
                        {
                            if (invalue.Equals((float)value))
                                return false;
                        }
                        break;
                    case enumDataType.Int16:
                        foreach (double value in _InvalidValuesArray)
                        {
                            if (invalue.Equals((Int16)value))
                                return false;
                        }
                        break;
                    case enumDataType.UInt16:
                        foreach (double value in _InvalidValuesArray)
                        {
                            if (invalue.Equals((UInt16)value))
                                return false;
                        }
                        break;
                    default:
                        break;
                }
            }
            return true;
        }
        #endregion

        #region float类型的最大值、最小值、平均值计算
        private bool PeriodicMaxStatFloat(RasterMaper[] fileIns, RasterMaper[] fileOuts, Action<int, string> progressCallback = null)
        {
            float fillValues = 0;
            bool hasnullvalue = false;
            if (!string.IsNullOrWhiteSpace(_fillValues))
            {
                hasnullvalue = float.TryParse(_fillValues, out fillValues);
            }
            float invalidvalue = 0;
            bool hasinvalidvalue = false;
            if (!string.IsNullOrWhiteSpace(_invalidValues))
            {
                hasinvalidvalue = float.TryParse(_invalidValues, out invalidvalue);
            }

            RasterProcessModel<float, float> rfr = new RasterProcessModel<float, float>();
            rfr.SetRaster(fileIns, fileOuts);
            rfr.RegisterCalcModel(new RasterCalcHandlerFun<float, float>((inRasters, outRasters, aoi) =>
            {
                bool isUpdate = false;
                if (inRasters[0] == null || inRasters[0].RasterBandsData.Length == 0)
                    return false;
                if (outRasters[0] == null || outRasters[0].RasterBandsData[0].Length == 0)
                    return false;
                if (inRasters[0].RasterBandsData[0] == null)
                    return false;
                int bandCount = outRasters[0].RasterBandsData.Length;
                for (int l = 0; l < inRasters.Length; l++)
                {
                    if (progressCallback != null)
                        progressCallback(-1, "\t\t\t\t共" + inRasters.Length + "个文件,开始合成第" + (l + 1) + "个...,文件:" + Path.GetFileName(inRasters[l].Raster.fileName));
                    for (int i = 0; i < outRasters[0].RasterBandsData[0].Length; i++)
                    {
                        for (int b = 0; b < bandCount; b++)                                         //所有波段
                        {
                            if (inRasters[l].RasterBandsData[b][i].Equals(fillValues) || inRasters[l].RasterBandsData[b][i].Equals(invalidvalue))
                                continue;
                            if (outRasters[0].RasterBandsData[b][i].Equals(invalidvalue))       //输出也为特殊值，通过设置的键值对设置目标值
                            {
                                outRasters[0].RasterBandsData[b][i] = inRasters[l].RasterBandsData[b][i];
                                isUpdate = true;
                            }
                            else if (outRasters[0].RasterBandsData[b][i] < inRasters[l].RasterBandsData[b][i])
                            {
                                outRasters[0].RasterBandsData[b][i] = inRasters[l].RasterBandsData[b][i];//目前是后者数据覆盖目标数据的模式
                                isUpdate = true;
                            }
                        }
                    }

                }
                return isUpdate;    //如果为false，则不会执行对目标数据的更新写入操作
            }));
            //if (hasnullvalue)
            //    rfr.Excute(nullvalue);
            //else
            rfr.Excute();
            return true;
        }

        private bool PeriodicMinStatFloat(RasterMaper[] fileIns, RasterMaper[] fileOuts, Action<int, string> progressCallback = null)
        {
            float fillValues = 0;
            bool hasnullvalue = false;
            if (!string.IsNullOrWhiteSpace(_fillValues))
            {
                hasnullvalue = float.TryParse(_fillValues, out fillValues);
            }
            float invalidvalue = 0;
            bool hasinvalidvalue = false;
            if (!string.IsNullOrWhiteSpace(_invalidValues))
            {
                hasinvalidvalue = float.TryParse(_invalidValues, out invalidvalue);
            }
            RasterProcessModel<float, float> rfr = new RasterProcessModel<float, float>();
            rfr.SetRaster(fileIns, fileOuts);
            rfr.RegisterCalcModel(new RasterCalcHandlerFun<float, float>((inRasters, outRasters, aoi) =>
            {
                bool isUpdate = false;
                if (inRasters[0] == null || inRasters[0].RasterBandsData.Length == 0)
                    return false;
                if (outRasters[0] == null || outRasters[0].RasterBandsData[0].Length == 0)
                    return false;
                if (inRasters[0].RasterBandsData[0] == null)
                    return false;
                int bandCount = outRasters[0].RasterBandsData.Length;
                for (int l = 0; l < inRasters.Length; l++)
                {
                    if (progressCallback != null)
                        progressCallback(-1, "\t\t\t\t共" + inRasters.Length + "个文件,开始合成第" + (l + 1) + "个...,文件:" + Path.GetFileName(inRasters[l].Raster.fileName));
                    for (int i = 0; i < outRasters[0].RasterBandsData[0].Length; i++)
                    {
                        for (int b = 0; b < bandCount; b++)                                         //所有波段
                        {
                            if (inRasters[l].RasterBandsData[b][i].Equals(fillValues) || inRasters[l].RasterBandsData[b][i].Equals(invalidvalue))
                                continue;
                            if (outRasters[0].RasterBandsData[b][i].Equals(invalidvalue))       //输出也为特殊值，通过设置的键值对设置目标值
                            {
                                outRasters[0].RasterBandsData[b][i] = inRasters[l].RasterBandsData[b][i];
                                isUpdate = true;
                            }
                            else if (outRasters[0].RasterBandsData[b][i] > inRasters[l].RasterBandsData[b][i])
                            {
                                outRasters[0].RasterBandsData[b][i] = inRasters[l].RasterBandsData[b][i];//目前是后者数据覆盖目标数据的模式
                                isUpdate = true;
                            }
                        }
                    }

                }
                return isUpdate;    //如果为false，则不会执行对目标数据的更新写入操作
            }));
            rfr.Excute();
            return true;
        }

        private bool PeriodicAvgStatFloat(RasterMaper[] fileIns, RasterMaper[] fileOuts, object inaoi = null, Action<int, string> progressCallback = null)
        {
            float fillValues = 0;
            bool hasnullvalue = false;
            if (!string.IsNullOrWhiteSpace(_fillValues))
            {
                hasnullvalue = float.TryParse(_fillValues, out fillValues);
            }
            float invalidvalue = 0;
            bool hasinvalidvalue = false;
            if (!string.IsNullOrWhiteSpace(_invalidValues))
            {
                hasinvalidvalue = float.TryParse(_invalidValues, out invalidvalue);
            }
            RasterProcessModel<float, float> rfr = new RasterProcessModel<float, float>();
            rfr.SetRaster(fileIns, fileOuts);
            if ((inaoi) != null)
            {
                rfr.SetCustomAOI("自定义", inaoi);
            }
            rfr.RegisterCalcModel(new RasterCalcHandlerFun<float, float>((inRasters, outRasters, aoi) =>
            {
                bool isUpdate = false;
                if (inRasters[0] == null || inRasters[0].RasterBandsData.Length == 0)
                    return false;
                if (outRasters[0] == null || outRasters[0].RasterBandsData[0].Length == 0)
                    return false;
                if (inRasters[0].RasterBandsData[0] == null)
                    return false;
                int bandCount = outRasters[0].RasterBandsData.Length;
                for (int b = 0; b < bandCount; b++)                                         //所有波段
                {
                    if (progressCallback != null)
                        progressCallback(-1, "\t\t\t\t共" + bandCount + "个波段,开始合成第" + (b + 1) + "个...");
                    double[] sumValues = new double[outRasters[0].RasterBandsData[0].Length];
                    Int16[] sumCount = new short[outRasters[0].RasterBandsData[0].Length];
                    for (int l = 0; l < inRasters.Length; l++)
                    {
                        if (progressCallback != null)
                            progressCallback(-1, "\t\t\t\t共" + inRasters.Length + "个文件,开始合成第" + (l + 1) + "个...,文件:" + Path.GetFileName(inRasters[l].Raster.fileName));
                        if (aoi == null)
                        {
                            for (int i = 0; i < outRasters[0].RasterBandsData[0].Length; i++)
                            {
                                if (inRasters[l].RasterBandsData[b][i].Equals(fillValues) || inRasters[l].RasterBandsData[b][i].Equals(invalidvalue))
                                    continue;
                                else if (!IsValidValue<float>(inRasters[l].RasterBandsData[b][i], enumDataType.Float))
                                    continue;
                                //else
                                {
                                    sumValues[i] += inRasters[l].RasterBandsData[b][i];//目前是后者数据覆盖目标数据的模式
                                    sumCount[i] += 1;
                                    isUpdate = true;
                                }
                            }
                        }
                        else
                        {
                            for (int k = 0; k < aoi.Length; k++)
                            {
                                int i = aoi[k];
                                if (inRasters[l].RasterBandsData[b][i].Equals(fillValues) || inRasters[l].RasterBandsData[b][i].Equals(invalidvalue))
                                    continue;
                                else if (!IsValidValue<float>(inRasters[l].RasterBandsData[b][i], enumDataType.Float))
                                    continue;
                                //else
                                {
                                    sumValues[i] += inRasters[l].RasterBandsData[b][i];//目前是后者数据覆盖目标数据的模式
                                    sumCount[i] += 1;
                                    isUpdate = true;
                                }
                            }
                        }
                    }
                    for (int i = 0; i < outRasters[0].RasterBandsData[0].Length; i++)
                    {
                        if (sumCount[i] != 0)
                        {
                            outRasters[0].RasterBandsData[b][i] = (float)(sumValues[i]*1000 / (sumCount[i]*1.0D))/1000.0f;
                        }
                        //else
                        //    outRasters[0].RasterBandsData[b][i] = 0;
                    }
                }
                return isUpdate;    //如果为false，则不会执行对目标数据的更新写入操作
            }));
            rfr.Excute();
            return true;
        }
        #endregion

        private void SetDefaultInvalidValues(IRasterDataProvider outRaster)
    {
        enumDataType dataType = outRaster.DataType;
        int bandcount = outRaster.BandCount;
        switch (dataType)
            {
                case enumDataType.Byte:
                    for (int i = 1; i <= bandcount; i++)
                    {
                        outRaster.GetRasterBand(i).Fill(Byte.Parse(_invalidValues));
                    }
                    break;
                case enumDataType.Double:
                    for (int i = 1; i <= bandcount; i++)
                    {
                        outRaster.GetRasterBand(i).Fill(double.Parse(_invalidValues));
                    }
                    break;
                case enumDataType.Float:
                    for (int i = 1; i <= bandcount; i++)
                    {
                        outRaster.GetRasterBand(i).Fill(float.Parse(_invalidValues));
                    }
                    break;
                case enumDataType.Int16:
                    for (int i = 1; i <= bandcount; i++)
                    {
                        outRaster.GetRasterBand(i).Fill(Int16.Parse(_invalidValues));
                    }
                    break;
                case enumDataType.Unknow:
                    break;
                default:
                    break;
            }

    }
    }
}
