using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace GeoDo.RSS.MIF.Core
{
    public abstract class FileFinder : IFileFinder
    {
        public abstract string[] Find(string currentRasterFile, ref string extinfo, string argument);

        protected bool ParseArugment(string argument, out Dictionary<string, string> argDic)
        {
            argDic = new Dictionary<string, string>();
            string[] split = argument.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (split == null || split.Length == 0)
                return false;
            int length = split.Length;
            string key = string.Empty;
            string value = string.Empty;
            string[] values = null;
            for (int i = 0; i < length; i++)
            {
                values = split[i].Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                if (values != null && values.Length == 2)
                {
                    key = values[0].Trim().ToUpper();
                    value = values[1].Trim().ToUpper();
                    if (argDic.ContainsKey(key))
                        argDic[key] = value;
                    else
                        argDic.Add(key, value);
                }
            }
            if (argDic.Count == 0)
                return false;
            return true;
        }

        protected bool GetArg(Dictionary<string, string> argDic, string argName, out string argValue)
        {
            argValue = string.Empty;
            if (!argDic.ContainsKey(argName))
                return false;
            argValue = argDic[argName];
            return string.IsNullOrEmpty(argValue) ? false : true;
        }

        protected string GetWorkspaceDir(string pro, string subPro)
        {
            return MifEnvironment.GetWorkspaceDir() + "\\" + pro + "\\";
        }
    }

}
