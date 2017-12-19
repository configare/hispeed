using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.HDF4;

namespace GeoDo.Smart.Tools.MODLSTPro
{
    public class DataSetMosaicInfo
    {
        private string _dataSetName;
        private float _offset = 0;
        private float _scale = 1;
        private int _height = 0;
        private int _width = 0;
        private string _bandnames;
        private float _minValue;
        private float _maxValue;
        private int _bandCount = 0;
        private HDF4Helper.DataTypeDefinitions _hdf4DataType = HDF4Helper.DataTypeDefinitions.DFNT_NUINT16;

        public DataSetMosaicInfo(string datasetname)
        {
            _dataSetName = datasetname;
        }

        public DataSetMosaicInfo(string datasetname, float scale, float offset, int height, int width, string bandnames, float minValue, float maxValue, int bandCount) :
            this(datasetname)
        {
            _scale = scale;
            _offset = offset;
            _height = height;
            _width = width;
            _bandnames = bandnames;
            _minValue = minValue;
            _maxValue = maxValue;
            _bandCount = bandCount;
        }

        public string DataSetName
        {
            get { return _dataSetName; }
        }

        public float Scale
        {
            get { return _scale; }
            set { _scale = value; }
        }

        public float Offset
        {
            get { return _offset; }
            set { _offset = value; }
        }

        public int Height
        {
            get { return _height; }
            set { _height = value; }
        }

        public int Width
        {
            get { return _width; }
            set { _width = value; }
        }

        public string BandNames
        {
            get { return _bandnames; }
            set { _bandnames = value; }
        }

        public float MinValue
        {
            get { return _minValue; }
            set { _minValue = value; }
        }

        public float MaxValue
        {
            get { return _maxValue; }
            set { _maxValue = value; }
        }

        public int BandCount
        {
            get { return _bandCount; }
            set { _bandCount = value; }
        }

        public HDF4Helper.DataTypeDefinitions HDF4DataType
        {
            get { return _hdf4DataType; }
            set { _hdf4DataType = value; }
        }
    }
}
