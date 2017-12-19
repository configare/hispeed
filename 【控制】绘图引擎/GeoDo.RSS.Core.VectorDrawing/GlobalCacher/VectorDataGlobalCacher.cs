using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeCell.AgileMap.Core;

namespace GeoDo.RSS.Core.VectorDrawing
{
    public class VectorDataGlobalCacher:IVectorDataGlobalCacher
    {
        private bool _enabled = true;
        private Dictionary<string, ICachedVectorData> _cachedDatas = new Dictionary<string, ICachedVectorData>();
        private Dictionary<string, int> _refCounteds = new Dictionary<string, int>();
        private GeoDo.Project.IProjectionTransform _coordTransform;
        private Dictionary<string, ICachedFeatures> _cachedFeatures = new Dictionary<string, ICachedFeatures>();
        //检查活动视图是否为等经纬度投影
        private Func<bool> _gllPrjChecker; 

        public VectorDataGlobalCacher()
        {
            _coordTransform = GeoDo.Project.ProjectionTransformFactory.GetDefault();
        }

        public void SetGllPrjChecker(Func<bool> isGllPrj)
        {
            _gllPrjChecker = isGllPrj;
        }

        public bool IsEnabled
        {
            get { return _enabled; }
            set { _enabled = value; }
        }

        public bool IsGllPrjOfActiveViewer
        {
            get { return _gllPrjChecker(); }
        }

        public ICachedVectorData GetData(string shpFileName)
        {
            string identify = CachedVectorData.GetIdenfity(shpFileName);
            if (identify == null)
                return null;
            if (_cachedDatas.ContainsKey(identify))
            {
                return _cachedDatas[identify];
            }
            return null;
        }

        public ICachedVectorData Request(string shpFileName)
        {
            string identify = CachedVectorData.GetIdenfity(shpFileName);
            if (identify == null)
                return null;
            if (_cachedDatas.ContainsKey(identify))
            {
                _refCounteds[identify]++;
                return _cachedDatas[identify];
            }
            else
            {
                ICachedVectorData data = null;
                if (_coordTransform == null)
                    data = new CachedVectorData(shpFileName);
                else
                    data = new CachedVectorData(shpFileName, _coordTransform);
                _cachedDatas.Add(data.Identify, data);
                _refCounteds.Add(data.Identify, 1);
                return data;
            }
        }

        public void Release(string identify)
        {
            if (string.IsNullOrEmpty(identify))
                return;
            if (_refCounteds.ContainsKey(identify))
            {
                _refCounteds[identify]--;
                TryDisposeCache(identify);
            }
        }

        private void TryDisposeCache(string identify)
        {
            if (_refCounteds[identify] > 0)
                return;
            _cachedDatas[identify].Dispose();
            _cachedDatas.Remove(identify);
            _refCounteds.Remove(identify);
            GC.Collect();
        }

        public void PutFeatures(ICachedFeatures features)
        {
            if (features == null || string.IsNullOrEmpty(features.Identify))
                return;
            if (_cachedFeatures.ContainsKey(features.Identify))
                _cachedFeatures[features.Identify] = features;
            else
                _cachedFeatures.Add(features.Identify, features);
        }

        public ICachedFeatures GetFeatures(string identify)
        {
            if (string.IsNullOrEmpty(identify))
                return null;
            if (_cachedFeatures.ContainsKey(identify))
                return _cachedFeatures[identify];
            return null;
        }

        public void Dispose()
        {
            _coordTransform = null;
            if (_cachedDatas != null && _cachedDatas.Count > 0)
            {
                foreach (ICachedVectorData data in _cachedDatas.Values)
                    data.Dispose();
                _cachedDatas.Clear();
                _cachedDatas = null;
            }
            //
            if (_cachedFeatures != null)
            {
                foreach (ICachedFeatures cache in _cachedFeatures.Values)
                {
                    if (cache.Features != null && cache.Features.Length != 0)
                    {
                        foreach (Feature feature in cache.Features)
                        {
                            feature.Dispose(); 
                        }
                    }
                }
                _cachedFeatures.Clear();
                _cachedFeatures = null;
            }
        }
    }
}
