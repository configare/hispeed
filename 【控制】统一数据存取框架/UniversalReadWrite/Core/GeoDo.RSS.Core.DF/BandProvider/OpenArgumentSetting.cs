using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.Core.DF
{
    public class OpenArgumentSetting : IOpenArgumentSetting
    {
        protected Dictionary<string, string> _arguments = new Dictionary<string, string>();

        public OpenArgumentSetting()
        {
        }

        public OpenArgumentSetting(Dictionary<string, string> arguments)
        {
            _arguments = arguments;
        }

        public string GetArgument(string argumentName)
        {
            if (string.IsNullOrEmpty(argumentName) || _arguments == null || _arguments.Count == 0 || !_arguments.ContainsKey(argumentName))
                return null;
            return _arguments[argumentName];
        }
    }
}
