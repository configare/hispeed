using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using GeoDo.RSS.Core.DF;
using OSGeo.GDAL;
using GeoDo.HDF4;
using System.IO;

namespace GeoDo.RSS.DF.GDAL.HDF4Universal
{
    [Export(typeof(IBandProvider)), ExportMetadata("VERSION", "1")]
    public class BandProviderHDF4Universal : BandProvider
    {
        protected IRasterDataProvider _provider = null;
        protected Access _access = Access.GA_ReadOnly;
        protected string[] _datasetNames = null;
        protected string[] _allDatasetNames = null;

        public BandProviderHDF4Universal()
            : base()
        {
        }

        public BandProviderHDF4Universal(string[] datasets)
            : base()
        {
            _datasetNames = datasets;
        }

        public override void Init(string fname, enumDataProviderAccess access, IRasterDataProvider provider)
        {
            _provider = provider;
            _access = access == enumDataProviderAccess.ReadOnly ? Access.GA_ReadOnly : Access.GA_Update;

            if (_provider != null)
                _allDatasetNames = GetDatasetNames();
        }

        public override void Reset()
        {
            if (_provider != null)
            {
                _provider = null;
            }
        }

        public override IRasterBand[] GetDefaultBands()
        {
            return GetBandFromDatasetNames(_datasetNames);
        }

        private IRasterBand[] GetBandFromDatasetNames(string[] datasetNames)
        {
            if (datasetNames == null || datasetNames.Length == 0)
                return null;
            if (_allDatasetNames == null || _allDatasetNames.Length == 0)
                return null;

            List<IRasterBand> rasterBands = new List<IRasterBand>();
            foreach (string dsName in datasetNames)
            {
                string dsPath = GetDatasetFullPath(dsName);
                Dataset dataset = Gdal.Open(dsPath, _access);
                IRasterBand[] gdalDatasets = ReadBandsFromDataset(dataset, _provider);
                rasterBands.AddRange(gdalDatasets);
            }
            return rasterBands.Count == 0 ? null : rasterBands.ToArray();
        }

        public override IRasterBand[] GetBands(string datasetName)
        {
            if (string.IsNullOrEmpty(datasetName))
                return null;
            string dsFullPath = GetDatasetFullPath(datasetName);
            return GetBandsByFullName(dsFullPath);
        }

        private string GetDatasetFullPath(string datasetName)
        {
            List<string> allDatasetNames = new List<string>(_allDatasetNames);
            int index = allDatasetNames.IndexOf(datasetName);
            return string.Format("HDF4_SDS:UNKNOWN:\"{0}\":{1}", _provider.fileName, index);
        }

        private IRasterBand[] GetBandsByFullName(string dsFullPath)
        {
            if (string.IsNullOrEmpty(dsFullPath))
                return null;
            Dataset ds = Gdal.Open(dsFullPath, _access);
            return ReadBandsFromDataset(ds, _provider);
        }

        private IRasterBand[] ReadBandsFromDataset(Dataset ds, IRasterDataProvider provider)
        {
            if (ds == null || ds.RasterCount == 0)
                return null;
            IRasterBand[] bands = new IRasterBand[ds.RasterCount];
            for (int i = 1; i <= ds.RasterCount; i++)
                bands[i - 1] = new GDALRasterBand(provider, ds.GetRasterBand(i), new GDALDataset(ds));
            return bands;
        }

        public override string[] GetDatasetNames()
        {
            using (Hdf4Operator hdf4 = new Hdf4Operator(_provider.fileName))
            {
                return hdf4.GetDatasetNames;
            }
        }

        public override Dictionary<string, string> GetAttributes()
        {
            using(Hdf4Operator hdf4 = new Hdf4Operator(_provider.fileName))
            {
                return hdf4.GetAttributes();
            }
        }

        public override Dictionary<string, string> GetDatasetAttributes(string datasetName)
        {
            using(Hdf4Operator hdf4 = new Hdf4Operator(_provider.fileName))
            {
                return hdf4.GetAttributes(datasetName);
            }
        }

        public override bool IsSupport(string fname, byte[] header1024, Dictionary<string, string> datasetNames)
        {
            string ext = Path.GetExtension(fname).ToUpper();
            if (ext != ".HDF" || !HDF4Helper.IsHdf4(header1024))
                return false;
            return HasDatasets(fname);
        }

        private bool HasDatasets(string filename)
        {
            return _datasetNames != null && _datasetNames.Length != 0;
        }
    }
}
