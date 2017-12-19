#region Version Info
/*========================================================================
* 功能概述：
* 
* 创建者：Administrator     时间：2014-6-15 19:59:55
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
using System.Drawing;
using GeoDo.RSS.MIF.Prds.Comm;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.Core.DF;
using CodeCell.AgileMap.Core;
using GeoDo.RSS.DF.AeronetData;
using System.Text.RegularExpressions;
namespace GeoDo.RSS.MIF.Prds.PrdVal
{
    /// <summary>
    /// 类名：SubProductValASL
    /// 属性描述：
    /// 创建者：Administrator   创建日期：2014-6-15 19:59:55
    /// 修改者：             修改日期：
    /// 修改描述：
    /// 备注：
    /// </summary>
    public class SubProductValASL:CmaMonitoringSubProduct
    {
        internal class AslDataSet
        {
            private string _dateString;
            private double[] _siteValue;

            public AslDataSet(string dateString,double[] siteValue)
            {
                _dateString = dateString;
                _siteValue = siteValue;
            }

            //观测时间
            public string DateString
            {
                get { return _dateString; }
                set { _dateString = value; }
            }

            //待验证数据与验证数据
            public double[] SiteValue
            {
                get { return _siteValue; }
                set { _siteValue = value; }
            }

        }

        private IArgumentProvider _curArguments = null;
        private IContextMessage _contextMessage;
        private string pattern = "[1-9][0-9][0-9]";

        public SubProductValASL(SubProductDef subProductDef)
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
            if (_curArguments.GetArg("AlgorithmName").ToString() == "SiteValAlgorithm")
            {
                return SiteValAlgorithm(progressTracker);
            }
            return null;
        }

        private IExtractResult SiteValAlgorithm(Action<int, string> progressTracker)
        {
            object siteArgs = _argumentProvider.GetArg("SiteArgs");
            if (siteArgs == null)
                return null;
            ValArguments args = siteArgs as ValArguments;
            if (args == null)
                return null;
            if (args.FileNamesForVal == null || args.FileNamesForVal.Length < 1)
                return null;
            if (args.FileNamesToVal == null || args.FileNamesToVal.Length < 1)
                return null;
            return DataVal(args);
        }

        private IExtractResult DataVal(ValArguments args)
        {
            //分波长，每个波长多时次进行验证（前提各时次地理位置相同）
            //2,3,4
            IVectorFeatureDataReader reader = AeronetDataReaderFactory.GetVectorFeatureDataReader(args.FileNamesToVal[0],new object[]{"LEV20"});
            Dictionary<string, List<AslDataSet>>[] fieldValue = new Dictionary<string, List<AslDataSet>>[]{new Dictionary<string,List<AslDataSet>>{},
            new Dictionary<string,List<AslDataSet>>{},new Dictionary<string,List<AslDataSet>>{}};      ///分别为470,550,650波长，每个字典分别为站点名为关键字，值为观测站点值的时间序列
            foreach(string item in args.FileNamesForVal)
            {
                RasterIdentify rid=new RasterIdentify(item);
                using (IRasterDataProvider dataPrd2Val = GeoDataDriver.Open(item) as IRasterDataProvider)
                {
                    //470
                    for (int bandNo = 2; bandNo < 5; bandNo++)
                    {
                        string description = dataPrd2Val.GetRasterBand(bandNo).Description;
                        string key = Regex.Match(description, pattern).ToString();
                        int bandLength = Int32.Parse(key);
                        int fieldIndex = 0;
                        //求验证数据中的Field
                        for (int i = 5; i < 20; i++)
                        {
                            if (bandLength >= GetBandLength(reader.Fields[i]) &&
                                bandLength < GetBandLength(reader.Fields[i - 1]))
                            {
                                fieldIndex = i;
                                break;
                            }
                        }
                        //验证数据中在范围内时间内的Feature
                        string timeString = rid.OrbitDateTime.ToString("dd:MM:yyyy");
                        string t3 = rid.OrbitDateTime.ToString("yyyy/MM/dd");
                        Feature[] feature4Vals = GetValFeature(reader.Features, dataPrd2Val.CoordEnvelope, timeString);
                        foreach (Feature fet in feature4Vals)
                        {
                            //已存在该站点
                            string location = fet.FieldValues[0];
                            double value4Val = StringToDouble(fet.FieldValues, fieldIndex);
                            if (value4Val == -9999)
                                continue;
                            double value2Val = ReadDataValueInRaster(dataPrd2Val, fet.Geometry.Envelope.MinX, fet.Geometry.Envelope.MinY, bandNo);
                            if (value2Val < 0)//去除无效值
                                continue;
                            if (fieldValue[bandNo - 2].ContainsKey(location))
                            {
                                fieldValue[bandNo - 2][location].Add(new AslDataSet(t3, new double[] { value2Val, value4Val }));
                                
                            }
                            else
                            {
                                fieldValue[bandNo - 2].Add(location, new List<AslDataSet> { new AslDataSet(t3, new double[] { value2Val, value4Val }) });
                            }
                        }
                    }
                }
            }
            IExtractResultArray array = new ExtractResultArray("统计表格");
            //生成结果
            if (args.CreatScatter)
            {
                for (int i = 0; i < fieldValue.Length; i++)
                {
                    foreach (string key in fieldValue[i].Keys)
                    {
                        List<string[]> rowList = new List<string[]>();
                        foreach (AslDataSet set in fieldValue[i][key])
                        {
                            rowList.Add(new string[]{set.DateString, set.SiteValue[0].ToString(), set.SiteValue[1].ToString()});
                        }
                        string[][] rows = rowList.ToArray();
                        IStatResult result = new StatResult("站点名称:" + key + "波长:" + GetBandLength(i), new string[] { "时间", "待验证ASL", "Aeronet LST" }, rows);
                        string title = "气溶胶产品数据对比";
                        string filename = GetOutputFileName(args.FileNamesForVal, "SCAT", key, GetBandLength(i));
                        try
                        {
                            using (StatResultToChartInExcelFile excelControl = new StatResultToChartInExcelFile())
                            {
                                excelControl.Init(masExcelDrawStatType.xlXYScatter);
                                excelControl.Add(title, result, true, 1, false, result.Columns[1], result.Columns[2]);
                                if (!filename.ToUpper().EndsWith(".XLSX"))
                                    filename += ".XLSX";
                                excelControl.SaveFile(filename);
                                IFileExtractResult res = new FileExtractResult("SCAT", filename);
                                array.Add(res);
                            }
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                }
            }
            if (args.CreatTimeSeq)
            {
                for (int i = 0; i < fieldValue.Length; i++)
                {
                    foreach (string key in fieldValue[i].Keys)
                    {
                        List<string[]> rowList = new List<string[]>();
                        foreach (AslDataSet set in fieldValue[i][key])
                        {
                            rowList.Add(new string[] { set.DateString, set.SiteValue[0].ToString(), set.SiteValue[1].ToString() });
                        }
                        string[][] rows = rowList.ToArray();
                        IStatResult result = new StatResult("站点名称:" + key + "波长:" + GetBandLength(i), new string[] { "时间", "待验证ASL", "Aeronet LST" }, rows);
                        string title = "气溶胶产品数据对比";
                        string filename = GetOutputFileName(args.FileNamesForVal, "SEQD", key, GetBandLength(i));
                        try
                        {
                            using (StatResultToChartInExcelFile excelControl = new StatResultToChartInExcelFile())
                            {
                                excelControl.Init(masExcelDrawStatType.xlLineMarkers);
                                excelControl.Add(title, result, true, 0, true, result.Columns[0], "ASL");
                                if (!filename.ToUpper().EndsWith(".XLSX"))
                                    filename += ".XLSX";
                                excelControl.SaveFile(filename);
                                IFileExtractResult res = new FileExtractResult("SEQD", filename);
                                array.Add(res);
                            }
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                }
            }
            return array;
        }

        private double StringToDouble(string[] fieldValues, int fieldIndex)
        {
            double value;
            if (double.TryParse(fieldValues[fieldIndex], out value))
                return value;
            else
            {
                for (int i = (fieldIndex + 1); i < fieldValues.Length; i++)
                {
                    if (double.TryParse(fieldValues[i], out value))
                        return value;
                }
            }
            return -9999;
        }

        private int GetBandLength(int index)
        {
            switch (index)
            {
                case 0: return 470;
                case 1: return 550;
                case 2: return 650;
                default: return 470;
            }
        }

        private unsafe double ReadDataValueInRaster(IRasterDataProvider dataProvider, double x, double y,int bandNo)
        {
            //计算行、列偏移
            CoordEnvelope envelope = dataProvider.CoordEnvelope;
            int xoffset = (int)(Math.Round((x - envelope.MinX) / dataProvider.ResolutionX));
            int yoffset = (int)(Math.Round((envelope.MaxY - y) / dataProvider.ResolutionY));
            Int16[] buffer = new Int16[1];
            fixed (Int16* ptr = buffer)
            {
                IntPtr bufferPtr = new IntPtr(ptr);
                dataProvider.GetRasterBand(bandNo).Read(xoffset, yoffset, 1, 1, bufferPtr, enumDataType.Int16,1,1);
            }
            //有效范围为0-32767
            if (buffer[0] > 0)
                return (buffer[0] * 1.0E-4);
            return buffer[0];
        }

        private int GetBandLength(string field)
        {
            return Int32.Parse(field.Replace("AOT_", ""));
        }

        private Feature[] GetValFeature(Feature[] featureSet, CoordEnvelope envelope, string dateString)
        {
            double x,y;
            List<Feature> featureList = new List<Feature>();
            for (int i = 0; i < featureSet.Length; i++)
            {
                if (featureSet[i].FieldValues[1] == dateString)
                {
                    x=featureSet[i].Geometry.Envelope.MaxX;
                    y=featureSet[i].Geometry.Envelope.MaxY;
                    if (x < envelope.MaxX && x >= envelope.MinX && y >= envelope.MinY && y < envelope.MaxY)
                    {
                        featureList.Add(featureSet[i]);
                    }
                }
            }
            return featureList.ToArray();
        }

        private string GetOutputFileName(string[] inputValFileNames,string outputIdentify,string location,int bandLength)
        {
            string filename=GetFileName(inputValFileNames, "VAL", outputIdentify, ".XLSX", null);
            filename = Path.Combine(Path.GetDirectoryName(filename),Path.GetFileNameWithoutExtension(filename) + "_" + location + "_" + bandLength + ".xlsx");
            return filename;
        }
    }
}
