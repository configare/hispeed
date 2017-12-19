using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.MIF.Core
{
    public abstract class MonitoringTheme:IMonitoringTheme,IDisposable
    {
        protected string _name;
        protected string _identify;
        protected List<IMonitoringProduct> _products = new List<IMonitoringProduct>();
        protected ThemeDef _themeDef;

        public MonitoringTheme(ThemeDef themeDef)
        {
            _themeDef = themeDef;
        }

        public string Name
        {
            get { return _name; }
        }

        public string Identify
        {
            get { return _identify; }
        }

        public List<IMonitoringProduct> Products
        {
            get { return _products; }
        }

        public IMonitoringProduct GetProductByIdentify(string identify)
        {
            if (_products == null || _products.Count == 0 || identify == null)
                return null;
            foreach (IMonitoringProduct prd in _products)
                if (prd.Identify != null && prd.Identify.ToUpper() == identify.ToUpper())
                    return prd;
            return null;
        }

        public virtual void Dispose()
        {
            if (_products != null && _products.Count > 0)
            {
                foreach (IMonitoringProduct prd in _products)
                    prd.Dispose();
                _products.Clear();
                _products = null;
            }
            _themeDef = null;
        }
    }
}
