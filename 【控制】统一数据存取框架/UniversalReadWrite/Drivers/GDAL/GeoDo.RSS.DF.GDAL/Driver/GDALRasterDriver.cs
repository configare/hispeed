using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;
using OSGeo.GDAL;
using System.ComponentModel.Composition;
using System.Runtime.InteropServices;
using System.IO;

namespace GeoDo.RSS.DF.GDAL
{
    [Export(typeof(IGeoDataDriver)), ExportMetadata("VERSION", "1")]
    public class GDALRasterDriver:RasterDataDriver,IGDALRasterDriver
    {
        static GDALRasterDriver()
        {
            Gdal.AllRegister();
            //Gdal.SetCacheMax(0);
            //Gdal.GetConfigOption("GDAL_FILENAME_IS_UTF8", "NO");
        }

        public GDALRasterDriver()
            : base()
        {
            _name = "GDAL";
            _fullName = "GDAL Data Driver";
        }

        public GDALRasterDriver(string name, string fullName)
            :base(name,fullName)
        { 
        }

        protected override IGeoDataProvider BuildDataProvider(string fileName, byte[] header1024,enumDataProviderAccess access, params object[] args)
        {
            return new GDALRasterDataProvider(fileName,header1024, this,access);
        }

        protected override IGeoDataProvider BuildDataProvider(string fileName, enumDataProviderAccess access, params object[] args)
        {
            return new GDALRasterDataProvider(fileName,null, this, access);
        }

        protected override bool IsCompatible(string fileName, byte[] header1024,params object[] args)
        {
            string extName = Path.GetExtension(fileName).ToUpper();
            if (extName == ".TXT")
                return false;
            return true;
        }

        public override void Delete(string fileName)
        {
            throw new NotImplementedException();
        }

        public override IRasterDataProvider Create(string fileName, int xSize, int ySize, int bandCount, enumDataType dataType, params object[] options)
        {
            string[] driverOptions;
            string drvName = GetDriverName(options, out driverOptions);
            if (string.IsNullOrEmpty(drvName))
                throw new ArgumentException("没有指定或者指定了错误的驱动名称！");
            if (driverOptions == null)
                driverOptions = new string[] { "INTERLEAVE=BSQ" };
            using (Driver drv = Gdal.GetDriverByName(drvName))
            {
                double[] geoTransform;
                string wktPrj;
                GetGDALOptions(driverOptions, out geoTransform, out wktPrj);
                using (Dataset ds = drv.Create(fileName, xSize, ySize, bandCount, GetDataType(dataType), driverOptions))
                {
                    if (geoTransform != null && geoTransform.Length == 6)
                        ds.SetGeoTransform(geoTransform);
                    if (!string.IsNullOrWhiteSpace(wktPrj))
                        ds.SetProjection(wktPrj);
                    //FillZero(ds,dataType);
                }
            }
            return GeoDataDriver.Open(fileName, enumDataProviderAccess.Update, null) as IRasterDataProvider;
        }

        private void GetGDALOptions(string[] driverOptions, out double[] geoTransform, out string wktPrj)
        {
            geoTransform = null;
            wktPrj = "";
            for (int i = 0; i < driverOptions.Length; i++)
            {
                string option = driverOptions[i];
                string[] kv = option.Split('=');
                if (kv.Length == 2)
                {
                    switch (kv[0].ToLower())
                    {
                        case "wkt":
                            wktPrj = kv[1];
                            break;
                        case "geotransform":
                            TryParseGeoTransform(kv[1], out geoTransform);
                            break;
                        default:
                            break;
                    }
                }
            }
            return;
        }

        //1,1,12,12,.01,0.1
        private void TryParseGeoTransform(string p, out double[] geoTransform)
        {
            geoTransform = null;
            string[] geos = p.Split(',');
            if (geos.Length == 6)
            {
                geoTransform = new double[6];
                for (int i = 0; i < 6; i++)
                {
                    double result;
                    if (double.TryParse(geos[i], out result))
                        geoTransform[i] = result;
                }
            }
        }

        private void FillZero(Dataset ds,enumDataType dataType)
        {
            int xSize = ds.RasterXSize;
            byte[] buffer = new byte[xSize * 1 * DataTypeHelper.SizeOf(dataType)];
            GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            IntPtr ptr = handle.AddrOfPinnedObject();
            try
            {
                for (int i = 0; i < ds.RasterCount; i++)
                    for (int row = 0; row < ds.RasterYSize; row++)
                        ds.GetRasterBand(i + 1).WriteRaster(0, row, ds.RasterXSize, 1, ptr, xSize, 1, DataType.GDT_Byte, 0, 0);
            }
            finally
            {
                handle.Free();
            }
        }

        private DataType GetDataType(enumDataType dataType)
        {
            switch (dataType)
            {
                case enumDataType.Byte:
                    return DataType.GDT_Byte;
                case enumDataType.UInt16:
                    return DataType.GDT_UInt16;
                case enumDataType.Int16:
                    return DataType.GDT_Int16;
                case enumDataType.UInt32:
                    return DataType.GDT_UInt32;
                case enumDataType.Int32:
                    return DataType.GDT_Int32;
                case enumDataType.Float:
                    return DataType.GDT_Float32;
                case enumDataType.Double:
                    return DataType.GDT_Float64;
            }
            throw new NotSupportedException(dataType.ToString());
        }

        private string GetDriverName(object[] options,out string[] gdalOptions)
        {
            string gdalDriver = "";
            gdalOptions = null;
            if (options == null || options.Length == 0)
                return null;
            List<string> gdalOptionlst = new List<string>();
            foreach (string arg in options)
            {
                if (arg.ToUpper().Contains("DRIVERNAME"))
                {
                    string[] parts = arg.Split('=');
                    if (parts.Length < 2)
                        continue;
                    gdalDriver = parts[1];
                }
                else
                {
                    gdalOptionlst.Add(arg);
                }
            }
            gdalOptions = gdalOptionlst.ToArray();
            return gdalDriver;
        }

        public override IRasterDataProvider CreateCopy(string fileName, IRasterDataProvider dataProvider, params object[] options)
        {
            throw new NotImplementedException();
        }
    }
}
