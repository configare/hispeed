using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using System.IO;
using GeoDo.RSS.Core.DF;
using OSGeo.GDAL;

namespace GeoDo.RSS.DF.NOAA.BandPrd
{
    [Export(typeof(IBandProvider)), ExportMetadata("VERSION", "1")]
    public class BandProvider1A5 : IBandProvider
    {
        protected IRasterDataProvider _provider = null;
        protected List<string> _datasetNames = new List<string>();
        protected DataIdentify _dataIdentify = new DataIdentify();

        public BandProvider1A5()
        {

        }

        public DataIdentify DataIdentify
        {
            get { return _dataIdentify; }
            set { _dataIdentify = value; }
        }

        public void Init(string fname, enumDataProviderAccess access, IRasterDataProvider provider)
        {
            _provider = provider;
            _datasetNames.Add("Latitude");
            _datasetNames.Add("Longitude");
            _datasetNames.Add("SolarZenith");
            _datasetNames.Add("SatelliteZenith");
            _datasetNames.Add("RelativeAzimuth");
        }

        public void Reset()
        {
            _provider = null;
        }

        public IRasterBand[] GetDefaultBands()
        {
            return null;
        }

        public IRasterBand[] GetBands(string datasetName)
        {
            if (_datasetNames == null || _datasetNames.Count == 0 || string.IsNullOrEmpty(datasetName))
                return null;
            SecondaryBand_1A5 secBand = new SecondaryBand_1A5(_provider);
            secBand.BandName = datasetName;
            return new IRasterBand[] { secBand };
        }

        public string[] GetDatasetNames()
        {
            return _datasetNames.ToArray();
        }

        public Dictionary<string, string> GetAttributes()
        {
            Dictionary<string, string> atr = new Dictionary<string, string>();
            return atr;
        }

        public Dictionary<string, string> GetDatasetAttributes(string datasetNames)
        {
            Dictionary<string, string> atr = new Dictionary<string, string>();
            return atr;
        }

        public bool IsSupport(string fname, byte[] header1024,Dictionary<string,string> datasetNames)
        {
            //string fileExtension = Path.GetExtension(fname).ToUpper();
            //if (fileExtension != ".1A5")
                return false;
            //return true;
        }

        public void Dispose()
        {

        }
    }
}
