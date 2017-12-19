using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;

namespace GeoDo.FileProject
{
    public class BandMap
    {
        private IRasterDataProvider _file = null;
        private string _datasetName = null;
        private int _bandIndex = 0;

        public BandMap()
        { }

        public BandMap(IRasterDataProvider file, string datasetName, int bandIndex)
        {
            _file = file;
            _datasetName = datasetName;
            _bandIndex = bandIndex;
        }

        public IRasterDataProvider File
        {
            get { return _file; }
            set { _file = value; }
        }

        public string DatasetName
        {
            get { return _datasetName; }
            set { _datasetName = value; }
        }

        public int BandIndex
        {
            get { return _bandIndex; }
            set { _bandIndex = value; }
        }
    }
}
