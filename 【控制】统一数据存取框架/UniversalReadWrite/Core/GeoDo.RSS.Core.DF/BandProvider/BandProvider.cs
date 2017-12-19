using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.Core.DF
{
    public abstract class BandProvider:IBandProvider
    {
        protected DataIdentify _dataIdentify;

        public BandProvider()
        {
        }

        public DataIdentify DataIdentify
        {
            get { return _dataIdentify; }
            set { _dataIdentify = value; }
        }

        public abstract void Init(string fname, enumDataProviderAccess access, IRasterDataProvider provider);

        public abstract void Reset();

        public abstract IRasterBand[] GetDefaultBands();

        public abstract IRasterBand[] GetBands(string datasetName);

        public abstract string[] GetDatasetNames();

        public abstract Dictionary<string, string> GetAttributes();

        public abstract Dictionary<string,string> GetDatasetAttributes(string datasetName);

        public abstract bool IsSupport(string fname, byte[] header1024,Dictionary<string,string> datasetNames);

        public virtual void Dispose() { }
    }
}
