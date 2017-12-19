using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OSGeo.GDAL;
using GeoDo.RSS.Core.DF;
using System.ComponentModel.Composition;
using GeoDo.HDF5;
using System.IO;
using System.Xml.Linq;
using System.Text.RegularExpressions;

namespace GeoDo.RSS.DF.GDAL.HDF5Universal
{
    /// <summary>
    /// 功能描述：通用hdf5格式读取
    /// 读取文件下生成的*****.h5band.xml中记录的数据集信息,并打开
    /// 修改记录：
    /// 2013-8-19 更新功能实现，去除.h5band.xml记录数据集信息的读取方式，加入数据驱动，驱动中直接定义要读取的数据集信息
    /// 2013-11-26
    ///     GDAl在处理HDF的数据集时候，数据集名字中的"空格"会被替换为"_"
    /// </summary>
    [Export(typeof(IBandProvider)), ExportMetadata("VERSION", "1")]
    public class BandProviderHDF5Universal : BandProvider
    {
        //protected const string GDAL_SUBDATASETS_NAME = "SUBDATASETS";
        protected IRasterDataProvider _provider = null;
        protected Access _access = Access.GA_ReadOnly;
        protected string[] _datasetNames = null;
        //protected string[] _allDatasetNames = null;
        //所有gdal的SUBDATASETS
        protected string[] _allGdalSubDatasets = null;
        protected string[] _gdalDatasets = null;
        private string _lonDatasetName = null;
        private string _latDatasetName = null;

        public BandProviderHDF5Universal()
            : base()
        {
        }

        public BandProviderHDF5Universal(string[] datasets, string lonDatasetName,string latDatasetName)
            : base()
        {
            _datasetNames = datasets;
            ToGdalDatasetsName(_datasetNames);
            _lonDatasetName = lonDatasetName;
            _latDatasetName = latDatasetName;
        }

        /// <summary>
        /// 将hdf数据集处理成GDAl的数据集名称（即将空格处理为"_"）
        /// </summary>
        /// <param name="datasetNames"></param>
        private void ToGdalDatasetsName(string[] datasetNames)
        {
            if (datasetNames == null)
                return;
            Regex reg = new Regex("\\s+", RegexOptions.Compiled);
            for (int i = 0; i < datasetNames.Length; i++)
            {
                datasetNames[i] = reg.Replace(datasetNames[i], "_"); //_allDatasetNames[i].Replace(" ", "_");
            }
        }

        public override void Init(string fname, enumDataProviderAccess access, IRasterDataProvider provider)
        {
            _provider = provider;
            _access = access == enumDataProviderAccess.ReadOnly ? Access.GA_ReadOnly : Access.GA_Update;
            if (_provider != null)
            {
                Dictionary<string, string> allGdalSubDatasets = _provider.Attributes.GetAttributeDomain("SUBDATASETS");
                _allGdalSubDatasets = RecordAllSubDatasetNames(allGdalSubDatasets);
                //_allDatasetNames = GetDatasetNames();
                //ToGdalDatasetsName(_allDatasetNames);
            }
        }

        private string[] RecordAllSubDatasetNames(Dictionary<string, string> subdatasets)
        {
            List<string> dss = new List<string>();
            int idx = 0;
            foreach (string key in subdatasets.Keys)
                if (idx++ % 2 == 0)
                    dss.Add(subdatasets[key]);
            return dss.ToArray();
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
            if (_allGdalSubDatasets == null || _allGdalSubDatasets.Length == 0)
                return null;
            List<IRasterBand> rasterBands = new List<IRasterBand>();
            foreach (string dsName in datasetNames)
            {
                string dsPath = GetDatasetFullPath(dsName);
                if (string.IsNullOrWhiteSpace(dsPath))
                    throw new Exception(string.Format("文件中不存在指定的数据集[{0}]{1}", dsName,_provider.fileName));
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
            if(datasetName=="Longitude")//对经纬度数据集做特殊处理
            {   
                if(!string.IsNullOrWhiteSpace(_lonDatasetName))
                    datasetName = _lonDatasetName;
            }
            else if(datasetName=="Latitude")
            {
                if (!string.IsNullOrWhiteSpace(_latDatasetName))
                    datasetName = _latDatasetName;
            }
            string dsFullPath = GetDatasetFullPath(datasetName);
            return GetBandsByFullName(dsFullPath);
        }

        /// <summary>
        /// 修改以前拼接GDALFullPath的方式，应为gdal将hdf中的文件夹中的空格做了处理，直接拼会发生很多未知的bug。
        /// 现在修改为了从gdal的所有datasetname中匹配查找的方式。
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
            for (int i = 0; i < _allGdalSubDatasets.Length; i++)
            {
                string shortGdalDatasetName = GetDatasetShortName(_allGdalSubDatasets[i]);
                if (shortGdalDatasetName == datasetShortName)
                    return _allGdalSubDatasets[i];
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

        private IRasterBand[] ReadBandsFromDataset(Dataset ds, IRasterDataProvider provider)
        {
            if (ds == null || ds.RasterCount == 0)
                return null;
            int bandNo = 1;
            IRasterBand[] bands = new IRasterBand[ds.RasterCount];
            for (int i = 1; i <= ds.RasterCount; i++)
            {
                bands[i - 1] = new GDALRasterBand(provider, ds.GetRasterBand(i), new GDALDataset(ds));
                bands[i - 1].BandNo = bandNo++;
            }
            return bands;
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
            if (!HDF5Helper.IsHdf5(header1024))
                return false;
            return HasDatasets(fname);
        }

        private bool HasDatasets(string filename)
        {
            return _datasetNames != null && _datasetNames.Length != 0;
        }
    }

    //public class HDFRasterBand : GDALRasterBand
    //{
    //    public HDFRasterBand(IRasterDataProvider rasterDataProvider, Band band, GDALDataset dataset)
    //        : base(rasterDataProvider, band, dataset)
    //    { 
    //    }

    //    public new override 
    //}
}
