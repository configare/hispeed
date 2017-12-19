using System.Collections.Generic;

namespace GeoDo.Smart.Tools.MODLSTPro
{
    public class Pair
    {
        public Pair()
        {
            KeyValues = new KeyValues();
            Pairs = new List<Pair>();
        }

        public string Name { get; private set; }
        public KeyValues KeyValues { get; private set; }
        public List<Pair> Pairs { get; private set; }
        private string _beginString;

        public string BeginString
        {
            get { return _beginString; }
            set
            {
                _beginString = value;
                EndString = string.Format("END_{0}", _beginString);
                var kv = new KeyValue(_beginString);
                Name = kv.Value;
            }
        }

        public Pair GetGroup(string groupName)
        {
            foreach (var pair in Pairs)
            {
                if (pair.Name == groupName)
                    return pair;
                var pairNew = pair.GetGroup(groupName);
                if (pairNew != null)
                    return pairNew;
            }
            return null;
        }

        public string GetAttrValue(string key)
        {
            var kvalue = KeyValues[key];
            if (kvalue != null)
                return kvalue;

            foreach (var pair in Pairs)
            {
                var pairValue = pair.GetAttrValue(key);
                if (pairValue != null)
                    return pairValue;
            }
            return null;
        }

        public string EndString { get; private set; }
    }
}