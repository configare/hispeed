using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.Core.DF
{
    public class AttributeManager
    {
        private Dictionary<string, Dictionary<string, string>> _attributes = new Dictionary<string, Dictionary<string, string>>();

        public AttributeManager()
        {
            //
        }

        public Dictionary<string, string> GetAttributeDomain(string domain)
        {
            if (string.IsNullOrEmpty(domain))
                return null;
            return _attributes.ContainsKey(domain) ? _attributes[domain] : null;
        }

        public Dictionary<string, string> CreateAttributeDomain(string domain)
        {
            if (string.IsNullOrEmpty(domain))
                return null;
            if (_attributes.ContainsKey(domain))
                return GetAttributeDomain(domain);
            _attributes.Add(domain, new Dictionary<string, string>());
            return GetAttributeDomain(domain);
        }

        public void Clear()
        {
            foreach (Dictionary<string, string> dic in _attributes.Values)
                dic.Clear();
            _attributes.Clear();
        }
    }
}
