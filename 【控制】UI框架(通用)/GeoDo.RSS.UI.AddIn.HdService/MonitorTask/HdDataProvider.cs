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

        public HdDataProvider()
        {
            try
            {
                _client = new HdDataServer.DataSearchServerPortTypeClient();
                _client.getProjectionsCompleted += new EventHandler<HdDataServer.getProjectionsCompletedEventArgs>(_client_getProjectionsCompleted);
                _client.getMosaicsCompleted += new EventHandler<HdDataServer.getMosaicsCompletedEventArgs>(_client_getMosaicsCompleted);
                _client.getBlocksCompleted += new EventHandler<HdDataServer.getBlocksCompletedEventArgs>(_client_getBlocksCompleted);
                _client.Open();
                _canUse = true;
            }
            catch
            {
                _canUse = false;
            }
        }

        void _client_getBlocksCompleted(object sender, HdDataServer.getBlocksCompletedEventArgs e)
        {
            if (getBlocksCompleted != null)
                getBlocksCompleted(sender, e);
        }

        void _client_getMosaicsCompleted(object sender, HdDataServer.getMosaicsCompletedEventArgs e)
        {
            if (getMosaicsCompleted != null)
                getMosaicsCompleted(sender, e);
        }

        void _client_getProjectionsCompleted(object sender, HdDataServer.getProjectionsCompletedEventArgs e)
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

        public HdDataServer.OrbitInfo[] getOrbits(DateTime beginDt,DateTime endDt)
        {
            return _client.getOrbits(beginDt,endDt, null, null, null, null, null);
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
            return _client.getMosaics(beginDt,endDt, null, null, null, null, null);
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

        public void Close()
        {
            _client.Close();
        }
    }
}
