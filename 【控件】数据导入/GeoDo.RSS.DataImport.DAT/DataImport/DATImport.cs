using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.DI;
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.MIF.Core;
using System.IO;
using System.ComponentModel.Composition;
using System.Text.RegularExpressions;
using GeoDo.RSS.DF.MEM;
using System.Drawing;

namespace GeoDo.RSS.DI.DAT
{
    [Export(typeof(IDataImportDriver)), ExportMetadata("VERSION", "1")]
    public class DATImport : DataImport, IDataImportDriver
    {
        private TransDef _transDef = null;
        private string[] REGIONREGEXSTR = new string[] { @"_(?<region>\S{2,3})_" };

        public bool CanDo(string productIdentify, string subProductIdentify, string filename, out string error)
        {
            error = string.Empty;
            if (string.IsNullOrEmpty(filename) || !File.Exists(filename))
            {
                error = "待转换文件为空或不存在!";
                return false;
            }
            if (Path.GetExtension(filename).ToUpper() != ".DAT")
            {
                error = "待转换文件非DAT文件!";
                return false;
            }
            if (_transDef == null)
                _transDef = (new DATXMLParser()).GetTransDef();
            if (_transDef == null)
            {
                error = "DAT转化参数初始化错误!";
                return false;
            }
            ProductDef product = _transDef.GetProductBySmartProductIdentify(productIdentify);
            if (product == null)
            {
                error = "DAT转换尚不支持【" + productIdentify + "】产品!";
                return false;
            }
            SubProductDef subProduct = product.GetSubProductBySmartIdentfy(subProductIdentify);
            if (subProduct == null)
            {
                error = "DAT转换尚不支持【" + subProductIdentify + "】子产品!";
                return false;
            }
            return true;
        }

        public unsafe IExtractResult Do(string productIdentify, string subProductIdentify, IRasterDataProvider dataProvider, string filename, out string error)
        {
            error = string.Empty;
            if (!CanDo(productIdentify, subProductIdentify, filename, out error))
                return null;
            if (_transDef == null)
                _transDef = (new DATXMLParser()).GetTransDef();
            ProductDef product = _transDef.GetProductBySmartProductIdentify(productIdentify);
            SubProductDef subProduct = product.GetSubProductBySmartIdentfy(subProductIdentify);
            IPixelIndexMapper map = null;
            RasterMaper[] fileIns = null;
            RasterMaper[] fileOuts = null;
            string outRasterFile = null;
            IRasterDataProvider dblvPrd = null;
            try
            {
                dblvPrd = GeoDataDriver.Open(filename) as IRasterDataProvider;
                Dictionary<Int16, Int16> dic = subProduct.GetTableDic<Int16, Int16>();
                //创建临时与当前影像大小一致的Int16类型文件
                int index = 0;
                using (IRasterDataProvider outRaster = GetTempRaster(dataProvider, "MEM", enumDataType.Int16))
                {
                    outRasterFile = outRaster.fileName;
                    map = PixelIndexMapperFactory.CreatePixelIndexMapper(productIdentify + "_" + subProductIdentify, dataProvider.Width, dataProvider.Height, dataProvider.CoordEnvelope, dataProvider.SpatialRef);
                    List<RasterMaper> rms = new List<RasterMaper>();
                    RasterMaper rm = new RasterMaper(dataProvider, new int[] { 1 });
                    RasterMaper oldRm = new RasterMaper(dblvPrd, new int[] { 1 });
                    rms.AddRange(new RasterMaper[] { rm, oldRm });
                    //栅格数据映射
                    fileIns = rms.ToArray();
                    fileOuts = new RasterMaper[] { new RasterMaper(outRaster, new int[] { 1 }) };
                    //创建处理模型
                    RasterProcessModel<Int16, Int16> rfr = new RasterProcessModel<Int16, Int16>(null);
                    rfr.SetRaster(fileIns, fileOuts);
                    rfr.RegisterCalcModel(new RasterCalcHandlerFun<Int16, Int16>((rvInVistor, rvOutVistor, aoi) =>
                    {
                        if (rvInVistor[1].RasterBandsData[0] == null)
                        {
                            index += rvInVistor[0].RasterBandsData[0].Length;
                            return false;
                        }
                        else
                        {
                            for (int i = 0; i < rvInVistor[1].RasterBandsData[0].Length; i++)
                            {
                                if (dic.ContainsKey(rvInVistor[1].RasterBandsData[0][i]))
                                    map.Put(index + i);
                            }
                            index += rvInVistor[0].RasterBandsData[0].Length;
                            return true;
                        }
                    }));
                    rfr.Excute(-1);
                }
            }
            finally
            {
                if (dblvPrd != null)
                    dblvPrd.Dispose();
                foreach (RasterMaper item in fileOuts)
                {
                    if (item.Raster != null)
                        item.Raster.Dispose();
                }
                if (File.Exists(outRasterFile))
                    File.Delete(outRasterFile);
            }
            return map.Indexes == null || map.Indexes.Count() == 0 ? null : map;
        }

        public bool Do(string productIdentify, string subProductIdentify, string filename, string dstFilename, out string error)
        {
            throw new NotImplementedException();
        }

        public ImportFilesObj[] AutoFindFilesByDirver(string productIdentify, string subProIdentify, IRasterDataProvider dataProvider, string dir)
        {
            if (string.IsNullOrEmpty(dir))
                return null;
            if (_transDef == null)
                _transDef = (new DATXMLParser()).GetTransDef();
            if (_transDef == null)
                return null;
            RasterIdentify rs = new RasterIdentify(dataProvider.fileName);
            string searchStr = "*";
            if (rs == null)
                return null;
            ProductDef proDef = _transDef.GetProductBySmartProductIdentify(productIdentify);
            if (proDef != null && !string.IsNullOrEmpty(proDef.FileIdentify))
                searchStr += proDef.FileIdentify + "*";
            SubProductDef subProDef = proDef.GetSubProductBySmartIdentfy(subProIdentify);
            if (subProDef != null && !string.IsNullOrEmpty(subProDef.FileIdentify))
                searchStr += subProDef.FileIdentify + "*";
            if (!string.IsNullOrEmpty(rs.Satellite))
                searchStr += rs.Satellite + "*";
            if (!string.IsNullOrEmpty(rs.Sensor))
                searchStr += rs.Sensor + "*";
            if (rs.OrbitDateTime != DateTime.MinValue)
                searchStr += rs.OrbitDateTime.ToString("yyyyMMddHHmmss") + "*";
            string[] files = Directory.GetFiles(dir, searchStr + ".dat", SearchOption.AllDirectories);
            if (files == null || files.Length == 0)
                return null;
            string datRegion = null;
            if (!string.IsNullOrEmpty(rs.RegionIdentify) && files.Length > 1)
            {
                for (int i = 0; i < files.Length; i++)
                {
                    datRegion = GetRegionName(files[i]);
                    if (string.IsNullOrEmpty(datRegion) || rs.RegionIdentify.IndexOf(datRegion) == -1)
                        files[i] = string.Empty;
                }
            }
            List<ImportFilesObj> importFiles = new List<ImportFilesObj>();
            ImportFilesObj obj = null;
            foreach (string file in files)
            {
                if (string.IsNullOrEmpty(file))
                    continue;
                obj = new ImportFilesObj(productIdentify, subProIdentify, Path.GetFileName(file), Path.GetDirectoryName(file));
                importFiles.Add(obj);
            }
            return importFiles.Count == 0 ? null : importFiles.ToArray();
        }

        private string GetRegionName(string filename)
        {
            string regionName = string.Empty;
            int length = REGIONREGEXSTR.Length;
            string exp;
            for (int i = 0; i < length; i++)
            {
                exp = REGIONREGEXSTR[i];
                Match m = Regex.Match(filename, exp);
                if (m.Success)
                {
                    regionName = m.Groups["region"].Value;
                    return regionName;
                }
            }
            return string.Empty;
        }

        private IRasterDataProvider GetTempRaster(IRasterDataProvider currentRasterPrd, string driver, enumDataType dataType)
        {
            string outFileName = AppDomain.CurrentDomain.BaseDirectory + "TemporalFileDir//" + Path.GetFileName(currentRasterPrd.fileName);
            if (!Directory.Exists(Path.GetDirectoryName(outFileName)))
                Directory.CreateDirectory(Path.GetDirectoryName(outFileName));
            IRasterDataDriver raster = RasterDataDriver.GetDriverByName(driver) as IRasterDataDriver;
            CoordEnvelope outEnv = currentRasterPrd.CoordEnvelope.Clone();
            int width = currentRasterPrd.Width;
            int height = currentRasterPrd.Height;
            string mapInfo = outEnv.ToMapInfoString(new Size(width, height));
            RasterDataProvider outRaster = raster.Create(outFileName, width, height, 1, dataType, mapInfo) as RasterDataProvider;
            return outRaster;
        }
    }
}
