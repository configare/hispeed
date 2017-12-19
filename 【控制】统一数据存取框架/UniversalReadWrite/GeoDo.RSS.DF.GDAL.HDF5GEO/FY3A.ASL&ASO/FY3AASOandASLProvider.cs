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

namespace GeoDo.RSS.DF.GDAL.HDF5GEO.FY3A.ASL_ASO
{
    public class FY3AASOandASLProvider : RasterDataProvider
    {
        private object[] _args = null;
        private Dictionary<string, string> _fileAttributes = new Dictionary<string, string>();
        protected string[] _allGdalSubDatasets = null;
        private string[] _selectedsets = null;
        public static string[][] _datasets = new string[][] {
            new string[] { "Aerosol_Angstrom_Coefficient","Aerosol_Optical_Thickness_of_MERSI_470nm","Aerosol_Optical_Thickness_of_MERSI_550nm","Aerosol_Optical_Thickness_of_MERSI_650nm","Aerosol_Small_Particle_Ratio","Aerosol_Type_Flag","Pixel_Scan_Time","Pixel_Sensor_Azimuth_Angle","Pixel_Sensor_Zenit_Angle","Pixel_Sun_Azimuth_Angle","Pixel_Sun_Zenit_Angle","Processing_Flags" }, 
             new string[] { "AOT_1030SDS","AOT_1640SDS","AOT_2130SDS","AOT_412SDS","AOT_443SDS","AOT_490SDS","AOT_520SDS","AOT_565SDS","AOT_650SDS","AOT_685SDS","AOT_765SDS","AOT_865SDS","AngstromSDS","L2_FlagsSDS","MinuteSDS","Sen_AzimuthSDS","Sen_ZenithSDS","Sun_AzimuthSDS","Sun_ZenithSDS" } 
        };

        public FY3AASOandASLProvider(string fileName, byte[] header1024, IGeoDataDriver driver, params object[] args)
            : base(fileName, driver)
        {
            _fileName = fileName;
            _args = args;
            using (Dataset dataset = Gdal.Open(fileName, Access.GA_ReadOnly))
            {
                GDALHelper.GetDatasetAttributes(dataset, _attributes);
            }
            Dictionary<string, string> allGdalSubDatasets = this.Attributes.GetAttributeDomain("SUBDATASETS");
            _allGdalSubDatasets = RecordAllSubDatasetNames(allGdalSubDatasets);
            _coordEnvelope =TrySetGeoInfo();
            _selectedsets = TryGetSelectedSets();
            TryCreateBandProvider();
            _bandCount = _rasterBands.Count;
            _dataType = _rasterBands[0].DataType;
            _width = _rasterBands[0].Width;
            _height = _rasterBands[0].Height;
            _spatialRef = SpatialReference.GetDefault();
            _coordType = _spatialRef.ProjectionCoordSystem != null ? enumCoordType.PrjCoord : enumCoordType.GeoCoord;
            _resolutionX = (float)(_coordEnvelope.Width / (_width));
            _resolutionY = (float)(_coordEnvelope.Height / (_height));
            if (_dataIdentify != null)
            {
                _dataIdentify.OrbitDateTime = IceConDataProvider.TryGetFileDate(Path.GetFileName(fileName));
            }
        }


        private CoordEnvelope TrySetGeoInfo()
        {
            using (Hdf5Operator oper = new Hdf5Operator(_fileName))
            {
                _fileAttributes = oper.GetAttributes();
            }            
            if (_fileAttributes == null)
                return null;
            double minX = 0d, maxX = 0d, minY = 0d, maxY = 0d;
            if (_fileAttributes.ContainsKey("Left-Bottom Latitude"))
            {
                string minLat = _fileAttributes["Left-Bottom Latitude"];
                if (!double.TryParse(minLat, out minY))
                {
                    return null;
                }
            }
            if (_fileAttributes.ContainsKey("Left-Bottom Longitude"))
            {
                string minlon = _fileAttributes["Left-Bottom Longitude"];
                if (!double.TryParse(minlon, out minX))
                    return null;
            }
            if (_fileAttributes.ContainsKey("Left-Top Latitude"))
            {
                string maxLat = _fileAttributes["Left-Top Latitude"];
                if (!double.TryParse(maxLat, out maxY))
                {
                    return null;
                }
            }
            if (_fileAttributes.ContainsKey("Left-Top Longitude"))
            {
                string maxLon = _fileAttributes["Right-Top Longitude"];
                if (!double.TryParse(maxLon, out maxX))
                    return null;
            }
            return new CoordEnvelope(minX, maxX, minY, maxY);
        }

        #region 设置波段
        private void TryCreateBandProvider()
        {
            foreach (string dsName in _selectedsets)
            {
                string dsPath = GetDatasetFullPath(dsName);
                Dataset dataset = Gdal.Open(dsPath, Access.GA_ReadOnly);
                IRasterBand[] gdalDatasets = ReadBandsFromDataset(dsName, dataset, this);
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

        private IRasterBand[] ReadBandsFromDataset(string dsname, Dataset ds, IRasterDataProvider provider)
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

        private string[] TryGetSelectedSets()
        {
            bool matched = false;
            foreach (String[] sets in _datasets)
            {
                foreach(string set in sets)
                {
                    matched = false;
                    for (int i = 0; i < _allGdalSubDatasets.Length; i++)
                    {
                        string shortGdalDatasetName = GetDatasetShortName(_allGdalSubDatasets[i]);
                        if (shortGdalDatasetName == set)
                        {
                            matched = true;
                        }
                    }
                    if (!matched)
                    {
                        break;
                    }
                }
                if(matched)
                    return sets;
            }
            return null;
        }

    }
}
