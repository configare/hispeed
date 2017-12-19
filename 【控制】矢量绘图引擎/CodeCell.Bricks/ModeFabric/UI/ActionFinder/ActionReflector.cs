using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using System.Runtime.Remoting;

namespace CodeCell.Bricks.ModelFabric
{
    public class ActionReflector : IActionReflector
    {
        private List<Assembly> _assemblies = null;

        public ActionReflector()
        { 
        }

        #region IActionReflector 成员

        public void AddScanDir(string dir, SearchOption searchOption)
        {
            if (_assemblies == null)
                _assemblies = new List<Assembly>();
            List<string> fs = new List<string>();
            string[] fExes = Directory.GetFiles(dir, "*.exe", searchOption);
            string[] fDlls = Directory.GetFiles(dir, "*.dll", searchOption);
            if (fExes != null && fExes.Length >0)
                fs.AddRange(fExes);
            if (fDlls != null && fDlls.Length >0)
                fs.AddRange(fDlls);
            if (fs.Count == 0)
                return;
            foreach (string f in fs)
            {
                try
                {
                    Assembly ass = Assembly.LoadFrom(f);
                    _assemblies.Add(ass);
                }
                catch
                { 
                }
            }
        }

        public void AddScanAssembly(string assemblyUrl)
        {
            if (_assemblies == null)
                _assemblies = new List<Assembly>();
            try
            {
                Assembly ass = Assembly.LoadFrom(assemblyUrl);
                _assemblies.Add(ass);
            }
            catch
            {
            }
        }

        public ActionInfo[] Reflector()
        {
            if (_assemblies == null || _assemblies.Count == 0)
                return null;
            List<ActionInfo> actionNames = new List<ActionInfo>();
            foreach (Assembly ass in _assemblies)
            {
                Type[] types = ass.GetTypes();
                if (types == null || types.Length == 0)
                    continue;
                foreach (Type t in types)
                {
                    if (!t.IsClass || t.IsAbstract)
                        continue;
                    if (t.GetInterface("CodeCell.Bricks.ModelFabric.IAction") == null)
                        continue;
                    ActionAttribute name = null;
                    object[] atts = t.GetCustomAttributes(typeof(ActionAttribute),true);
                    if (atts == null || atts.Length == 0)
                    {
                        ObjectHandle actObj = Activator.CreateInstance(ass.FullName, t.FullName);
                        object nameobj = actObj.Unwrap().GetType().InvokeMember("Name", BindingFlags.GetProperty, null, actObj.Unwrap(), null);
                        if (nameobj != null)
                        {
                            name = new ActionAttribute(nameobj.ToString(), string.Empty);
                        }
                    }
                    else
                    {
                        name = atts[0] as ActionAttribute;
                    }
                    actionNames.Add(new ActionInfo(ass.Location, t.FullName, name));
                }
            }
            return actionNames.Count > 0 ? actionNames.ToArray() : null;
        }

        #endregion

        #region IDisposable 成员

        public void Dispose()
        {
            if (_assemblies != null)
            {
                _assemblies.Clear();
                _assemblies = null;
            }
        }

        #endregion
    }
}
