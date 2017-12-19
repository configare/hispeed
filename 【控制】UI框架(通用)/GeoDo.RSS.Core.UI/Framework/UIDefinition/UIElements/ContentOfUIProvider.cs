using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;

namespace GeoDo.RSS.Core.UI
{
    public class ContentOfUIProvider
    {
        private string _provider;
        private object _control;

        public ContentOfUIProvider(string provider)
        {
            _provider = provider;
            LoadControl();
        }

        public string Provider
        {
            get { return _provider; }
        }

        public object Control
        {
            get { return _control; }
        }

        private void LoadControl()
        {
            if (string.IsNullOrEmpty(_provider) || _provider == "assembly:class")
                return;
            string[] parts = _provider.Split(':');
            string fname = AppDomain.CurrentDomain.BaseDirectory + parts[0];
            if (!File.Exists(fname))
                return;
            Assembly assembly = Assembly.LoadFile(fname);
            if (assembly == null)
                return;
            _control = assembly.CreateInstance(parts[1]);
        }
    }
}
