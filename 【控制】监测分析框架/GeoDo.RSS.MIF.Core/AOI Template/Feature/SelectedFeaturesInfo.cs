using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.MIF.Core
{
    public class SelectedFeaturesInfo
    {
        private string _url;
        private string _fieldName;
        private string[] _featureFieldValues;

        public SelectedFeaturesInfo(string url, string fieldName, string[] featureFieldValues)
        {
            _url = url;
            _fieldName = fieldName;
            _featureFieldValues = featureFieldValues;
        }

        public string Url
        {
            get { return _url; }
        }

        public string FieldName
        {
            get { return _fieldName; }
        }

        public string[] FeatureFieldValues
        {
            get { return _featureFieldValues; }
        }
    }
}
