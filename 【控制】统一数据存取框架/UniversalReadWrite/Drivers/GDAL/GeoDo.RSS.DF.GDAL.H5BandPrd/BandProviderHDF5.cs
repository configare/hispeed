using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OSGeo.GDAL;
using GeoDo.RSS.Core.DF;
using System.ComponentModel.Composition;
using GeoDo.HDF5;
using System.IO;

namespace GeoDo.RSS.DF.GDAL.H5BandPrd
{
    [Export(typeof(IBandProvider)), ExportMetadata("VERSION", "1")]
    internal class BandProviderHDF5 : BandProvider
    {
        protected IRasterDataProvider _provider = null;
        protected Access _access = Access.GA_ReadOnly;
        protected const string GDAL_SUBDATASETS_NAME = "SUBDATASETS";
        protected BandProviderDef _matchedBandProviderDef = null;
        protected List<string> _allGdalSubDatasetFullpaths = new List<string>();
        protected List<string> _allSubDatasets = new List<string>();

        public BandProviderHDF5()
        {

        }

        public override void Init(string fname, enumDataProviderAccess access, IRasterDataProvider provider)
        {
            _provider = provider;
            _access = access == enumDataProviderAccess.ReadOnly ? Access.GA_ReadOnly : Access.GA_Update;
            Dictionary<string, string> subdatasets = _provider.Attributes.GetAttributeDomain(GDAL_SUBDATASETS_NAME);
            _allSubDatasets = GetDatasetNames(subdatasets);
            RecordAllSubDatasetNames(subdatasets);
            TryGetBandProviderDefinition(fname, subdatasets);
        }

        private void TryGetBandProviderDefinition(string fname, Dictionary<string, string> subdatasets)
        {
            List<string> datasetNames = null;
            if (subdatasets != null)
                datasetNames = GetDatasetNames(subdatasets);
            BandProviderDef[] bandProviderDefs = null;
            string configfile = Path.Combine(Path.GetDirectoryName(this.GetType().Assembly.Location), "GeoDo.RSS.DF.GDAL.H5BandPrd.xml");
            using (H5BandProviderXmlParser xml = new H5BandProviderXmlParser(configfile))
            {
                bandProviderDefs = xml.GetBandProviderDefs();
            }
            if (bandProviderDefs == null)
                return;
            string orbitDirection = "";
            string dayornight = "";
            using (Hdf5Operator hdf5 = new Hdf5Operator(fname))
            {
                foreach (BandProviderDef prddef in bandProviderDefs)
                {
                    bool isMatched = true;
                    if (datasetNames != null)
                    {
                        foreach (DefaultBandDatasetDef bandDef in prddef.DefaultBandDatasetDefs)
                        {
                            if (!datasetNames.Contains(bandDef.Name))
                            {
                                isMatched = false;
                                break;
                            }
                        }
                        if (!isMatched)
                            continue;
                    }
                    foreach (IdentifyAttDef id in prddef.IdentifyAttDefs)
                    {
                        string attvalue = hdf5.GetAttributeValue(id.Name);
                        if (attvalue != id.Value)
                        {
                            isMatched = false;
                            break;
                        }
                    }
                    if (isMatched)
                    {
                        _matchedBandProviderDef = prddef;
                        break;
                    }
                }
                orbitDirection = hdf5.GetAttributeValue("Orbit Direction");
                dayornight = hdf5.GetAttributeValue("Day Or Night Flag");
            }
            if (_matchedBandProviderDef != null)
            {
                _dataIdentify = new DataIdentify(_matchedBandProviderDef.Satellite, _matchedBandProviderDef.Sensor);
                _dataIdentify.IsOrbit = true;
                TryGetOrbitDirection(orbitDirection);
                TryGetDayOrNight(dayornight);
            }
        }

        private void TryGetDayOrNight(string orbitDirection)
        {
            if (orbitDirection == "D" || orbitDirection == "Day" || orbitDirection == "DAY")
                _dataIdentify.DayOrNight = enumDayOrNight.Day;
            else if (orbitDirection == "N" || orbitDirection == "Night" || orbitDirection == "NIGHT")
                _dataIdentify.DayOrNight = enumDayOrNight.Night;
        }

        private void TryGetOrbitDirection(string orbitDirection)
        {
            if (orbitDirection == "A")
                _dataIdentify.IsAscOrbitDirection = true;
            else
                _dataIdentify.IsAscOrbitDirection = false;
        }

        public override void Reset()
        {
            if (_allGdalSubDatasetFullpaths != null)
            {
                _allGdalSubDatasetFullpaths.Clear();
                _allGdalSubDatasetFullpaths = null;
            }
            _provider = null;
            _matchedBandProviderDef = null;
        }

        private List<string> GetDatasetNames(Dictionary<string, string> subdatasets)
        {
            List<string> datasetNames = new List<string>();
            int idx = 0;
            foreach (string key in subdatasets.Keys)
            {
                if (idx++ % 2 == 0)
                {
                    string[] parts = subdatasets[key].Replace(']',' ').Replace("://", "$").Split('$');
                    if (parts.Length == 2)
                        datasetNames.Add(parts[1].Trim());
                }
            }
            return datasetNames;
        }

        private void RecordAllSubDatasetNames(Dictionary<string, string> subdatasets)
        {
            int idx = 0;
            foreach (string key in subdatasets.Keys)
                if (idx++ % 2 == 0)
                    _allGdalSubDatasetFullpaths.Add(subdatasets[key]);
        }

        public override string[] GetDatasetNames()
        {
            using (Hdf5Operator hdf5 = new Hdf5Operator(_provider.fileName))
            {
                return hdf5.GetDatasetNames;
            }
        }

        public override Dictionary<string, string> GetAttributes()
        {
            using (Hdf5Operator hdf5 = new Hdf5Operator(_provider.fileName))
            {
                return hdf5.GetAttributes();
            }
        }

        public override Dictionary<string, string> GetDatasetAttributes(string datasetName)
        {
            using (Hdf5Operator hdf5 = new Hdf5Operator(_provider.fileName))
            {
                string dsFullPath = GetDatasetFullNames(datasetName);
                return hdf5.GetAttributes(dsFullPath);
            }
        }

        public override IRasterBand[] GetDefaultBands()
        {
            if (_matchedBandProviderDef == null || _matchedBandProviderDef.DefaultBandDatasetDefs == null || _matchedBandProviderDef.DefaultBandDatasetDefs.Count == 0)
                return null;
            IBandNameParser bandNameParser = new DefaultBandNameParser();
            using (Hdf5Operator hdf = new Hdf5Operator(_provider.fileName))
            {
                List<IRasterBand> rasterBands = new List<IRasterBand>();
                foreach (DefaultBandDatasetDef dsdef in _matchedBandProviderDef.DefaultBandDatasetDefs)
                {
                    string bandNos = hdf.GetAttributeValue(dsdef.Name, dsdef.BandNoAttribute);
                    BandName[] bNames = bandNameParser.Parse(bandNos);
                    string datasetName = dsdef.Name;
                    string dsFullPath = GetDatasetFullPath(datasetName);
                    IRasterBand[] rBands = GetBandsByFullName(dsFullPath);
                    rasterBands.AddRange(rBands);
                    if (bNames == null || bNames.Length != rBands.Length)
                    {
                        continue;
                    }
                    else
                    {
                        for (int i = 0; i < rBands.Length; i++)
                        {
                            rBands[i].Description = bNames[i].Name;
                            rBands[i].BandNo = bNames[i].Index;
                        }
                    }
                }
                rasterBands.Sort();
                return rasterBands.Count > 0 ? rasterBands.ToArray() : null;
            }
        }

        //private string ToDatasetFullName(string datasetName)
        //{
        //    return _datasetNames[0].Split(new string[] { "://" }, StringSplitOptions.None)[0] + "://" + datasetName;
        //}
        /// <summary>
        /// 更新获取数据集的方法，以支持存在Group的数据集。
        /// 返回:HDF5:....://...
        /// </summary>
        /// <param name="datasetName"></param>
        /// <returns></returns>
        private string GetDatasetFullPath(string datasetName)
        {
            string shortDatasetName = GetDatasetShortName(datasetName);
            return FindGdalSubDataset(shortDatasetName);
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

        private string FindGdalSubDataset(string datasetShortName)
        {
            for (int i = 0; i < _allGdalSubDatasetFullpaths.Count; i++)
            {
                string shortGdalDatasetName = GetDatasetShortName(_allGdalSubDatasetFullpaths[i]);
                if (shortGdalDatasetName == datasetShortName)
                    return _allGdalSubDatasetFullpaths[i];
            }
            return null;
        }

        private string GetDatasetFullNames(string datasetName)
        {
            string shortDatasetName = GetDatasetShortName(datasetName);
            return FindDatasetFullNames(shortDatasetName);
        }

        private string FindDatasetFullNames(string shortDatasetName)
        {
            for (int i = 0; i < _allGdalSubDatasetFullpaths.Count; i++)
            {
                string shortGdalDatasetName = GetDatasetShortName(_allSubDatasets[i]);
                if (shortGdalDatasetName == shortDatasetName)
                    return _allSubDatasets[i];
            }
            return null;
        }

        private IRasterBand[] GetBandsByFullName(string dsFullPath)
        {
            if (string.IsNullOrEmpty(dsFullPath))
                return null;
            Dataset ds = Gdal.Open(dsFullPath, _access);
            return ReadBandsFromDataset(ds, _provider);
        }

        public override IRasterBand[] GetBands(string datasetName)
        {
            if (string.IsNullOrEmpty(datasetName))
                return null;
            string dsFullPath = GetDatasetFullPath(datasetName);
            return GetBandsByFullName(dsFullPath);
            //return GetBandsByFullName(ToDatasetFullName(datasetName));
        }

        //private IRasterBand[] GetBandsByFullName(string datasetName)
        //{
        //    if (_allGdalSubDatasets == null || _allGdalSubDatasets.Count == 0 || string.IsNullOrEmpty(datasetName))
        //        return null;
        //    foreach (string dsname in _allGdalSubDatasets)
        //    {
        //        if (string.IsNullOrEmpty(dsname))
        //            continue;
        //        if (dsname.Contains(datasetName))
        //        {
        //            Dataset ds = Gdal.Open(datasetName, _access);

        //            return ReadBandsFromDataset(ds, _provider);
        //        }
        //    }
        //    return null;
        //}

        private IRasterBand[] ReadBandsFromDataset(Dataset ds, IRasterDataProvider provider)
        {
            if (ds == null || ds.RasterCount == 0)
                return null;
            IRasterBand[] bands = new IRasterBand[ds.RasterCount];
            for (int i = 1; i <= ds.RasterCount; i++)
                bands[i - 1] = new GDALRasterBand(provider, ds.GetRasterBand(i), new GDALDataset(ds));
            return bands;
        }

        public override bool IsSupport(string fname, byte[] header1024, Dictionary<string, string> datasetNames)
        {
            string ext = Path.GetExtension(fname).ToUpper();
            if (ext != ".HDF" || !HDF5Helper.IsHdf5(header1024))
                return false;
            TryGetBandProviderDefinition(fname, datasetNames);
            return _matchedBandProviderDef != null;
        }

        public override void Dispose()
        {
            base.Dispose();
            if (_allGdalSubDatasetFullpaths != null)
            {
                _allGdalSubDatasetFullpaths.Clear();
                _allGdalSubDatasetFullpaths = null;
            }
            _provider = null;
            _matchedBandProviderDef = null;
        }
    }
}
