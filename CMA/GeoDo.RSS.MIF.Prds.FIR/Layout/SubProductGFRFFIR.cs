#region Version Info
/*========================================================================
* 功能概述：
* 
* 创建者：DongW     时间：2013/9/11 15:12:17
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
using CodeCell.AgileMap.Core;
using System.IO;
using System.Drawing;

namespace GeoDo.RSS.MIF.Prds.FIR
{
    /// <summary>
    /// 类名：SubProductGFRFFIR
    /// 属性描述：
    /// 创建者：zyb   创建日期：2013/9/11 15:12:17
    /// 修改者：zyb             修改日期：8:44 2013/10/14
    /// 修改描述：
    /// 备注：
    /// </summary>
    public class SubProductGFRFFIR : CmaMonitoringSubProduct
    {
        private IContextMessage _contextMessage;
        private double _resolution;
        private int _datheight = 0;
        private int _datwidth = 0;
        private Int32 _firePointsCount = 0;
        private string _cycFlag = "POAM";

        public SubProductGFRFFIR(SubProductDef subProductDef)
            : base(subProductDef)
        {

        }

        public override IExtractResult Make(Action<int, string> progressTracker)
        {
            return Make(progressTracker, null);
        }

        public override IExtractResult Make(Action<int, string> progressTracker, IContextMessage contextMessage)
        {
            if (_argumentProvider == null)
                return null;
            if (_argumentProvider.GetArg("AlgorithmName") == null)
                return null;
            if (_argumentProvider.GetArg("AlgorithmName").ToString() == "GFRFAlgorithm")
            {
                bool yearStat = bool.Parse(_argumentProvider.GetArg("YearStat").ToString());
                _cycFlag = yearStat ? "POAY" : "POAM";
                SetOutIdentify(yearStat);
                _resolution = Obj2Double(_argumentProvider.GetArg("Resolution"));
                if (_resolution == 0)
                {
                    PrintInfo("输出分辨率不能为0");
                    return null;
                }
                string[] files = GetStringArray("SelectedRGFRFiles");
                if (files == null || files.Length == 0)
                    return null;
                for (int i = 0; i < files.Length; i++)
                {
                    if (!File.Exists(files[i]) || (Path.GetExtension(files[i]).ToUpper() != ".SHP" && Path.GetExtension(files[i]).ToUpper() != ".TXT"))
                    {
                        PrintInfo("请选择正确格式的文件！");
                        return null;
                    }
                }
                IExtractResultArray array = CalcGFRF(files);
                return array;
            }
            return null;
        }

        private void SetOutIdentify(bool yearStat)
        {
            if (yearStat)
                _argumentProvider.SetArg("OutFileIdentify", "GFRY");
        }

        private IExtractResultArray CalcGFRF(string[] files)
        {
            string datFname = GetDatFilename(files);
            string GFInfoList = GenGFILfname(files);
            using (StreamWriter sw = new StreamWriter(GFInfoList, false, Encoding.Default))
            {
                sw.WriteLine("NO.\t" + "Latitude\t" + "Longitude\t" + "Size/ha\t" + "Temperature/K\t" + "Fire_Intensity\t" + "Reliability");
            }
            int[,] fireCountArray = InitValueArray();
            int[] firPositionArray = null;
            for (int i = 0; i < files.Length; i++)
            {
                firPositionArray = ProcessVectorToArray(files[i], GFInfoList);
                int col, row;
                foreach (int cr in firPositionArray)
                {
                    row = cr / _datwidth;
                    col = (cr % _datwidth);
                    fireCountArray[row, col] += 1;
                }
            }
            _firePointsCount = 0;
            ProcessArrayToRaster(datFname, fireCountArray);
            _argumentProvider.SetArg("SelectedPrimaryFiles", datFname);
            IFileExtractResult GFRF = GFRFAlgorithm() as IFileExtractResult;
            IExtractResultArray array = new ExtractResultArray("全球火点累计");
            IFileExtractResult GFIL = new FileExtractResult("GFIL", GFInfoList, true);
            GFIL.SetDispaly(false);
            array.Add(GFRF);
            array.Add(GFIL);
            return array;
        }

        private Double Obj2Double(object v)
        {
            if (v == null || v.ToString() == string.Empty)
                return double.NaN;
            return (Double)v;
        }

        /// <summary>
        /// 生成栅格化火点文件名
        /// </summary>
        /// <param name="shpFile"></param>
        /// <returns></returns>
        private string GetDatFilename(string[] shpFile)
        {
            RasterIdentify id = new RasterIdentify(shpFile);
            id.ThemeIdentify = "CMA";
            id.ProductIdentify = "FIR";
            id.SubProductIdentify = "GFRT";
            id.CYCFlag = _cycFlag;
            id.IsOutput2WorkspaceDir = true;
            return id.ToWksFullFileName(".dat");
        }

        private string GenGFILfname(string[] shpFile)
        {
            RasterIdentify id = new RasterIdentify(shpFile);
            id.ThemeIdentify = "CMA";
            id.ProductIdentify = "FIR";
            id.SubProductIdentify = "GFIL";
            id.CYCFlag = _cycFlag;
            id.IsOutput2WorkspaceDir = true;
            return id.ToWksFullFileName(".TXT");
        }


        public int[,] InitValueArray()
        {
            CoordEnvelope envelope = new CoordEnvelope(-180, 180, -90, 90);
            _datheight = (int)Math.Ceiling((envelope.MaxY - envelope.MinY) / _resolution);
            _datwidth = (int)Math.Ceiling((envelope.MaxX - envelope.MinX) / _resolution);
            return new int[_datheight, _datwidth];
        }

        public int[] ProcessVectorToArray(string shpFileName, string firlistfname)
        {
            //sw.WriteLine("NO.\t" + "Latitude\t" + "Longitude\t" + "Size/ha\t" + "Temperature/K\t" + "Fire_Intensity\t" + "Reliability");
            Feature[] features = null;
            IVectorFeatureDataReader dr = null;
            if (Path.GetExtension(shpFileName).ToUpper() == ".SHP")
            {
                dr = VectorDataReaderFactory.GetUniversalDataReader(shpFileName) as IVectorFeatureDataReader;
                if (dr == null)
                    return null;
                features = dr.Features;
            }
            else if (Path.GetExtension(shpFileName).ToUpper() == ".TXT")
            {
                string[] pointContext = File.ReadAllLines(shpFileName);
                List<Feature> featureList = new List<Feature>();
                Feature tempFeature = null;
                if (pointContext != null && pointContext.Length != 0)
                {
                    int length = pointContext.Length;
                    for (int i = 1; i < length; i++)
                    {
                        string[] tempSplit = pointContext[i].Split(new char[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);
                        if (tempSplit == null || tempSplit.Length != 7)
                            continue;
                        tempFeature = new Feature(int.Parse(tempSplit[0]), new ShapePoint(double.Parse(tempSplit[2]), double.Parse(tempSplit[1])),
                            new string[] { "0", "1", "2", "Latitude", "Longitude", "Size", "Temperature", "Fire_Intensity", "Reliability" },
                            new string[] { "0", "1", "2", tempSplit[1], tempSplit[2], tempSplit[3], tempSplit[4], tempSplit[5], tempSplit[6] }, null);
                        featureList.Add(tempFeature);
                    }
                }
                features = featureList.Count == 0 ? null : featureList.ToArray();
            }
            if (features == null || features.Length < 1)
                return null;
            using (StreamWriter sw = new StreamWriter(firlistfname, true, Encoding.Default))
            {
                List<int> index = new List<int>();
                int position;
                string[] fieldValues;
                foreach (Feature fea in features)
                {
                    fieldValues = fea.FieldValues;
                    _firePointsCount++;
                    if (fea.FieldValues != null)
                    {
                        //_firePointsCount++;
                        sw.WriteLine(_firePointsCount + "\t" + fieldValues[3].PadLeft(7) + "\t\t" + fieldValues[4].PadLeft(7) + "\t\t" + fieldValues[5].PadLeft(7) + "\t\t" + fieldValues[6] + "\t\t" + fieldValues[7] + "\t\t" + fieldValues[8]);
                        ShapePoint pt = fea.Geometry as ShapePoint;
                        int col = (int)Math.Ceiling((pt.X + 180) / _resolution) - 1;
                        int row = (int)Math.Ceiling((90 - pt.Y) / _resolution) - 1;
                        position = _datwidth * row + col;
                        index.Add(position);
                    }
                    else
                    {
                        sw.WriteLine(_firePointsCount);
                    }
                }
                return index.ToArray();
            }
            if (dr != null)
                dr.Dispose();
        }

        public void ProcessArrayToRaster(string dstFileName, int[,] arrayValue)
        {
            CoordEnvelope envelope = new CoordEnvelope(-180, 180, -90, 90);
            IRasterDataProvider dataPrd = null;
            enumDataType dataType = enumDataType.Int16;
            try
            {
                if (Path.GetExtension(dstFileName).ToUpper() == ".DAT")
                {
                    IRasterDataDriver driver = GeoDataDriver.GetDriverByName("MEM") as IRasterDataDriver;
                    string mapInfo = envelope.ToMapInfoString(new Size(_datwidth, _datheight));
                    dataPrd = driver.Create(dstFileName, _datwidth, _datheight, 1, dataType, mapInfo);
                }
                else
                    return;
                if (arrayValue == null)
                    return;
                ProcessArrayToRaster(arrayValue, dataPrd);
            }
            finally
            {
                if (dataPrd != null)
                    dataPrd.Dispose();
            }
        }

        private void ProcessArrayToRaster(int[,] array, IRasterDataProvider dataPrd)
        {
            for (int j = 0; j < _datwidth; j++)
            {
                for (int i = 0; i < _datheight; i++)
                {
                    if (array[i, j] == 0)
                        continue;
                    Int16 value = (Int16)array[i, j];
                    RasterWriterInt16(dataPrd, j, i, value);
                }
            }
        }

        private unsafe void RasterWriterInt16(IRasterDataProvider rasterProvider, int column, int row, Int16 value)
        {
            Int16[] buffer = new Int16[] { value };
            fixed (Int16* ptr = buffer)
            {
                IntPtr bufferPtr = new IntPtr(ptr);
                rasterProvider.GetRasterBand(1).Write(column, row, 1, 1, bufferPtr, enumDataType.Int16, 1, 1);
            }
        }

        private IExtractResult GFRFAlgorithm()
        {
            string instanceIdentify = _argumentProvider.GetArg("OutFileIdentify") as string;
            if (string.IsNullOrWhiteSpace(instanceIdentify))
                return null;
            SubProductInstanceDef instance = FindSubProductInstanceDefs(instanceIdentify);
            if (instance != null)
                return ThemeGraphyResult(null);
            return null;
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
