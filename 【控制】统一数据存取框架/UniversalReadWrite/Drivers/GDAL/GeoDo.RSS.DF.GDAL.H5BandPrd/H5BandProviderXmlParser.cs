using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace GeoDo.RSS.DF.GDAL.H5BandPrd
{
    internal class H5BandProviderXmlParser:IH5BandProviderConfiger, IDisposable
    {
        protected string _cnfgFile = null;

        public H5BandProviderXmlParser(string cnfgfile)
        {
            _cnfgFile = cnfgfile;
        }

        public BandProviderDef[] GetBandProviderDefs()
        {
            XDocument doc = XDocument.Load(_cnfgFile);
            XElement root = doc.Root;
            var elePrds = root.Elements(XName.Get("BandProvider"));
            if (elePrds == null || elePrds.Count() == 0)
                return null;
            List<BandProviderDef> prds = new List<BandProviderDef>();
            foreach (XElement ele in elePrds)
            {
                BandProviderDef prd = BandProviderDef.FromXElement(ele);
                if (prd != null)
                    prds.Add(prd);
            }
            return prds.Count > 0 ? prds.ToArray() : null;
        }

        public void Dispose()
        {
            //
        }
    }
}
