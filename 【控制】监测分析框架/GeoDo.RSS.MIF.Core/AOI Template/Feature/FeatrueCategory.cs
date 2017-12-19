using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeCell.AgileMap.Core;

namespace GeoDo.RSS.MIF.Core
{
    public class VectorTempFeatureCategory
    {
        private string _name;
        private string[] _urls;

        public VectorTempFeatureCategory(string name)
        {
            _name = name;
        }

        public VectorTempFeatureCategory(string name, string url)
        {
            _urls = new string[] { url };
            _name = name;
            //if (!string.IsNullOrEmpty(url))
            //    LoadFeatures();
        }

        public VectorTempFeatureCategory(string name, string[] urls)
        {
            _urls = urls;
            _name = name;
            //if (urls == null || urls.Length == 0)
            //    return;
            //LoadFeatures();
        }

        public string Name
        {
            get
            {
                return _name;
            }
        }

        public string[] Urls
        {
            get
            {
                return _urls;
            }
        }

        public Feature[] LoadFeatures()
        {
            if (_urls == null)
                return null;
            List<Feature> fets = new List<Feature>();
            IVectorFeatureDataReader dr = null;
            Feature[] temp = null;
            foreach (string url in _urls)
            {
                dr = VectorDataReaderFactory.GetUniversalDataReader(url) as IVectorFeatureDataReader;
                if (dr == null)
                    continue;
                try
                {
                    temp = dr.FetchFeatures();
                    if (temp == null || temp.Length == 0)
                        continue;
                    fets.AddRange(temp);
                }
                finally
                {
                    dr.Dispose();
                }
            }
            return fets.Count == 0 ? null : fets.ToArray();
        }

    }
}
