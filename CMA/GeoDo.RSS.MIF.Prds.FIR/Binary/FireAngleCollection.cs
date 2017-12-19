using GeoDo.RSS.MIF.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.MIF.Prds.FIR
{
    internal class FireAngleCollection : ICursorDisplayedFeatures
    {
        private Dictionary<int, FireAngleFeature> _features;
        private string _name;

        public FireAngleCollection(string name, Dictionary<int, FireAngleFeature> features)
        {
            _name = name;
            _features = features;
        }

        public string Name
        {
            get { return _name; }
        }
        /// <summary>
        /// 该方法用于实现鼠标悬停显示内容
        /// </summary>
        /// <param name="pixelIndex"></param>
        /// <returns></returns>
        public string GetCursorInfo(int pixelIndex)
        {
            if (_features == null)
                return string.Empty;
            if (_features.ContainsKey(pixelIndex))
                return _features[pixelIndex].ToString();
            return string.Empty;
        }
    }
}
