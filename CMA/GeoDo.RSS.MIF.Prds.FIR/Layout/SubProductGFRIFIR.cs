#region Version Info
/*========================================================================
* 功能概述：
* 
* 创建者：DongW     时间：2013/9/11 15:10:12
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
    /// 类名：SubProductGFRIFIR
    /// 属性描述：
    /// 创建者：DongW   创建日期：2013/9/11 15:10:12
    /// 修改者：             修改日期：
    /// 修改描述：
    /// 备注：
    /// </summary>
    public class SubProductGFRIFIR : CmaMonitoringSubProduct
    {
        private double _resolution = double.NaN;

        public SubProductGFRIFIR(SubProductDef subProductDef)
            : base(subProductDef)
        {
        }

        public override IExtractResult Make(Action<int, string> progressTracker)
        {
            if (_argumentProvider == null)
                return null;
            if (_argumentProvider.GetArg("AlgorithmName") == null)
                return null;
            if (_argumentProvider.GetArg("AlgorithmName").ToString() == "GFRIAlgorithm")
            {
                _resolution = Obj2Double(_argumentProvider.GetArg("Resolution"));
                if (_resolution == 0||double.IsNaN(_resolution))
                {
                    Console.WriteLine("输出分辨率不能为0");
                    return null;
                }
                string[] files = GetStringArray("SelectedRGFRFile");
                if (files != null && files.Length > 0)
                {
                    string shpFilename = files[0];
                    if (File.Exists(files[0]) && Path.GetExtension(files[0]).ToUpper()==".SHP")
                    {
                        string datFilename = GenDatFilename(shpFilename);
                        ProcessVectorToRaster(shpFilename, enumDataType.Int16, _resolution, datFilename);
                        _argumentProvider.SetArg("SelectedPrimaryFiles", datFilename);
                        return GFRIAlgorithm(); 
                    }
                    else
                        return null;
                }
            }
            return null;
        }

        private double Obj2Double(object v)
        {
            if (v == null || v.ToString() == string.Empty)
                return double.NaN;
            return (Double)v;
        }

        private string GenDatFilename(string shpFilename)
        {
            RasterIdentify id = new RasterIdentify(shpFilename);
            id.ThemeIdentify = "CMA";
            id.ProductIdentify = "FIR";
            id.SubProductIdentify = "GFRR";
            id.IsOutput2WorkspaceDir = true;
            return id.ToWksFullFileName(".dat");
        }

        public void ProcessVectorToRaster(string shpFileName, enumDataType dataType, double resolution, string rasterFileName)
        {
            if (string.IsNullOrEmpty(rasterFileName))
                return;
            using (IVectorFeatureDataReader dr = VectorDataReaderFactory.GetUniversalDataReader(shpFileName)
                as IVectorFeatureDataReader)
            {
                if (dr == null)
                    return;
                CoordEnvelope envelope = new CoordEnvelope(-180, 180, -90, 90);
                int height = (int)Math.Ceiling((envelope.MaxY - envelope.MinY) / resolution);
                int width = (int)Math.Ceiling((envelope.MaxX - envelope.MinX) / resolution);
                IRasterDataProvider dataPrd = null;
                try
                {
                    if (Path.GetExtension(rasterFileName).ToUpper() == ".DAT")
                    {
                        IRasterDataDriver driver = GeoDataDriver.GetDriverByName("MEM") as IRasterDataDriver;
                        string mapInfo = envelope.ToMapInfoString(new Size(width, height));
                        dataPrd = driver.Create(rasterFileName, width, height, 1, dataType, mapInfo);
                    }
                    else
                        return;
                    Feature[] features = dr.FetchFeatures();
                    if (features == null || features.Length < 1)
                        return;
                    ProcessVectorToRaster(features, dataPrd);
                }
                finally
                {
                    if (dataPrd != null)
                        dataPrd.Dispose();
                }
            }
        }

        public void ProcessVectorToRaster(Feature[] features, IRasterDataProvider rasterProvider)
        {
            Size size = new Size(rasterProvider.Width, rasterProvider.Height);
            string fieldValue;
            int persent = -1;
            foreach (Feature fea in features)
            {
                persent++;
                if (fea.FieldValues == null)
                    continue;
                fieldValue = fea.FieldValues[7];
                if (String.IsNullOrEmpty(fieldValue))
                    continue;
                Int16 value;
                if (Int16.TryParse(fieldValue, out value))
                {
                    ShapePoint pt = fea.Geometry as ShapePoint;
                    int col = (int)Math.Ceiling((pt.X + 180) / _resolution);
                    int row = (int)Math.Ceiling((90 - pt.Y) / _resolution);
                    RasterWriterInt16(rasterProvider, col, row, value);
                    continue;
                }
                else
                {
                    throw new Exception("设置的属性无法进行栅格化。");
                }
            }
        }

        private unsafe void RasterWriterInt16(IRasterDataProvider rasterProvider, int column, int row, Int16 value)
        {
            Int16[] buffer = new Int16[] { value };
            fixed (Int16* ptr = buffer)
            {
                IntPtr bufferPtr = new IntPtr(ptr);
                rasterProvider.GetRasterBand(1).Write(column - 1, row - 1, 1, 1, bufferPtr, enumDataType.Int16, 1, 1);
            }
        }
        
        private IExtractResult GFRIAlgorithm()
        {
            string instanceIdentify = _argumentProvider.GetArg("OutFileIdentify") as string;
            if (string.IsNullOrWhiteSpace(instanceIdentify))
                return null;
            SubProductInstanceDef instance = FindSubProductInstanceDefs(instanceIdentify);
            if (instance != null)
                return ThemeGraphyResult(null);
            return null;
        }
    }
}
