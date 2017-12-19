using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.Core
{
    public class PersistContextEnvironment:IPersistContextEnvironment
    {
        private Dictionary<string, object> _varLists = new Dictionary<string, object>();

        public void Reset()
        {
            _varLists.Clear();
        }

        public void Put(string varName, object varValue)
        {
            if (string.IsNullOrEmpty(varName))
                return;
            if (_varLists.ContainsKey(varName))
                _varLists[varName] = varValue;
            else
                _varLists.Add(varName, varValue);
        }

        public object Get(string varName)
        {
            if (string.IsNullOrEmpty(varName) || !_varLists.ContainsKey(varName))
                return null;
            return _varLists[varName];
        }
    }
}
