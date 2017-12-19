using System.Collections.Generic;
using GeoDo.HDF5;

namespace GeoDo.Smart.Tools.MODLSTPro
{
    public class HDFDef
    {
        public HDFAttributeDefCollection AttCollection = null;
        public HDFAttributeDef LeftTopLonAtt = null;
        public HDFAttributeDef LeftTopLatAtt = null;
        public HDFAttributeDef LeftBottomLonAtt = null;
        public HDFAttributeDef LeftBottomLatAtt = null;
        public HDFAttributeDef RightTopLonAtt = null;
        public HDFAttributeDef RightTopLatAtt = null;
        public HDFAttributeDef RightBottomLonAtt = null;
        public HDFAttributeDef RightBottomLatAtt = null;
        public HDFAttributeDef CenterLonAtt = null;
        public HDFAttributeDef CenterLatAtt = null;
        public HDFAttributeDef ProjectAtt = null;      
        public HDFAttributeDef ResolutionLonAtt = null;
        public HDFAttributeDef ResolutionLatAtt = null;
        public HDFAttributeDef UnitOfResolution = null;  
        private List<HDFDatasetDef> _datasets = null;
        private List<HDFDatasetDef> _bandDatasets = null;
        private EnumEndian _endian = EnumEndian.LITTLE;//默认小端序
        private EnumEndian _attEndian = EnumEndian.LITTLE;//默认小端序

        public HDFDef()
        {
            _datasets = new List<HDFDatasetDef>();
            _bandDatasets = new List<HDFDatasetDef>();
        }

        public HDFDatasetDef[] Datasets
        {
            get { return _datasets != null ? _datasets.ToArray() : null; }
            set 
            {
                _datasets.Clear();
                _datasets.AddRange(value); 
            }
        }

        public HDFDatasetDef[] BandDatasets
        {
            get { return _bandDatasets != null ? _bandDatasets.ToArray() : null; }
            set
            {
                if (value == null)
                    _bandDatasets = null;
                _bandDatasets.Clear();
                _bandDatasets.AddRange(value); 
            }
        }

        public EnumEndian Endian
        {
            get { return _endian; }
            set { _endian = value; }
        }

        /// <summary>
        /// HDF文件属性值的端序
        /// </summary>
        public EnumEndian AttEndian
        {
            get { return _attEndian; }
            set { _attEndian = value; }
        }

        public HDFDatasetDef GetDatasetDef(string dataSetName)
        {
            if (_bandDatasets == null)
                return null;
            foreach (HDFDatasetDef hdfDatasetDef in _datasets)
            {
                if (hdfDatasetDef.Name.ToUpper() == dataSetName.ToUpper())
                {
                    return hdfDatasetDef;
                }
            }
            return null;
        }

        /// <summary>
        /// 获取可见光波段数据集
        /// </summary>
        /// <returns></returns>
        public HDFDatasetDef[] GetVisibleBand()
        {
            List<HDFDatasetDef> visBand = new List<HDFDatasetDef>();
            foreach (HDFDatasetDef ds in _datasets)
            {
                if (ds.IsVisibleBand)
                    visBand.Add(ds);
            }
            return visBand.ToArray();
        }

        /// <summary>
        /// 获取红外波段数据集
        /// </summary>
        /// <returns></returns>
        public HDFDatasetDef[] GetInfraredBand()
        {
            List<HDFDatasetDef> visBand = new List<HDFDatasetDef>();
            foreach (HDFDatasetDef ds in _datasets)
            {
                if (ds.IsInfraredBand)
                    visBand.Add(ds);
            }
            return visBand.ToArray();
        }
    }
}
