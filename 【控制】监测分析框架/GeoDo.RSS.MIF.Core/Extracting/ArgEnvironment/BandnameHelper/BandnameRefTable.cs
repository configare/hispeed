using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.MIF.Core
{
    public class BandnameRefTable
    {
        private string _satellite;
        private string _sensor;
        private BandnameItem[] _bandnameItems = null;

        public BandnameRefTable(string satellite,string sensor,BandnameItem[] bandnameItems)
        {
            _satellite = satellite;
            _sensor = sensor;
            _bandnameItems = bandnameItems;
        }

        public string Satellite
        {
            get { return _satellite; }
        }

        public string Sensor
        {
            get { return _sensor; }
        }

        /// <summary>
        /// 根据波段名称获取波段序号，从1开始
        /// </summary>
        /// <param name="bandname"></param>
        /// <returns></returns>
        public int GetBandIndex(string bandname)
        {
            if (string.IsNullOrEmpty(bandname) || _bandnameItems == null || _bandnameItems.Length == 0)
                return -1;
            foreach (BandnameItem it in _bandnameItems)
                if (it.Name.ToUpper() == bandname.ToUpper())
                    return it.Index;
            return -1;
        }

        /// <summary>
        /// 根据波段范围获取波段序号，从1开始
        /// </summary>
        /// <param name="waveLength"></param>
        /// <returns></returns>
        public int GetBandIndex(float waveLength)
        {
            if (waveLength < 0)
                return -1;
            foreach (BandnameItem it in _bandnameItems)
                if (it.WaveLength != null && it.WaveLength.IsContains(waveLength))
                    return it.Index;
            return -1;
        }

        /// <summary>
        /// 根据中心波长和分辨率获取波段序号，从1开始
        /// 20090813 add by luoke 
        /// </summary>
        /// <param name="waveLength"></param>
        /// <param name="resolution"></param>
        /// <returns></returns>
        public int GetBandIndex(float waveLength, float resolution)
        {
            if (waveLength < 0)
                return -1;
            foreach (BandnameItem it in _bandnameItems)
                if (it.Resolution==resolution &&it.WaveLength != null && it.WaveLength.IsContains(waveLength))
                    return it.Index;
            return -1;
        }

        public int GetBandIndexByType(string type)
        {
            if (string.IsNullOrEmpty(type))
                return -1;
            foreach (BandnameItem it in _bandnameItems)
                if (it.Type == type)
                    return it.Index;
            return -1;
        }

        public BandnameItem GetBandItem(float waveLength)
        {
            if (waveLength < 0)
                return null;
            if (_bandnameItems.Length < 1)
                return null;
            foreach (BandnameItem it in _bandnameItems)
                if (it.WaveLength != null && it.WaveLength.IsContains(waveLength))
                    return it;
            return null;
        }

        public BandnameItem GetBandItem(int bandIndex)
        {
            if (bandIndex < 0)
                return null;
            if (_bandnameItems.Length < 1)
                return null;
            foreach (BandnameItem it in _bandnameItems)
                if (it.Index == bandIndex)
                    return it;
            return null;
        }

        /// <summary>
        /// 20090813 add by luoke 
        /// </summary>
        /// <param name="waveLength"></param>
        /// <param name="resolution"></param>
        /// <returns></returns>
        public BandnameItem GetBandItem(float waveLength, float resolution)
        {
            if (waveLength < 0)
                return null;
            if (_bandnameItems.Length < 1)
                return null;
            foreach (BandnameItem it in _bandnameItems)
                if (it.Resolution == resolution && it.WaveLength != null && it.WaveLength.IsContains(waveLength))
                    return it;
            return null;
        }

        public BandnameItem[] BandnameItems
        {
            get { return _bandnameItems; }
            set { _bandnameItems = value; }
        }

        public BandnameItem[] GetBandnameItem(string waveLenType)
        {
            List<BandnameItem> its = new List<BandnameItem>();
            foreach (BandnameItem it in _bandnameItems)
                if (it.WaveLength != null && it.WaveLength.WaveLengthType == waveLenType)
                    its.Add(it);
            return its.Count > 0 ? its.ToArray() : null;
        }

        /// <summary>
        /// 20090813 add by luoke
        /// </summary>
        /// <param name="resolution"></param>
        /// <returns></returns>
        public BandnameItem[] GetBandnameItem(float resolution)
        {
            List<BandnameItem> its = new List<BandnameItem>();
            foreach (BandnameItem it in _bandnameItems)
                if (it.Resolution == resolution)
                    its.Add(it);
            return its.Count > 0 ? its.ToArray() : null; 
        }

        public float GetCenterBandItem(int bandIndex)
        {
            if (bandIndex < 0)
                return 0;
            if (_bandnameItems.Length < 1)
                return 0;
            foreach (BandnameItem it in _bandnameItems)
                if (it.Index == bandIndex)
                    return it.CenterWaveNumber;
            return 0;
        }

        public object GetBandItem(string waveLength)
        {
            throw new NotImplementedException();
        }
    }
}
