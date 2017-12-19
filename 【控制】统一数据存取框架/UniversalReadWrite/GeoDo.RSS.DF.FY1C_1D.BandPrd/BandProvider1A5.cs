#region Version Info
/*========================================================================
* 功能概述：
* 
* 创建者：DongW     时间：2013/9/28 14:50:07
* ------------------------------------------------------------------------
* 变更记录：
* 时间：                 修改者：                
* 修改说明：
* 
* ------------------------------------------------------------------------
* ========================================================================
*/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;
using System.ComponentModel.Composition;
using GeoDo.RSS.DF.FY1D;

namespace GeoDo.RSS.DF.FY1D.BandPrd
{
    /// <summary>
    /// 类名：BandProvider1A5
    /// 属性描述：
    /// 创建者：DongW   创建日期：2013/9/28 14:50:07
    /// 修改者：             修改日期：
    /// 修改描述：
    /// 备注：
    /// </summary>
    [Export(typeof(IBandProvider)), ExportMetadata("VERSION", "1")]
    public class BandProvider1A5 : IBandProvider
    {
        protected IRasterDataProvider _provider = null;
        protected List<string> _datasetNames = new List<string>();
        protected DataIdentify _dataIdentify = new DataIdentify();
        Dictionary<string, string> _fileAttr = new Dictionary<string, string>();

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
            SecondaryBandFor1A5 secBand = new SecondaryBandFor1A5(_provider);
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
            if (ToLocalEndian.ToInt16FromBig(new byte[] { header1024[0], header1024[1] }) != 113
                && ToLocalEndian.ToInt16FromBig(new byte[] { header1024[0], header1024[1] }) != 114)
                return false;
            return true;
        }

        public void Dispose()
        {

        }
    }
}
