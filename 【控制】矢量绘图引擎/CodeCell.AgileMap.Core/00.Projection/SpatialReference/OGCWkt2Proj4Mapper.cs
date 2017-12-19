using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeCell.AgileMap.Core
{
    internal class OGCWkt2Proj4Mapper : IDisposable
    {
        private Dictionary<string, string> _datums = null;
        private List<NameMapItem> _prjNameMapItems = null;
        private List<NameMapItem> _prjParameterItems = null;
        private List<CoordinateDomain> _coordinateDomainItems = null;

        public OGCWkt2Proj4Mapper()
        {
            LoadPrjStdsMapTable();
        }

        private void LoadPrjStdsMapTable()
        {
            using (IPrjStdsMapTableParser p = new PrjStdsMapTableParser())
            {
                _datums = p.GetDatumsItems();
                _prjNameMapItems = p.GetPrjNameMapItems();
                _prjParameterItems = p.GetPrjParameterItems();
                _coordinateDomainItems = p.GetCoordinateDomainItems();
            }
        }


        public NameMapItem GetParameterFromEsriName(string esriName)
        {
            foreach (NameMapItem it in _prjParameterItems)
            {
                if (it.EsriName.ToUpper() == esriName.ToUpper())
                    return it;
            }
            return null;
        }

        public NameMapItem GetParameterFromWKTName(string wktName)
        {
            foreach (NameMapItem it in _prjParameterItems)
            {
                if (it.WktName.ToUpper() == wktName.ToUpper())
                    return it;
            }
            return null;
        }

        public NameMapItem GetPrjNameFromEsriName(string esriName)
        {
            foreach (NameMapItem it in _prjNameMapItems)
            {
                if (it.EsriName.ToUpper() == esriName.ToUpper())
                    return it;
            }
            return null;
        }

        public NameMapItem GetPrjNameFromWKTName(string wktName)
        {
            foreach (NameMapItem it in _prjNameMapItems)
            {
                if (it.WktName.ToUpper() == wktName.ToUpper())
                    return it;
            }
            return null;
        }

        public string ToProj4Datum(Datum datum)
        {
            foreach (string wkt in _datums.Keys)
            {
                if (wkt.ToUpper() == datum.Name.ToUpper())
                    return _datums[wkt];
            }
            return "WGS84";
        }

        #region IDisposable Members

        public void Dispose()
        {
            _datums.Clear();
            //_datums = null;
            _prjNameMapItems.Clear();
            //_prjNameMapItems = null;
            _prjParameterItems.Clear();
            //_prjParameterItems = null;
        }

        #endregion
    }
}
