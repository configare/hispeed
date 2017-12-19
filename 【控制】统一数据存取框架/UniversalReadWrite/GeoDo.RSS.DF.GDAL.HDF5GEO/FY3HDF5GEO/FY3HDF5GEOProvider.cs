using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;
using OSGeo.GDAL;
using GeoDo.Project;
using System.IO;
using GeoDo.HDF5;

namespace GeoDo.RSS.DF.GDAL.HDF5GEO.FY3HDF5GEO
{
    public class FY3HDF5GEOProvider : RasterDataProvider
    {
        private object[] _args = null;
        private Dictionary<string, string> _fileAttributes = new Dictionary<string, string>();
        protected string[] _allGdalSubDatasets = null;
        private string[] _selectedsets = null;

        public FY3HDF5GEOProvider(string fileName, byte[] header1024, IGeoDataDriver driver, params object[] args)
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

        #region 从文件属性获取经纬度
        private CoordEnvelope TrySetGeoInfo()
        {
            using (Hdf5Operator oper = new Hdf5Operator(_fileName))
            {
                _fileAttributes = oper.GetAttributes();
            }
            if (_fileAttributes == null)
                return null;
            double lulat = 0, lulon = 0, rdlat = 0, rdlon = 0;
            double ldlat = 0, ldlon = 0, rulat = 0, rulon = 0;
            double lulatorbit=0,lulonorbit=0,rdlatorbit=0,rdlonorbit=0,ldlatorbit=0,ldlonorbit=0,rulatorbit=0,rulonorbit=0;
            string lulatstr = "Left-Top Latitude", lulonstr = "Left-Top Longitude", rdlatstr = "Right-Bottom Latitude", rdlonstr = "Right-Bottom Longitude";
            string ldlatstr = "Left-Bottom Latitude", ldlonstr = "Left-Bottom Longitude", rulatstr = "Right-Top Latitude", rulonstr = "Right-Top Longitude";
            string lulatstrOrbit = "Left-Top X", lulonstrOrbit = "Left-Top Y", rdlatstrOrbit = "Right-Bottom X", rdlonstrOrbit = "Right-Bottom Y";
            string ldlatstrOrbit = "Left-Bottom X", ldlonstrOrbit = "Left-Bottom Y", rulatstrOrbit = "Right-Top X", rulonstrOrbit = "Right-Top Y";
            foreach (KeyValuePair<string, string> fileAttribute in _fileAttributes)
            {
                if (fileAttribute.Key == lulatstr)
                {
                    if (!double.TryParse(fileAttribute.Value, out lulat))
                        return null;
                }
                else if (fileAttribute.Key == lulonstr)
                {
                    if (!double.TryParse(fileAttribute.Value, out lulon))
                        return null;
                }
                else if (fileAttribute.Key == rdlatstr)
                {
                    if (!double.TryParse(fileAttribute.Value, out rdlat))
                        return null;
                }
                else if (fileAttribute.Key == rdlonstr)
                {
                    if (!double.TryParse(fileAttribute.Value, out rdlon))
                        return null;
                }
                else if (fileAttribute.Key == ldlonstr)
                {
                    if (!double.TryParse(fileAttribute.Value, out ldlon))
                        return null;
                }
                else if (fileAttribute.Key == ldlatstr)
                {
                    if (!double.TryParse(fileAttribute.Value, out ldlat))
                        return null;
                }
                else if (fileAttribute.Key == rulonstr)
                {
                    if (!double.TryParse(fileAttribute.Value, out rulon))
                        return null;
                }
                else if (fileAttribute.Key == rulatstr)
                {
                    if (!double.TryParse(fileAttribute.Value, out rulat))
                        return null;
                }
                else if (fileAttribute.Key == lulatstrOrbit)
                {
                    if (!double.TryParse(fileAttribute.Value, out lulatorbit))
                        return null;
                }
                else if (fileAttribute.Key == lulonstrOrbit)
                {
                    if (!double.TryParse(fileAttribute.Value, out lulonorbit))
                        return null;
                }
                else if (fileAttribute.Key == ldlatstrOrbit)
                {
                    if (!double.TryParse(fileAttribute.Value, out ldlatorbit))
                        return null;
                }
                else if (fileAttribute.Key == ldlonstrOrbit)
                {
                    if (!double.TryParse(fileAttribute.Value, out ldlonorbit))
                        return null;
                }
                else if (fileAttribute.Key == rdlatstrOrbit)
                {
                    if (!double.TryParse(fileAttribute.Value, out rdlatorbit))
                        return null;
                }
                else if (fileAttribute.Key == rdlonstrOrbit)
                {
                    if (!double.TryParse(fileAttribute.Value, out rdlonorbit))
                        return null;
                }
                else if (fileAttribute.Key == rulatstrOrbit)
                {
                    if (!double.TryParse(fileAttribute.Value, out rulatorbit))
                        return null;
                }
                else if (fileAttribute.Key == rulonstrOrbit)
                {
                    if (!double.TryParse(fileAttribute.Value, out rulonorbit))
                        return null;
                }
            }
            double minlon, maxlon, minlat, maxlat;
            bool lonreverse = false, latreverse = false;
            if (lulon != 0 || rdlon != 0 || rdlat != 0 || lulat != 0 || ldlat != 0 || ldlon != 0 || rulat != 0 || rulon != 0)
            {
                if (lulon != 0 && rdlon != 0&&lulon > rdlon)
                    lonreverse = true;
                if (rdlat != 0 && lulat != 0 &&rdlat > lulat)
                    latreverse = true;
                if (!lonreverse)
                {
                    lulon = Math.Min(lulon, ldlon);
                    rdlon = Math.Max(rdlon, rulon);
                }
                else
                {
                    lulon = Math.Max(lulon, ldlon);
                    rdlon = Math.Min(rdlon, rulon);
                }
                if (!latreverse)
                {
                    lulat = Math.Max(lulat, rulat);
                    rdlat = Math.Min(ldlat, rdlat);
                }
                else
                {
                    lulat = Math.Min(lulat, rulat);
                    rdlat = Math.Max(rdlat, ldlat);
                }
                minlon = !lonreverse ? lulon : rdlon;
                maxlon = lonreverse ? lulon : rdlon;
                minlat = !latreverse ? rdlat : lulat;
                maxlat = latreverse ? rdlat : lulat;
            }
            else
            {
                lulon = lulonorbit < ldlonorbit ? lulonorbit : ldlonorbit;//左侧经度取小的
                lulat = lulatorbit > rulatorbit ? lulatorbit : rulatorbit;//上侧纬度取大的
                rdlon = rulonorbit > rdlonorbit ? rulonorbit : rdlonorbit;//右侧经度取大的
                rdlat = ldlatorbit < rdlatorbit ? ldlatorbit : rdlatorbit;//下侧纬度取小的
                minlon = lulon < rdlon ? lulon : rdlon;
                maxlon = lulon > rdlon ? lulon : rdlon;
                minlat = rdlat < lulat ? rdlat : lulat;
                maxlat = rdlat > lulat ? rdlat : lulat;
            }
            return new CoordEnvelope(minlon, maxlon, minlat, maxlat);
        }
        #endregion

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
            List<string> allsets = new List<string>();
            for (int i = 0; i < _allGdalSubDatasets.Length; i++)
            {
                string shortGdalDatasetName = GetDatasetShortName(_allGdalSubDatasets[i]);
                allsets.Add(shortGdalDatasetName);
            }
            return allsets.ToArray();
        }


    }

}
