using System;
using System.Collections.Generic;

namespace GeoDo.Smart.Tools.MODLSTPro
{
    public class HDFDatasetDef
    {
        private string _name = null;
        private HDFAttributeDefCollection _attCollection = null;
        private HDFAttributeDef _bandnameAtt = null;
        private Type _valueType = null;
        private bool _isBand = false;
        private bool _isVisibleBand = false;
        private bool _isInfraredBand = false;
        private bool _isCanDisplay = false;//是否能够显示，比如：太阳高度角等等，要投影校正，但是不显示

        public HDFDatasetDef()
        { }

        public HDFDatasetDef(string name, HDFAttributeDefCollection attCollection, HDFAttributeDef bandnameAtt, Type valueType)
        {
            _name = name;
            _attCollection = attCollection;
            _bandnameAtt = bandnameAtt;
            _valueType = valueType;
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public HDFAttributeDefCollection AttCollection
        {
            get { return _attCollection; }
            set { _attCollection = value; }
        }

        public HDFAttributeDef BandnameAtt
        {
            get { return _bandnameAtt; }
            set { _bandnameAtt = value; }
        }

        public Type ValueType
        {
            get { return _valueType; }
            set { _valueType = value; }
        }

        public bool IsBand
        {
            get { return _isBand; }
            set { _isBand = value; }
        }

        public bool IsVisibleBand
        {
            get { return _isVisibleBand; }
            set { _isVisibleBand = value; }
        }

        public bool IsInfraredBand
        {
            get { return _isInfraredBand; }
            set { _isInfraredBand = value; }
        }

        public bool IsCanDisplay
        {
            get { return _isCanDisplay; }
            set { _isCanDisplay = value; }
        }

        /// <summary>
        /// 处理为从0开始
        /// </summary>
        /// <returns></returns>
        public int[] GetBandIndexs()
        {
            if (_bandnameAtt == null)
                return null;
            List<int> bandIndexs = new List<int>();
            int bandindexValue = 0;
            foreach (string bandIndex in _bandnameAtt.Value.ToString().Split(','))
            {
                if (int.TryParse(bandIndex, out bandindexValue))
                    bandIndexs.Add(bandindexValue);
            }
            return bandIndexs.ToArray();
        }

        public override string ToString()
        {
            return !string.IsNullOrEmpty(_name) ?_name: base.ToString();
        }
    }
}
