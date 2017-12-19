using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace GeoDo.RSS.MIF.Core
{
    public class ContextEnvironment:IContextEnvironment
    {
        public const string ENV_VAR_NAME_CURRENT_RASTER_FILE = "CurrentRasterFile";
        public const string ENV_VAR_NAME_BINARY_FILE = "DBLV";

        private Dictionary<string, string> _contextVars = new Dictionary<string, string>();

        public ContextEnvironment()
        { 
        }

        public void PutContextVar(string varIdentify, string varValue)
        {
            if (string.IsNullOrEmpty(varIdentify))
                return;
            if (_contextVars.ContainsKey(varIdentify))
                _contextVars[varIdentify] = varValue;
            else
                _contextVars.Add(varIdentify, varValue);
        }

        public void Reset()
        {
            _contextVars.Clear();
        }

        public string GetContextVar(string varIdentify)
        {
            if (string.IsNullOrEmpty(varIdentify))
                return null;
            if (_contextVars.ContainsKey(varIdentify))
                return _contextVars[varIdentify];
            if (_contextVars.ContainsKey("DBLV"))
                return TryGetVarIdentityFile(varIdentify);
            return null;
        }

        private string TryGetVarIdentityFile(string varIdentify)
        {
            string dblvFile = _contextVars[ENV_VAR_NAME_BINARY_FILE];
            string resultFile = dblvFile.Replace("_" + ENV_VAR_NAME_BINARY_FILE + "_", "_" + varIdentify + "_");
            if (File.Exists(resultFile))
                return resultFile;
            else
                return null;
        }
        private object _session;

        public object Session
        {
            get
            {
                return _session;
            }
            set
            {
                _session = value;
            }
        }
    }
}
