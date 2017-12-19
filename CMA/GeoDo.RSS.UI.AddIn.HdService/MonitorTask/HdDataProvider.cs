using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.UI.AddIn.HdService
{
    public class HdDataProvider
    {
        private HdDataServer.DataSearchServerPortTypeClient _client = null;
        private static HdDataProvider _hdDataProvider = null;
        private bool _canUse = false;
        public event EventHandler<HdDataServer.getProjectionsCompletedEventArgs> getProjectionsCompleted;
        public event EventHandler<HdDataServer.getBlocksCompletedEventArgs> getBlocksCompleted;
        public event EventHandler<HdDataServer.getMosaicsCompletedEventArgs> getMosaicsCompleted;
        public event EventHandler<HdDataServer.getRasterDatsCompletedEventArgs> getRasterDatsCompleted;

        public HdDataProvider()
        {
            try
            {
                _client = new HdDataServer.DataSearchServerPortTypeClient();
                _client.getProjectionsCompleted += new EventHandler<HdDataServer.getProjectionsCompletedEventArgs>(_client_getProjectionsCompleted);
                _client.getMosaicsCompleted += new EventHandler<HdDataServer.getMosaicsCompletedEventArgs>(_client_getMosaicsCompleted);
                _client.getBlocksCompleted += new EventHandler<HdDataServer.getBlocksCompletedEventArgs>(_client_getBlocksCompleted);
                _client.getRasterDatsCompleted += new EventHandler<HdDataServer.getRasterDatsCompletedEventArgs>(_client_getRasterDatsCompleted);
                _client.Open();
                _canUse = true;
            }
            catch (Exception ex)
            {
                _canUse = false;
                LogFactory.WriteLine("创建实例失败" + ex.Message);
            }
        }

        #region 栅格数据查询
        /**
	     * 监测产品栅格统计
	     *  1.	satellite(例：NOAA18)
	     *	2.	sensor(例：VIRR\AVHRR)
	     *	3.	resolution(例：1000M)
	     *	4.	productName(例：FIR)
	     *  5.	countPeroid(例：上旬：firstTenDay,下旬:lastTenDay,中旬：middleTenDay,月：month,季：season,年：year)
	     *  6.	countIdentify(例：面积统计:areaStat、频次统计:freqStat)
	     *  7.	开始时间 (例：yyyy-MM-dd)
	     *  8.	结束时间(例：yyyy-MM-dd)
         * /**
	     * 监测产品栅格统计
	     *  1.	satellite(例：NOAA18)
	     *  2.	sensor(例：AVHRR)
	     *  3.	resolution(例：1000M)
	     *  4.	productName(例：ICE)
	     *  5.	countPeroid(例：firstTenDay)
	     *  6.	countIdentify(例：areaStat)
	     *  7.	开始时间 (例：2013-08-01)
	     *  8.	结束时间(例：2013-08-01)
	     */
        public void getRasterDatsAsync(string satellite, string sensor, string resolution, string productName,
            string countPeroid, string countIdentify, string beginDate, string endDate)
        {//.ToString("yyyy-MM-dd")
            _client.getRasterDatsAsync(satellite, sensor, resolution, productName, countPeroid, countIdentify,
                beginDate, endDate);
        }

        void _client_getRasterDatsCompleted(object sender, HdDataServer.getRasterDatsCompletedEventArgs e)
        {
            try
            {
                if (getRasterDatsCompleted != null)
                    getRasterDatsCompleted(sender, e);
            }
            catch (Exception ex)
            {
                LogFactory.WriteLine("_client_getRasterDatsCompleted：" + ex.Message);
            }
        }
        #endregion

        void _client_getBlocksCompleted(object sender, HdDataServer.getBlocksCompletedEventArgs e)
        {
            try
            {
                if (getBlocksCompleted != null)
                    getBlocksCompleted(sender, e);
            }
            catch (Exception ex)
            {
                LogFactory.WriteLine("_client_getBlocksCompleted：" + ex.Message);
            }
        }

        void _client_getMosaicsCompleted(object sender, HdDataServer.getMosaicsCompletedEventArgs e)
        {
            try
            {
                if (getMosaicsCompleted != null)
                    getMosaicsCompleted(sender, e);
            }
            catch (Exception ex)
            {
                LogFactory.WriteLine("_client_getMosaicsCompleted：" + ex.Message);
            }
        }

        void _client_getProjectionsCompleted(object sender, HdDataServer.getProjectionsCompletedEventArgs e)
        {
            try
            {
                if (e.Error == null)
                {
                    HdDataServer.ProjectionInfo[] s = e.Result;
                    if (e.Result.Length == 0)
                    {
                        if (getProjectionsCompleted != null)
                            getProjectionsCompleted(this, e);
                    }
                    else
                    {
                        var r = from item in s
                                where !string.IsNullOrWhiteSpace(item.datapath)
                                select item;
                        if (getProjectionsCompleted != null)
                            getProjectionsCompleted(this, new HdDataServer.getProjectionsCompletedEventArgs(new object[] { r.ToArray() }, e.Error, e.Cancelled, e.UserState));
                    }
                }
                else
                {
                    if (getProjectionsCompleted != null)
                        getProjectionsCompleted(this, e);
                }
            }
            catch (Exception ex)
            {
                LogFactory.WriteLine("_client_getProjectionsCompleted:" + ex.Message);
            }
        }

        public static HdDataProvider Instance
        {
            get
            {
                if (_hdDataProvider == null)
                    _hdDataProvider = new HdDataProvider();
                return _hdDataProvider;
            }
        }

        public bool CanUse()
        {
            return _canUse;
        }

        public HdDataServer.OrbitInfo[] getOrbits(DateTime beginDt, DateTime endDt)
        {
            return _client.getOrbits(beginDt, endDt, null, null, null, null, null);
        }

        public HdDataServer.ProjectionInfo[] getProjections(DateTime beginDt, DateTime endDt)
        {
            HdDataServer.ProjectionInfo[] s = _client.getProjections(beginDt, endDt, null, null, null, null, null);
            var r = from item in s
                    where !string.IsNullOrWhiteSpace(item.datapath)
                    select item;
            return r.ToArray();
        }

        public void getProjectionsAsync(DateTime beginDt, DateTime endDt)
        {
            _client.getProjectionsAsync(beginDt, endDt, null, null, null, null, null);
        }

        public void getProjectionsAsync(DateTime beginDt, DateTime endDt, string satellite, string sensor)
        {
            _client.getProjectionsAsync(beginDt, endDt, satellite, sensor, null, null, null);
        }

        public HdDataServer.MosaicInfo[] getMosaics(DateTime beginDt, DateTime endDt)
        {
            return _client.getMosaics(beginDt, endDt, null, null, null, null, null);
        }

        public void getMosaicsAsync(DateTime beginDt, DateTime endDt)
        {
            _client.getMosaicsAsync(beginDt, endDt, null, null, null, null, null);
        }

        public void getMosaicsAsync(DateTime beginDt, DateTime endDt, string satellite, string sensor)
        {
            _client.getMosaicsAsync(beginDt, endDt, satellite, sensor, null, null, null);
        }

        public HdDataServer.BlockInfo[] getBlocks(DateTime beginDt, DateTime endDt)
        {
            return _client.getBlocks(beginDt, endDt, null, null, null, null, null);
        }

        public void getBlocksAsync(DateTime beginDt, DateTime endDt)
        {
            _client.getBlocksAsync(beginDt, endDt, null, null, null, null, null);
        }

        public void getBlocksAsync(DateTime beginDt, DateTime endDt, string satellite, string sensor)
        {
            _client.getBlocksAsync(beginDt, endDt, satellite, sensor, null, null, null);
        }

        public HdDataServer.BlockInfo[] GetBlockByFile(string filename, string identify)
        {
            return _client.getBlocksByFile(filename, identify);
        }

        public void Close()
        {
            if (_client != null)
                _client.Close();
        }
    }
}
