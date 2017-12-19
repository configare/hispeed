using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;
using System.ComponentModel.Composition;
using GeoDo.HDF5;
using System.IO;
using OSGeo.GDAL;
using GeoDo.RSS.DF.GDAL;

namespace GeoDo.RSS.DF.FY2NOM
{
    [Export(typeof(IBandProvider)), ExportMetadata("VERSION", "1")]
    internal class FY2NOMBandProvider : BandProvider
    {
        protected const string GDAL_SUBDATASETS_NAME = "SUBDATASETS";
        //可见光通道是用的byte存储，不能跟前四个通道放一起读取。
        protected string[] defaultBandnames = new string[] { "NOMChannelIR1", "NOMChannelIR2", "NOMChannelIR3", "NOMChannelIR4" };//, "NOMChannelVIS"
        protected IRasterDataProvider _provider = null;
        protected Access _access = Access.GA_ReadOnly;
        protected List<string> _datasetNames = new List<string>();
        protected string _gdalDatasetStrPrdfix = "";
        
        public FY2NOMBandProvider()
        {
        }

        public override void Init(string fname, enumDataProviderAccess access, IRasterDataProvider provider)
        {
            _provider = provider;
            _access = access == enumDataProviderAccess.ReadOnly ? Access.GA_ReadOnly : Access.GA_Update;
            Dictionary<string, string> subdatasets = _provider.Attributes.GetAttributeDomain(GDAL_SUBDATASETS_NAME);
            RecordAllSubDatasetNames(subdatasets);
            TryGetDef();
            _dataIdentify.Satellite = provider.DataIdentify.Satellite;
            _dataIdentify.Sensor = provider.DataIdentify.Sensor;
            provider.DataIdentify.IsOrbit = true;
        }

        private void TryGetDef()
        {
            _dataIdentify = new Core.DF.DataIdentify();
            _dataIdentify.IsOrbit = true;
        }

        private void RecordAllSubDatasetNames(Dictionary<string, string> subdatasets)
        {
            int idx = 0;
            foreach (string key in subdatasets.Keys)
                if (idx++ % 2 == 0)
                    _datasetNames.Add(subdatasets[key]);
            _gdalDatasetStrPrdfix = _datasetNames[0].Split(new string[] { "://" }, StringSplitOptions.None)[0];
        }

        public override void Reset()
        {
            if (_datasetNames != null)
            {
                _datasetNames.Clear();
                _datasetNames = null;
            }
            _provider = null;
        }

        public override IRasterBand[] GetDefaultBands()
        {
            int bandNo = 0;
            using (Hdf5Operator hdf = new Hdf5Operator(_provider.fileName))
            {
                List<IRasterBand> rasterBands = new List<IRasterBand>();
                foreach (string bandname in defaultBandnames)
                {
                    Dataset ds = Gdal.Open(ToDatasetFullName(bandname), _access);
                    IRasterBand[] rBands = ReadBandsFromGDAL(ds, _provider);
                    rasterBands.AddRange(rBands);
                    for (int i = 0; i < rBands.Length; i++)
                    {
                        rBands[i].Description = bandname;
                        rBands[i].BandNo = bandNo++;
                    }
                }
                rasterBands.Sort();
                return rasterBands.Count > 0 ? rasterBands.ToArray() : null;
            }
        }

        private string ToDatasetFullName(string datasetName)
        {
            return _gdalDatasetStrPrdfix + "://" + datasetName;
        }

        private IRasterBand[] ReadBandsFromGDAL(Dataset ds, IRasterDataProvider provider)
        {
            if (ds == null || ds.RasterCount == 0)
                return null;
            IRasterBand[] bands = new IRasterBand[ds.RasterCount];
            GDALDataset gdalDs = new GDALDataset(ds);
            for (int i = 1; i <= ds.RasterCount; i++)
            {
                Band band =ds.GetRasterBand(i);
                bands[i - 1] = new GDALRasterBand(provider, band, gdalDs);
            }
            return bands;
        }

        public override IRasterBand[] GetBands(string datasetName)
        {
            if (string.IsNullOrEmpty(datasetName))
                return null;
            IRasterBand[] bands = GetBandsByFullName(ToDatasetFullName(datasetName));
            if (bands != null)
                return bands;
            else
            {
                IRasterBand band = new FY2NOMBand(_provider, datasetName);
                if (band != null)
                    return new IRasterBand[] { band };
            }
            return null;
        }

        private IRasterBand[] GetBandsByFullName(string datasetName)
        {
            if (_datasetNames == null || _datasetNames.Count == 0 || string.IsNullOrEmpty(datasetName))
                return null;
            foreach (string dsname in _datasetNames)
            {
                if (string.IsNullOrEmpty(dsname))
                    continue;
                if (dsname.Contains(datasetName))
                {
                    Dataset ds = Gdal.Open(datasetName, _access);
                    return ReadBandsFromGDAL(ds, _provider);
                }
            }
            return null;
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
                return hdf5.GetAttributes(datasetName);
            }
        }

        public override bool IsSupport(string fname, byte[] header1024, Dictionary<string, string> datasetNames)
        {
            string ext = Path.GetExtension(fname).ToUpper();
            if (ext != ".HDF" || !HDF5Helper.IsHdf5(header1024))
                return false;
            using (Hdf5Operator hdf5 = new Hdf5Operator(fname))
            {
                if (hdf5 == null)
                    return false;
                string[] dss = hdf5.GetDatasetNames;
                if (dss == null)
                    return false;
                foreach (string bandname in defaultBandnames)
                {
                    if (!dss.Contains(bandname))
                        return false;
                }
            }
            TryGetDef();
            return true;
        }
    }
}
