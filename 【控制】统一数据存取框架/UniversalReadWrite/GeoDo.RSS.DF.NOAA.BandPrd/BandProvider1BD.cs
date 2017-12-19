using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using GeoDo.RSS.Core.DF;
using OSGeo.GDAL;
using GeoDo.RSS.DF.GDAL;
using GeoDo.RSS.DF.NOAA;

namespace GeoDo.RSS.DF.NOAA.BandPrd
{
    [Export(typeof(IBandProvider)), ExportMetadata("VERSION", "1")]
    public class BandProvider1BD:IBandProvider
    {
        protected IRasterDataProvider _provider = null;
        protected List<string> _datasetNames = new List<string>();
        protected DataIdentify _dataIdentify = new DataIdentify();
        Dictionary<string, string> _fileAttr = new Dictionary<string, string>();

        public BandProvider1BD()
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
            CreateFileAttr();
        }

        private void CreateFileAttr()
        {
            string dayOrnight = (_provider as ID1BDDataProvider).Header.NomalHeaderInfo.DayOrNight == 1 ? "D" : "N";
            _fileAttr.Add("DAYNIGHTFLAG", dayOrnight);
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
            SecondaryBand_1BD secBand = new SecondaryBand_1BD(_provider);
            secBand.BandName = datasetName;
            return new IRasterBand[] { secBand };
        }

        public string[] GetDatasetNames()
        {
            return _datasetNames.ToArray();
        }

        public Dictionary<string, string> GetAttributes()
        {
            return _fileAttr;
        }

        public Dictionary<string, string> GetDatasetAttributes(string datasetNames)
        {
            Dictionary<string, string> atr = new Dictionary<string, string>();
            return atr;
        }

        public bool IsSupport(string fname, byte[] header1024, Dictionary<string, string> datasetNames)
        {
            if (ToLocalEndian_Core.ToInt16FromBig(new byte[] { header1024[10], header1024[11] }) != 22016
                &&ToLocalEndian_Core.ToInt16FromLittle(new byte[] {header1024[10],header1024[11]})!=22016)
                return false;
            return true;
        }

        public void Dispose()
        {

        }
    }
}
