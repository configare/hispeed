#region Version Info
/*========================================================================
* 功能概述：
* 
* 创建者：Zhangyb     时间：2014-1-7 09:23:20 
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
using GeoDo.RSS.Core.DF;
using GeoDo.HDF5;
using OSGeo.GDAL;
using GeoDo.RSS.DF.GDAL;
using System.Text.RegularExpressions;
using GeoDo.Project;
using System.IO;

namespace GeoDo.RSS.DF.GDAL.HDF5GEO
{
    /// <summary>
    /// 类名：IceConDataProvider
    /// 属性描述：极区海冰覆盖度日产品
    /// 创建者：Zhangyb   创建日期：2014-1-7 09:23:20 
    /// 修改者：             修改日期：
    /// 修改描述：
    /// 备注：
    /// </summary>
    public class IceConDataProvider :  RasterDataProvider
    {
        private object[] _args = null;
        private string[] _selectedsets =null;
        protected string[] _allGdalSubDatasets = null;
        protected const string NORTH_PROJ4 = "+proj=stere +lat_0=90 +lat_ts=70 +lon_0=-45 +k_0=1 +x_0=0 +y_0=0 +a=6378273 +b=6356889.449 +units=m +no_defs";
        protected const string SOUTH_PROJ4 = "+proj=stere +lat_0=-90 +lat_ts=-70 +lon_0=0 +k=1 +x_0=0 +y_0=0 +a=6378273 +b=6356889.449 +units=m +no_defs ";
        private CoordEnvelope _northEnvelope = new CoordEnvelope(-3850000, 3750000, -5350000, 5850000);
        private CoordEnvelope _southEnvelope = new CoordEnvelope(-3950000, 3950000, -3950000, 4350000);

        public IceConDataProvider(string fileName, byte[] header1024, IGeoDataDriver driver, params object[] args)
            : base(fileName, driver)
        {
            _args = args;
            TryParseArgs();
            using (Dataset dataset = Gdal.Open(fileName, Access.GA_ReadOnly))
            {
                GDALHelper.GetDatasetAttributes(dataset, _attributes);
            }
            Dictionary<string, string> allGdalSubDatasets = this.Attributes.GetAttributeDomain("SUBDATASETS");
            _allGdalSubDatasets = RecordAllSubDatasetNames(allGdalSubDatasets);
            TryCreateBandProvider();
            _bandCount = _rasterBands.Count;
            _dataType = _rasterBands[0].DataType;
            _width = _rasterBands[0].Width;
            _height = _rasterBands[0].Height;
            _coordType = _spatialRef.ProjectionCoordSystem != null ? enumCoordType.PrjCoord : enumCoordType.GeoCoord;
            _resolutionX = (float)(_coordEnvelope.Width / (_width));
            _resolutionY = (float)(_coordEnvelope.Height / (_height));
            if (_dataIdentify != null)
            {
                _dataIdentify.Sensor = "MWRI";
                _dataIdentify.OrbitDateTime = TryGetFileDate(Path.GetFileName(fileName));
            }
        }

        private void TryParseArgs()
        {
            if (_args == null || _args.Length == 0)
            {
                _selectedsets = new string[] { "icecon_north_asc"};
                TrySetSpatialRef(NORTH_PROJ4, _northEnvelope);
            }
            else
            {
                string[] selectedsets = TryGetSelectedSets();
                if (selectedsets.Length == 0)
                {
                    _selectedsets = new string[] { "icecon_north_asc" };
                    TrySetSpatialRef(NORTH_PROJ4, _northEnvelope);
                    return;
                }
                _selectedsets = selectedsets.ToArray();
                string ns = (_selectedsets[0].Substring(7, 5)).ToLower();
                switch (ns)
                {
                    case "north":
                        TrySetSpatialRef(NORTH_PROJ4,_northEnvelope);
                        break;
                    case "south":
                        TrySetSpatialRef(SOUTH_PROJ4, _southEnvelope);
                        break;
                }
            }
        }

        private string[] TryGetSelectedSets()
        {
            string[] alldatasets = new string[6] { "icecon_north_asc", "icecon_north_avg", "icecon_north_des", "icecon_south_asc", "icecon_south_avg", "icecon_south_des" };
            string[] arguments = _args as string[];
            List<string> datas = new List<string>();
            string[] parts;
            foreach (string set in arguments)
            {
                if (set.Contains("icecon_north") || set.Contains("icecon_south"))
                {
                    parts = set.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string part in parts)
                    {
                        foreach (string data in alldatasets)
                        {
                            if (data == part)
                            {
                                datas.Add(part);
                                break;
                            }
                        }
                    }
                }
            }
            return datas.ToArray();
        }

        private void TrySetSpatialRef(string PROJ4, CoordEnvelope envelop)
        {
            _spatialRef = SpatialReference.FromProj4(PROJ4);
            _coordEnvelope = envelop;
        }

        #region 设置波段
        private void TryCreateBandProvider()
        {
            foreach (string dsName in _selectedsets)
            {
                string dsPath = GetDatasetFullPath(dsName);
                Dataset dataset = Gdal.Open(dsPath, Access.GA_ReadOnly);
                IRasterBand[] gdalDatasets = ReadBandsFromDataset(dsName,dataset, this);
                _rasterBands.AddRange(gdalDatasets);
            }
        }

        private string[] RecordAllSubDatasetNames(Dictionary<string, string> subdatasets)
        {
            List<string> dss = new List<string>();
            int idx = 0;
            foreach (string key in subdatasets.Keys)
                if (idx++ % 2 == 0)
                    dss.Add(subdatasets[key]);
            return dss.ToArray();
        }

        private string GetDatasetFullPath(string datasetName)
        {
            for (int i = 0; i < _allGdalSubDatasets.Length; i++)
            {
                string shortGdalDatasetName = GetDatasetShortName(_allGdalSubDatasets[i]);
                if (shortGdalDatasetName == datasetName)
                    return _allGdalSubDatasets[i];
            }
            return null;
        }

        private string GetDatasetShortName(string datasetName)
        {
            string shortDatasetName = null;
            int groupIndex = datasetName.LastIndexOf("/");
            if (groupIndex == -1)
                shortDatasetName = datasetName;
            else
                shortDatasetName = datasetName.Substring(groupIndex + 1);
            return shortDatasetName;
        }
        
        private IRasterBand[] ReadBandsFromDataset(string dsname,Dataset ds, IRasterDataProvider provider)
        {
            int bandNo = 1;
            IRasterBand[] bands = new IRasterBand[ds.RasterCount];
            for (int i = 1; i <= ds.RasterCount; i++)
            {
                bands[i - 1] = new GDALRasterBand(provider, ds.GetRasterBand(i), new GDALDataset(ds));
                bands[i - 1].BandNo = bandNo++;
                bands[i - 1].Description = dsname;
            }
            return bands;
        }

        #endregion      
        
        public override void AddBand(enumDataType dataType)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 根据卫星传感器，获取默认显示的波段。
        /// </summary>
        /// <returns>返回的是映射后的bandNo</returns>
        public override int[] GetDefaultBands()
        {
            return new int[] { 1, 1, 1 };
        }

        public  static DateTime TryGetFileDate(string fname)
        {
            string date = "";
            foreach (Regex dt in DataReg)
            {
                Match m = dt.Match(fname);
                if (m.Success)
                {
                    date = m.Value;
                    int year = Int16.Parse(date.Substring(0, 4));
                    int month = Int16.Parse(date.Substring(4, 2));
                    int day = Int16.Parse(date.Substring(6, 2));
                    return new DateTime(year, month, day);
                }
            }
            return DateTime.Now;
        }

        private static Regex[] DataReg = new Regex[]
         {
            new Regex(@"(?<year>\d{4})(?<month>\d{2})(?<day>\d{2})",RegexOptions.Compiled),
          };

    }
}
