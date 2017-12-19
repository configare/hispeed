using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.DI;
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.MIF.Core;
using System.IO;
using GeoDo.RSS.DF.MVG;
using System.ComponentModel.Composition;
using System.Text.RegularExpressions;

namespace GeoDo.RSS.DI.MVG
{
    [Export(typeof(IDataImportDriver)), ExportMetadata("VERSION", "1")]
    public class MVGImport : DataImport, IDataImportDriver
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
            if (Path.GetExtension(filename).ToUpper() != ".MVG")
            {
                error = "待转换文件非MVG文件!";
                return false;
            }
            if (_transDef == null)
                _transDef = (new MVGXMLParser()).GetTransDef();
            if (_transDef == null)
            {
                error = "MVG转化参数初始化错误!";
                return false;
            }
            ProductDef product = _transDef.GetProductBySmartProductIdentify(productIdentify);
            if (product == null)
            {
                error = "MVG转换尚不支持【" + productIdentify + "】产品!";
                return false;
            }
            SubProductDef subProduct = product.GetSubProductBySmartIdentfy(subProductIdentify);
            if (subProduct == null)
            {
                error = "MVG转换尚不支持【" + subProductIdentify + "】子产品!";
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
                _transDef = (new MVGXMLParser()).GetTransDef();
            ProductDef product = _transDef.GetProductBySmartProductIdentify(productIdentify);
            SubProductDef subProduct = product.GetSubProductBySmartIdentfy(subProductIdentify);
            IGeoDataProvider provider = GeoDataDriver.Open(filename, enumDataProviderAccess.ReadOnly, null);
            if (provider != null)
            {
                MvgDataProvider mvgProvider = provider as MvgDataProvider;
                if (mvgProvider == null)
                {
                    error = "MVG转换尚不支持文件【" + Path.GetFileName(filename) + "】!";
                    return null;
                }
                float xResolution = dataProvider.ResolutionX, mvgXResolution = (float)mvgProvider.CoordEnvelope.Width / mvgProvider.Width;
                float yResolution = dataProvider.ResolutionY, mvgYResolution = (float)mvgProvider.CoordEnvelope.Height / mvgProvider.Height;
                int width = dataProvider.Width, mvgWidth = mvgProvider.Width;
                double minX = dataProvider.CoordEnvelope.MinX, mvgMinX = mvgProvider.CoordEnvelope.MinX;
                double maxY = dataProvider.CoordEnvelope.MaxY, mvgMaxY = mvgProvider.CoordEnvelope.MaxY;
                int xIndex = 0;
                int yIndex = 0;
                if (dataProvider.DataType == enumDataType.UInt16)
                {
                    Int16[] dataBlock = new Int16[mvgProvider.Width * mvgProvider.Height];
                    fixed (Int16* buffer = dataBlock)
                    {
                        IntPtr ptr = new IntPtr(buffer);
                        mvgProvider.Read(0, 0, mvgProvider.Width, mvgProvider.Height, ptr, enumDataType.Int16, mvgProvider.Width, mvgProvider.Height, 1, new int[] { 1 }, enumInterleave.BSQ);
                    }
                    Dictionary<Int16, Int16> dic = subProduct.GetTableDic<Int16, Int16>();
                    IPixelIndexMapper map = PixelIndexMapperFactory.CreatePixelIndexMapper(productIdentify + "_" + subProductIdentify, width, dataProvider.Height, dataProvider.CoordEnvelope, dataProvider.SpatialRef);
                    RasterPixelsVisitor<UInt16> visitor = new RasterPixelsVisitor<UInt16>(new ArgumentProvider(dataProvider, null));
                    visitor.VisitPixel(new int[] { 1 }, (index, values) =>
                        {
                            xIndex = (int)Math.Round((minX + (index % width * xResolution) - mvgMinX) / mvgXResolution);
                            yIndex = (int)Math.Round((mvgMaxY - (maxY - (index / width * yResolution))) / mvgYResolution);
                            if (xIndex >= 0 && yIndex >= 0 && yIndex * mvgWidth + xIndex < dataBlock.Length)
                            {
                                if (dic.ContainsKey(dataBlock[yIndex * mvgWidth + xIndex]))
                                    map.Put(index);
                            }
                        });
                    return map;
                }
            }
            return null;
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
                _transDef = (new MVGXMLParser()).GetTransDef();
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
            string[] files = Directory.GetFiles(dir, searchStr + ".mvg", SearchOption.AllDirectories);
            if (files == null || files.Length == 0)
                return null;
            string mvgRegion = null;
            if (!string.IsNullOrEmpty(rs.RegionIdentify) && files.Length > 1)
            {
                for (int i = 0; i < files.Length; i++)
                {
                    mvgRegion = GetRegionName(files[i]);
                    if (string.IsNullOrEmpty(mvgRegion) || rs.RegionIdentify.IndexOf(mvgRegion) == -1)
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
    }
}
