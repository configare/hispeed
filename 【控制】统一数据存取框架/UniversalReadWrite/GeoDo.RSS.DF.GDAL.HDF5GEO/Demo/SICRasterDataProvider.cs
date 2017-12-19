/* 产品描述
 * FY3B_MWRIX_GBAL_L2_SIC_MLT_PSG_20131006_AOAD_012KM_MS.HDF
 * http://satellite.cma.gov.cn/PortalSite/Data/Satellite.aspx
 * 海冰监测(SIC)
 * MWRI极区海冰覆盖度产品是指在12.5km分辨率通用横球面投影网格内，海冰面积在该网格总面积之中所占比例。用0-100表示0%-100%的海冰覆盖度，无量纲。
 * 
 * 该产品参考了NSIDC网站SeaICE产品：
 * http://nsidc.org/data/docs/daac/ae_si12_12km_tb_sea_ice_and_snow.gd.html
 */
/* 投影参数
 * EPSG:3411: NSIDC Sea Ice Polar Stereographic North
 * http://spatialreference.org/ref/epsg/3411/
 * ESRI WKT：
 * PROJCS["NSIDC Sea Ice Polar Stereographic North",GEOGCS["Unspecified datum based upon the Hughes 1980 ellipsoid",DATUM["D_",SPHEROID["Hughes_1980",6378273,298.279411123064]],PRIMEM["Greenwich",0],UNIT["Degree",0.017453292519943295]],PROJECTION["Stereographic_North_Pole"],PARAMETER["standard_parallel_1",70],PARAMETER["central_meridian",-45],PARAMETER["scale_factor",1],PARAMETER["false_easting",0],PARAMETER["false_northing",0],UNIT["Meter",1]]
 * proj4：
 * +proj=stere +lat_0=90 +lat_ts=70 +lon_0=-45 +k=1 +x_0=0 +y_0=0 +a=6378273 +b=6356889.449 +units=m +no_defs 
 * 
 * EPSG:3412: NSIDC Sea Ice Polar Stereographic South 
 * http://spatialreference.org/ref/epsg/3412/
 * proj4：
 * +proj=stere +lat_0=-90 +lat_ts=-70 +lon_0=0 +k=1 +x_0=0 +y_0=0 +a=6378273 +b=6356889.449 +units=m +no_defs 
 * ESRI WKT：
 * PROJCS["NSIDC Sea Ice Polar Stereographic South",GEOGCS["Unspecified datum based upon the Hughes 1980 ellipsoid",DATUM["D_",SPHEROID["Hughes_1980",6378273,298.279411123064]],PRIMEM["Greenwich",0],UNIT["Degree",0.017453292519943295]],PROJECTION["Stereographic_South_Pole"],PARAMETER["standard_parallel_1",-70],PARAMETER["central_meridian",0],PARAMETER["scale_factor",1],PARAMETER["false_easting",0],PARAMETER["false_northing",0],UNIT["Meter",1]]
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.HDF5;
using GeoDo.RSS.Core.DF;
using OSGeo.GDAL;
using GeoDo.Project;

namespace GeoDo.RSS.DF.GDAL.MWRIX.SIC
{
    /// <summary>
    /// 风三MWRIX传感器SIC产品数据读取器
    /// "icecon_north_asc","icecon_north_avg","icecon_north_des"
    /// "icecon_south_asc", "icecon_south_avg", "icecon_south_des"
    /// 使用此读取器时候，可以使用如下参数
    /// bandname=""
    /// selectedbands=1,2,3;可选。
    /// </summary>
    public class SICRasterDataProvider : RasterDataProvider
    {
        protected Access _access = Access.GA_ReadOnly;
        protected string[] _allGdalSubDatasets = null;
        protected string _args;
        protected const string NORTH_PROJ4 = "+proj=stere +lat_0=90 +lat_ts=70 +lon_0=-45 +k_0=1 +x_0=0 +y_0=0 +a=6378273 +b=6356889.449 +units=m +no_defs";
        protected const string SOUTH_PROJ4 = "+proj=stere +lat_0=-90 +lat_ts=-70 +lon_0=0 +k=1 +x_0=0 +y_0=0 +a=6378273 +b=6356889.449 +units=m +no_defs ";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="header1024"></param>
        /// <param name="driver"></param>
        /// <param name="access"></param>
        /// <param name="args">
        /// selectedband=1,2,3
        /// 
        /// </param>
        public SICRasterDataProvider(string fileName, byte[] header1024, IGeoDataDriver driver, enumDataProviderAccess access, params object[] args)
            : base(fileName, driver)
        {
            _access = (access == enumDataProviderAccess.ReadOnly ? Access.GA_ReadOnly : Access.GA_Update);
            TryParseArgs(args);
            //通过GDAL获取GDAL识别的通道 
            using (Dataset _dataset = Gdal.Open(fileName, Access.GA_ReadOnly))
            {
                GDALHelper.GetDatasetAttributes(_dataset, _attributes);
            }
            Dictionary<string, string> allGdalSubDatasets = _attributes.GetAttributeDomain("SUBDATASETS");
            _allGdalSubDatasets = RecordAllSubDatasetNames(allGdalSubDatasets);

            //...根据参数取得当前需要的数据集
            //如果是北极
            string[] currentDsNames = new string[] { _allGdalSubDatasets[0], _allGdalSubDatasets[1], _allGdalSubDatasets[2] };
            //如果是南极
            currentDsNames = new string[] { _allGdalSubDatasets[3], _allGdalSubDatasets[4], _allGdalSubDatasets[5] };
            //设置_rasterBands
            List<IRasterBand> rasterBands = new List<IRasterBand>();
            foreach (string dsName in currentDsNames)
            {
                //这里测试时候看dsName是否是全路径，如果不是需要用全路径
                string dsFullPath = dsName;
                Dataset ds = Gdal.Open(dsFullPath, _access);
                IRasterBand[] gdalDatasets = ReadBandsFromDataset(ds, this);
                rasterBands.AddRange(gdalDatasets);
            }
            _rasterBands = rasterBands;
            _bandCount = 3;
            _dataType = rasterBands[0].DataType;
            _width = rasterBands[0].Width;
            _height = rasterBands[0].Height;
            _resolutionX = 12500f;
            _resolutionY = 12500f;
            //设置坐标
            _coordType = enumCoordType.PrjCoord;
            //如果是北极
            _coordEnvelope = new Core.DF.CoordEnvelope(-3850000, 3750000, -5350000, 5850000);
            _spatialRef = SpatialReference.FromProj4(NORTH_PROJ4);
            //如果是南极
            _coordEnvelope = new Core.DF.CoordEnvelope(-3950000, 3950000, -3950000, 4350000);
            _spatialRef = SpatialReference.FromProj4(SOUTH_PROJ4);
        }

        private void TryParseArgs(object[] args)
        {
            if (args == null)
                return;
            string arg = null;
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == null || !(args[i] is string))
                    continue;
                arg = args[i] as string;
                arg.Contains("BandName");
            }
        }

        private static string[] RecordAllSubDatasetNames(Dictionary<string, string> subdatasets)
        {
            List<string> dss = new List<string>();
            int idx = 0;
            foreach (string key in subdatasets.Keys)
                if (idx++ % 2 == 0)
                    dss.Add(subdatasets[key]);
            return dss.ToArray();
        }

        private IRasterBand[] ReadBandsFromDataset(Dataset ds, IRasterDataProvider provider)
        {
            if (ds == null || ds.RasterCount == 0)
                return null;
            int bandNo = 1;
            IRasterBand[] bands = new IRasterBand[ds.RasterCount];
            for (int i = 1; i <= ds.RasterCount; i++)
            {
                bands[i - 1] = new GDALRasterBand(provider, ds.GetRasterBand(i), new GDALDataset(ds));
                bands[i - 1].BandNo = bandNo++;
            }
            return bands;
        }

        public override IRasterBand GetRasterBand(int bandNo)
        {
            return _rasterBands[bandNo - 1];
        }

        public override void AddBand(enumDataType dataType)
        {
            throw new NotImplementedException();
        }

        public override object GetStretcher(int bandNo)
        {
            object stratcher = RgbStretcherFactory.GetStretcher("MWRIX.SIC");
            if (stratcher == null)
                stratcher = RgbStretcherFactory.CreateStretcher(enumDataType.Byte, 0, 100);
            return stratcher;
        }

        public override int[] GetDefaultBands()
        {
            return new int[] { 1, 1, 1 };
        }

        #region static IsSupport
        /// <summary>
        /// 这里增加一个判断一个文件是否该驱动支持的静态方法
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        internal static bool IsSupport(string fileName)
        {
            try
            {
                AttributeManager attributes = new AttributeManager();
                using (Dataset dataset = Gdal.Open(fileName, Access.GA_ReadOnly))
                {
                    GDALHelper.GetDatasetAttributes(dataset, attributes);
                }
                Dictionary<string, string> allGdalSubDatasets = attributes.GetAttributeDomain("SUBDATASETS");
                string[] allGdalSubDatasetArray = RecordAllSubDatasetNames(allGdalSubDatasets);
                if (allGdalSubDatasetArray == null || allGdalSubDatasetArray.Length != 6)
                    return false;
                using(Hdf5Operator hdf = new Hdf5Operator(fileName))
                {
                    allGdalSubDatasetArray = hdf.GetDatasetNames;
                }
                string[] DATASETS = new string[] { "icecon_north_asc", "icecon_north_avg", "icecon_north_des", "icecon_south_asc", "icecon_south_avg", "icecon_south_des" };
                for (int i = 0; i < 6; i++)
                {
                    if (!allGdalSubDatasetArray[i].Contains(DATASETS[i]))//数据集必须完全匹配，才能用此数据集
                        return false;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion
    }
}
