using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;

namespace GeoDo.MEF
{
    public class ComponentLoader<T>:IDisposable,IComponentLoader<T> where T:class
    {
        public const string METADATA_VERSION_IDENTIFY = "VERSION";
        private enumVersionControlMode _loadModel = enumVersionControlMode.FullTypeMatch;
        [ImportMany(AllowRecomposition = true)]
        public IEnumerable<Lazy<T, Dictionary<string, object>>> _loadedTs;

        public enumVersionControlMode LoadMode
        {
            get { return _loadModel; }
            set { _loadModel = value; }
        }

        public T[] LoadComponentsByCatalogName(string name)
        {
            return null;
        }

        public T[] LoadComponents(params string[] dirsOrfiles)
        {
            var catalog = new AggregateCatalog();
            foreach (string dir in dirsOrfiles)
            {
                if(dir.ToUpper().EndsWith(".DLL"))
                    catalog.Catalogs.Add(new AssemblyCatalog(dir));
                else
                    catalog.Catalogs.Add(new DirectoryCatalog(dir));
            }
            var container = new CompositionContainer(catalog);
            container.ComposeParts(this);
            T[] retComponents = null;
            if(_loadModel != enumVersionControlMode.AllowRepeat)
                retComponents = FilterByVersion();
            else
                retComponents = GetAllTs();
            return TryAdjustOrder(retComponents, dirsOrfiles[0]);
        }

        private T[] TryAdjustOrder(T[] retComponents, string dir)
        {
            string xmldir = Path.Combine(dir, "LoadOrder.xml");
            if (!File.Exists(xmldir))
                return retComponents;
            string[] fullanems = (new LoadOrderParser()).Parse(xmldir);
            if (fullanems == null || fullanems.Length == 0)
                return retComponents;
            T[] orderedTs = new T[retComponents.Length];
            List<T> uOrderedTs = new List<T>();
            foreach (T t in retComponents)
            {
                int idx = Array.IndexOf<string>(fullanems, t.GetType().FullName);
                if (idx < 0)
                    uOrderedTs.Add(t);
                else
                    orderedTs[idx] = t;
            }
            for(int i=orderedTs.Length-1;i>=0;i--)
                if (orderedTs[i] != null)
                    uOrderedTs.Insert(0, orderedTs[i]);
            return uOrderedTs.ToArray<T>();
        }

        private T[] GetAllTs()
        {
            List<T> ts = new List<T>();
            foreach (Lazy<T, Dictionary<string, object>> t in _loadedTs)
                ts.Add(t.Value);
            return ts.ToArray();
        }

        private T[] FilterByVersion()
        {
            Dictionary<T, int> ps = new Dictionary<T, int>();
            foreach (Lazy<T, Dictionary<string, object>> t in _loadedTs)
            {
                int v = 0;
                if (t.Metadata.ContainsKey(METADATA_VERSION_IDENTIFY))
                    v = int.Parse(t.Metadata[METADATA_VERSION_IDENTIFY].ToString());
                T preT = CheckIsHaveRepeated(ps, t);
                if (preT != null)
                {
                    if (ps[preT] < v)
                    {
                        ps.Remove(preT);
                        ps.Add(t.Value, v);
                    }
                }
                else
                    ps.Add(t.Value, v);
            }
            return ps.Keys.ToArray();
        }

        private T CheckIsHaveRepeated(Dictionary<T, int> ps, Lazy<T, Dictionary<string, object>> t)
        {
            if (ps.Count > 0)
            {
                T newp = t.Value;
                foreach (T op in ps.Keys)
                {
                    if (_loadModel == enumVersionControlMode.FullTypeMatch)
                    {
                        if (op.GetType().ToString() == newp.GetType().ToString())
                            return op;
                    }
                    else if (_loadModel == enumVersionControlMode.FullNameMatch)
                    {
                        if (op.GetType().Name == newp.GetType().Name)
                            return op;
                    }
                }
            }
            return null;
        }

        public void Dispose()
        {
            _loadedTs = null;
        }
    }
}
