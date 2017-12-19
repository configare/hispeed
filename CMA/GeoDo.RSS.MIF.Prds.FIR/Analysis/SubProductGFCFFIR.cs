#region Version Info
/*========================================================================
* 功能概述：
* 
* 创建者：DongW     时间：2013/9/17 10:02:41
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
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.MIF.Prds.Comm;
using System.IO;
using GeoDo.RSS.Core.DF;
using CodeCell.AgileMap.Core;
using System.Drawing;
using GeoDo.RSS.Core.UI;
using GeoDo.RSS.UI.AddIn.Theme;

namespace GeoDo.RSS.MIF.Prds.FIR
{
    /// <summary>
    /// 类名：SubProductGFCFFIR
    /// 属性描述：
    /// 创建者：DongW   创建日期：2013/9/17 10:02:41
    /// 修改者：             修改日期：
    /// 修改描述：
    /// 备注：
    /// </summary>
    public class SubProductGFCFFIR : CmaMonitoringSubProduct
    {
        private IContextMessage _contextMessage;
        private double _resolution = 1;

        public SubProductGFCFFIR(SubProductDef subProductDef)
            : base(subProductDef)
        {
        }

        public override IExtractResult Make(Action<int, string> progressTracker)
        {
            if (_argumentProvider == null)
                return null;
            if (_argumentProvider.GetArg("AlgorithmName") == null)
                return null;
            if (_argumentProvider.GetArg("AlgorithmName").ToString() == "GFCFAlgorithm")
            {
                if (!CheckContinentshp())
                    return null;
                string[] files = GetStringArray("SelectedPrimaryFiles");
                if (files == null || files.Length == 0)
                {
                    if (!TrySetSelectedPrimaryFiles(ref files))
                    {
                        PrintInfo("请输入待统计的文件!");
                        return null;
                    }
                }
                return GFCFAlgorithm(files);
            }
            return null;
        }

        #region 检查统计模板矢量

        private bool CheckContinentshp()
        {
            string fname = AppDomain.CurrentDomain.BaseDirectory + @"数据引用\基础矢量\行政区划\面\大洲.shp";
            if (!File.Exists(fname))
            {
                PrintInfo("文件\"" + fname + "\"未找到,无法应用默认统计矢量模板。");
                return false;
            }
            return true;
        }

        #endregion

        #region 处理shp文件为dat栅格文件

        private string GetRasterFilename(string[] shpFile)
        {
            RasterIdentify id = new RasterIdentify(shpFile);
            id.ThemeIdentify = "CMA";
            id.ProductIdentify = "FIR";
            id.SubProductIdentify = "GFRT";
            id.IsOutput2WorkspaceDir = true;
            return id.ToWksFullFileName(".dat");       
        }

        public int[,] NewDatArray()
        {
            CoordEnvelope envelope = new CoordEnvelope(-180, 180, -90, 90);
            int height = (int)Math.Ceiling((envelope.MaxY - envelope.MinY) / _resolution);
            int width = (int)Math.Ceiling((envelope.MaxX - envelope.MinX) / _resolution);
            return new int[height, width];
        }

        public int[,] GetPointPosition(string shpFileName)
        {
            using (IVectorFeatureDataReader dr = VectorDataReaderFactory.GetUniversalDataReader(shpFileName)
                as IVectorFeatureDataReader)
            {
                if (dr == null)
                    return null;
                Feature[] features = dr.FetchFeatures();
                if (features == null || features.Length < 1)
                    return null;
                int[,] index = new int[2,features.Length];
                int i = 0;
                foreach (Feature fea in features)
                {
                    ShapePoint pt = fea.Geometry as ShapePoint;
                    int col = (int)Math.Ceiling((pt.X + 180) / _resolution) - 1;
                    int row = (int)Math.Ceiling((90 - pt.Y) / _resolution) - 1;
                    if (col < 0)
                        col = 0;
                    if (row < 0)
                        row = 0;
                    index[0, i] = row;
                    index[1, i] = col;
                    i++;
                }
                return index;
            }
        }

        public void CreateRasterDataPrd(string rasterFileName, int[,] dataValue)
        {
            CoordEnvelope envelope = new CoordEnvelope(-180, 180, -90, 90);
            IRasterDataProvider dataPrd = null;
            try
            {
               IRasterDataDriver driver = GeoDataDriver.GetDriverByName("MEM") as IRasterDataDriver;
               int height = (int)Math.Ceiling((envelope.MaxY - envelope.MinY) / _resolution);
               int width = (int)Math.Ceiling((envelope.MaxX - envelope.MinX) / _resolution);
               string mapInfo = envelope.ToMapInfoString(new Size(width, height));
               dataPrd = driver.Create(rasterFileName, width, height, 1, enumDataType.Int16, mapInfo);
               WriteValueToRaster(dataValue, dataPrd);
            }
            finally
            {
                if (dataPrd != null)
                    dataPrd.Dispose();
            }
        }

        private void WriteValueToRaster(int[,] values, IRasterDataProvider dataPrd)
        {
            for (int j = 0; j < values.GetLength(0); j++)
            {
                for (int i = 0; i < values.GetLength(1); i++)
                {
                    if (values[j, i] == 0)
                        continue;
                    RasterWriterInt16(dataPrd, i, j, (Int16)values[j, i]);
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

        #endregion

        private IExtractResult GFCFAlgorithm(string[] fileNames)
        {
            //检查文件类型
            if (fileNames == null || fileNames.Length < 1)
                return null;
            if (Path.GetExtension(fileNames[0]).ToUpper() == ".DAT")
            {
                return StatFirePoint(fileNames[0]);
            }
            else
            {
                foreach (string name in fileNames)
                {
                    if (Path.GetExtension(name).ToUpper() != ".SHP")
                    {
                        PrintInfo("请选择文件类型一致的待统计文件！");
                        return null;
                    }
                }
                string rasterFname = GetRasterFilename(fileNames);
                if (System.IO.File.Exists(rasterFname))
                {
                    return StatFirePoint(rasterFname);
                } 
                else
                {     
                    int[,] dataValue = NewDatArray();
                    //2行n列数组，第一行记录行号，第二行对应位置记录列号
                    int[,] firposition;
                    for (int i = 0; i < fileNames.Length; i++)
                    {
                        firposition = GetPointPosition(fileNames[i]);
                        int width = dataValue.GetLength(1);
                        for (int j = 0; j < firposition.GetLength(1);j++)
                        {
                            dataValue[firposition[0,j], firposition[1,j]] ++;
                        }
                    }
                    CreateRasterDataPrd(rasterFname, dataValue);
                    return StatFirePoint(rasterFname);
                }
            }            
        }

        private IExtractResult StatFirePoint(string rasterFileName)
        {
            IRasterDataProvider inRaster = RasterDataDriver.Open(rasterFileName) as IRasterDataProvider;
            if (inRaster == null)
            {
                PrintInfo("读取栅格文件失败：" + inRaster);
                return null;
            }
            SortedDictionary<string, StatAreaItem> result;
            RasterStatByVector<Int16> stat = new RasterStatByVector<Int16>(null);
            result = stat.CountByVector(inRaster, "大洲.shp", "CONTINENT",
                (cur, cursum) =>
                {
                    return cursum += cur;
                });
            if (result.Count == 0)
                return null;
            List<string[]> resultList = new List<string[]>();
            foreach (string key in result.Keys)
            {
                resultList.Add(new string[] { key, result[key].GrandTotal.ToString() });
            }
            string sentitle = "统计日期：" + DateTime.Now.ToShortDateString();
            RasterIdentify id = new RasterIdentify(rasterFileName);
            id.ProductIdentify = "FIR";
            id.SubProductIdentify = "GFCF";
            if (id.OrbitDateTime != null)
                sentitle += "    轨道日期：" + id.OrbitDateTime.ToShortDateString();
            string[] columns = new string[] { "大洲", "累计火点数" };
            IStatResult fresult = new StatResult(sentitle, columns, resultList.ToArray());
            string outputIdentify = _argumentProvider.GetArg("OutFileIdentify").ToString();
            string title = "全球火点大洲统计";
            string filename = StatResultToFile(new string[] { rasterFileName }, fresult, "FIR", outputIdentify, title, null, 1, true, 1);
            return new FileExtractResult(outputIdentify, filename);
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
